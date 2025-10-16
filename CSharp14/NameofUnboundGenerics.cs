namespace CSharp14;

internal class NameofUnboundGenerics
{
	public void BasicUnboundGeneric()
	{
		// Before C# 14: needed to use a closed generic type
		var name = nameof(List<int>); // "List"
		
		// C# 14: can use unbound generic type
		name = nameof(List<>); // "List"
	}

	public void MultipleTypeParameters()
	{
		// Works with multiple type parameters
		var dict = nameof(Dictionary<,>);
		Console.WriteLine(dict); // "Dictionary"

		var keyValue = nameof(KeyValuePair<,>);
		Console.WriteLine(keyValue); // "KeyValuePair"
	}
}
