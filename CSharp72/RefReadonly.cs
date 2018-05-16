using System;

namespace CSharp72
{
	public class RefReadonly
	{
		readonly Point3D m_readonlyStructField;
		readonly ReadOnlyPoint3D m_readonlyReadOnlyStructField;

		public void CallMethodOnReadonlyField()
		{
			var d1 = m_readonlyStructField.DistanceFromOrigin();

			// https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/
			var tmp = m_readonlyStructField;
			var d2 = tmp.DistanceFromOrigin();

			var d3 = m_readonlyReadOnlyStructField.DistanceFromOrigin();
		}

		public double SlowComputeDistance(Point3D point1, Point3D point2)
		{
			double xDifference = point1.X - point2.X;
			double yDifference = point1.Y - point2.Y;
			double zDifference = point1.Z - point2.Z;

			return Math.Sqrt(xDifference * xDifference + yDifference * yDifference + zDifference * zDifference);
		}

		public double FastButUnsafeComputeDistance(ref Point3D point1, ref Point3D point2)
		{
			double xDifference = point1.X - point2.X;
			double yDifference = point1.Y - point2.Y;
			double zDifference = point1.Z - point2.Z;

			return Math.Sqrt(xDifference * xDifference + yDifference * yDifference + zDifference * zDifference);
		}

		public double SafeButWorseComputeDistance(in Point3D point1, in Point3D point2)
		// public double SafeButWorseComputeDistance([IsReadOnly] ref Point3D point1, [IsReadOnly] ref Point3D point2)
		{
			// "After adding support for in parameters and ref readonly returns the problem of defensive copying will get worse since readonly variables will become more common."
			// https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.2/readonly-ref.md

			double xDifference = point1.X - point2.X;
			double yDifference = point1.Y - point2.Y;
			double zDifference = point1.Z - point2.Z;

			return Math.Sqrt(xDifference * xDifference + yDifference * yDifference + zDifference * zDifference);
		}

		public double FastAndSafeComputeDistance(in ReadOnlyPoint3D point1, in ReadOnlyPoint3D point2)
		// public double FastAndSafeComputeDistance([IsReadOnly] ref ReadOnlyPoint3D point1, [IsReadOnly] ref ReadOnlyPoint3D point2)
		{
			double xDifference = point1.X - point2.X;
			double yDifference = point1.Y - point2.Y;
			double zDifference = point1.Z - point2.Z;

			return Math.Sqrt(xDifference * xDifference + yDifference * yDifference + zDifference * zDifference);
		}

		// can overload ref|in|out vs nothing
		// can't overload ref vs in vs out

		public void RefReadonlyReturn()
		{
			// OK, changes our copy
			var origin = Point3D.Origin;
			origin.X = 1;

			// illegal: can't mutate a readonly ref
			ref readonly var readonlyOrigin = ref Point3D.Origin;
			// readonlyOrigin.X = 1;
		}
	}

	public struct Point3D
	{
		public Point3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
			padding1 = 1;
			padding2 = 2;
			padding3 = 3;
			padding4 = 4;
		}

		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }

		public double DistanceFromOrigin() => Math.Sqrt(X * X + Y * Y + Z * Z);

		private static readonly Point3D origin;
		public static ref readonly Point3D Origin => ref origin;

		readonly long padding1;
		readonly long padding2;
		readonly long padding3;
		readonly long padding4;
	}

	public readonly struct ReadOnlyPoint3D
	{
		public ReadOnlyPoint3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
			padding1 = 1;
			padding2 = 2;
			padding3 = 3;
			padding4 = 4;
		}

		public double X { get; }
		public double Y { get; }
		public double Z { get; }

		public double DistanceFromOrigin() => Math.Sqrt(X * X + Y * Y + Z * Z);

		private static readonly ReadOnlyPoint3D origin = new ReadOnlyPoint3D();
		public static ref readonly ReadOnlyPoint3D Origin => ref origin;

		readonly long padding1;
		readonly long padding2;
		readonly long padding3;
		readonly long padding4;
	}
}
