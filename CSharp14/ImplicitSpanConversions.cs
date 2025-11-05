namespace CSharp14;

internal class ImplicitSpanConversions
{
	public void ExtensionMethodImplicitConversion()
	{
		// implicit conversion operators already exist (and were usable)
		int[] array = [1, 2, 3, 4, 5];
		Span<int> span = array;
		ReadOnlySpan<int> readOnlySpan = span;

		// MemoryExtensions.StartsWith<T>(this ReadOnlySpan<T> span, T value)
		readOnlySpan.StartsWith(1);

		// previously: tried to pick (this Span<int>, ReadOnlySpan<int>) overload and failed
		span.StartsWith(1);

		// previously: error CS1929: 'int[]' does not contain a definition for 'StartsWith' and the best extension method overload 'MemoryExtensions.StartsWith<int>(ReadOnlySpan<int>, int)' requires a receiver of type 'System.ReadOnlySpan<int>'
		array.StartsWith(1);
	}

	public void Betterness()
	{
		int[] array = [1, 2, 3, 4, 5];

		// extension methods:
		// public static void DoSomething<T>(this IEnumerable<T> enumerable) { }
		// public static void DoSomething<T>(this ReadOnlySpan<T> span) { }

		// C# 13 picks IEnumerable<T> (because no implicit conversions)
		// C# 14 picks ReadOnlySpan<T> overload as a "better conversion target"
		array.DoSomething();
	}

	public void ReverseAmbiguity()
	{
		int[] array = [1, 2, 3, 4, 5];

		// was IEnumerable<T> Enumerable.Reverse<T>(this IEnumerable<T>)
		// would have become 'void MemoryExtensions.Reverse<T>(this Span<T>)' <-- BREAKING CHANGE
		// but now IEnumerable<T> Enumerable.Reverse<T>(this T[]) <-- NEW METHOD
		var reversed = array.Reverse();
	}

	public void AssertAmbiguity()
	{
		int[] array = [1, 2, 3, 4, 5];

		// now ambiguous
		// error CS0121: The call is ambiguous between the following methods or properties: 'Assert.AreEqual<T>(T, T)' and 'Assert.AreEqual<T>(ReadOnlySpan<T>, Span<T>)'
		// Assert.AreEqual([1], array);

		// workaround
		Assert.AreEqual([1], array.AsSpan());

		// or library could add [OverloadResolutionPriority]
	}
}

internal static class Assert
{
	public static void AreEqual<T>(T a, T b) { }

	public static void AreEqual<T>(ReadOnlySpan<T> a, Span<T> b) { }
}

internal static class ImplicitSpanExtensions
{
	public static void DoSomething<T>(this IEnumerable<T> enumerable) { }
	public static void DoSomething<T>(this ReadOnlySpan<T> span) { }
}
