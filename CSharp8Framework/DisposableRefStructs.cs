using System;

namespace CSharp8Framework
{
	public class DisposableRefStructs
	{
		ref struct RefStruct
			// error CS8343: ref structs cannot implement interfaces
			// : IDisposable
		{
			public void Dispose() => Console.WriteLine("Disposed!");
		}

		public void Method()
		{
			// uses "Duck Typing", not IDisposable
			using var rs = new RefStruct();

			// Dispose is called at end of method
		}

		public void NotDuckTypingInGeneral()
		{
			// error CS1674: type used in a using statement must be implicitly convertible to 'System.IDisposable' or implement a suitable 'Dispose' method.
			// using var s = new Struct();

			// "or implement a suitable 'Dispose' method" is a compiler bug: https://github.com/dotnet/roslyn/issues/33746
		}

		struct Struct
		{
			public void Dispose() => Console.WriteLine("Disposed!");
		}
	}
}
