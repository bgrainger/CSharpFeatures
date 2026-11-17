using System.Runtime.CompilerServices;

namespace CSharp15;

// C# 15: a union holds exactly one value from a closed set of "case types" (which don't need a common base type)
internal readonly union DocumentSource(FileInfo, Uri, Stream, string)
{
	// unions can have a body with methods and computed properties (but no instance fields or auto-properties)
	public string DisplayName => this switch
	{
		FileInfo file => file.Name,
		Uri uri => uri.Host,
		Stream => "(stream)",
		string => "(text)",
	};
}

internal class UnionTypes
{
	public void ImplicitConversion()
	{
		// implicit conversion from each case type
		DocumentSource source = new FileInfo("notes.txt");
		source = new Uri("https://example.com/notes.txt");
		source = "inline text";

		// and from types that convert to a case type
		source = new MemoryStream();

		// the contents are exposed as 'object? Value'
		Console.WriteLine(source.Value); // System.IO.MemoryStream

		// ToString isn't forwarded to the contents
		Console.WriteLine(source); // CSharp15.DocumentSource

		DocumentSource? nullableSource = "initialize nullable union";
	}

	private static async Task<string> LoadAsync(DocumentSource source) =>
		source switch
		{
			FileInfo file => await File.ReadAllTextAsync(file.FullName),
			Uri uri => await s_httpClient.GetStringAsync(uri),
			Stream stream => await new StreamReader(stream).ReadToEndAsync(),
			string text => text,
		};

	public async Task ExhaustiveSwitch()
	{
		// one method instead of four overloads; callers pass whatever they have
		Console.WriteLine(await LoadAsync(new FileInfo("notes.txt")));
		Console.WriteLine(await LoadAsync(new Uri("https://example.com/")));
		Console.WriteLine(await LoadAsync("inline text"));

		// add 'byte[]' to the union and every switch like the one in LoadAsync gets a warning:
		// warning CS8509: The switch expression does not handle all possible values of its input type (it is not exhaustive). For example, the pattern 'byte[]' is not covered.
	}

	public void BeforeCSharp15()
	{
		// overloads: one per source type, each with its own copy of any shared logic
		// Task<string> LoadAsync(FileInfo file);
		// Task<string> LoadAsync(Uri uri);
		// Task<string> LoadAsync(Stream stream);
		// Task<string> LoadAsync(string text);

		// or 'object': anything can be passed in
		object source = new Uri("https://example.com/");

		string kind = source switch
		{
			FileInfo => "file",
			Uri => "URL",
			Stream => "stream",
			string => "text",
			_ => throw new ArgumentException($"Unsupported source: {source}"), // found at run time, not compile time
		};

		// or a library such as OneOf: OneOf<FileInfo, Uri, Stream, string> with source.Match(file => ..., uri => ..., stream => ..., text => ...)
	}

	public void PatternsTestTheContents()
	{
		DocumentSource source = new Uri("https://example.com/notes.txt");

		// type patterns (and recursive patterns with a type) test the contents, i.e., 'source.Value is Uri { ... }'
		Console.WriteLine(source is Uri { Scheme: "https" }); // True

		// recursive patterns without a type use the union's own members (i.e., DocumentSource.DisplayName)
		Console.WriteLine(source is { DisplayName: "example.com" }); // True

		// 'var' captures the union itself, not the contents
		if (source is var captured)
			Console.WriteLine(captured.GetType()); // CSharp15.DocumentSource

		// error CS8121: An expression of type 'DocumentSource' cannot be handled by a pattern of type 'DocumentSource'.
		// Console.WriteLine(source is DocumentSource);
	}

	public void DefaultValue()
	{
		// a generated union is a struct, so 'default(DocumentSource)' has a null Value
		DocumentSource source = default;
		Console.WriteLine(source.Value is null); // True
		Console.WriteLine(source is null); // True (the null pattern tests the contents)
		Console.WriteLine(source is not null); // False
		Console.WriteLine(source is { }); // True (the struct itself is never null)
		if (source is var value)
			Console.WriteLine(value.Value is null); // True

		// the compiler assumes the contents are non-null when all case types are non-nullable,
		// so this compiles without warning but throws SwitchExpressionException
		// string kind = source switch { FileInfo => "file", Uri => "URL", Stream => "stream", string => "text" };

		// runtime error for `default(DocumentSource).DisplayName` if `null` not handled: System.Runtime.CompilerServices.SwitchExpressionException: Non-exhaustive switch expression failed to match its input.

		// prefer an explicit null arm over '_' so that you still get a warning when a new case type is added
		string kind = source switch
		{
			FileInfo => "file",
			Uri => "URL",
			Stream => "stream",
			string => "text",
			null => "(nothing)", // don't use _ => ...
		};
	}

