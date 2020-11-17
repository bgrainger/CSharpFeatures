using System.Collections.Generic;
using System.Linq;

namespace CSharp9Framework
{
	public class Lambdas
	{
		public void StaticLambda(IEnumerable<int> numbers)
		{
			numbers.Select(static x => x % 3 is 0 || x % 5 is 0).ToList();
		}

		public void LambdaDiscards(IEnumerable<object> first, IEnumerable<object> second)
		{
			first.Zip(second, (_, _) => 42);
		}
	}
}
