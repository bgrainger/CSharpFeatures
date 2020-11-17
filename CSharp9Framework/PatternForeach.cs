using System;
using System.Text;

namespace CSharp9Framework
{
	public static class PatternForeach
	{
		public static EnumeratorType GetEnumerator(this StringBuilder sb) => new(sb);

		public class EnumeratorType
		{
			public EnumeratorType(StringBuilder sb) => m_sb = sb;
			public bool MoveNext() => (++m_pos < m_sb.Length);
			public char Current => m_sb[m_pos];

			private readonly StringBuilder m_sb;
			private int m_pos = -1;
		}

		public static void UsePattern()
		{
			var sb = new StringBuilder("test");
			foreach (var ch in sb)
			{
				Console.WriteLine(ch);
			}
		}
	}
}
