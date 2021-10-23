using System.Security.Cryptography;
using System.Threading;

namespace CSharp10;

internal record class Product(string Name, string Sku, string Category, decimal Price);

internal class Net6Enhancements
{
	public IEnumerable<Product> Products = Enumerable.Empty<Product>();

	public void EnumerableExtensions()
	{
		var mostExpensive = Products.MaxBy(x => x.Price);
		var cheapest = Products.MinBy(x => x.Price);

		var onePerCategory = Products.DistinctBy(x => x.Category);

		var desiredPrices = new[] { 9.95m, 49.95m, 99.95m };
		var productsWithDesiredPrices = Products.IntersectBy(desiredPrices, x => x.Price);
		var productsWithoutDesiredPrices = Products.ExceptBy(desiredPrices, x => x.Price);

		var cheap = Products.Where(x => x.Price < 10m);
		var ebooks = Products.Where(x => x.Category == "Ebook");
		var cheapOrEbook = cheap.UnionBy(ebooks, x => x.Sku);

		// similar to EnumerateBatches, but returns arrays
		var batches = Products.Chunk(100);
		foreach (Product[] batch in batches)
		{
		}

		if (Products.TryGetNonEnumeratedCount(out var count))
		{
			// determined in constant time
		}

		var anEbook = Products.FirstOrDefault(x => x.Category == "Ebook", new("Fire Someone Today", "123", "Ebook", 0.99m));
		// same for LastOrDefault, SingleOrDefault
	}

	public void TakeRange()
	{
		var numbers = Enumerable.Range(0, 10);

		var firstTwo = numbers.Take(2);
		firstTwo = numbers.Take(..2);

		var skipTwo = numbers.Skip(2);
		skipTwo = numbers.Take(2..);

		var lastTwo = numbers.TakeLast(2);
		lastTwo = numbers.Take(^2..);

		var notLastTwo = numbers.SkipLast(2);
		notLastTwo = numbers.Take(..^2);

		var secondPage = numbers.Skip(2).Take(2);
		secondPage = numbers.Take(2..4);
	}

	public void Method(string name, string description)
	{
		ArgumentNullException.ThrowIfNull(name);
		ArgumentNullException.ThrowIfNull(description);
	}

	public async Task ParallelForEachAsync(CancellationToken cancellationToken)
	{
		var urls = new[] { "https://faithlife.com/", "https://www.logos.com", "https://beta.faithlife.com", "https://es.logos.com" };
		var client = new HttpClient();
		await Parallel.ForEachAsync(urls, cancellationToken, async (url, token) =>
		{
			var response = await client.GetAsync(url, token);
			Console.WriteLine($"{url} is {response.Content.Headers.ContentLength:n0} bytes");
		});
	}

	public async Task WaitAsync(CancellationToken token)
	{
		// result, TimeoutException, or TaskCanceledException
		await ParallelForEachAsync(token).WaitAsync(TimeSpan.FromSeconds(1), token);
	}

	public void DateOnlyTimeOnly()
	{
		var dateOnly = new DateOnly(2021, 10, 29);
		Console.WriteLine($"{dateOnly.Year} {dateOnly.Month} {dateOnly.Day}");

		var timeOnly = new TimeOnly(12, 15, 30, 534);
		Console.WriteLine($"{timeOnly.Hour} {timeOnly.Minute} {timeOnly.Second} {timeOnly.Millisecond}");

		dateOnly = DateOnly.FromDateTime(DateTime.Now);
		timeOnly = TimeOnly.FromDateTime(DateTime.Now);
		timeOnly = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay);

		var inEightHours = timeOnly.AddHours(8, out var wrappedDays);
	}

	public void RandomImprovements()
	{
		var randomNumber = Random.Shared.NextInt64();

		var bytes = RandomNumberGenerator.GetBytes(3);
	}

	public async Task Timer()
	{
		// https://stackoverflow.com/questions/10317088/why-there-are-5-versions-of-timer-classes-in-net

		var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
		while (await timer.WaitForNextTickAsync())
		{
			// do some work; single consumer only
		}

		// another thread is allowed to dispose
		timer.Dispose();
	}
}
