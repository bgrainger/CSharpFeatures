namespace CSharp72
{
	public class LeadingDigitSeparator
	{
		public void Previous()
		{
			// _ was allowed as digit separator in numbers
			var binary = 0b1100_0010_1101;
			var hex = 0xDEAD_BEEF;
			var base10 = 123_456_789;
		}

		public void Now()
		{
			// underscore can now immediately follow the prefix
			var binary = 0b_1100_0010_1101;
			var hex = 0x_DEAD_BEEF;

			// can't have leading underscore on non-prefixed number; this is an identifier
			// var base10 = _123_456_789;
		}
	}
}
