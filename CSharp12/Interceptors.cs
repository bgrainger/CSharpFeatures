using System.Data.Common;
using System.Runtime.CompilerServices;

namespace CSharp12
{
	internal class Interceptors
	{
		public void Method(int value)
		{
			Console.WriteLine("Original method: {0}", value);
		}

		public void Usage()
		{
			var interceptors = new Interceptors();
			interceptors.Method(1);
			interceptors.Method(2);
			interceptors.Method(3);

			// prints:
			// Original method: 1
			// Intercepted: 2
			// Intercepted: 3
		}
	}

	static file class GeneratedCode
	{
		[InterceptsLocation(@"C:\Code\Projects\CSharpFeatures\CSharp12\Interceptors.cs", line: 17, character: 17)]
		[InterceptsLocation(@"C:\Code\Projects\CSharpFeatures\CSharp12\Interceptors.cs", line: 18, character: 17)]
		public static void InterceptingMethod(this Interceptors interceptors, int value)
		{
			// currently required to be an extension method, must have same parameters and return type
			Console.WriteLine("Intercepted: {0}", value);
		}
	}

	public record class Person(string Name, int Age);

	public class DatabaseExample
	{
		public void DatabaseQuery()
		{
			// see https://github.com/DapperLib/DapperAOT for an actual working implementation of this
			DbConnection connection = default!;
			var people = connection.Query<Person>(@"select Name, Age from people").ToList();
		}
	}

	static file class DapperGeneratedCode
	{
		[InterceptsLocation(@"C:\Code\Projects\CSharpFeatures\CSharp12\Interceptors.cs", line: 46, character: 28)]
		public static IEnumerable<Person> Query(this DbConnection connection, string sql)
		{
			if (connection.State != System.Data.ConnectionState.Open)
				connection.Open();
			using var command = connection.CreateCommand();
			command.CommandText = sql;
			using var reader = command.ExecuteReader();
			var results = new List<Person>();
			while (reader.Read())
			{
				var name = reader.GetString(0);
				var age = reader.GetInt32(1);
				results.Add(new Person(name, age));
			}
			return results;
		}
	}

	public static class DapperConnectionExtensions
	{
		public static IEnumerable<T> Query<T>(this DbConnection connection, string sql) => default;
	}
}

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public sealed class InterceptsLocationAttribute(string filePath, int line, int character) : Attribute
	{
	}
}
