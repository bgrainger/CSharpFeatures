namespace CSharp9Framework
{
	public partial class PartialMethods
	{
		// C# 8 partial method
		partial void Method1();

		public void Test()
		{
			// call to non-implemented partial method is elided at compile time
			Method1();
		}

		public partial void Method2();

		private partial void Method3();

		public partial string Method4();

		public partial void Method5(out string s);
	}

	// The following code would be code-generated.
	public partial class PartialMethods
	{
		public partial void Method2() { }

		private partial void Method3() { }

		public partial string Method4() => "test";

		public partial void Method5(out string s) => s = "test";
	}
}
