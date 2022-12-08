using System.Text;

namespace CSharp11;

public class Utf8Literals
{
	public void Example()
	{
		// done at runtime; allocates
		byte[] bytes = Encoding.UTF8.GetBytes("A string");

		// done at compile time; no allocation
		ReadOnlySpan<byte> efficientBytes = "A string"u8;
	}
}
