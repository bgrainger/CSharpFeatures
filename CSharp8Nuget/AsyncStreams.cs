using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CSharp8Nuget
{
	public class AsyncStreams
	{
		public async IAsyncEnumerable<int> Generating()
		{
			for (int i = 0; i < 10; i++)
			{
				await Task.Delay(i * 10);
				yield return i;
			}
		}

		public async Task Consuming()
		{
			await foreach (var i in Generating())
			{
				Console.WriteLine(i);
			}
		}

		public async IAsyncEnumerable<T> ExecuteSqlAsync<T>(string sql)
		{
			using var connection = CreateConnection(); // await using in .NET Core 3.0
			await connection.OpenAsync();
			using var command = connection.CreateCommand(); // await using in .NET Core 3.0
			command.CommandText = sql;
			using var reader = await command.ExecuteReaderAsync(); // await using in .NET Core 3.0
			while (await reader.ReadAsync())
				yield return await reader.GetFieldValueAsync<T>(0);
		}

		public async Task StreamDatabaseResults()
		{
			await foreach (var userId in ExecuteSqlAsync<int>("select user_id from users;"))
			{
				// ...
			}
		}

		// IAsyncEnumerator<T> IAsyncEnumerable.GetAsyncEnumerator(CancellationToken)

		public async IAsyncEnumerable<T> ExecuteSqlAsync2<T>(string sql, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			using var connection = CreateConnection(); // await using in .NET Core 3.0
			await connection.OpenAsync(cancellationToken);
			using var command = connection.CreateCommand(); // await using in .NET Core 3.0
			command.CommandText = sql;
			using var reader = await command.ExecuteReaderAsync(cancellationToken); // await using in .NET Core 3.0
			while (await reader.ReadAsync(cancellationToken))
				yield return await reader.GetFieldValueAsync<T>(0);
		}

		public async Task StreamDatabaseResults2(CancellationToken cancellationToken)
		{
			await foreach (var userId in ExecuteSqlAsync2<int>("select user_id from users;", cancellationToken).WithCancellation(cancellationToken))
			{
				// ...
			}
		}

		public IAsyncEnumerable<T> ExecuteSqlAsync3<T>(string sql) => ExecuteSqlAsync2<T>(sql);

		public async Task StreamDatabaseResults3(CancellationToken cancellationToken)
		{
			var asyncStream = ExecuteSqlAsync3<int>("select user_id from users;");

			await foreach (var userId in asyncStream.WithCancellation(cancellationToken))
			{
				// ...
			}
		}

		public IAsyncEnumerable<int> ManualImplementation(int count) => new AsyncEnumerable(count);

		class AsyncEnumerable : IAsyncEnumerable<int>
		{
			public AsyncEnumerable(int count) => m_count = count;

			public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken) =>
				new AsyncEnumerator(m_count, cancellationToken);

			readonly int m_count;
		}

		class AsyncEnumerator : IAsyncEnumerator<int>
		{
			public AsyncEnumerator(int count, CancellationToken cancellationToken)
			{
				m_count = count;
				m_cancellationToken = cancellationToken;
				m_index = -1; // before beginning of sequence
			}

			public int Current => m_index;

			public ValueTask DisposeAsync() => default;

			public async ValueTask<bool> MoveNextAsync()
			{
				m_cancellationToken.ThrowIfCancellationRequested();
				m_index++;

				// check for end of sequence
				if (m_index == m_count)
					return false;

				await Task.Delay(m_index * 10, m_cancellationToken);
				return true;
			}

			readonly int m_count;
			readonly CancellationToken m_cancellationToken;
			int m_index;
		}

		private static DbConnection CreateConnection() => null;
	}
}
