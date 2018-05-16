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
		FixedStruct m_fs;

		public void CSharp7()
		{
			// can access field directly
			var value2 = m_fs.FixedField;

			// must pin the (movable) struct before accessing an array value by index
			fixed (FixedStruct* p = &m_fs)
			{
				var value = p->FixedArray[5];
			}
		}

		public void CSharp73()
		{
			// direct access without pinning
			// error CS1666: You cannot use fixed size buffers contained in unfixed expressions. Try using the fixed statement.
			var value = m_fs.FixedArray[5];
		}
	}
}
