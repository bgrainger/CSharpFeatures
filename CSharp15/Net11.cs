using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace CSharp15;

internal partial class Net11
{
	public void LinqJoins()
	{
		(int Id, string Name)[] products = [(1, "Laptop"), (2, "Mouse"), (3, "Orphan")];
		(int ProductId, int Quantity)[] orders = [(1, 3), (1, 1), (2, 5), (4, 9)];

		// .NET 11: Join without a result selector returns tuples
		foreach (var (product, order) in products.Join(orders, p => p.Id, o => o.ProductId))
			Console.WriteLine($"{product.Name}: {order.Quantity}");
		// Laptop: 3
		// Laptop: 1
		// Mouse: 5

		// .NET 10 added LeftJoin and RightJoin; .NET 11 adds FullJoin
		foreach (var (product, order) in products.FullJoin(orders, p => p.Id, o => o.ProductId))
			Console.WriteLine($"{product.Name ?? "(none)"}: {order.Quantity}");
		// Laptop: 3
		// Laptop: 1
		// Mouse: 5
		// Orphan: 0
		// (none): 9

		// GroupJoin without a result selector returns groupings keyed by the outer element
		foreach (var productOrders in products.GroupJoin(orders, p => p.Id, o => o.ProductId))
			Console.WriteLine($"{productOrders.Key.Name}: {productOrders.Sum(x => x.Quantity)}");
		// Laptop: 4
		// Mouse: 5
		// Orphan: 0

		// all available on Enumerable, Queryable, and AsyncEnumerable
	}

	public void EqualityComparerCreate()
	{
		// .NET 8: EqualityComparer<T>.Create(equals, getHashCode)
		// .NET 11: create from a key selector (and optional key comparer)
		var byName = EqualityComparer<Person>.Create(x => x?.Name, StringComparer.OrdinalIgnoreCase);

		// (with a C# 15 collection expression argument)
		HashSet<Person> people = [with(byName), new("Alice", 30), new("Bob", 25), new("ALICE", 40)];
		Console.WriteLine(people.Count); // 2
	}

	public void StringBuilderMoveChunks()
	{
		var source = new StringBuilder("Hello, ");
		source.Append("World!");

		// moves the chunks instead of copying characters; the source is left empty
		StringBuilder destination = StringBuilder.MoveChunks(source);
		Console.WriteLine(destination); // Hello, World!
		Console.WriteLine(source.Length); // 0

		// main reason is for source generators: https://andrewlock.net/exploring-the-dotnet-11-preview-3-avoiding-tostring-allocations-with-stringbuilder-movechunks/
		// or any API where you want to transfer ownership of a StringBuilder
	}

	public void RuneMethods()
	{
		// Rune is a Unicode scalar value; emoji don't fit in a single 'char'
		var thumbsUp = new Rune(0x1F44D);
		string text = "good 👍 great 👍 grand";

		Console.WriteLine(text.Contains(thumbsUp)); // True
		Console.WriteLine(text.IndexOf(thumbsUp)); // 5
		Console.WriteLine(string.Join(" | ", text.Split(thumbsUp, StringSplitOptions.TrimEntries))); // good | great | grand
		Console.WriteLine(text.Replace(thumbsUp, new Rune('+'))); // good + great + grand
		Console.WriteLine("👍👍yes👍".Trim(thumbsUp)); // yes

		// also: StartsWith(Rune), EndsWith(Rune), LastIndexOf(Rune), TextInfo.ToUpper(Rune)
		Console.WriteLine(CultureInfo.InvariantCulture.TextInfo.ToUpper(new Rune('ß'))); // ß

		// char.Equals with a StringComparison
		Console.WriteLine('a'.Equals('A', StringComparison.OrdinalIgnoreCase)); // True
	}

