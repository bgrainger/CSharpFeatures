using System;
using System.Threading.Tasks;

namespace CSharp71
{
	class Program
	{
		static int CSharp7Main(string[] args)
		{
			async Task<int> RealMain()
			{
				await Console.Out.WriteLineAsync("Hello world");
				return 0;
			}

			return RealMain().GetAwaiter().GetResult();
		}

		static int CSharp7MainExpression(string[] args) => RealMain(args).GetAwaiter().GetResult();

		static async Task<int> RealMain(string[] args)
		{
			await Console.Out.WriteLineAsync("Hello world");
			return 0;
		}

		static async Task<int> Main(string[] args)
		{
			await Console.Out.WriteLineAsync("Hello world");
			return 0;
		}
	}
}
