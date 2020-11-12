using System;

namespace CSharp9Framework
{
	public unsafe class FunctionPointers
	{
		public static void AnAction(string s) { }
		public static int AFunction(string s) => s.Length;

		public delegate int ADelegate(string s);
		public delegate void AVoidDelegate(string s);

		public void DelegateSyntax()
		{
			Func<string, int> f = s => s.Length;
			ADelegate d1 = delegate (string s) { return s.Length; };
			ADelegate d2 = AFunction;
			delegate*<string, int> fp1 = &AFunction;

			Action<string> a = AnAction;
			AVoidDelegate d3 = AnAction;
			delegate*<string, void> fp2 = &AnAction;
		}

		public void InvokeDelegate()
		{
			Func<string, int> func = AFunction;
			func("test");
			func.Invoke("test"); // callvirt System.Func<string, int>::Invoke

			delegate*<string, int> fp = &AFunction;
			fp("test"); // calli
		}

		// delegate* unmanaged[Cdecl, SuppresGCTransition]<int, int> attributes;
	}
}
