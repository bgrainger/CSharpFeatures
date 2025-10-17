namespace CSharp14;

using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

internal enum LogLevel { Debug, Info, Warning, Error, Fatal }

internal record LogEntry(DateTime Timestamp, LogLevel Level, string Message, string? Exception = null);

internal partial class AsyncEnumerable
{
	// Convert lines to structured log entries
	public static IAsyncEnumerable<LogEntry> GetLogEntriesAsync(string filePath, CancellationToken token = default) =>
		ReadLogLinesAsync(filePath, token)
			.Select(ParseLogEntry)
			.Where(entry => entry is not null)
			.Cast<LogEntry>();

	public static async IAsyncEnumerable<(DateOnly Date, int Count)> CountWarningsAsync(string filePath, [EnumeratorCancellation] CancellationToken token = default)
	{
		await foreach (var entries in GetLogEntriesAsync(filePath, token)
			.Where(entry => entry.Level >= LogLevel.Warning)
			.DistinctBy(entry => entry.Message)
			.Take(100)
			.GroupBy(entry => DateOnly.FromDateTime(entry.Timestamp))
			.WithCancellation(token))
		{
			yield return (entries.Key, entries.Count());
		}
	}

	public async Task AnyAndAll()
	{
		var logs = GetLogEntriesAsync("app.log");

		var hasFatal = await logs.AnyAsync(e => e.Level == LogLevel.Fatal);

		var allWarningsOrHigher = await GetLogEntriesAsync("app.log")
			.AllAsync(e => e.Level >= LogLevel.Warning);
	}

	public async Task FirstAndLast()
	{
		var firstError = await GetLogEntriesAsync("app.log")
			.FirstOrDefaultAsync(e => e.Level == LogLevel.Error);
		
		var lastEntry = await GetLogEntriesAsync("app.log").LastAsync();
	}

	private static async IAsyncEnumerable<IReadOnlyList<TResult>> IterateBatchesAsync<TResult>(Func<string?, Task<(IReadOnlyList<TResult> Results, string? Next)>> getPageAsync)
	{
		string? next = null;
		do
		{
			var page = await getPageAsync(next);
			if (page.Results.Count == 0)
				yield break;
			yield return page.Results;
			next = page.Next;
		}
		while (true);
	}

	public async Task<IReadOnlyList<int>> GetAllUserGroupsAsync()
	{
		Func<string?, Task<(IReadOnlyList<UserGroup> Results, string? Next)>> getUserGroupsAsync = async next => default;

		var allGroupIds =
			from batch in IterateBatchesAsync(getUserGroupsAsync)
			from userGroup in batch
			select userGroup.GroupId;

		return await allGroupIds.ToArrayAsync();
	}

	// Simulate reading a large log file asynchronously
	private static async IAsyncEnumerable<string> ReadLogLinesAsync(string filePath, [EnumeratorCancellation] CancellationToken token = default)
	{
		// In real code: using var reader = new StreamReader(filePath);
		// Simulating with sample data
		string[] sampleLogs =
		[
			"2025-10-17 10:00:00 [INFO] Application started",
			"2025-10-17 10:00:01 [DEBUG] Loading configuration",
			"2025-10-17 10:00:02 [INFO] Database connection established",
			"2025-10-17 10:00:05 [WARNING] Slow query detected: 2.3s",
			"2025-10-17 10:00:10 [ERROR] Failed to connect to external API",
			"2025-10-17 10:00:10 [ERROR] System.Net.Http.HttpRequestException: Connection timeout",
			"2025-10-17 10:00:15 [INFO] Retry attempt 1",
			"2025-10-17 10:00:20 [ERROR] Failed to connect to external API",
			"2025-10-17 10:00:25 [FATAL] Critical service unavailable",
			"2025-10-17 10:00:30 [INFO] Application shutting down",
		];

		foreach (var line in sampleLogs)
		{
			token.ThrowIfCancellationRequested();
			await Task.Delay(10, token); // Simulate I/O delay
			yield return line;
		}
	}

	// Use .NET 9+ Regex source generator for performance
	[GeneratedRegex(@"^([0-9]{4}-[0-9]{2}-[0-9]{2}\s+[0-9]{2}:[0-9]{2}:[0-9]{2})\s+\[(\w+)\]\s+(.+)$", RegexOptions.Compiled)]
	private static partial Regex LogLineRegex { get; }

	private static LogEntry? ParseLogEntry(string line)
	{
		if (LogLineRegex.Match(line) is not { Success: true } match)
			return null;

		var timestamp = DateTime.Parse(match.Groups[1].Value);
		var level = Enum.Parse<LogLevel>(match.Groups[2].Value, ignoreCase: true);
		var message = match.Groups[3].Value;
		return new(timestamp, level, message);
	}

	private record UserGroup(int UserId, int GroupId);
}
