using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace CSharp15;

internal class CollectionExpressionArguments
{
	public void Capacity()
	{
		string[] values = ["one", "two", "three"];

		// C# 12: collection expressions, but no way to pass constructor arguments
		List<string> names1 = ["zero", .. values];

		// Before C# 15: fall back to 'new' + collection initializer + AddRange
		List<string> names2 = new(capacity: values.Length * 2) { "zero" };
		names2.AddRange(values);

		// C# 15: 'with(...)' passes arguments to the constructor; it must be the first element
		List<string> names3 = [with(capacity: values.Length * 2), "zero", .. values];
		Console.WriteLine(names3.Capacity); // 6
	}

	public void Comparer()
	{
		// Before C# 15
		var set1 = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Hello", "HELLO", "hello" };

		// C# 15
		HashSet<string> set2 = [with(StringComparer.OrdinalIgnoreCase), "Hello", "HELLO", "hello"];
		Console.WriteLine(set2.Count); // 1

		// works with any constructor, picked by normal overload resolution
		SortedSet<string> sorted = [with(StringComparer.OrdinalIgnoreCase), "banana", "Apple", "cherry"];
		Console.WriteLine(string.Join(", ", sorted)); // Apple, banana, cherry
	}

	public void CollectionBuilder()
	{
		// [CollectionBuilder] types: 'with' arguments are passed before the ReadOnlySpan<T> parameter
		// ImmutableHashSet.Create<T>(IEqualityComparer<T>? equalityComparer, ReadOnlySpan<T> items)
		ImmutableHashSet<string> immutableSet = [with(StringComparer.OrdinalIgnoreCase), "a", "A", "b"];
		Console.WriteLine(immutableSet.Count); // 2

		// also works for your own [CollectionBuilder] types
		CaseInsensitiveSet custom = [with(ignoreCase: true), "x", "X", "y"];
		Console.WriteLine(custom.Count); // 2
	}

	public void InterfaceTargetTypes()
	{
		// IList<T> and ICollection<T> create a List<T> and accept (int capacity)
		IList<int> list = [with(capacity: 100), 1, 2, 3];

		// IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T> only accept an empty 'with()'
		IReadOnlyList<int> readOnlyList = [with(), 1, 2, 3];
		// error CS9357: 'with(...)' element for a read-only interface must be empty if present
		// IReadOnlyList<int> readOnlyList2 = [with(capacity: 100), 1, 2, 3];
	}

	public void RequiredConstructorArguments()
	{
		// Before C# 15: no parameterless constructor means no collection expression
		// error CS9214: Collection expression type must have an applicable constructor that can be called with no arguments.
		// NamedValues primes = [2, 3, 5, 7];

		NamedValues primes = [with("primes"), 2, 3, 5, 7];
		Console.WriteLine($"{primes.Name}: {string.Join(", ", primes)}"); // primes: 2, 3, 5, 7
	}

	public void EvaluationOrder()
	{
		// 'with' arguments are evaluated first, then the elements, left-to-right
		List<int> list = [with(Log("capacity", 10)), Log("first", 1), Log("second", 2)];
		// capacity
		// first
		// second

		static int Log(string message, int value)
		{
			Console.WriteLine(message);
			return value;
		}
	}

	public void NotSupported()
	{
		// not supported for arrays or spans:
		// error CS9355: 'with(...)' elements are not supported for type 'int[]'
		// int[] array = [with(10), 1, 2, 3];
		// Span<int> span = [with(10), 1, 2, 3];

		// 'with' arguments don't participate in overload resolution of the enclosing call:
		// error CS0121: The call is ambiguous between 'Print(List<string>)' and 'Print(HashSet<string>)'
		// Print([with(StringComparer.OrdinalIgnoreCase), "one", "ONE"]);

		// 'with' is only a keyword as the first element; it's still a normal identifier elsewhere
		int with = 3;
		List<int> list = [with, with + 1]; // [3, 4]
	}

	private static void Print(List<string> values) { }
	private static void Print(HashSet<string> values) { }
}

internal sealed class NamedValues(string name) : List<int>
{
	public string Name => name;
}

[CollectionBuilder(typeof(CaseInsensitiveSet), nameof(Create))]
internal sealed class CaseInsensitiveSet : IEnumerable<string>
{
	// the elements are always the last parameter
	public static CaseInsensitiveSet Create(bool ignoreCase, ReadOnlySpan<string> items) =>
		new(ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal, items);

	public static CaseInsensitiveSet Create(ReadOnlySpan<string> items) => Create(ignoreCase: true, items);

	public int Count => m_set.Count;

	public IEnumerator<string> GetEnumerator() => m_set.GetEnumerator();
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

	private CaseInsensitiveSet(StringComparer comparer, ReadOnlySpan<string> items)
	{
		m_set = new(comparer);
		foreach (var item in items)
			m_set.Add(item);
	}

	private readonly HashSet<string> m_set;
}
