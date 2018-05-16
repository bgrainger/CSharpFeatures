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
	class IndexingFixedFields
	{
		FixedStruct m_fs;

		public unsafe void CSharp7()
		{
			// can access field directly
			var value2 = m_fs.FixedField;

			// must pin the (movable) struct before accessing an array value by index
			fixed (FixedStruct* p = &m_fs)
			{
				var value = p->FixedArray[5];
			}
		}

		public unsafe void CSharp73()
		{
			// direct access without pinning
			// error CS1666: You cannot use fixed size buffers contained in unfixed expressions. Try using the fixed statement.
			var value = m_fs.FixedArray[5];

			// NOTE: method still needs to be 'unsafe'
			// error CS0214: Pointers and fixed size buffers may only be used in an unsafe context
			var outOfBoundsRead = m_fs.FixedArray[15];
		}
	}
}
