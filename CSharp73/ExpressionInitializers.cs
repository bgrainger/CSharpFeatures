using System.Linq;

namespace CSharp73
{
	class ExpressionInitializers
	{
		public ExpressionInitializers(int value)
		{
		}

		// error CS8200: Out variable and pattern variable declarations are not allowed within constructor initializers, field initializers, or property initializers.
		static int Value = int.TryParse("123", out var value) ? value : 0;
	}

	class Derived : ExpressionInitializers
	{
		public Derived(string s)
			: base(int.TryParse(s, out var value) ? value : -1)
		{
		}
	}

	class Linq
	{
		public static void CSharp7()
		{
			var r = new string[1].Select(x => int.TryParse(x, out var value) ? value : -1);
		}

		public static void Query()
		{
			// error CS8201: Out variable and pattern variable declarations are not allowed within a query clause.
			var r = from s in new string[1]
				select int.TryParse(s, out var value) ? value : -1;
		}
	}
}
