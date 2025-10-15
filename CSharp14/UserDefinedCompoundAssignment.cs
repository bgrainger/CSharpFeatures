using System.Text;

namespace CSharp14;

public sealed class Length(double meters)
{
	public double Meters { get; private set; } = meters;
	public override string ToString() => $"{Meters} m";

	// C# 13 overloaded operators: addition / subtraction
	public static Length operator +(Length a, Length b) => new(a.Meters + b.Meters);
	public static Length operator -(Length a, Length b) => new(a.Meters - b.Meters);

	// C# 13 overloaded operators: multiply / divide by scalar
	public static Length operator *(Length a, double factor) => new(a.Meters * factor);
	public static Length operator *(double factor, Length a) => new(a.Meters * factor);
	public static Length operator /(Length a, double divisor) => new(a.Meters / divisor);

	// C# 14 user-defined compound assignment
	public void operator +=(Length other) => Meters += other.Meters;
	public void operator *=(double factor) => Meters *= factor;

	// C# 14 user-defined increment/decrement
	public void operator ++() => Meters++;
	public static Length operator ++(Length a) => new(a.Meters + 1);
}

internal class UserDefinedCompoundAssignment
{
	public void CompoundAssignment()
	{
		var length = new Length(5);

		// calls user-defined operator +=
		length += new Length(3);

		// compiler lowers to `length = length - new Length(2);`; length is now a new object
		length -= new Length(2);

		// calls user-defined operator *=
		length *= 2;

		// compiler lowers to `length = length / 3;`
		length /= 3;

		/* 
		 * Motivation: benefit for types that are expensive to copy:

		var sb = new StringBuilder();
		sb += "Hello, ";
		sb += "World!";

		// it would be very bad to create a copy of the whole StringBuilder each time
		sb = sb + "Hello, ";

		In practice: BigInteger, Tensor, etc.
		*/
	}

	public void IncrementDecrement()
	{
		var length = new Length(5);

		// calls `void operator ++()`
		length++;
		++length;
		var newValue = ++length;

		// var temp = length; length = Length.operator ++(temp); oldValue = temp;
		var oldValue = length++;

		// compiler can't simulate these:
		// length--;
		// --length;
		// var newValue = --length;
		// var oldValue = length--;
	}
}
