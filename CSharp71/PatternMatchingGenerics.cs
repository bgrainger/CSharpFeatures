using System;
using System.Globalization;
using System.IO;

namespace CSharp71
{
	public static class PatternMatchingGenerics
	{
		public static void CopyToBuffer<T>(T stream, byte[] buffer)
			where T : Stream
		{
			// C# 7.0: CS8121 An expression of type T cannot be handled by a pattern of type MemoryStream.
			// C# 7.0 workaround: if ((Stream) stream is MemoryStream memoryStream)
			if (stream is MemoryStream memoryStream)
				Buffer.BlockCopy(memoryStream.GetBuffer(), (int) memoryStream.Position, buffer, 0, buffer.Length);
			else
				stream.Read(buffer, 0, buffer.Length);
		}

		public static string ToSql<T>(T value)
		{
			switch (value) // C# 7.0 workaround: switch ((object) value)
			{
			case string str: // C# 7.0: CS8121 An expression of type T cannot be handled by a pattern of type string.
				return $"'{str.Replace("'", "''")}'";

			case bool b:
				return b ? "true" : "false";

			case IFormattable f:
				return f.ToString(null, CultureInfo.InvariantCulture);

			default:
				return value == null ? "NULL" : value.ToString();
			}
		}

		public static void GenericAsPattern<T>(string input)
		{
			if (input is T t)
				Console.WriteLine("typeof(T) must be string.");
		}
	}
}
