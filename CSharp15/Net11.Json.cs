using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace CSharp15;

internal union IntOrString(int, string);

[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
internal union JsonShape(Circle, Rectangle);
internal sealed record class Circle(double Radius);
internal sealed record class Rectangle(double Width, double Height);

internal partial class Net11
{
	public void Unions()
	{
		// C# 15 unions serialize as the active case: no wrapper object, no discriminator
		Console.WriteLine(JsonSerializer.Serialize<IntOrString>(42)); // 42
		Console.WriteLine(JsonSerializer.Serialize<IntOrString>("hello")); // "hello"
		Console.WriteLine(JsonSerializer.Serialize<IntOrString>(default)); // null

		// deserialization picks the case from the JSON token type (number, string, Boolean, array, object)
		var value = JsonSerializer.Deserialize<IntOrString>("\"hello\"");
		Console.WriteLine(value.Value); // hello
	}

	public void UnionStructuralClassifier()
	{
		// two object cases have the same token type, so pick the case by its property names
		var shape = JsonSerializer.Deserialize<JsonShape>("""{"Radius":2}""");
		Console.WriteLine(shape.Value); // Circle { Radius = 2 }

		shape = JsonSerializer.Deserialize<JsonShape>("""{"Width":3,"Height":4}""");
		Console.WriteLine(shape.Value); // Rectangle { Width = 3, Height = 4 }

		// for other rules, derive from JsonTypeClassifierFactory
	}

	public void ClosedHierarchyPolymorphism()
	{
		CalculatorOperation operation = new Multiply(6, 7);

		// Before .NET 11: no type information unless every derived type is listed with [JsonDerivedType]
		Console.WriteLine(JsonSerializer.Serialize(operation)); // {}

		// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/union-types#choose-unions-or-closed-hierarchies
		// Use a union when you need to preserve a discriminator-free JSON format you don't control, or when the cases have distinct JSON shapes.
		// When you control the types and JSON contract, a closed hierarchy with inferred polymorphism can identify object cases with a discriminator instead.

		// .NET 11: infer the derived types from a C# 15 closed hierarchy
		// (or per-hierarchy with [JsonPolymorphic(InferClosedTypePolymorphism = true)], or in [JsonSourceGenerationOptions])
		var options = new JsonSerializerOptions { InferClosedTypePolymorphism = true };
		string json = JsonSerializer.Serialize(operation, options);
		Console.WriteLine(json); // {"$type":"Multiply","Left":6,"Right":7}

		var roundTripped = JsonSerializer.Deserialize<CalculatorOperation>(json, options);
		Console.WriteLine(roundTripped); // Multiply { Left = 6, Right = 7 }
	}

	public void OpenGenericDerivedTypes()
	{
		// Before .NET 11: InvalidOperationException ("Derived types ... must not be generic"); each closed type had to be listed
		Envelope<int> envelope = new SignedEnvelope<int>(42, "abc");
		var json = JsonSerializer.Serialize(envelope);
		Console.WriteLine(json); // {"$type":"signed","Signature":"abc","Payload":42}
		Console.WriteLine(JsonSerializer.Deserialize<Envelope<int>>(json)); // SignedEnvelope { Payload = 42, Signature = abc }
	}

	[JsonDerivedType(typeof(SignedEnvelope<>), "signed")]
	internal record class Envelope<T>(T Payload);
	internal sealed record class SignedEnvelope<T>(T Payload, string Signature) : Envelope<T>(Payload);

	public void NamingPolicies()
	{
		// built-in PascalCase naming policy (useful when the default options are camelCase, e.g., ASP.NET Core)
		var options = new JsonSerializerOptions(JsonSerializerDefaults.Web) { PropertyNamingPolicy = JsonNamingPolicy.PascalCase };
		Console.WriteLine(JsonSerializer.Serialize(new { firstName = "Ada" }, options)); // {"FirstName":"Ada"}

		// [JsonNamingPolicy] and [JsonIgnore(Condition = ...)] can now be applied to a type
		// precedence: [JsonPropertyName] > member [JsonNamingPolicy] > type [JsonNamingPolicy] > options
		var release = new ReleaseInfo { ProductName = ".NET", ReleaseVersion = "11", Notes = null };
		Console.WriteLine(JsonSerializer.Serialize(release)); // {"productName":".NET","release_version":"11"}
	}

	[JsonNamingPolicy(JsonKnownNamingPolicy.SnakeCaseLower)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	internal sealed class ReleaseInfo
	{
		[JsonNamingPolicy(JsonKnownNamingPolicy.CamelCase)]
		public string ProductName { get; set; } = "";

		public string ReleaseVersion { get; set; } = "";

		public string? Notes { get; set; }
	}

	public async Task JsonLines()
	{
		// default: a JSON array
		using var arrayStream = new MemoryStream();
		await JsonSerializer.SerializeAsyncEnumerable(arrayStream, GetNumbersAsync());
		Console.WriteLine(System.Text.Encoding.UTF8.GetString(arrayStream.ToArray())); // [0,1,2]

		// topLevelValues: true writes JSON Lines (JSONL): one compact value per line
		using var jsonlStream = new MemoryStream();
		await JsonSerializer.SerializeAsyncEnumerable(jsonlStream, GetNumbersAsync(), topLevelValues: true);
		Console.WriteLine(System.Text.Encoding.UTF8.GetString(jsonlStream.ToArray()));
		// 0
		// 1
		// 2

		static async IAsyncEnumerable<int> GetNumbersAsync()
		{
			for (int i = 0; i < 3; i++)
			{
				await Task.Yield();
				yield return i;
			}
		}
	}

	public void ReadOnlySet()
	{
		// IReadOnlySet<T> is now supported (deserializes as HashSet<T>)
		var tags = JsonSerializer.Deserialize<IReadOnlySet<string>>("""["a","b","a"]""")!;
		Console.WriteLine(tags.Count); // 2
	}
}
