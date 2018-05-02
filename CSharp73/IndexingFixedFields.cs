namespace CSharp73
{
	unsafe struct FixedStruct
	{
		public int FixedField;
		public fixed int FixedArray[10];
	}

	/// <summary>
	/// https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.3/indexing-movable-fixed-fields.md
	/// </summary>
	unsafe class IndexingFixedFields
	{
		static FixedStruct fs;

		public void CSharp7()
		{
			// can access field directly
			var value2 = fs.FixedField;

			// must pin the (movable) struct before accessing an array value by index
			fixed (FixedStruct* p = &fs)
			{
				var value = p->FixedArray[5];
			}
		}

		public void CSharp73()
		{
			// direct access without pinning
			var value = fs.FixedArray[5];
		}
	}
}
