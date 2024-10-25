namespace CSharp13;

internal class RefUnsafeInIteratorAsync
{
	public async Task<int> GetSpaceIndexCSharp12Async(string str, char ch)
	{
		// avoid error CS4012: Parameters or locals of type 'ReadOnlySpan<char>' cannot be declared in async methods or async lambda expressions. 
		static int GetSpaceIndex(string str, char ch)
		{
			ReadOnlySpan<char> chars = str.AsSpan();
			return chars.IndexOf(ch);
		}

		var index = GetSpaceIndex(str, ch);
		return index;
	}

	public async Task<int> GetCharIndexAsync(string str, char ch)
	{
		// can use 'ref struct' types like Span
		ReadOnlySpan<char> chars = str.AsSpan();
		var index = chars.IndexOf(ch);

		// can use 'unsafe'
		unsafe
		{
			fixed (char* p = chars)
			{
			}
		}

		// see https://github.com/mysql-net/MySqlConnector/commit/92787d15 for a real-world example

		return index;
	}

	public async Task<int> GetCharIndexAcrossBoundaryAsync(string str, char ch)
	{
		ReadOnlySpan<char> chars = str.AsSpan();
		// await Task.Delay(1);

		// error CS4007: Instance of type 'System.ReadOnlySpan<char>' cannot be preserved across 'await' or 'yield' boundary.
		var index = chars.IndexOf(ch);

		unsafe
		{
			// error CS4004: Cannot await in an unsafe context
			// await Task.Delay(1);
		}

		return index;
	}
}
