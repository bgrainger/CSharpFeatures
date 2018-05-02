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
			var a = new[] { 123 };
			fixed (int* p = a)
			{
				Console.WriteLine(p[0]); // 123
			}

			// compiler has special support for System.String: implicit conversion to char* pointer
			var s = "string";
			fixed (char* p = s)
			{
				Console.WriteLine(p[0]); // 's'
			}
		}

		public void CSharp73()
		{
			// for other types, had to use helper methods
			var span = new Span<int>(new[] { 1, 2, 3 });
			// used to be: fixed (int* p = &span.DangerousGetPinnableReference())
			fixed (int* p = &MemoryMarshal.GetReference(span))
			// coming soon: fixed (int* p = span)
			{
				Console.WriteLine(p[0]); // 1
			}

			// implement for your own types
			var fixable = new Fixable();
			fixed (int* p = fixable)
			{
				Console.WriteLine(p[0]); // 1
			}
		}
	}

	public class Fixable
	{
		readonly int[] _array = { 1, 2, 3, 4, 5 };

		public ref int GetPinnableReference() => ref _array[0];
	}
}
