namespace CSharp10;

internal class ConstantStringExpressions
{
	public void Example()
	{
		const string Language = "C#";
		const string MajorVersion = "10";
		const string MinorVersion = "0";
		const string BestLanguage = $"{Language} {MajorVersion}.{MinorVersion}";

		const int MajorVersionNumber = 10;
		const int MinorVersionNumber = 0;

		// can't use numbers: error CS0133: The expression being assigned to 'BestLanguage' must be constant
		// const string BestLanguage = $"{Language} {MajorVersionNumber}.{MinorVersionNumber}";
	}
}
