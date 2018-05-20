namespace CSharp73
{
	/// <summary>
	/// https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.3/stackalloc-array-initializers.md
	/// </summary>
	unsafe class StackallocArrayInitializers
	{
		public void Example()
		{
			// allocate the array, then set its elements
			int* x = stackalloc int[3];
			x[0] = 1;
			x[1] = 2;
			x[2] = 3;

			// no verification of array length
			x[3] = 4;
			x[4] = 5;
		}
	}
}
