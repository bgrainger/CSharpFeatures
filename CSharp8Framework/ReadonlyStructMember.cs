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
			m_label = null;
			Label3 = null;
		}

		public int X { get; set; }
		public int Y { get; set; }

		// regular member
		public double Distance => Math.Sqrt(X * X + Y * Y);

		// method or property can now be marked "readonly"
		public readonly double DistanceReadonly => Math.Sqrt(X * X + Y * Y);

		// an override can be "readonly"
		public readonly override string ToString() => $"({X}, {Y})";

		// CS8656: Call to non-readonly member 'Point.Distance.get' from a 'readonly' member results in an implicit copy of 'this'.
		public readonly string Description => $"{ToString()} is {Distance} from the origin";

		// You can--and should--apply readonly to non-auto-implemented getters, or just replace them with auto properties.
		public string Label
		{
			readonly get => m_label;
			set => m_label = value;
		}
		string m_label;

		public string Label2
		{
			get => m_label;
			readonly set => Console.WriteLine($"You tried to set it to {value}");
		}

		// You can do this, but it's redundant; the compiler applies the readonly attribute automatically.
		public string Label3 { readonly get; set; }
	}
}
