using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CSharp15;

// .NET 11 "Runtime Async": the async/await state machine transformation moves from the C# compiler into the runtime (JIT)
// - opt in with <Features>runtime-async=on</Features>
// - same programming model and semantics; any observable behavior difference is a bug
// - most of the .NET 11 shared framework is already compiled this way; it may be on by default in .NET 12
// - supports Task, Task<T>, ValueTask, ValueTask<T>; async void, async iterators, and custom task-like types still use the compiler's state machine
// Source: https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-11/ (Runtime Async section)

internal class RuntimeAsync
{
	// a tiny async method
	public static async Task<int> ReadLengthAsync(Stream stream, CancellationToken cancellationToken)
	{
		var buffer = new byte[4096];
		int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
		return bytesRead;
	}

	// since C# 5, the compiler has rewritten every async method into a stub that creates and starts a state machine
	[AsyncStateMachine(typeof(ReadLengthAsyncStateMachine))]
	public static Task<int> ReadLengthAsyncLoweredByCompiler(Stream stream, CancellationToken cancellationToken)
	{
		ReadLengthAsyncStateMachine stateMachine = default;
		stateMachine.builder = AsyncTaskMethodBuilder<int>.Create();
		stateMachine.state = -1;
		stateMachine.stream = stream;
		stateMachine.cancellationToken = cancellationToken;
		stateMachine.builder.Start(ref stateMachine);
		return stateMachine.builder.Task;
	}

	// ... plus a generated type (real name: '<ReadLengthAsync>d__0') whose MoveNext contains the transformed user code;
	// parameters, locals that live across an 'await', awaiters, and the current state all become fields
	private struct ReadLengthAsyncStateMachine : IAsyncStateMachine
	{
		public int state;
		public AsyncTaskMethodBuilder<int> builder;
		public Stream stream;
		public CancellationToken cancellationToken;

		private TaskAwaiter<int> awaiter;

		public void MoveNext()
		{
			int result;
			try
			{
				TaskAwaiter<int> localAwaiter;

				if (state != 0)
				{
					byte[] buffer = new byte[4096];
					localAwaiter = stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).GetAwaiter();
					if (!localAwaiter.IsCompleted)
					{
						// not finished: remember where we were, register MoveNext as the continuation, and return
						state = 0;
						awaiter = localAwaiter;
						builder.AwaitUnsafeOnCompleted(ref localAwaiter, ref this);
						return;
					}
				}
				else
				{
					// resumed by the continuation: restore the awaiter and keep going
					localAwaiter = awaiter;
					awaiter = default;
					state = -1;
				}

				result = localAwaiter.GetResult();
			}
			catch (Exception e)
			{
				// every async method has a catch-all that stores the exception in its Task
				state = -2;
				builder.SetException(e);
				return;
			}

			state = -2;
			builder.SetResult(result);
		}

		public void SetStateMachine(IAsyncStateMachine stateMachine) => builder.SetStateMachine(stateMachine);
	}

	// with runtime-async=on, the compiler emits a small method marked 'async' in metadata (this isn't valid C#):
	//
	// [MethodImpl(MethodImplOptions.Async)]
	// public static Task<int> ReadLengthAsync(Stream stream, CancellationToken cancellationToken)
	// {
	//     var buffer = new byte[4096];
	//     int bytesRead = AsyncHelpers.Await(stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken));
	//     return bytesRead; // an 'int' from a 'Task<int>' method!
	// }
	//
	// IL:
	// .method public hidebysig static class Task`1<int32> ReadLengthAsync(...) cil managed async
	// {
	//     ...
	//     callvirt instance class Task`1<int32> Stream::ReadAsync(uint8[], int32, int32, CancellationToken)
	//     call int32 System.Runtime.CompilerServices.AsyncHelpers::Await<int32>(class Task`1<int32>)
	//     ret
	// }
	//
	// no '<ReadLengthAsync>d__0' type, no IAsyncStateMachine, no MoveNext, no AsyncTaskMethodBuilder<int>, no [AsyncStateMachine]
	// the JIT does the transformation, where it knows which locals are actually live at each suspension point

	// The runtime gives a Task-returning async method two entry points:
	// - the normal one, 'Task<int> CallerAsync()', used by ordinary callers; it's a thunk that creates the Task<int>
	// - an "AsyncCall" variant that effectively returns 'int' and has a hidden channel for a continuation
	// When one runtime-async method directly awaits another, the JIT calls the AsyncCall variant:
	// - if it completes synchronously, the 'int' flows back directly: no intermediate Task<int> allocation
	// - if it suspends, the runtime links continuations for the whole chain; only the outermost caller sees a Task
	// Existing (compiler state machine) methods still work: the runtime creates an AsyncCall adapter for them.

	// MySqlConnector
	// net10.0 - DLL is 885 KB
	// net11.0 - DLL is 755 KB

	public async Task StackTraces()
	{
		// 'new StackTrace()' (and profilers, and the debugger's Call Stack window) see the real call chain
		await RuntimeOuterAsync();
		//    at CSharp15.RuntimeAsync.RuntimeInnerAsync()
		//    at CSharp15.RuntimeAsync.RuntimeMiddleAsync()
		//    at CSharp15.RuntimeAsync.RuntimeOuterAsync()
		//    at CSharp15.RuntimeAsync.StackTraces()

		// compiler-generated state machines interleave infrastructure frames
		await ClassicOuterAsync();
		//    at CSharp15.RuntimeAsync.ClassicInnerAsync()
		//    at System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start[TStateMachine](TStateMachine& stateMachine)
		//    at CSharp15.RuntimeAsync.ClassicInnerAsync()
		//    at CSharp15.RuntimeAsync.ClassicMiddleAsync()
		//    at System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start[TStateMachine](TStateMachine& stateMachine)
		//    at CSharp15.RuntimeAsync.ClassicMiddleAsync()
		//    at CSharp15.RuntimeAsync.ClassicOuterAsync()
		//    at System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start[TStateMachine](TStateMachine& stateMachine)
		//    at CSharp15.RuntimeAsync.ClassicOuterAsync()
		//    at CSharp15.RuntimeAsync.StackTraces()

		// after a real suspension (e.g., 'await Task.Yield()'), the resumed method's callers aren't on the thread's stack in either model
	}

	private static async Task RuntimeOuterAsync() { await Task.CompletedTask; await RuntimeMiddleAsync(); }
	private static async Task RuntimeMiddleAsync() { await Task.CompletedTask; await RuntimeInnerAsync(); }
	private static async Task RuntimeInnerAsync() { await Task.CompletedTask; Console.WriteLine(new StackTrace()); }

	[RuntimeAsyncMethodGeneration(false)]
	private static async Task ClassicOuterAsync() { await Task.CompletedTask; await ClassicMiddleAsync(); }
	[RuntimeAsyncMethodGeneration(false)]
	private static async Task ClassicMiddleAsync() { await Task.CompletedTask; await ClassicInnerAsync(); }
	[RuntimeAsyncMethodGeneration(false)]
	private static async Task ClassicInnerAsync() { await Task.CompletedTask; Console.WriteLine(new StackTrace()); }
}
