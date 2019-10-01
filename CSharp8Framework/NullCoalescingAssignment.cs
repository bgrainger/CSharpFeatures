using System.Collections.Generic;

namespace CSharp8Framework
{
	public class NullCoalescingAssignment
	{
		public int GetLength(string s)
		{
			s ??= "";
			return s.Length;
		}

		public static List<string> LazyList => s_list ??= new List<string>();

		private static List<string> s_list;
	}
}
