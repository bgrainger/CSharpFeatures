namespace CSharp14;

internal class SimpleLambdaParametersWithModifiers
{
	delegate bool TryParse<T>(string text, out T result);

	public void OutParameterWithoutType()
	{
		// Before C# 14: needed to specify parameter types when using modifiers
		TryParse<int> parse1 = (string text, out int result) => int.TryParse(text, out result);

		// C# 14: can omit type and let it be inferred
		TryParse<int> parse2 = (text, out result) => int.TryParse(text, out result);

		var success = parse2("42", out int value);

		// works for: ref, ref readonly, scoped, in, out
	}

	delegate void RefModifier(ref int value);

	public void RefParameter()
	{
		// Can use ref modifier without specifying type
		RefModifier refMod = (ref value) => value *= 2;

		int num = 10;
		refMod(ref num);
		Console.WriteLine($"After: {num}"); // 20
	}
}

