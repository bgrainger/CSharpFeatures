using System;

namespace CSharp15Framework;

// C# 8 added static interface members, but only for runtimes that support default interface methods (.NET Core 3.0+)
// C# 15 allows non-virtual static members on any runtime, including .NET Framework and netstandard2.0

internal interface IMeasurement
{
	double Meters { get; }

	// static methods
	public static double ToMeters(double centimeters) => centimeters / 100;

	// static fields and constants
	public static readonly IMeasurement Zero = new Measurement(0);
	public const double CentimetersPerMeter = 100;

	// static properties
	public static IMeasurement OneMeter { get; } = new Measurement(1);

	// operators
	public static IMeasurement operator +(IMeasurement left, IMeasurement right) => new Measurement(left.Meters + right.Meters);

	// static constructors, static events, and private/internal static members also work

	// still require runtime support:
	// error CS8701: Target runtime doesn't support default interface implementation.
	// public double Centimeters => Meters * CentimetersPerMeter;

	// error CS8919: Target runtime doesn't support static abstract members in interfaces.
	// public static abstract IMeasurement Parse(string text);

	// error CS8707: Target runtime doesn't support 'protected', 'protected internal', or 'private protected' accessibility for a member of an interface.
	// protected static void Helper() { }
}

internal sealed class Measurement(double meters) : IMeasurement
{
	public double Meters => meters;
}

internal class StaticInterfaceMembers
{
	public void CallStaticMembers()
	{
		Console.WriteLine(IMeasurement.ToMeters(250)); // 2.5
		Console.WriteLine((IMeasurement.OneMeter + new Measurement(0.5)).Meters); // 1.5
	}

	public void BeforeCSharp15()
	{
		// with C# 14 targeting .NET Framework:
		// error CS9327: Feature 'static members in interfaces' is not available in C# 14.0. Please use language version 15.0 or greater.

		// the workaround was a separate static class
		Console.WriteLine(MeasurementUtility.ToMeters(250));
	}

	private static class MeasurementUtility
	{
		public static double ToMeters(double centimeters) => centimeters / 100;
	}
}
