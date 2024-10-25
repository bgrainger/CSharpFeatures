using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp13;

internal class Params
{
	public void ParamsWithArray()
	{
		// since C# 1.0
		// equivalent to ParamsWithArray(new[] { 1, 2, 3, 4, 5 });
		ParamsWithArray(1, 2, 3, 4, 5);

		static void ParamsWithArray(params int[] values)
		{
			Console.WriteLine(string.Join(", ", values));
		}
	}

	public void ParamsWithList()
	{
		// new List<int>(); CollectionsMarshal.AsSpan; fill entries
		ParamsWithList(1, 2, 3, 4, 5);

		static void ParamsWithList(params List<int> values)
		{
			Console.WriteLine(string.Join(", ", values));
		}
	}

	public void ParamsWithIList()
	{
		// same as List<int> above
		ParamsWithIList(1, 2, 3, 4, 5);

		static void ParamsWithIList(params IList<int> values)
		{
			Console.WriteLine(string.Join(", ", values));
		}
	}

	public void ParamsWithIEnumerable()
	{
		// uses compiler-synthesized '<>z__ReadOnlyArray<int>' type
		ParamsWithIEnumerable(1, 2, 3, 4, 5);

		static void ParamsWithIEnumerable(params IEnumerable<int> values)
		{
			Console.WriteLine(string.Join(", ", values));
		}
	}

	public void ParamsWithIReadOnlyList()
	{
		// same as IEnumerable<int> above
		ParamsWithIReadOnlyList(1, 2, 3, 4, 5);

		static void ParamsWithIReadOnlyList(params IReadOnlyList<int> values)
		{
			Console.WriteLine(string.Join(", ", values));
		}
	}

	public void ParamsWithSpan()
	{
		// uses InlineArray (from C# 12)
		ParamsWithSpan(1, 2, 3, 4, 5);

		static void ParamsWithSpan(params Span<int> values)
		{
			Console.WriteLine(string.Join(", ", values.ToArray()));
		}
	}

	public void ParamsWithReadOnlySpan()
	{
		// uses hard-coded data in the assembly
		ParamsWithSpan(1, 2, 3, 4, 5);

		static void ParamsWithSpan(params ReadOnlySpan<int> values)
		{
			Console.WriteLine(string.Join(", ", values.ToArray()));
		}
	}

	public void ParamsOverload()
	{
		// picks the best overload of all the 'params' methods
		ParamsOverload(1, 2, 3, 4, 5);
		
		// was this whole feature even necessary when you can just use collection expressions?
		ParamsOverload([1, 2, 3, 4, 5]);

		// Recommendations:
		// - add "params ReadOnlySpan<T>" overloads (or params IReadOnlyList<T>) to methods that take "params T[]"
		// - consider removing "params" from the "T[]" method
	}

	private static void ParamsOverload(/* [ParamArray] */ params int[] values)
	{
		Console.WriteLine(string.Join(", ", values));
	}

	private static void ParamsOverload(/* [ParamCollection] */ params IEnumerable<int> values)
	{
		Console.WriteLine(string.Join(", ", values.ToArray()));
	}

	private static void ParamsOverload(/* [ParamCollection] */ params ReadOnlySpan<int> values)
	{
		Console.WriteLine(string.Join(", ", values.ToArray()));
	}
}
