using System.IO;

namespace CSharp9Framework
{
	class ConditionalExpressions
	{
		// C# 8 needed:
		// useFile ? (Stream) File.OpenRead(@"C:\Temp\test.txt") : new MemoryStream();
		public Stream OpenData(bool useFile) =>
			useFile ? File.OpenRead(@"C:\Temp\test.txt") : new MemoryStream();
	}
}
