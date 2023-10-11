using System.Runtime.CompilerServices;
using System.Text;

namespace CSharp12;

internal sealed record class PointRecord(int X, int Y);

internal sealed class GeneratedPointRecord : IEquatable<GeneratedPointRecord>
{
	public int X { get; init; }
	public int Y { get; init; }

	private Type EqualityContract => typeof(GeneratedPointRecord);

	public override string ToString()
	{
		StringBuilder builder = new StringBuilder();
		builder.Append(nameof(PointRecord));
		builder.Append(" { ");
		if (this.PrintMembers(builder))
			builder.Append(' ');
		builder.Append('}');
		return builder.ToString();
	}

	private bool PrintMembers(StringBuilder builder)
	{
		RuntimeHelpers.EnsureSufficientExecutionStack();
		builder.Append("X = ");
		builder.Append(X.ToString());
		builder.Append(", Y = ");
		builder.Append(Y.ToString());
		return true;
	}

	public static bool operator!=(GeneratedPointRecord left, GeneratedPointRecord right) =>
		!(left == right);

	public static bool operator ==(GeneratedPointRecord left, GeneratedPointRecord right) =>
		ReferenceEquals(left, right) || (left is not null && left.Equals(right));

	public override int GetHashCode() =>
		EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 +
		EqualityComparer<int>.Default.GetHashCode(X) * -1521134295 +
		EqualityComparer<int>.Default.GetHashCode(Y);

	public override bool Equals(object? obj) =>
		Equals(obj as GeneratedPointRecord);

	public bool Equals(GeneratedPointRecord? other)
	{
		if (this == (object?) other)
			return true;
		return other is not null &&
			EqualityContract == other.EqualityContract &&
			EqualityComparer<int>.Default.Equals(X, other.X) &&
			EqualityComparer<int>.Default.Equals(Y, other.Y);
	}

	public void Deconstruct(out int x, out int y)
	{
		x = X;
		y = Y;
	}

	private GeneratedPointRecord(GeneratedPointRecord other)
	{
		X = other.X;
		Y = other.Y;
	}
}

internal class Point(int x, int y);

internal class GeneratedPoint
{
	private int _xField;
	private int _yField;

	public GeneratedPoint(int x, int y)
	{
		this._xField = x;
		this._yField = y;
	}
}

internal class PointB(int x, int y)
{
	public int X { get; } = x;
	public int Y { get; } = y;
}

internal class PolarPointB(int x, int y)
{
	public double Radius { get; } = Math.Sqrt(x * x + y * y);
	public double Angle { get; } = Math.Atan2(y, x);
}

internal class PointC(int x, int y, int z)
{
	public int X { get; } = x; // warning CS9124: Parameter 'int x' is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
	public int Y { get; } = y;

	public override string ToString() =>
		$"({x},{Y},{z})";
}

internal class GeneratedPointC
{
	public GeneratedPointC(int x, int y, int z)
	{
		xField = x;
		zField = z;
		xProperty = xField;
		yProperty = y;
	}
	public int X => xProperty;
	public int Y => yProperty;

	public override string ToString() =>
		$"({xField},{Y},{zField})";

	private int xField;
	private int zField;
	private readonly int xProperty;
	private readonly int yProperty;
}

internal class PointD(int x, int y)
{
	public int X => x;
	public int Y => y;

	public void Move(int xDelta, int yDelta)
	{
		x += xDelta;
		y += yDelta;

		// NOTE: "x" is a pseudo name that represents the compiler-generated field, not an actual field
		// this.x += this.y; // error CS1061: 'PointC' does not contain a definition for 'x' 
	}
}

internal class PointE(int x, int y)
{
	public PointE(int x, int y, int z)
		: this(x, y)
	{
		Console.WriteLine($"Didn't use {z}");
	}

	public PointE()
		: this(0, 0)
	{
		// have to initialize all primary constructor parameters via ": this(...)" syntax
		// error CS8862: A constructor declared in a type with parameter list must have 'this' constructor initializer.
	}

	public int X { get; } = x;

	public override string ToString() => $"({X},{y})";
}

internal class PointF(int x, int y, int z) : PointE(x, y)
{
	public int Z { get; } = z;

	public override string ToString()
	{
		// Don't do this! It creates an extra field in this class for "x"; use "X" instead.
		return $"({x},{y},{Z})";
	}
}

internal readonly struct StructConstructor(int x, int y)
{
	public int X { get; } = x;
	public int Y { get; } = y;
}

internal interface ILogger
{
	void LogInformation(string message);
}

internal class MyController(ILogger logger)
{
	[HttpGet]
	public ActionResult<Point> Get()
	{
		logger.LogInformation("The controller method was invoked!");
		return new(new(1, 2));
	}
}

internal class HttpGetAttribute() : Attribute;
internal class ActionResult<T>(T t);
