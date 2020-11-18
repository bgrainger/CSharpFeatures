using System;
using System.Collections.Generic;
using System.IO;

namespace CSharp9Framework
{
	class ConditionalExpressions
	{
		// C# 8 needed:
		// useFile ? (Stream) File.OpenRead(@"C:\Temp\test.txt") : new MemoryStream();
		public Stream OpenData(bool useFile) =>
			useFile ? File.OpenRead(@"C:\Temp\test.txt") : new MemoryStream();

		public void NullCoalescing(List<int> list)
		{
			// error CS0019: Operator '??' cannot be applied to operands of type 'List<int>' and 'int[]'
			// IEnumerable<int> seq2 = list ?? Array.Empty<int>();

			IEnumerable<int> seq1 = list is null ? list : Array.Empty<int>();

			IEnumerable<int> seq2 = (IEnumerable<int>) list ?? Array.Empty<int>();
		}
	}
}
