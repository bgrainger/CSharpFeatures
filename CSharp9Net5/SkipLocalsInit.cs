using System;
using System.Runtime.CompilerServices;

namespace CSharp9Net5
{
	public class SkipLocalsInit
	{
		[SkipLocalsInit]
		public unsafe int ReadUninitializedData(int size)
		{
			// not zeroed!
			Span<int> data = stackalloc int[size];

			// returns random data!
			return data[3];
		}
	}
}
