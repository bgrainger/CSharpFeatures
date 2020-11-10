using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSharp9Framework
{
	class TargetTypedNew
	{
		private static readonly object s_lock = new();
		private static readonly Regex s_regex = new(@"^C# [1-9]\.0$");
		private static readonly Dictionary<Tuple<int, int>, List<KeyValuePair<string, double>>> s_complicated = new();

		private readonly Dictionary<Version, DateTime> m_versions;

		public TargetTypedNew()
		{
			m_versions = new()
			{
				[new(5, 0)] = new(2020, 11, 10),
				[new(3, 1)] = new(2019, 12, 3),
				[new(3, 0)] = new(2019, 9, 23),
				[new(2, 2)] = new(2018, 12, 4),
				[new(2, 1)] = new(2018, 5, 30),
			};
		}

		private Dictionary<int, string> m_cache;

		public void UseCache()
		{
			m_cache ??= new();
		}

		public class SomeMethodSettings
		{
			public bool HasOption { get; set; }
		}

		public void SomeMethod(string operation, SomeMethodSettings settings)
		{
			// do something
		}

		public void CallSomeMethod()
		{
			SomeMethod("test", new() { HasOption = true });
		}
	}
}
