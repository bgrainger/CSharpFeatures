using System.Collections;
using System.Runtime.CompilerServices;

namespace CSharp14;

// Extension members allow you to add properties, operators, and static members
// to existing types without modifying them

internal static class EnumerableExtensions
{
	// C# 13
	public static IEnumerable<TSource> SomeExtension<TSource>(this IEnumerable<TSource> source) => source;

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

internal class UseExtensionMembers
{
	public void InstanceExtensionMethod()
	{
		var numbers = new int?[] { 1, null, 3 };
		Console.WriteLine(string.Join(", ", numbers.WhereNotNull())); // 1, 3
	}

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

internal static class ExtensionPropertySetter
{
	extension(object obj)
	{
		public string? Name
		{
			get => s_lookup.TryGetValue(obj, out var result) ? result : null;
			set
			{
				if (value is null)
					s_lookup.Remove(obj);
				else
					s_lookup.AddOrUpdate(obj, value);
			}
		}
	}

	private static readonly ConditionalWeakTable<object, string> s_lookup = [];
}

internal class UseExtensionProperty
{
	public void SetProperty()
	{
		var list = new List<int>();
		list.Name = "My cool list";
		Console.WriteLine(list.Name);

		list.Name = null;
		Console.WriteLine(list.Name);

		this.Name = "A fun class";
	}
}
