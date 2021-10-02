namespace CSharp10;

internal class WithExpressions
{
	public void Example()
	{
		var person1 = ValueTuple.Create("John", 23);

		// C# 10 allows the LHS to be any struct
		var person2 = person1 with { Item1 = "Mary" };

		// can use tuple element names
		var person3 = (Name: "John", Age: 23);
		var person4 = person3 with { Name = "Mary" };
	}
}

