using System;

namespace CSharp8Framework
{
	public class SwitchExpressions
	{
		public string ConvertEnum(ConsoleColor color) =>
			color switch
			{
				ConsoleColor.Red => "red",
				ConsoleColor.Green => "green",
				ConsoleColor.Blue => "blue",
				ConsoleColor.White => "white",
				ConsoleColor.Black => "black",
				_ => "other",
			};

		public string ConvertInteger(int value) =>
			value switch
			{
				1 => "one",
				2 => "two",
				_ => "many",
			};
	}
}
;
