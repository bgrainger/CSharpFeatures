using System.Collections.Generic;
using System.Linq;

namespace CSharp9Framework
{
	public class StaticLambdas
	{
		public void StaticLambda(IEnumerable<int> numbers)
		{
			numbers.Select(static x => x % 3 is 0 || x % 5 is 0).ToList();
		}

		public void StaticAnonymousMethod(IEnumerable<int> numbers)
		{
			var threes = numbers.Select(IsMultipleOfThree);
			var fives = numbers.Select(IsMultipleOfFive);

			static bool IsMultipleOfThree(int x) => x % 3 == 0;
			static bool IsMultipleOfFive(int x) => x % 5 == 0;
		}
	}
}
