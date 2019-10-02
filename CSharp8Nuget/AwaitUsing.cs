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

		public async Task UsingDuckTyping()
		{
			await using var duck = new DuckTyping();

			// throws InvalidCastException
			// var disposable = (IAsyncDisposable) duck;

			// await duck.DisposeAsync called here
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

	// don't need to implement the interface, just the right method signature
	public class DuckTyping
	{
		public ValueTask DisposeAsync() => default;
	}

	public class ImplementBoth : IAsyncDisposable, IDisposable
	{
		public void Dispose()
		{
			// implement this if there's a way to dispose synchronously
			// don't just call DisposeAsync().GetAwaiter().GetResult();
		}

		public async ValueTask DisposeAsync()
		{
			// implement this if there's a way to dispose asynchronously
			// don't just call Task.Run(() => Dispose());
		}
	}
}
