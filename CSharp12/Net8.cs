using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CSharp12;

internal class Net8
{
	public void ReplaceLibronixUtility()
	{
		// replaces EnumerableUtility
		var pairs = new KeyValuePair<string, int>[] { new("one", 1), new("two", 2), new("three", 3) };
		var dictionary = pairs.ToDictionary();

		// replaces GenericEqualityComparer
		var comparer = EqualityComparer<int>.Create((left, right) => left == right);

		// replaces ListUtility.CreateReadOnlyCollection<T>()
		var collection = ReadOnlyCollection<int>.Empty;
	}

	public void ThrowHelpers(string notNull, int value)
	{
		// annotated with [[CallerArgumentExpression] and [DoesNotReturn] as necessary
		ArgumentNullException.ThrowIfNull(notNull);
		ArgumentOutOfRangeException.ThrowIfLessThan(value, 3);
		ArgumentException.ThrowIfNullOrWhiteSpace(notNull);
	}

	public async Task TimeProviderAbstraction()
	{
		// tests can use https://www.nuget.org/packages/Microsoft.Extensions.TimeProvider.Testing
		var timeProvider = TimeProvider.System;
		var start = timeProvider.GetTimestamp();
		await Task.Delay(TimeSpan.FromSeconds(30), timeProvider);
		var now = timeProvider.GetLocalNow();
		now = timeProvider.GetUtcNow();
		var elapsed = timeProvider.GetElapsedTime(start);
	}

	public void DeconstructDateTime()
	{
		var now = DateTime.Now;
		var (date, time) = now;
		var (year, month, day) = date;
		var (hour, minute, second) = time;
	}

	public System.Web.IHtmlString GetHtmlString() => default!;

	public void RandomMethods(Card[] cards)
	{
		// draws with replacement -- don't use this for most card games!
		var drawThree = Random.Shared.GetItems(cards, 3);

		// new method to shuffle in-place
		Random.Shared.Shuffle(cards);
		drawThree = cards.Take(3).ToArray();
	}

	public void FrozenTypes()
	{
		// "Frozen" collections are immutable and thread-safe and optimized for reading
		var frozenDictionary = new Dictionary<string, int> { { "one", 1 }, { "two", 2 } }.ToFrozenDictionary();
		var frozenSet = frozenDictionary.Keys.ToFrozenSet();
	}

	public void FasterReflection()
	{
		// invoke private DateTime(ulong) constructor
		var constructorInfo = typeof(DateTime).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, [ typeof(ulong) ])!;
		var constructorInvoker = ConstructorInvoker.Create(constructorInfo); // cache this
		var dateTime = (DateTime) constructorInvoker.Invoke(((ulong)DateTime.UtcNow.Ticks) | 0x4000_0000_0000_0000UL);

		// invoke private DateTime.ValidateLeapSecond() method
		var methodInfo = dateTime.GetType().GetMethod("ValidateLeapSecond", BindingFlags.NonPublic | BindingFlags.Instance)!;
		var methodInvoker = MethodInvoker.Create(methodInfo);
		methodInvoker.Invoke(dateTime);
	}

	public void AotReflection()
	{
		var dateTime = CreateDateTime(((ulong) DateTime.UtcNow.Ticks) | 0x4000_0000_0000_0000UL);

		// pass by ref because this is a struct (so that a copy isn't made)
		ValidateLeapSecond(ref dateTime);

		ref ulong data = ref DateData(ref dateTime);
		data = 0UL;

		// throws MissingMethodException
		MissingMethod(ref dateTime);
	}

	[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
	private static extern DateTime CreateDateTime(ulong ticks);

	[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "ValidateLeapSecond")]
	private static extern void ValidateLeapSecond(ref DateTime dateTime);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_dateData")]
	private static extern ref ulong DateData(ref DateTime dateTime);

	[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "ThisMethodDoesNotExist")]
	private static extern void MissingMethod(ref DateTime dateTime);

	// ... and much more; see Stephen Toub's .NET post for Vector512, IUtf8SpanFormattable, CompositeFormat, JsonNamingPolicy, SearchValues, Parallel.ForAsync, Ascii, and more
	// https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-8/

	// ... and Stephen Cleary for ConfigureAwaitOptions
	// https://blog.stephencleary.com/2023/11/configureawait-in-net-8.html

	public sealed record class Card(int Suit, int Value);
}
