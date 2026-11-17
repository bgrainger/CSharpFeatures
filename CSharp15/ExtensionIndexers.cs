namespace CSharp15;

// C# 14 added extension methods, properties, and operators in 'extension' blocks
// C# 15 adds extension indexers

internal static class SequenceExtensions
{
	extension<T>(IEnumerable<T> source)
	{
		// indexers are always instance members, so the receiver parameter must be named
		public T this[int index] => source.ElementAt(index);
	}

	extension<T>(IReadOnlyList<T> list)
	{
		// IReadOnlyList<T> has an int indexer but no Slice method, so 'list[1..3]' was an error
		public IEnumerable<T> this[Range range]
		{
			get
			{
				var (offset, length) = range.GetOffsetAndLength(list.Count);
				for (int i = offset; i < offset + length; i++)
					yield return list[i];
			}
		}
	}

	extension<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary)
	{
		// indexers can have multiple parameters
		public TValue this[TKey key, TValue defaultValue] =>
			dictionary.TryGetValue(key, out var value) ? value : defaultValue;
	}
}

internal static class BitExtensions
{
	// use a 'ref' receiver to write a setter for a value type
	extension(ref int bits)
	{
		public bool this[int bit]
		{
			get => (bits & (1 << bit)) != 0;
			set => bits = value ? bits | (1 << bit) : bits & ~(1 << bit);
		}
	}
}

internal class ExtensionIndexers
{
	public void SequenceIndexer()
	{
		IEnumerable<int> numbers = Enumerable.Range(1, 10).Where(x => x % 2 == 1);

		// Before C# 15
		var third = numbers.ElementAt(2);

		// C# 15
		third = numbers[2];
		Console.WriteLine(third); // 5

		// implicit Index support isn't provided by an extension indexer (it needs Length/Count and an instance int indexer)
		// error CS0021: Cannot apply indexing with [] to an expression of type 'IEnumerable<int>'
		// var last = numbers[^1];

		// public T this[Index index] => source.ElementAt(index.GetOffset(source.Count()));
	}

	public void RangeIndexer()
	{
		IReadOnlyList<int> list = [1, 2, 3, 4, 5];
		Console.WriteLine(string.Join(", ", list[1..3])); // 2, 3
		Console.WriteLine(string.Join(", ", list[^2..])); // 4, 5
	}

	public void MultipleParameters()
	{
		IReadOnlyDictionary<string, int> ages = new Dictionary<string, int> { ["Alice"] = 30 };

		// Before C# 15
		var age = ages.GetValueOrDefault("Bob", -1);

		// C# 15
		Console.WriteLine(ages["Alice", -1]); // 30
		Console.WriteLine(ages["Bob", -1]); // -1
	}

	public void Setter()
	{
		int flags = 0;
		flags[3] = true;
		flags[0] = true;
		Console.WriteLine(flags); // 9
		Console.WriteLine(flags[3]); // True

		// 'ref' receiver needs a variable
		// error CS1510: A ref or out value must be an assignable variable
		// Console.WriteLine(42[1]);
	}

	public void FullyWrittenOutInvocation()
	{
		IEnumerable<int> numbers = [1, 2, 3];

		// numbers[2]
		var third = SequenceExtensions.get_Item(numbers, 2);

		int flags = 0;

		// flags[3] = true
		BitExtensions.set_Item(ref flags, 3, true);
	}
}
