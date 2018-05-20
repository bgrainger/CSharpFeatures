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

		public unsafe void AccessArray()
		{
			// can access field directly
			var value2 = m_fs.FixedField;

			// must pin the (movable) struct before accessing an array value by index
			fixed (FixedStruct* p = &m_fs)
			{
				var value = p->FixedArray[5];
			}
		}
	}
}
