using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace CSharp9Framework
{
	public class Lambdas
	{
		public void StaticLambda(IEnumerable<int> numbers)
		{
			numbers.Select(static x => x % 3 is 0 || x % 5 is 0).ToList();
		}

		public void StaticAnonymousMethod(IEnumerable<int> numbers)
		{
			numbers.Select(static delegate(int x) { return x % 3 == 0 || x % 5 == 0; }).ToList();
		}

		public void LambdaDiscards(IEnumerable<object> first, IEnumerable<object> second)
		{
			first.Zip(second, (_, _) => 42);
		}

		public void AttributesOnLocalMethods()
		{
			// [MaybeNull] in netstandard2.1, .NET 5.0
			[return: MarshalAs(UnmanagedType.Bool)]
			static bool ReturnTrue() => true;
		}
	}
}
