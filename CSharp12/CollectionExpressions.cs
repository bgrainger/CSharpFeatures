using System.Collections;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace CSharp12;

internal class CollectionExpressions
{
	public void InitializeDifferentTypesEmpty()
	{
		int[] array = [];
		List<int> list = [];
		Dictionary<int, string> dictionary = [];
		HashSet<int> hashSet = [];
		LinkedList<int> linkedList = [];
		Queue<int> queue = [];
		Stack<int> stack = [];
		SortedSet<int> sortedSet = [];
		ImmutableArray<int> immutableArray = [];
		ImmutableHashSet<int> immutableHashSet = [];

		Span<int> span = [];
		ReadOnlySpan<int> readOnlySpan = [];
	}

	public void InitializeDifferentTypesWithAdd()
	{
		// new Array
		int[] array = [1, 2, 3];

		// special-cased - new List; CollectionsMarshal.SetCount; CollectionsMarshal.AsSpan<int>
		List<int> list = [1, 2, 3];

		// new(); Add(); Add(); Add();
		HashSet<int> hashSet = [1, 2, 3];
		SortedSet<int> sortedSet = [1, 2, 3];

		// InlineArray (essentially "stackalloc")
		Span<int> span = [1, 2, 3];

		// references assembly data
		ReadOnlySpan<int> readOnlySpan = [1, 2, 3];

		// special-cased - ImmutableCollectionsMarshal.AsImmutableArray<int>(new[] { 1, 2, 3 });
		ImmutableArray<int> immutableArray = [1, 2, 3];

		// via [CollectionBuilder] - ImmutableHashSet.Create<int>(readOnlySpan)
		ImmutableHashSet<int> immutableHashSet = [1, 2, 3]; 

		// didn't make C# 12
		// Dictionary<int, string> dictionary = [1: "one"];

		// no Add method and no [CollectionBuilder] (see below)
		// LinkedList<int> linkedList = [1, 2, 3];
		// Queue<int> queue = [1, 2, 3];
		// Stack<int> stack = [1, 2, 3];
	}

	public void CustomType()
	{
		// needs to implement IEnumerable<T> to be used in a collection expression; replacement for new()
		MyCustomType custom = [];

		// implement an Add method to be initializable
		MyCustomType2 custom2 = [1, 2, 3];

		// alternatively, no Add method but a [CollectionBuilder] attribute
		MyCustomType3 custom3 = [1, 2, 3];
	}

	private class MyCustomType : IEnumerable<int>
	{
		public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();
		IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
	}

	private class MyCustomType2 : MyCustomType
	{
		public void Add(int x) { }
	}

	[CollectionBuilder(typeof(MyCustomType3Builder), nameof(MyCustomType3Builder.Create))]
	private class MyCustomType3 : MyCustomType
	{
	}

	private class MyCustomType3Builder
	{
		public static MyCustomType3 Create(ReadOnlySpan<int> span) => default;
	}
}
