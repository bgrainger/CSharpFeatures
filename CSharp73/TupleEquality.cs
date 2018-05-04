using System;

namespace CSharp73
{
	class TupleEquality
	{
		public void EqualityOperator()
		{
			var one = (1, "one");
			var two = (2, "two");
			var anotherOne = (1, "one");
			Console.WriteLine(one.Equals(two)); // false
			Console.WriteLine(one.Equals(anotherOne)); // true

			// C# 7.2: error CS0019: Operator '==' cannot be applied to operands of type '(int, string)' and '(int, string)'
			Console.WriteLine(one == anotherOne); // true
			Console.WriteLine(one != two); // true
		}

		public void ComparingTypes()
		{
			(bool?, string) a = (true, "true");
			(bool, string) b = (true, "true");

			// can compare 'bool?' to 'bool'
			Console.WriteLine(a == b); // true

			// get one error for each mismatched type
			// error CS0019: Operator '==' cannot be applied to operands of type 'string' and 'bool?'
			// error CS0019: Operator '==' cannot be applied to operands of type 'int' and 'string'
			// Console.WriteLine(("true", 0) == a);

			// error CS8384: Tuple types used as operands of an == or != operator must have matching cardinalities. But this operator has tuple types of cardinality 3 on the left and 2 on the right.
			// Console.WriteLine((1, 2, 3) == a);

			(bool?, string) c = (null, "true");
			// var d = (null, "true"); // error CS0815: Cannot assign (<null>, string) to an implicitly-typed variable
			Console.WriteLine(c == (null, "true")); // true
		}

		public void Nested()
		{
			var doc1 = ("html", ("head", ("title", "A Title")), ("body", ("p", "Hello World")));
			var doc2 = ("html", ("head", ("title", "New Title")), ("body", ("p", "Goodbye World")));
			Console.WriteLine(doc1 == doc2); // false
		}

		public void ElementNames()
		{
			var a = (ordinal: 1, name: "one");
			var b = (count: 1, value: "one");
			Console.WriteLine(a == b); // true

			// warning CS8383: The tuple element name 'count' is ignored because a different name or no name is specified on the other side of the tuple == or != operator.
			// warning CS8383: The tuple element name 'value' is ignored because a different name or no name is specified on the other side of the tuple == or != operator.
			Console.WriteLine(a == (count: 1, value: "one")); // true
		}

		public void Deconstruction()
		{
			var deconstructable = new Deconstructable();
			var (a, b) = deconstructable; // a = 1, b = 2

			// error CS0019: Operator '==' cannot be applied to operands of type '(int, int)' and 'Deconstructable'
			// Console.WriteLine((1, 2) == deconstructable);
		}
	}

	public class Deconstructable
	{
		public void Deconstruct(out int a, out int b)
		{
			a = 1;
			b = 2;
		}
	}
}
