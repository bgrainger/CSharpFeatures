using System;
using System.Diagnostics;

namespace CSharp72
{
	class Program
	{
		static int Main(string[] args)
		{
			new Examples();

			var rr = new RefReadonly();
			rr.CallMethodOnReadonlyField();

			var a = new Point3D(1, 2, 3);
			var b = new Point3D(4, 5, 6);
			var ra = new ReadOnlyPoint3D(1, 2, 3);
			var rb = new ReadOnlyPoint3D(4, 5, 6);

			var c = new RefReadonly();
			var sw = Stopwatch.StartNew();
			double dist = 0;
			for (int i = 0; i < 100000000; i++)
				dist += c.SlowComputeDistance(a, b);
			sw.Stop();
			Console.WriteLine("{0} {1}", dist, sw.ElapsedMilliseconds);

			sw = Stopwatch.StartNew();
			dist = 0;
			for (int i = 0; i < 100000000; i++)
				dist += c.FastButUnsafeComputeDistance(ref a, ref b);
			sw.Stop();
			Console.WriteLine("{0} {1}", dist, sw.ElapsedMilliseconds);

			sw = Stopwatch.StartNew();
			dist = 0;
			for (int i = 0; i < 100000000; i++)
				dist += c.SafeButWorseComputeDistance(in a, in b);
			sw.Stop();
			Console.WriteLine("{0} {1}", dist, sw.ElapsedMilliseconds);

			sw = Stopwatch.StartNew();
			dist = 0;
			for (int i = 0; i < 100000000; i++)
				dist += c.FastAndSafeComputeDistance(in ra, in rb);
			sw.Stop();
			Console.WriteLine("{0} {1}", dist, sw.ElapsedMilliseconds);

			return 0;
		}
	}
}
