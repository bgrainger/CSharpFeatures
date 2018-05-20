using System;
using System.Runtime.InteropServices;

namespace CSharp73
{
	/// <summary>
	/// https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.3/pattern-based-fixed.md
	/// </summary>
	unsafe class PatternBasedFixed
	{
		public void CSharp7()
		{
			// take address of array
			var a = new[] { 1, 2, 3 };
			fixed (int* p = a)
			{
				Console.WriteLine(p[1]); // 2
			}

			// compiler has special support for System.String: implicit conversion to char* pointer
			var s = "string";
			fixed (char* p = s)
			{
				Console.WriteLine(p[1]); // 't'
			}
		}

		public void CSharp73()
		{
			// for other types, had to use helper methods
			var span = new Span<int>(new[] { 1, 2, 3 });
			// used to be: fixed (int* p = &span.DangerousGetPinnableReference()) // returned ref T
			fixed (int* p = &MemoryMarshal.GetReference(span)) // returns ref T
			{
				Console.WriteLine(p[1]); // 2
			}

			// implement for your own types, similar to the "duck typing" for 'foreach', 'await', etc
			// if you get it wrong: warning CS0280: 'Fixable' does not implement the 'fixed' pattern. 'Fixable.GetPinnableReference()' has the wrong signature.
			var fixable = new Fixable();
			fixed (int* p = fixable)
			{
				Console.WriteLine(p[1]); // 2
			}

			// implement for someone else's type (assuming its internals are sufficiently exposed)
			var fixable2 = new Fixable2();
			fixed (int* p = fixable2)
			{
				Console.WriteLine(p[1]); // 2
			}
		}
	}

	public class Fixable
	{
		readonly int[] _array = { 1, 2, 3, 4, 5 };

		public ref int GetPinnableReference() => ref _array[0];
	}

	public class Fixable2
	{
		public readonly int[] _array = { 1, 2, 3, 4, 5 };
	}

	public static class ExtensionMethods
	{
		public static ref int GetPinnableReference(this Fixable2 fixable2) => ref fixable2._array[0];
	}
}