	public void UtfValidation()
	{
		// find where invalid data is, instead of just "invalid"
		ReadOnlySpan<byte> utf8 = [0x48, 0x69, 0xC3, 0x28]; // "Hi" + invalid two-byte sequence
		Console.WriteLine(Utf8.IndexOfInvalidSubsequence(utf8)); // 2

		Console.WriteLine(Utf16.IsValid("valid 👍")); // True
		Console.WriteLine(Utf16.IsValid("lone surrogate \uD83D")); // False
	}

	public void HexFloatingPoint()
	{
		// round-trip every bit of a double through text; matches C's printf("%a")
		double value = Math.PI;
		string hex = value.ToString("X");
		Console.WriteLine(hex); // 0X1.921FB54442D18P+1
		Console.WriteLine(double.Parse(hex, NumberStyles.HexFloat) == value); // True
	}

	public void RegexAnyNewLine()
	{
		string text = "line1\r\nline2\u0085line3\u2028line4\nline5";

		// '$' only matches before '\n', so "line1\r" doesn't end with a digit
		Console.WriteLine(Regex.Matches(text, @"^line\d$", RegexOptions.Multiline).Count); // 1

		// .NET 11: ^, $, and . understand \r\n, \r, \u0085, \u2028, \u2029
		Console.WriteLine(Regex.Matches(text, @"^line\d$", RegexOptions.Multiline | RegexOptions.AnyNewLine).Count); // 5
	}

	public void DecimalFloatingPoint()
	{
		// IEEE 754-2019 decimal floating point: Decimal32, Decimal64, Decimal128 (not the same as System.Decimal)
		Console.WriteLine(0.1 + 0.2 == 0.3); // False (binary floating point)
		Console.WriteLine(Decimal64.Parse("0.1") + Decimal64.Parse("0.2") == Decimal64.Parse("0.3")); // True

		// supports generic math, infinity, NaN
		Console.WriteLine(Decimal64.One / Decimal64.Zero); // ∞

		// Binary (Intel) vs Decimal (IBM) encodings
		Decimal64.EncodeBinary(Decimal64.One).ToString("X"); // 31C0000000000001
		Decimal64.EncodeDecimal(Decimal64.One).ToString("X"); // 2238000000000001
	}

	public void InMemoryStreams()
	{
		// Before .NET 11: copy into a MemoryStream (or write your own Stream wrapper)
		using var oldStream = new MemoryStream(Encoding.UTF8.GetBytes("key: value"));

		// .NET 11: wrap existing data as a Stream without copying
		using Stream text = new StringStream("key: value", Encoding.UTF8);
		Console.WriteLine(new StreamReader(text).ReadToEnd()); // key: value

		ReadOnlyMemory<byte> payload = "{}"u8.ToArray();
		using Stream readOnly = new ReadOnlyMemoryStream(payload);

		// fixed-size writable stream over a buffer you own
		byte[] buffer = new byte[16];
		using Stream writable = new WritableMemoryStream(buffer);
		writable.Write("hello"u8);
	}

	public void Zstandard()
	{
		// Zstandard is now in the box, alongside Deflate, GZip, ZLib, and Brotli
		byte[] data = Encoding.UTF8.GetBytes(string.Concat(Enumerable.Repeat("Zstandard! ", 100)));

		using var compressed = new MemoryStream();
		using (var zstd = new ZstandardStream(compressed, CompressionLevel.Optimal, leaveOpen: true))
			zstd.Write(data);
		Console.WriteLine($"{data.Length} -> {compressed.Length} bytes"); // 1100 -> 27 bytes
	}

	public void SpanBasedCompression()
	{
		// encode/decode spans without a Stream
		ReadOnlySpan<byte> source = Encoding.UTF8.GetBytes(string.Concat(Enumerable.Repeat("Hello ", 100)));
		byte[] destination = new byte[ZLibEncoder.GetMaxCompressedLength(source.Length)];

		if (ZLibEncoder.TryCompress(source, destination, out var bytesWritten))
			Console.WriteLine($"TryCompress: {source.Length} -> {bytesWritten} bytes"); // TryCompress: 600 -> 23 bytes

		using var encoder = new ZLibEncoder();
		var status = encoder.Compress(source, destination, out int bytesConsumed, out bytesWritten, isFinalBlock: true);
		Console.WriteLine($"{status}: {bytesConsumed} -> {bytesWritten} bytes"); // Done: 600 -> 23 bytes

		encoder.Reset();

		// also DeflateEncoder/Decoder, GZipEncoder/Decoder, BrotliEncoder/Decoder, ZstandardEncoder/Decoder
	}

