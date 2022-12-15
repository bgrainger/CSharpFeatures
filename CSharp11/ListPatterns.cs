namespace CSharp11;

public class ListPatterns
{
	public void Example()
	{
		var list = new List<int> { 1, 2, 3, 4 };

		_ = list is [1, 2, 3, 4]; // true
		_ = list is [1, 2, 3, 4, 5]; // false
		_ = list is [1, 2, 3]; // false

		// use discards to ignore elements but require them to be present
		_ = list is [1, _, _, _]; // true

		// use a "slice pattern" to ignore the end
		_ = list is [1, ..]; // true

		// ... or the middle
		_ = list is [1, .., 4]; // true

		// use patterns within the list
		_ = list is [1 or 3, 2 or 4, 1 or 3, 2 or 4]; // true
		_ = list is [< 2, <= 2, >= 3, > 3]; // true
	}

	class VirtualList
	{
		public VirtualList(int length) => Length = length;
		public int this[Index index] => index.IsFromEnd ? Length - index.Value : index.Value;
		public int Length { get; }
	}

	public void TestVirtualList()
	{
		var list = new VirtualList(3);
		_ = list is [0, 1, 2]; // true

		_ = new VirtualList(4) is [.., 3]; // true
	}

	public string TrimBackquotes(string value) =>
		value switch
		{
			['`', .. var middle, '`'] => middle,
			_ => value,
		};

	public string TrimBackquotesOld(string value)
	{
		if (value is { Length: >= 2 } && value[0] == '`' && value[^1] == '`')
			return value[1..^1];

		if (!string.IsNullOrEmpty(value) && value.Length >= 2 && value[0] == '`' && value[value.Length - 1] == '`')
			return value.Substring(1, value.Length - 2);

		return value;
	}
	
	public bool IsPartitioned(int[] values) =>
		values switch
		{
			[] => false,
			[0] => true,
			[< 0, .. { Length: > 0 } middle, > 0] => IsPartitioned(middle),
			_ => false,
		};

	public int StringMatching(ReadOnlySpan<char> str) =>
		str switch
		{
			"Hello" => 0,
			"Goodbye" => 1,
			_ => 2,
		};

	// The previous method generates more efficient code than using list patterns.
	public int SpanCharMatching(ReadOnlySpan<char> str) =>
		str switch
		{
			['H', 'e', 'l', 'l', 'o'] => 0,
			['G', 'o', 'o', 'd', 'b', 'y', 'e'] => 1,
			_ => 2,
		};

}
