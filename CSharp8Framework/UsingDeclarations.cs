using System;
using System.Data.Common;
using System.Threading;

namespace CSharp8Framework
{
	public class UsingDeclarationsExample
	{
		public void UsingStatements()
		{
			using (var connection = CreateConnection())
			{
				connection.Open();
				using (var command = connection.CreateCommand())
				{
					command.CommandText = "SELECT field FROM table;";
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
						}
					}
				}
			}
		}

		public void UsingDeclarations()
		{
			using var connection = CreateConnection();
			connection.Open();
			using var command = connection.CreateCommand();
			command.CommandText = "SELECT field FROM table;";
			using var reader = command.ExecuteReader();
			while (reader.Read())
			{
			}
		}

		public void NotAnonymous(CancellationToken token)
		{
			//  error CS1001: Identifier expected
			// using token.Register(() => Console.WriteLine("was cancelled"));

			using var needsANameHere = token.Register(() => Console.WriteLine("was cancelled"));
		}

		public void CannotUseExistingVariable()
		{
			// error CS1001: Identifier expected
			// using m_connection;
			// m_connection.Open();

			using var needsANameHere = m_connection;
		}

		private DbConnection CreateConnection() => null;

		private DbConnection m_connection;
	}
}
