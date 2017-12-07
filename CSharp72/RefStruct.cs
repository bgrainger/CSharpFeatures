using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSharp72
{
    public class Examples
    {
		public void UseInMethod()
		{
			var rs = new RefStruct(1);
		}

		// error CS8345: Field or auto-implemented property cannot be of type 'RefStruct' unless it is an instance member of a ref struct.
		// RefStruct cannotBeField;

		public void CannotBox()
		{
			var rs = new RefStruct();

			// error CS0029: Cannot implicitly convert type 'CSharp72.RefStruct' to 'object'
			// object boxed = rs;
			// Console.WriteLine("{0}", rs);
			// Console.WriteLine($"{rs}");
		}

		public IEnumerable<int> CannotUseInIterator()
		{
			var rs = new RefStruct(1);
			if (DateTime.Now.Second % 2 == 0)
				yield return rs.Value;
			else
				rs = new RefStruct(2);
			//  error CS4013: Instance of type 'RefStruct' cannot be used inside a nested function, query expression, iterator block or async method
			// yield return rs.Value;
		}

		public async Task CannotUseInAsyncMethod()
		{
			// error CS4012: Parameters or locals of type 'RefStruct' cannot be declared in async methods or lambda expressions.
			// var rs = new RefStruct(1);
		}

		public void CannotCaptureInLambda()
		{
			var rs = new RefStruct(1);

			// error CS8175: Cannot use ref local 'rs' inside an anonymous method, lambda expression, or query expression
			// Enumerable.Range(1, 10).Where(x => x == rs.Value);
		}

		// enables Span<T> with any kind of memory
		// enables composing Span<T> in other types
	}

	public ref struct RefStruct
		// error CS8343: 'RefStruct': ref structs cannot implement interfaces
		// : IDisposable
	{
		public RefStruct(int value) => Value = value;

		public int Value { get; }
	}
}
