using System;
using System.Collections.Generic;

namespace CSharp73
{
	class UnmanagedConstraint
	{
		public static unsafe int Hash<T>(in T value) where T : unmanaged
		{
			fixed (T* ptr = &value)
			{
				uint hash = 5381;
				byte* p = (byte*) ptr;
				for (int i = 0; i < sizeof(T); i++)
					hash = hash * 33 + p[i];
				return (int) hash;
			}
		}

		public static unsafe int Hash<T>(T* ptr) where T : unmanaged
		{
			uint hash = 5381;
			byte* p = (byte*) ptr;
			for (int i = 0; i < sizeof(T); i++)
				hash = hash * 33 + p[i];
			return (int) hash;
		}

		public static void Example()
		{
			Console.WriteLine(Hash(42));
			Console.WriteLine(Hash(new Point { X = 3, Y = 7 }));
		}
	}

	struct Point
	{
		public int X { get; set; }
		public int Y { get; set; }
	}
}
