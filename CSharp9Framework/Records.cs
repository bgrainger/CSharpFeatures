using System;
using System.Text;

namespace CSharp9Framework
{
	class Records
	{
		public record Point
		{
			public int X { get; init; }
			public int Y { get; init; }
		}

		public void ToString()
		{
			var p = new Point { X = 1, Y = 2 };

			// Point { X = 1, Y = 2 }
			Console.WriteLine(p.ToString());
		}

		public record PointB
		{
			public int X { get; init; }
			public int Y { get; init; }

			public string PrintMembers()
			{
				var sb = new StringBuilder();
				PrintMembers(sb);
				return sb.ToString();
			}
		}

		public void PrintMembers()
		{
			var p = new PointB { X = 1, Y = 2 };

			// X = 1, Y = 2
			Console.WriteLine(p.PrintMembers());
		}

		public void Equality()
		{
			var p1 = new Point { X = 1, Y = 2 };
			var p2 = new Point { X = 2, Y = 1 };
			var p3 = new Point { X = 1, Y = 2 };

			// 809708355 -711425941 809708355
			Console.WriteLine($"{p1.GetHashCode()} {p2.GetHashCode()} {p3.GetHashCode()}");

			// False True
			Console.WriteLine($"{p1 == p2} {p1 == p3}");

			// False True
			Console.WriteLine($"{p1.Equals(p2)} {p1.Equals(p3)}");
		}

		public record Point3D : Point
		{
			public Point3D(int x, int y, int z) => (X, Y, Z) = (x, y, z);

			public int Z { get; }
		}

		public void DerivedEquality()
		{
			Point p1 = new Point { X = 3, Y = 2 };
			Point p2 = new Point3D(3, 2, 0);

			// False
			Console.WriteLine(p1.Equals(p2));
		}

		// "Positional" records

		public record PointC(int X, int Y);

		public void ConstructAndSet()
		{
			var p = new PointC(1, 2);
			var p2 = new PointC(0, 0) { X = 1, Y = 2 };

			var p3 = p with { X = 2 };

			// PointC { X = 2, Y = 2 }
			Console.WriteLine(p3);

			// need to force cloning
			var p4 = p with { };
		}

		public record PointD(int X = 0, int Y = 0);

		public void ConstructWithDefaultParameters()
		{
			var p = new PointD();
			var p2 = p with { X = 2 };
		}

		public Point WithOnExpressions(Point p, Point3D p3d) =>
			(p ?? p3d) with { X = 100 };

		public void Deconstruct()
		{
			var p = new PointC(1, 2);
			var (x, y) = p;
		}

		// Inheritance

		public record PointC3D(int X, int Y, int Z) : PointC(X, Y);

		public record PointD3D(int A, int B, int Z) : PointC(A, B);

		public void DedupedByName()
		{
			// PointC3D { X = 1, Y = 2, Z = 3 }
			var p1 = new PointC3D(1, 2, 3);
			Console.WriteLine(p1);

			// PointD3D { X = 2, Y = 3, A = 2, B = 3, Z = 4 }
			var p2 = new PointD3D(2, 3, 4);
			Console.WriteLine(p2);

			// deconstruction only returns the arguments to the constructor
			var (a, b, z) = p2;
		}

		// Base class protected constructors

		public record PointE3D : PointC
		{
			public int Z { get; }

			public PointE3D(int x, int y, int z) : base(x, y)
			{
				Z = z;
			}

			public PointE3D(PointC p3, int z)
				: base(p3)
			{
				Z = z;
			}
		}

		// Customise equality

		public record DistancePoint(int X, int Y)
		{
			public double DistanceFromOrigin => Math.Sqrt(X * X + Y * Y);

			public virtual bool Equals(DistancePoint other) => DistanceFromOrigin == other?.DistanceFromOrigin;
			public override int GetHashCode() => DistanceFromOrigin.GetHashCode();
		}

		public void CustomEquality()
		{
			var x = new DistancePoint(1, 0);
			var y = new DistancePoint(0, 1);

			Console.WriteLine(x.Equals(y)); // True
			Console.WriteLine(x == y); // True

			object objX = x;
			object objNull = null;

			Console.WriteLine(x.Equals(objX)); // True
			Console.WriteLine(x.Equals(objNull)); // False
		}
	}
}
