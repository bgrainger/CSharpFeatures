using System.Runtime.CompilerServices;

namespace CSharp9Net5
{
	class ModuleInitializer
	{
		[ModuleInitializer]
		public static void Initialize()
		{
			System.Console.WriteLine("Module initializer");
		}
	}
}
