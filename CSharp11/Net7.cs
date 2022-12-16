using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace CSharp11;

public partial class Net7
{
	public void Examples()
	{
		var dictionary = new Dictionary<string, int>();
		var readOnlyDictionary = dictionary.AsReadOnly();

		IList<string> list = new List<string>();
		var readOnlyList = list.AsReadOnly();

		var orderedList = list.Order();
		var orderedList2 = list.Order(StringComparer.OrdinalIgnoreCase);

		var micros = DateTime.Now.Microsecond;

		using var stream = new MemoryStream();
		Span<byte> buffer = stackalloc byte[100];
		stream.ReadExactly(buffer);
		
		var bytesRead = stream.ReadAtLeast(buffer, minimumBytes: 10, throwOnEndOfStream: true);

		bytesRead = stream.ReadAtLeastAsync(new byte[100], minimumBytes: 10, throwOnEndOfStream: false).Result; // example only; never use .Result
	}

	const string regex = "^([Xx]-)?[A-Za-z]{2,3}$";
	
	[StringSyntax(StringSyntaxAttribute.Regex)]
	const string regex2 = "^([Xx]-)?[A-Za-z]{2,3}$";

	[GeneratedRegex(regex2, RegexOptions.CultureInvariant)]
	private static partial Regex LanguageRegex();

	public void RegexExample()
	{
		LanguageRegex().IsMatch("en"); // true
		LanguageRegex().IsMatch("English"); // false

		ReadOnlySpan<char> stackChar = stackalloc char[2] { 'e', 's' };
		LanguageRegex().Count(stackChar); // 1

		foreach (var match in LanguageRegex().EnumerateMatches(stackChar))
		{
			Console.WriteLine($"{match.Index} {match.Length}");
		}
	}
}
