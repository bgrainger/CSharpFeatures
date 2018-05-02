namespace CSharp73
{
	/// <summary>
	/// https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.3/stackalloc-array-initializers.md
	/// </summary>
	unsafe class StackallocArrayInitializers
	{
		public void CSharp7()
		{
			// allocate the array, then set its elements
			var x = stackalloc int[3];
			x[0] = 1;
			x[1] = 2;
			x[2] = 3;

			// no verification of array length
			x[3] = 4;
			x[4] = 5;
		}

		public void CSharp73()
		{
			// analogous to 'new int[3] { 1, 2, 3 }'
			var x = stackalloc int[3] { 1, 2, 3 };

			// can't write past the end of the array
			// var y = stackalloc int[3] { 1, 2, 3, 4, 5 };
			// error CS0847: An array initializer of length '3' is expected
		}

		public void InferredLength()
		{
			// analogous to 'new int[] { 1, 2, 3 }'
			var x = stackalloc int[] { 1, 2, 3 };
		}

		public void InferredType()
		{
			// analogous to 'new[] { 1, 2, 3 }'
			var x = stackalloc[] { 1, 2, 3 };
		}
	}
}
