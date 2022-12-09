using System.Text;

namespace CSharp11;

public class Utf8Literals
{
	public void Example()
	{
		// done at runtime; allocates
		byte[] bytes = Encoding.UTF8.GetBytes("A string");

		// old way: efficient, but very hard to maintain
		ReadOnlySpan<byte> clunkyBytes = new byte[] { 0x41, 0x20, 0x73, 0x74, 0x72, 0x69, 0x6E, 0x67 }; // "A string"
		
		// done at compile time; no allocation
		ReadOnlySpan<byte> efficientBytes = "A string"u8;
	}

	public void RawUtf8Literals()
	{
		var bytes = """
			{
				"this": "is UTF-8"
			}
			"""u8;
	}
}
