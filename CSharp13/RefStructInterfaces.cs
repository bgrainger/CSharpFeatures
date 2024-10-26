using System.Collections;

namespace CSharp13;

internal class RefStructInterfaces
{
	public readonly ref struct DisposableStruct : IDisposable
	{
		public void Dispose()
		{
		}
	}

	public void UseIDisposable()
	{
		// Not actually necessary because C# 8.0 contained a workaround: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/using#pattern-based-using
		using var disposable = new DisposableStruct();
	}

	public ref struct InfiniteOnes : IEnumerator<int>
	{
		public bool MoveNext() => true;

		public int Current => 1;

		public void Dispose()
		{
		}

		public void Reset()
		{
		}

		object IEnumerator.Current => 1;
	}

	public void Enumerate<TIterator>(TIterator iterator)
		where TIterator : IEnumerator<int>, allows ref struct
	{
		while (iterator.MoveNext())
			Console.WriteLine(iterator.Current);
	}

	public void UseIEnumerator()
	{
		var ones = new InfiniteOnes();
		Enumerate(ones);

		// can't box it because it's a ref struct
		// error CS0029: Cannot implicitly convert type 'CSharp13.RefStructInterfaces.InfiniteOnes' to 'System.Collections.Generic.IEnumerator<int>'
		// IEnumerator<int> enumerator = ones;
	}

	// New ref structs ValueMatchEnumerator and ValueSplitEnumerator don't implement interfaces; just use duck typing.
	// New Lock.Scope ref struct doesn't implement IDisposable; just uses duck typing.
	// Found one hit in dotnet/runtime: https://github.com/dotnet/runtime/blob/2c471afb09122433dc8137a9827e47ff8cedd6ff/src/libraries/System.Private.CoreLib/src/System/SpanHelpers.BinarySearch.cs#L62-L63
}
