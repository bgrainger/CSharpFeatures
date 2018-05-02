using System.IO;

namespace CSharp72
{
	public class NonTrailingNamedArguments
	{
		public void Previous()
		{
			// option 1: don't name arguments (loses clarity)
			SomeMethod(@"C:\Temp\test.txt", false, true, FileShare.Read, FileAccess.ReadWrite);

			// option 2: name all arguments to right (verbose)
			SomeMethod(@"C:\Temp\test.txt", truncate: false, createIfMissing: true, fileShare: FileShare.Read, fileAccess: FileAccess.ReadWrite);
		}

		public void Now()
		{
			// name non-trailing arguments
			SomeMethod(@"C:\Temp\test.txt", truncate: false, createIfMissing: true, FileShare.Read, FileAccess.ReadWrite);

			// can't swap argument order if positional parameters follow
			// SomeMethod(@"C:\Temp\test.txt", createIfMissing: true, truncate: false, FileShare.Read, FileAccess.ReadWrite);
		}

		public void SomeMethod(string filePath, bool truncate, bool createIfMissing, FileShare fileShare, FileAccess fileAccess)
		{
		}
	}
}
