namespace CSharp13;

internal class ObjectInitializerReverseIndex
{
	public void Example()
	{
		// previous versions of C#
		var list = new List<int>([0, 0, 0])
		{
			[0] = 1,
			[1] = 2,
			[2] = 3,
		};

		// new: use "from the end" indexes
		list = new List<int>([0, 0, 0])
		{
			[^3] = 1,
			[^2] = 2,
			[^1] = 3,
		};
		Console.WriteLine(string.Join(",", list)); // 1,2,3
	}
}