	internal union MaybeFile(FileInfo?, Uri);

	// a nullable case type makes the contents "maybe null"
	public string NullableCaseTypes(MaybeFile value) => value switch
	{
		FileInfo file => file.Name,
		Uri uri => uri.Host,
		null => "(none)", // otherwise: warning CS8655: The switch expression does not handle some null inputs (it is not exhaustive). For example, the pattern 'null' is not covered.
	};

	internal union Result<T>(T, Exception);

	public void ResultOrError()
	{
		Console.WriteLine(Describe(ParseAge("42"))); // Age: 42
		Console.WriteLine(Describe(ParseAge("old"))); // Error: 'old' isn't a number

		static Result<int> ParseAge(string text) =>
			int.TryParse(text, out var age) ? age : new FormatException($"'{text}' isn't a number");

		static string Describe(Result<int> result) => result switch
		{
			int age => $"Age: {age}",
			Exception ex => $"Error: {ex.Message}",
		};

		// error CS0457: Ambiguous user defined conversions 'Result<Exception>.Result(Exception)' and 'Result<Exception>.Result(Exception)' when converting from 'Exception' to 'Result<Exception>'
		// Result<Exception> result = new Exception();
	}

	// Use case: replace Libronix.Utility.ThrowOnError

	/// <summary>
	/// Indicates whether a method should throw an exception if it fails.
	/// </summary>
	public enum ThrowOnError
	{
		/// <summary>
		/// The method should not throw an exception if it fails.
		/// </summary>
		False,

		/// <summary>
		/// The method should throw an exception if it fails.
		/// </summary>
		True,
	}

	public void WhatTheCompilerGenerates()
	{
		// 'internal union DocumentSource(FileInfo, Uri, Stream, string);' becomes:
		//
		// [Union]
		// internal struct DocumentSource : IUnion
		// {
		//     public DocumentSource(FileInfo value) => Value = value;
		//     public DocumentSource(Uri value) => Value = value;
		//     public DocumentSource(Stream value) => Value = value;
		//     public DocumentSource(string value) => Value = value;
		//     public object? Value { get; }
		// }
		//
		// 'source = new Uri(...)' becomes 'source = new DocumentSource(new Uri(...))'
		// 'source is FileInfo file' becomes 'source.Value is FileInfo file'

		// UnionAttribute and IUnion are new types in .NET 11's System.Runtime.CompilerServices
		IUnion union = new DocumentSource("inline text");
		Console.WriteLine(union.Value); // inline text
	}

	public void CustomUnionTypes()
	{
		// any type with [Union] that follows the "union pattern" gets union conversions, pattern matching, and exhaustiveness
		// (one-parameter public constructors define the case types; plus a public 'object? Value' property)
		// use this to adapt an existing type, make a class-based union, or control the storage

		// IntOrBool implements the optional non-boxing access pattern ('HasValue' and 'TryGetValue')
		IntOrBool value = 42;

		// compiler calls 'value.TryGetValue(out int i)' instead of 'value.Value is int i': no boxing
		var description = value switch
		{
			int i => $"int: {i}",
			bool b => $"bool: {b}",
		};

		// calls the HasValue property
		if (value is null)
			Console.WriteLine("is null");

		// class-based union: reference semantics; null pattern matches a null reference or a null Value
		ApiResult<string> result = new ApiResult<string>(new TimeoutException());
		description = result switch
		{
			string s => $"OK: {s}",
			Exception ex => $"Error: {ex.Message}",
			null => "null",
		};
	}

	[Union]
	internal readonly struct IntOrBool // [optional] : IUnion
	{
		public IntOrBool(int value) => (m_tag, m_int) = (Tag.Int, value);
		public IntOrBool(bool value) => (m_tag, m_bool) = (Tag.Bool, value);

		// required by the union pattern; boxes
		public object? Value => m_tag switch
		{
			Tag.Int => m_int,
			Tag.Bool => m_bool,
			_ => null,
		};

		// optional non-boxing access pattern
		public bool HasValue => m_tag != Tag.None;
		public bool TryGetValue(out int value) => (value = m_int, m_tag == Tag.Int).Item2;
		public bool TryGetValue(out bool value) => (value = m_bool, m_tag == Tag.Bool).Item2;

		private enum Tag : byte { None, Int, Bool }

		private readonly Tag m_tag;
		private readonly int m_int;
		private readonly bool m_bool;
	}

