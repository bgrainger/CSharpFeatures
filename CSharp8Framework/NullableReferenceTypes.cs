#nullable enable
using System;

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

	}
}
