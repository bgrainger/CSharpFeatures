using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;

namespace CSharp8Framework
{
	public class PatternMatching
	{
		// switch on type, discarding the variable
		public string SwitchOnType(Stream stream) =>
			stream switch
			{
				MemoryStream _ => "memory",
				FileStream _ => "file",
				NetworkStream _ => "network",
				{ } => "other",
				null => throw new ArgumentNullException(nameof(stream)),
			};

		// add a 'when' clause
		public bool IsAsynchronous(Stream stream) =>
			stream switch
			{
				FileStream f when f.IsAsync => true,
				MemoryStream _ => false,
				NetworkStream _ => true,
				DeflateStream d => IsAsynchronous(d.BaseStream),
				_ => false,
			};

		// can also write in C# 7 style
		public bool SwitchStatement(Stream stream)
		{
			switch (stream)
			{
			case FileStream f when f.IsAsync:
				return true;
			case MemoryStream _:
				return false;
			case NetworkStream _:
				return true;
			case DeflateStream d:
				return SwitchStatement(d.BaseStream);
			default:
				return false;
			}
		}

		// pattern matching tuples can match constants or introduce variables
		public string SwitchOnTuple(string input)
		{
			var success = int.TryParse(input, out var value);
			return (success, value) switch
			{
				(false, _) => "invalid input",
				var (_, v) when v == 0 => "zero",
				var (_, v) when v < 0 => "negative",
				var (_, v) when v > 0 => "positive",
				// warning CS8509: The switch expression does not handle all possible inputs (it is not exhaustive).
				// generated code throws InvalidOperationException if unhandled
				// _ => "other",
			};
		}

		public string SwitchOnProperty(DateTimeOffset dateTime) =>
			dateTime switch
			{
				{ Day: 1 } => "first of the month",
				{ Year: 2019 } => "this year",
				{ Month: 10 } => "October",
				{ DayOfWeek: DayOfWeek.Monday } => "Monday",
				{ DayOfWeek: DayOfWeek.Friday, Day: 13 } => throw new NotSupportedException("Friday the 13th"),
				{ Month: 12, Day: 25 } => "Christmas",
				// { Month: 1, Day: 1 } => "New Year's Day", // error CS8510: The pattern has already been handled by a previous arm of the switch expression.
				{ Offset: { Hours: 0, Minutes: 0 } } => "UTC",
				_ => "other",
			};

		public string SwitchTypeAndProperty(object obj) =>
			obj switch
			{
				DateTime { Kind: DateTimeKind.Utc } => "UTC",
				DateTime { Kind: DateTimeKind.Local } => "Local",
				DateTimeOffset { Offset: TimeSpan offset } when offset == TimeSpan.Zero => "UTC",
				DateTimeOffset _ => "Local",
				_ => "Unspecified",
			};
	}
}
