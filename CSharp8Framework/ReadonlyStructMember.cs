using System;

namespace CSharp8Framework
{
	public class ReadonlyStructMember
	{
	}

	// Not a "readonly struct", but has "readonly" members
	public struct Point
	{
		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}

		public int X { get; }
		public int Y { get; }

		// regular member
		public double Distance => Math.Sqrt(X * X + Y * Y);

		// method or property can now be marked "readonly"
		public readonly double DistanceReadonly => Math.Sqrt(X * X + Y * Y);

		// an override can be "readonly"
		public readonly override string ToString() => $"({X}, {Y})";

		// CS8656: Call to non-readonly member 'Point.Distance.get' from a 'readonly' member results in an implicit copy of 'this'.
		public readonly string Description => $"{ToString()} is {Distance} from the origin";
	}
}
