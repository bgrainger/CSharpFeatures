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
		PrintSuccessors(new MyInt(3), 5);
		PrintSuccessors(new MyChar('D'), 4);
	}
}
