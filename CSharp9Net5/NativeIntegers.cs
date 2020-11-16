using System;

namespace CSharp9Net5
{
	public class NativeIntegers
	{
		public void RuntimeConstants()
		{
			const int constantInt = int.MaxValue;

			// error CS0133: The expression being assigned to 'constantNInt' must be constant
			// const nint constantNInt = nint.MaxValue;
			nint maxValue = nint.MaxValue;
			Console.WriteLine(maxValue);

			// can assign constants up to int.MaxValue
			const nint constantNInt = int.MaxValue;

			// constant folding performed for many arithmetic expressions
			const nint another = (constantNInt >> 4) / 3 % 65536;

			// also an unsigned type
			const nuint canBeUnsigned = 3;
		}

		public nint ArrayIndex(nint size, nint index)
		{
			// can index into arrays
			var array = new nint[size];
			return array[index];
		}
	}
}
