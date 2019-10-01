using System;

namespace CSharp8Nuget
{
	// Requires System.Memory for ReadOnlySpan<T>
	public class StackallocExpressions
	{
		public int IndexOfEven(ReadOnlySpan<byte> data) =>
			data.IndexOfAny(stackalloc byte[] { 2, 4, 6, 8 });
	}
}
