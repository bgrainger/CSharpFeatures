using System.Runtime.CompilerServices;

namespace CSharp12
{
	internal class Interceptors
	{
		public void Method(int value)
		{
			Console.WriteLine("Original method: {0}", value);
		}

		public void Usage()
		{
			var interceptors = new Interceptors();
			interceptors.Method(1);
			interceptors.Method(2);
			interceptors.Method(3);

			// prints:
			// Original method: 1
			// Intercepted: 2
			// Intercepted: 3
		}
	}

	static file class GeneratedCode
	{
		[InterceptsLocation(@"C:\Code\Projects\CSharpFeatures\CSharp12\Interceptors.cs", line: 16, character: 17)]
		[InterceptsLocation(@"C:\Code\Projects\CSharpFeatures\CSharp12\Interceptors.cs", line: 17, character: 17)]
		public static void InterceptingMethod(this Interceptors interceptors, int value)
		{
			// currently required to be an extension method, must have same parameters and return type
			Console.WriteLine("Intercepted: {0}", value);
		}
	}
}

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public sealed class InterceptsLocationAttribute(string filePath, int line, int character) : Attribute
	{
	}
}