	public async Task RequestCompression(HttpClient httpClient, Uri uri)
	{
		// compress the request body; sets the Content-Encoding header and compresses as the content is serialized
		using HttpContent content = new GZipCompressedContent(JsonContent.Create(new { Name = "Logos", Version = 55 }), CompressionLevel.Fastest);
		using var response = await httpClient.PostAsync(uri, content);

		// also BrotliCompressedContent and ZstandardCompressedContent
	}

	public async Task DnsRecords()
	{
		// Before .NET 11: only A/AAAA (Dns.GetHostAddresses); SRV, MX, TXT, etc. needed a third-party library
		DnsResult<MxRecord> mx = await Dns.ResolveMxAsync("logos.com");
		Console.WriteLine(mx.ResponseCode); // NoError
		foreach (var record in mx.Records)
			Console.WriteLine($"{record.Preference} {record.Exchange} (TTL {record.Ttl})");

		// query specific DNS servers instead of the OS configuration
		// WARNING: on Windows, NRPT rules override Servers; with Tailscale DNS enabled (an NRPT rule for "."),
		// the query goes to Tailscale anyway, which returns NxDomain for this name
		var resolver = new DnsResolver(new DnsResolverOptions { Servers = { new IPEndPoint(IPAddress.Parse("1.1.1.1"), 53) } });
		DnsResult<SrvRecord> srv = await resolver.ResolveSrvAsync("_xmpp-server._tcp.logos.com");
		foreach (var record in srv.Records)
			Console.WriteLine($"{record.Target}:{record.Port}");

		// also ResolveTxt, ResolveCName, ResolvePtr, ResolveNs; results include the response code and negative-cache TTL

		DnsResult<TxtRecord> txt = await resolver.ResolveTxtAsync("logos.com");
		foreach (var record in txt.Records)
			Console.WriteLine(string.Join("", record.Values));
	}

	public void StringSyntax()
	{
		// StringSyntaxAttribute.CSharp, .FSharp, .VisualBasic for syntax highlighting of string literals
		Evaluate("""
			/* this is code */ 
			Console.WriteLine(42);

			class C { void M() { } }
			""");

		static void Evaluate([StringSyntax(StringSyntaxAttribute.CSharp)] string code) { }
	}

	public async Task AsyncValidation()
	{
		// System.ComponentModel.DataAnnotations can validate asynchronously (e.g., a database lookup)
		var user = new NewUser { UserName = "admin" };
		var results = new List<ValidationResult>();
		bool isValid = await Validator.TryValidateObjectAsync(user, new ValidationContext(user), results, validateAllProperties: true);
		Console.WriteLine($"{isValid}: {string.Join(", ", results.Select(x => x.ErrorMessage))}"); // False: That user name is taken.

		// ValidationAttribute.FormatMessage
		Console.WriteLine(new RangeAttribute(1, 10).FormatMessage("{0} must be between {1} and {2}", "Value")); // Value must be between 1 and 10
	}

	private sealed record Person(string Name, int Age);

	private sealed class NewUser
	{
		[Required]
		[UniqueUserName]
		public string? UserName { get; set; }
	}

	private sealed class UniqueUserNameAttribute : AsyncValidationAttribute
	{
		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) =>
			throw new InvalidOperationException("Use IsValidAsync.");

		protected override async Task<ValidationResult?> IsValidAsync(object? value, ValidationContext validationContext, CancellationToken cancellationToken)
		{
			// pretend to query the database
			await Task.Delay(10, cancellationToken);
			return value is "admin" ? new ValidationResult("That user name is taken.") : ValidationResult.Success;
		}
	}
}
