using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp73
{
	class OverloadResolution
	{
		public static void StaticVsInstance()
		{
			// error CS0121: The call is ambiguous between the following methods or properties: 'Program.M(string)' and 'Program.M(Exception)'
			M(null);
		}

		// error CS0121: The call is ambiguous between the following methods or properties: 'Program.M(string)' and 'Program.M(Exception)'
		public static readonly int Value = M(null);

		public int M(Exception ex) => 0;
		public static int M(string s) => 0;

		public static Task<int> ReturnType()
		{
			// error CS0121: The call is ambiguous between the following methods or properties: 'Program.Method(Program.D1)' and 'Program.Method(Program.D2)'
			Method(Helper);

			int GetAnswer() => 42;

			// error CS0121: The call is ambiguous between the following methods or properties: 'Task.Run(Func<Task>)' and 'Task.Run<TResult>(Func<TResult>)'
			return Task.Run(GetAnswer);
		}

		delegate int D1();
		delegate string D2();

		static void Method(D1 d1) { }
		static void Method(D2 d2) { }

		static int Helper() => 1;

		public static void CompilerGeneratedCode()
		{
			// error CS0121: The call is ambiguous between the following methods or properties: 'Program.MyCollection.Add(string)' and 'Program.MyCollection.Add(Exception)'
			new MyCollection { null };
		}
	}

	class MyCollection : IEnumerable
	{
		public void Add(string s) { }
		public static void Add(Exception e) { }
		IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
	}
}
