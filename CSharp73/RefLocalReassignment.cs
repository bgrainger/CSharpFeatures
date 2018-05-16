namespace CSharp73
{
	class RefLocalReassignment
	{
		public ref Point3D GetBest(ref Point3D a, ref Point3D b, ref Point3D c)
		{
			ref Point3D best = ref a;
			if (IsBetter(ref b, ref best))
				best = ref b; // error CS1073: Unexpected token 'ref'
			if (IsBetter(ref c, ref best))
				best = ref c;
			return ref best;
		}

		private static bool IsBetter(ref Point3D a, ref Point3D b) => false;
	}

	class RefLocalReassignmentUsingIn
	{
		public ref readonly Point3D GetBest(in Point3D a, in Point3D b, in Point3D c)
		{
			ref readonly Point3D best = ref a;
			if (IsBetter(b, best))
				best = ref b;
			if (IsBetter(c, best))
				best = ref c;
			return ref best;
		}

		private static bool IsBetter(in Point3D a, in Point3D b) => false;
	}

	public readonly struct Point3D
	{
		public Point3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public double X { get; }
		public double Y { get; }
		public double Z { get; }
	}
}
