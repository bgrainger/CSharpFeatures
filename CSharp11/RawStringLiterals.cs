namespace CSharp11;

public class RawStringLiterals
{
	public void StringLiterals()
	{
		// quoted string literals
		var quoted = "Column 1\tColumn 2\n\tSecond Line";
/* Output:
Column 1	Column 2
	Second Line
*/

		var verbatim = @"""Path\to\file.txt""";
/* Output:
"Path\to\file.txt"
*/

		var sql = @"SELECT *
			FROM Customers
			WHERE id = 1;";
/* Output:
SELECT *
			FROM Customers
			WHERE ID = 1;
*/

		var xml = @"<doc>
	<element attribute='at least we can use single quotes' />
</doc>";

		var json = @"{
	""quotes"": ""must be doubled"",
	""which"": ""is annoying""
}";

		var raw = """This is a "raw" string with embedded "" double quotes.""";

		var keepAddingQuotes = """"C# 11 code: """This is a "raw" string with embedded "" double quotes."""."""";

		// equivalent to: \"\"\"This is a \"raw\" string with embedded \"\" double quotes.\"\"\"
		var multiLine = """"
			"""This is a "raw" string with embedded "" double quotes."""
			"""";

		var multiLineJson = """
			{
				"quotes": "are not doubled"
			}
			""";
/* Output:
{
	"quotes": "are not doubled"
}
*/

		var multiLineSql = """
			SELECT *
			FROM Customers
			WHERE id = 1;
			""";
/* Output:
SELECT *
FROM Customers
WHERE ID = 1;
*/
	}

	public void Interpolation()
	{
		const string interpolation = "interpolation";
		const string placeholders = "placeholders";

		// formatted strings
		// interpolation		via		placeholders
		Console.WriteLine("{0} \"via\" {1}", interpolation, placeholders);

		// interpolated strings
		Console.WriteLine($"{interpolation} \"via\" {placeholders}");

		// interpolated verbatim strings
		Console.WriteLine($@"{interpolation} ""via"" {placeholders}");

		// interpolated raw strings
		Console.WriteLine($"""{interpolation} "via" {placeholders}""");

		// the number of parentheses match the number of dollar signs
		Console.WriteLine($$"""
			{
				"{{interpolation}}": "{{placeholders}}"
			}
			""");

		// increase the number of dollars and quotes to be greater than what's used in the string
		Console.WriteLine($$$""""
			Console.WriteLine($$"""
			{
				"{{interpolation}}": "{{placeholders}}"
			}
			{{{interpolation}}} "via" {{{placeholders}}}
			"""");

		// interpolation expressions can now span new lines
		Console.WriteLine($"{interpolation switch
		{
			"interpolation" => "placeholders",
			_ => "unknown"
		}} \"via\" {placeholders switch
		{
			"placeholders" => "interpolation",
			_ => "unknown"
		}}");
	}
}
