using System;
using System.IO;
using System.Threading.Tasks;

namespace CSharp8Nuget
{
	public class AsyncDisposable : IAsyncDisposable
	{
		public async ValueTask DisposeAsync()
		{
			await Task.Delay(100);
		}
	}

	public class AwaitUsing
	{
		public async Task Using()
		{
			await using (var obj = new AsyncDisposable())
			{
				// "await DisposeAsync" will be called at the end of this block
			}

			// C# 8 style
			await using var obj2 = new AsyncDisposable();

			// "await DisposeAsync" will be called at the end of this scope
		}

		public async Task InBclInNetStandard21()
		{
			// requires netstandard2.1 BCL; can't use a NuGet package
#if NETSTANDARD2_1
			await using var file = File.OpenRead(@"C:\Windows\system.ini");
			await using var memory = new MemoryStream();
			await file.CopyToAsync(memory);
#endif
		}
	}
}
