using System;

namespace CSharp8Framework
{
	public class StaticLocalFunctions
	{
		public double Method(int a, int b)
		{
			var c1 = GetHypotenuse();
			var c2 = GetHypotenuseStatic(a, b);
			return c1 + c2;

			double GetHypotenuse() => Math.Sqrt(a * a + b * b);

			// static function can't capture anything, including 'this'
			static double GetHypotenuseStatic(double a, double b) => Math.Sqrt(a * a + b * b);

			// error CS8421: A static local function cannot contain a reference to 'a'.
			// static double GetHypotenuseStatic() => Math.Sqrt(a * a + b * b);

			// error CS8422: A static local function cannot contain a reference to 'this' or 'base'.
			// static double GetHypotenuseStatic(int a, int b) => Method2(a, b);
		}

		public double Method2(int a, int b) => a + b;
	}
}
