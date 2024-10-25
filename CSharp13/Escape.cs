namespace CSharp13;

internal class Escape
{
	public void EscapeLiteral()
	{
		Console.WriteLine("\x1b"); // U+001B
		Console.WriteLine("\u001b"); // U+001B

		// \x is not limited to two hex digits; it's greedy
		Console.WriteLine("\x1ban unexpected character"); // U+01BA

		Console.WriteLine("\ean expected character"); // U+001B
	}
}
