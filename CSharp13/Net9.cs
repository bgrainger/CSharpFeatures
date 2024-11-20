using System.Buffers.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace CSharp13;

internal partial class Net9
{
	public void LinqMethods()
	{
		string[] strings = ["one", "two", "three"];
		strings.CountBy(x => x[0]); // ['o', 1], ['t', 2]
		strings.AggregateBy(x => x[0], x => 0, (acc, str) => acc + str.Length); // ['o', 3], ['t', 8]
	}

	public void ConvertBase64()
	{
		byte[] bytes = [1, 2, 0xFF, 4];
		Convert.ToBase64String(bytes); // AQL/BA==
		Base64Url.EncodeToString(bytes); // AQL_BA
	}

	// BinaryFormatter is removed

	public void OrderedDictionary()
	{
		var oldOrderedDictionary = new OrderedDictionary()
		{
			{ "one", 1 },
			{ "two", 2 },
			{ "three", true },
		};

		var sortedDictionary = new SortedDictionary<string, int>()
		{
			{ "one", 1 },
			{ "two", 2 },
			{ "three", 3 },
		};

		var sortedList = new SortedList<string, int>()
		{
			{ "one", 1 },
			{ "two", 2 },
			{ "three", 3 },
		};

		var orderedDictionary = new OrderedDictionary<string, int>()
		{
			{ "one", 1 },
			{ "two", 2 },
			{ "three", 3 },
		};

		Console.WriteLine(orderedDictionary["two"]); // 2
		Console.WriteLine(orderedDictionary.IndexOf("two")); // 1 (0-based)
		Console.WriteLine(orderedDictionary.GetAt(2)); // ["three", 3]

		Console.WriteLine(string.Join(", ", sortedDictionary)); // [one, 1], [three, 3], [two, 2]
		Console.WriteLine(string.Join(", ", sortedList)); // [one, 1], [three, 3], [two, 2]
		Console.WriteLine(string.Join(", ", orderedDictionary)); // [one, 1], [two, 2], [three, 3]
	}

	public void ReadOnlySet()
	{
		HashSet<int> set = [1, 2];
		ReadOnlySet<int> readOnlySet = new(set);

		// No .AsReadOnly() extension method; coming in .NET 10? https://github.com/dotnet/runtime/pull/106037
		// "Conflicts" with Libronix.Utility.ReadOnlyHashSet<T>
	}

	public void TimeSpanFrom()
	{
		TimeSpan timeSpan1 = TimeSpan.FromSeconds(value: 101.832);
		Console.WriteLine($"timeSpan1 = {timeSpan1}"); // timeSpan1 = 00:01:41.8319999

		TimeSpan timeSpan2 = TimeSpan.FromSeconds(seconds: 101, milliseconds: 832);
		Console.WriteLine($"timeSpan2 = {timeSpan2}"); // timeSpan2 = 00:01:41.8320000
	}

	public void DebugAssert()
	{
		var minutes = DateTime.UtcNow.Minute;
		var seconds = DateTime.UtcNow.Second;
		Debug.Assert(minutes == seconds); // no longer need Debug.Assert(minutes == seconds, "minutes == seconds");
	}

	public void EnumerateSplits()
	{
		// similar to String.Split, but doesn't allocate
		ReadOnlySpan<char> input = "1,2,345,678,90";
		foreach (Range range in input.Split(','))
		{
			ReadOnlySpan<char> segment = input[range];
			Console.WriteLine(segment.ToString()); // 1; 2; 345; 678; 90
		}
	}

	[GeneratedRegex("[aeiou]")]
	private partial Regex VowelsRegex { get; } // partial property, not partial method: VowelsRegex()

	public void UseGeneratedRegex()
	{
		Console.WriteLine(VowelsRegex.IsMatch("abc")); // True
		Console.WriteLine(VowelsRegex.IsMatch("123")); // False
	}

	public void EnumerateRegexSplits()
	{
		// similar to StringSegment.Split, but doesn't allocate an enumerator
		ReadOnlySpan<char> input = "Regular Expressions";
		foreach (Range range in VowelsRegex.EnumerateSplits(input)) // returns ref struct ValueSplitEnumerator
		{
			ReadOnlySpan<char> segment = input[range];
			Console.WriteLine(segment.ToString()); // R; g; l; r ; xpr; ss; ; ns
		}
	}

	public void GuidVersion7()
	{
		// xxxxxxxx-xxxx-4xxx-xxxx-xxxxxxxxxxxx
		Console.WriteLine(Guid.NewGuid());

		// 019xxxxx-xxxx-7xxx-xxxx-xxxxxxxxxxxx
		Console.WriteLine(Guid.CreateVersion7());

		// 01934b49-0495-7xxx-xxxx-xxxxxxxxxxxx
		Console.WriteLine(Guid.CreateVersion7(new DateTimeOffset(2024, 11, 20, 12, 34, 56, 789, TimeSpan.FromHours(-8))));
	}

	public async Task TaskWhenEach()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		Task[] tasks = [Task.Delay(1000), Task.Delay(2000), Task.Delay(3000)];
		await foreach (Task task in Task.WhenEach(tasks))
		{
			await task;
			Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds:f1}"); // 1.0, 2.0, 3.0
		}
	}

	public void AlternateLookups()
	{
		Dictionary<string, int> dictionary = new(new ReadOnlySpanCharEqualityComparer())
		{
			{ "one", 1 },
			{ "two", 2 },
			{ "three", 3 },
		};

		ReadOnlySpan<char> input = "one,two,three,four";
		var alternateDictionary = dictionary.GetAlternateLookup<ReadOnlySpan<char>>();

		foreach (var range in input.Split(','))
		{
			ReadOnlySpan<char> token = input[range];
			if (alternateDictionary.TryGetValue(token, out int value))
				Console.WriteLine($"{token.ToString()} = {value}");
			else
				Console.WriteLine($"{token.ToString()} not found");
		}

		// one = 1
		// two = 2
		// three = 3
		// four not found

		// Example: https://github.com/tryAGI/Tiktoken/pull/62
		// Blog: https://faithlife.codes/blog/2024/07/alternate-lookups-in-dotnet-9/
	}

	public sealed class ReadOnlySpanCharEqualityComparer :
		IEqualityComparer<string>,
		IAlternateEqualityComparer<ReadOnlySpan<char>, string>
	{
		// check if an alternate key is equal to a "real" key
		public bool Equals(ReadOnlySpan<char> x, string y) =>
			x.SequenceEqual(y);

		// get the hash code of an alternate key, which must match the hash code for the equivalent real key
		public int GetHashCode(ReadOnlySpan<char> span)
		{
			// djb2 is a simple hash function: http://www.cse.yorku.ca/~oz/hash.html
			uint hash = 5381;
			foreach (var ch in span)
				hash = hash * 33u + ch;
			return (int) hash;
		}

		// create a "real" key from an alternate key
		public string Create(ReadOnlySpan<char> alternate) =>
			alternate.ToString();

		// methods from IEqualityComparer<string> so we can provide identical behavior
		public bool Equals(string? x, string? y) =>
			string.Equals(x, y);

		public int GetHashCode([DisallowNull] string obj) =>
			obj is null ? 0 : GetHashCode(obj.AsSpan());
	}
}

