namespace CSharp10;

record Vector2D(int X, int Y);

record Vector3D(int X, int Y, int Z) : Vector2D(X, Y)
{
	public sealed override string ToString()
	{
		var builder = new StringBuilder();
		PrintMembers(builder);
		return $"3-D vector, members: {builder}";
	}
}

record Vector4D(int X, int Y, int Z, int W) : Vector3D(X, Y, Z)
{
	protected override bool PrintMembers(StringBuilder builder)
	{
		builder.Append("not telling");
		return true;
	}
}

internal class SealedToString
{
	public void Example()
	{
		// Vector2D { X = 1, Y = 2 }
		Console.WriteLine(new Vector2D(1, 2));

		// 3-D vector, members: X = 1, Y = 2, Z = 3
		Console.WriteLine(new Vector3D(1, 2, 3));

		// 3-D vector, members: not telling
		Console.WriteLine(new Vector4D(1, 2, 3, 4));
	}
}
