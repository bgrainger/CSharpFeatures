#nullable enable
using System;
using System.Collections.Generic;

namespace CSharp8Framework
{
	public class NullableReferenceTypes
	{
		// warning CS8603: Possible null reference return.
		public string GetString() => null;

		public string? GetNullableString() => null;

		public void FlowAnalysis()
		{
			var str = GetNullableString();

			// warning CS8602: Dereference of a possibly null reference.
			Console.WriteLine(str.Length);

			if (str is null)
				str = "value";

			// no warning
			Console.WriteLine(str.Length);
		}

		public int ArgumentValidation(string s)
		{
			if (s is null)
				throw new ArgumentNullException(nameof(s));

			return s.Length;
		}

		public void ComposingTypes()
		{
			List<string> nonNullableListOfNonNullableString = new List<string> { "test" };
			List<string>? nullableListOfNonNullableString = null;
			List<string?> nonNullableListOfNullableString = new List<string?> { null };
			List<string?>? nullableListOfNullableString = DateTime.Now.Month == 1 ? null : new List<string?> { null };

			string[]? nullableArray = null;
			string?[] arrayOfNullableString = new string?[] { null };
			string?[]? nullableArrayOfNullableString = nullableListOfNullableString?.ToArray();
		}
	}
}
