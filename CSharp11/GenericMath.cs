using System.Numerics;

namespace CSharp11;

public record struct IntVector(int X, int Y)
{
	public static IntVector operator+(IntVector left, IntVector right) =>
		new(left.X + right.X, left.Y + right.Y);

	public static IntVector operator*(IntVector vec, int scale) =>
		new(vec.X * scale, vec.Y * scale);
}

public record struct Vector<T>(T X, T Y) :
	IAdditionOperators<Vector<T>, Vector<T>, Vector<T>>,
	ISubtractionOperators<Vector<T>, Vector<T>, Vector<T>>,
	IMultiplyOperators<Vector<T>, T, Vector<T>>
	where T : IAdditionOperators<T, T, T>,
		ISubtractionOperators<T, T, T>,
		IMultiplyOperators<T, T, T>
{
	public static Vector<T> operator +(Vector<T> left, Vector<T> right) =>
		new(left.X + right.X, left.Y + right.Y);

	public static Vector<T> operator checked +(Vector<T> left, Vector<T> right) =>
		checked(new(left.X + right.X, left.Y + right.Y));

	public static Vector<T> operator -(Vector<T> left, Vector<T> right) =>
		new(left.X - right.X, left.Y - right.Y);

	public static Vector<T> operator *(Vector<T> left, T right) =>
		new(left.X * right, left.Y * right);
}

public class GenericMath
{
	public void CheckedOperators()
	{
		var v1 = new Vector<int>(int.MaxValue, int.MaxValue);
		var v2 = new Vector<int>(2, 2);

		var v3 = v1 + v2;

		// OverflowException
		var v4 = checked(v1 + v2);
	}
}

// Curiously Recurring Generic Pattern
// https://blog.stephencleary.com/2022/09/modern-csharp-techniques-1-curiously-recurring-generic-pattern.html
public interface IHasSuccessor<T>
	where T : IHasSuccessor<T>
{
	static abstract T operator ++(T t);

	static virtual string to_s(T t) => t.ToString() ?? "";
}

public record MyInt(int Value) : IHasSuccessor<MyInt>
{
	public static MyInt operator ++(MyInt i) =>
		new(i.Value + 1);

	public static string to_s(MyInt t) =>
		t.Value.ToString();
}

public record MyChar(char Value) : IHasSuccessor<MyChar>
{
	public static MyChar operator ++(MyChar c) =>
		new((char) (c.Value + 1));

	public override string ToString() =>
		Value.ToString();
}

public class StaticInterfaces
{
	public void PrintSuccessors<T>(T value, int count)
		where T : IHasSuccessor<T> // would actually want to use IIncrementOperators<T>
	{
		for (var i = 0; i < count; i++)
			Console.WriteLine(T.to_s(value++));
	}

	public void Example()
	{
		// 3 4 5 6 7
		PrintSuccessors(new MyInt(3), 5);

		// D E F G
		PrintSuccessors(new MyChar('D'), 4);
	}

	public void Example2<T>(T input)
		where T : INumber<T>
	{
		// lots of available properties and methods:
		// https://learn.microsoft.com/en-us/dotnet/api/system.numerics.inumberbase-1?view=net-7.0
		// https://learn.microsoft.com/en-us/dotnet/api/system.numerics.inumber-1?view=net-7.0
		// https://github.com/dotnet/core/blob/main/release-notes/7.0/preview/api-diff/preview5/Microsoft.NETCore.App/7.0-preview5_System.md
		var zero = T.Zero;
		var one = T.One;
		var two = one + one;
		var four = two + two;
		var eight = four + four;
		var ten = eight + two;

		var eleven = T.CreateChecked(11);

		var clamped = T.IsFinite(input) ? T.Clamp(input, one, eleven) : zero;
		var max = T.Max(T.Zero, T.AdditiveIdentity);
	}

	public void UnsignedShift()
	{
		// 0x8080_8080
		int input = -2139062144;

		// 0xF808_0808
		var signedShift = input >> 4;

		// 0x0808_0808
		var unsignedShift = input >>> 4;

		var csharp10 = (int)(((uint)input) >> 4);
	}
}
