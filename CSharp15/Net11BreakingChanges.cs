namespace CSharp15;

// selected behavioral changes that could affect existing code
// full list: https://learn.microsoft.com/en-us/dotnet/core/compatibility/11

internal class Net11BreakingChanges
{
	public void DecimalConversions()
	{
		// decimal <-> double/float (and BigInteger -> double/float) conversions are now correctly rounded
		double value = 1.23;
		Console.WriteLine((decimal) value);
		// .NET 10: 1.23 (rounded to 15 significant digits)
		// .NET 11: 1.2299999999999999822364316060 (the exact value of the double, rounded once)

		Console.WriteLine((decimal) (float) value);
		// .NET 10: 1.23
		// .NET 11: 1.230000019073486328125

		// constants are converted by the compiler, so '(decimal) 1.23' changes when you build with the .NET 11 SDK, even targeting net10.0

		// if the value is meant to be decimal, use a decimal literal (or decimal.Parse) instead of converting a double
		Console.WriteLine(1.23m); // 1.23
	}

	public void MathRound()
	{
		// Math.Round(double, digits) rounds the exact value of the input
		// 655.925 is actually 655.92499999999995452526491135...
		Console.WriteLine(Math.Round(655.925, 2, MidpointRounding.AwayFromZero));
		// .NET 10: 655.93
		// .NET 11: 655.92
	}

	public void SaturatingConversions()
	{
		// unchecked conversions from floating point to sbyte/byte/short/ushort/char now saturate (like int/long since .NET 9)
		double value = 298;
		Console.WriteLine(unchecked((byte) value));
		// .NET 10: 42 (low 8 bits of 298)
		// .NET 11: 255

		value = -42;
		Console.WriteLine(unchecked((ushort) value));
		// .NET 10: 65494
		// .NET 11: 0
	}
}