	[Union]
	internal sealed class ApiResult<T> : IUnion
	{
		public ApiResult(T value) => Value = value;
		public ApiResult(Exception value) => Value = value;

		public object? Value { get; }
	}

	// Advanced: IUnionMembers

	// Real-world example: LogosDesktop's StartupLayoutOptionSetting (LDLS4\StartupLayoutOptionSetting.cs)
	// is either a built-in startup option (an enum) or the name of a saved layout, never both

	public void StartupLayoutBefore()
	{
		var setting = StartupLayoutOptionSetting.Create("Sermon Prep");

		// AppModel.Initialization.cs: check one property, then unwrap the other
		if (setting.IsUsingNamedLayout)
			Console.WriteLine($"Load layout '{setting.NamedLayoutTitle}'");
		else
			Console.WriteLine($"Start with {setting.StartupLayoutOptionEnum!.Value}");

		Console.WriteLine(setting.GetStartProfile()); // layout/named
		Console.WriteLine(setting); // layout:Sermon Prep
	}

	public void StartupLayoutAfter()
	{
		StartupLayout layout = "Sermon Prep";

		// the compiler knows it's exactly one of these
		Console.WriteLine(layout switch
		{
			string title => $"Load layout '{title}'",
			StartupLayoutOption option => $"Start with {option}",
		});
		// Load layout 'Sermon Prep'

		Console.WriteLine(layout.GetStartProfile()); // layout/named
		Console.WriteLine(layout); // layout:Sermon Prep

		layout = StartupLayoutOption.LayoutBlank;
		Console.WriteLine(layout.GetStartProfile()); // layout/blank
		Console.WriteLine(layout); // custom:LayoutBlank
	}

	private static readonly HttpClient s_httpClient = new();
}

internal enum StartupLayoutOption
{
	Home,
	LayoutBlank,
	LayoutLastAny,
	LayoutLastLocal,
}

// Before: simplified from LogosDesktop
internal sealed class StartupLayoutOptionSetting
{
	public static StartupLayoutOptionSetting Create(StartupLayoutOption layoutOption) => new(layoutOption, null);

	public static StartupLayoutOptionSetting Create(string title) => new(null, title);

	public StartupLayoutOption? StartupLayoutOptionEnum { get; }

	public string? NamedLayoutTitle { get; }

	public bool IsUsingNamedLayout => NamedLayoutTitle != null;

	public override string ToString() =>
		IsUsingNamedLayout ? $"layout:{NamedLayoutTitle}" : $"custom:{StartupLayoutOptionEnum!.Value}";

	public string GetStartProfile()
	{
		if (IsUsingNamedLayout)
			return "layout/named";

		// the compiler can't tell that StartupLayoutOptionEnum has a value here, hence the '!'
		return StartupLayoutOptionEnum!.Value switch
		{
			StartupLayoutOption.Home => "home",
			StartupLayoutOption.LayoutBlank => "layout/blank",
			StartupLayoutOption.LayoutLastAny => "layout/last-any",
			StartupLayoutOption.LayoutLastLocal => "layout/last-local",
			_ => throw new ArgumentOutOfRangeException(),
		};
	}

	private StartupLayoutOptionSetting(StartupLayoutOption? startupLayoutOption, string? namedLayoutTitle)
	{
		// the "exactly one" rule is only enforced at runtime
		if (!(startupLayoutOption.HasValue ^ (namedLayoutTitle != null)))
			throw new ArgumentException("exactly one of startupLayoutOption or namedLayoutTitle must be null, and exactly one must be non null");
		StartupLayoutOptionEnum = startupLayoutOption;
		NamedLayoutTitle = namedLayoutTitle;
	}
}

// After: union of the two types
internal union StartupLayout(StartupLayoutOption, string)
{
	public override string ToString() => this switch
	{
		string title => $"layout:{title}",
		StartupLayoutOption option => $"custom:{option}",
		null => "",
	};

	public string GetStartProfile() => this switch
	{
		string => "layout/named",
		StartupLayoutOption option => option switch
		{
			StartupLayoutOption.Home => "home",
			StartupLayoutOption.LayoutBlank => "layout/blank",
			StartupLayoutOption.LayoutLastAny => "layout/last-any",
			StartupLayoutOption.LayoutLastLocal => "layout/last-local",

			// still needed: listing every named value doesn't make an enum switch exhaustive
			// warning CS8524: The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
			_ => throw new ArgumentOutOfRangeException(nameof(option)),
		},
		null => "home",
	};
}
