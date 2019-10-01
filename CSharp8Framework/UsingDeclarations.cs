using System.Data.Common;

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

		private DbConnection CreateConnection() => null;
	}
}
