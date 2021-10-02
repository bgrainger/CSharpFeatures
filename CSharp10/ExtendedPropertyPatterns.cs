namespace CSharp10;

internal class ExtendedPropertyPatterns
{
	public void Example()
	{
		var linkedList = CreateLinkedList();
		if (linkedList is { First.Next.Value: "def" })
			Console.WriteLine("The second node is 'def'");

		// Compare to C# 8.0:
		if (linkedList is { First: { Next: { Value: "def" } } })
			Console.WriteLine("The second node is 'def'");
	}

	public static LinkedList<string> CreateLinkedList()
	{
		var linkedList = new LinkedList<string>();
		linkedList.AddLast("abc");
		linkedList.AddLast("def");
		linkedList.AddLast("ghi");
		return linkedList;
	}
}
