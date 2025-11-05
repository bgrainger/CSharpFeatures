namespace CSharp14;

// Extension members allow you to add properties, operators, and static members
// to existing types without modifying them

internal static class EnumerableExtensions
{
	// Extension block for instance members
	extension<TSource>(IEnumerable<TSource> source)
	{
		// Extension method
		public IEnumerable<TSource> WhereNotNull() => source.Where(x => x is not null);

		// Extension property
		public bool IsEmpty => !source.Any();
	}

	// Extension block for static members (no parameter name)
	extension<TSource>(IEnumerable<TSource>)
	{
		// Static extension property
		public static IEnumerable<TSource> Empty => Enumerable.Empty<TSource>(); // could write as just []

		// User-defined operator as static extension
		public static IEnumerable<TSource> operator +(IEnumerable<TSource> left, IEnumerable<TSource> right)
			=> left.Concat(right);
	}
}

internal static class IntExtensions
{
	extension(int value)
	{
		public bool IsEven => value % 2 == 0;
		public bool IsOdd => value % 2 != 0;
	}
}

internal class ExtensionMembers
{
	public void InstanceExtensionProperty()
	{
		var numbers = new[] { 1, 2, 3 };
		Console.WriteLine(numbers.IsEmpty); // False

		var empty = Enumerable.Empty<int>();
		Console.WriteLine(empty.IsEmpty); // True
	}

	public void ExtensionPropertyOnValueType()
	{
		Console.WriteLine(42.IsEven); // True
		Console.WriteLine((-5).IsOdd); // True
	}

	public void StaticExtensionProperty()
	{
		var empty = IEnumerable<int>.Empty;
		Console.WriteLine(empty.IsEmpty); // True
	}

	public void ExtensionOperator()
	{
		IEnumerable<int> first = [1, 2];
		IEnumerable<int> second = [3, 4];
		var combined = first + second;
		Console.WriteLine(string.Join(", ", combined)); // 1, 2, 3, 4
	}

	public void FullyWrittenOutInvocation()
	{
		var numbers = new[] { 1, 2, 3 };

		// numbers.WhereNotNull
		var nonNullNumbers = EnumerableExtensions.WhereNotNull(numbers);

		// numbers.IsEmpty
		bool isEmpty = EnumerableExtensions.get_IsEmpty(numbers);

		// IEnumerable<int>.Empty
		var empty = EnumerableExtensions.get_Empty<int>();

		// first + second
		var combined = EnumerableExtensions.op_Addition([1, 2], [3, 4]);
	}

	// Argument*Exception.ThrowIfX polyfill: https://github.com/mysql-net/MySqlConnector/commit/901df6d495998371c371b8c0e9f34327873bb166#diff-cf1b255962f3d169d25c584e5e14c8671784d9c7fa8433f03b20ae9cd83b51f9
}
