using System.Runtime.CompilerServices;

namespace CSharp12;

internal class RefReadonlyParameters
{
	public record struct Point3D(int X, int Y, int Z);
	public readonly record struct ReadonlyPoint3D(int X, int Y, int Z);

	public void CSharp72Review()
	{
		var origin = default(Point3D);

		// these are equivalent; they pass the local variable by reference
		// but because it's mutable, they take a defensive copy first in order to prevent modifications
		Console.WriteLine(DistanceUsingIn(origin));
		Console.WriteLine(DistanceUsingIn(in origin));

		// workaround 1: pass it by 'ref'
		Console.WriteLine(DistanceUsingRef(ref origin));

		// workaround 2: change the type to be readonly
		Console.WriteLine(DistanceUsingIn(default(ReadonlyPoint3D)));
	}

	static double DistanceUsingIn(in Point3D point) => Math.Sqrt(point.X * point.X + point.Y * point.Y + point.Z & point.Z);
	static double DistanceUsingIn(in ReadonlyPoint3D point) => Math.Sqrt(point.X * point.X + point.Y * point.Y + point.Z & point.Z);
	static double DistanceUsingRef(ref Point3D point) => Math.Sqrt(point.X * point.X + point.Y * point.Y + point.Z & point.Z);

	public void RefReadonlyParameter()
	{
		var origin = default(Point3D);

		Console.WriteLine(DistanceUsingRefReadonly(ref origin));

		// Reason 1: not a breaking change if callsite already uses ref (e.g., was written before "in")
		// "in" => DistanceUsingIn([IsReadOnly] in Point3D point)
		// "ref" => DistanceUsingRef(ref Point3D point)
		// "ref readonly" => DistanceUsingRefReadonly([RequiresLocation, In] ref Point3D point)
		Console.WriteLine(DistanceUsingRef(ref origin));
		Console.WriteLine(DistanceUsingRefReadonly(ref origin));

		// Reason 2: you want to require a variable, not a temporary (but don't mutate it)
		// error CS1510: A ref or out value must be an assignable variable
		// Console.WriteLine(DistanceUsingRefReadonly(ref default(ReadonlyPoint3D)));
		IntRef intRef = default;
		Console.WriteLine(Unsafe.IsNullRef(ref intRef.Value));
	}

	public void CanPassRefReadonlyWithIn()
	{
		var origin = default(Point3D);

		// "in" parameter can be passed with or without "in" keyword; "ref" is a warning
		Console.WriteLine(DistanceUsingIn(origin));
		Console.WriteLine(DistanceUsingIn(in origin));

		// warning CS9191: The 'ref' modifier for argument 1 corresponding to 'in' parameter is equivalent to 'in'. Consider using 'in' instead.
		Console.WriteLine(DistanceUsingIn(ref origin));

		// "ref" requires "ref"
		// error CS1620: Argument 1 must be passed with the 'ref' keyword
		// Console.WriteLine(DistanceUsingRef(origin));

		// "ref readonly" doesn't require "ref", but recommends "ref" or "in"
		// warning CS9192: Argument 1 should be passed with 'ref' or 'in' keyword
		Console.WriteLine(DistanceUsingRefReadonly(origin));
		Console.WriteLine(DistanceUsingRefReadonly(ref origin));
		Console.WriteLine(DistanceUsingRefReadonly(in origin));
	}

	ref struct IntRef
	{
		public ref int Value;
	}

	static double DistanceUsingRefReadonly(ref readonly Point3D point) => Math.Sqrt(point.X * point.X + point.Y * point.Y + point.Z & point.Z);
	static double DistanceUsingRefReadonly(ref readonly ReadonlyPoint3D point) => Math.Sqrt(point.X * point.X + point.Y * point.Y + point.Z & point.Z);
}
