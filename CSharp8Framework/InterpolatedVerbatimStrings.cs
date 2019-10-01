using System;

namespace CSharp8Framework
{
	public class InterpolatedVerbatimStrings
	{
		public void Method()
		{
			var dollarAt = $@"month is {DateTime.Now.Month}";
			var atDollar = @$"day is {DateTime.Now.Day}";
		}
	}
}
