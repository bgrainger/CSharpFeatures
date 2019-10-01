namespace CSharp8Framework
{
	public class UnmanagedConstructedTypes
	{
		public readonly struct Point<T>
		{
			public Point(T x, T y)
			{
				X = x;
				Y = y;
			}

			public T X { get; }
			public T Y { get; }
		}

		public static unsafe void Method<T>(T* ptr) where T : unmanaged
		{
		}

		public unsafe void CallMethod()
		{
			Point<int> intPoint = new Point<int>(1, 2);
			Method(&intPoint);

			Point<string> stringPoint = new Point<string>("1", "2");
			// error CS0208: Cannot take the address of, get the size of, or declare a pointer to a managed type
			// Method(&stringPoint);
		}
	}
}
