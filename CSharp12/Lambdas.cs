namespace CSharp12;

internal class Lambdas
{
	public void DefaultValue()
	{
		var incrementBy = (int value, int increment = 1) => value + increment;

		// like all C# default values, the default value from metadata is hard-coded in at the call site
		Console.WriteLine(incrementBy(2)); // 3
		Console.WriteLine(incrementBy(3, 2)); // 5
	}

	public void Params()
	{
		var sum = (params int[] values) => values.Sum();
		Console.WriteLine(sum(1, 2, 3)); // 6
	}

	public void ConversionFromMethodGroup()
	{
		// can convert a method group to a delegate type; in this case the overload that takes (value, index)
		Enumerable.Range(0, 10).Select(IncrementBy); // 0+0, 1+1, 2+2, 3+3, ...

		// can assign to an inferred delegate type and invoke it
		var sum = Sum;
		sum(1, 2, 3); // 6

		// can convert to a Func type, but that's not the natural type; instead it's a compiler-synthesized delegate type
		var incrementBy = IncrementBy;
		Func<int, int, int> incrementByFunc = IncrementBy;
		IncrementDelegate incrementByDelegate = IncrementBy;

		// error CS0411: The type arguments for method 'Enumerable.Select<TSource, TResult>(IEnumerable<TSource>, Func<TSource, int, TResult>)' cannot be inferred from the usage. Try specifying the type arguments explicitly.
		// Enumerable.Range(0, 10).Select(incrementBy);
		Enumerable.Range(0, 10).Select(incrementByFunc);
		// Enumerable.Range(0, 10).Select(incrementByDelegate);

		static int IncrementBy(int value, int increment = 1) => value + increment;
		static int Sum(params int[] values) => values.Sum();
	}

	public delegate int IncrementDelegate(int value, int increment = 1);

	public void UseDefaultValueInAnonymousLambda()
	{
		var numbers = Enumerable.Range(0, 100);

		// warning CS9099: Parameter 2 has default value '1' in lambda but '<missing>' in the target delegate type.
		numbers.Select((int x, int n = 1) =>  x + n);
	}
}
