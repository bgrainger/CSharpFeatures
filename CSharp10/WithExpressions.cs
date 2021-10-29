namespace CSharp10;

internal class WithExpressions
{
	public void Example()
	{
		// can apply 'with' to record structs
		var origin = new Point2D();
		var point = origin with { X = 1, Y = 2 };

		// C# 10 allows the LHS to be any (mutable) struct
		var person1 = ValueTuple.Create("John", 23);
		var person2 = person1 with { Item1 = "Mary" };

		// can use tuple element names
		var person3 = (Name: "John", Age: 23);
		var person4 = person3 with { Name = "Mary" };

		var ten = new KeyValuePair<string, int>("ten", 10);
		// error CS0200: Property or indexer 'KeyValuePair<string, int>.Value' cannot be assigned to -- it is read only
		// var eleven = ten with { Value = 11 };
	}
}

