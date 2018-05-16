using System;

namespace CSharp73
{
	class UnmanagedConstraint
	{
		public static unsafe int Hash<T>(in T value) where T : unmanaged
		{
			// error CS0208: Cannot take the address of, get the size of, or declare a pointer to a managed type('T')
			fixed (T* ptr = &value)
			{
				// NOTE: This is the 'djb' hash function; see http://www.partow.net/programming/hashfunctions/#DJBHashFunction
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
			Console.WriteLine(Hash(42)); // 2087982959
			Console.WriteLine(Hash(new Point { X = 3, Y = 7 })); // 1081909071
			Console.WriteLine(Hash(0x0000_0007_0000_0003L)); // 1081909071 -- bytes have same layout in memory
		}
	}

	struct Point
	{
		public int X { get; set; }
		public int Y { get; set; }
	}
}
