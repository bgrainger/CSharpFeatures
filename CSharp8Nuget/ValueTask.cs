using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSharp8Nuget
{
	public class ValueTaskDemo
	{
		public async Task ConsumeValueTaskWithAwait(ValueTask<int> valueTask)
		{
			var result = await valueTask;
			Console.WriteLine(result);
		}

		public async Task ConsumeValueTaskWithAsTask(ValueTask<int> valueTask)
		{
			var task = valueTask.AsTask();

			var task2 = task.ContinueWith(x => { Console.WriteLine("continuation"); });

			var result = await task;

			await Task.WhenAll(task, task2);
		}

		public async ValueTask CompilerGeneratedStateMachine()
		{
			await Task.Delay(100);
		}

		public async ValueTask<int> EfficientUsingAsync()
		{
			if (TryGetCachedValue(out var value))
				return value;

			value = await ExpensiveGetValueAsync();
			return value;
		}

		public ValueTask<int> MoreEfficient()
		{
			if (TryGetCachedValue(out var value))
				return new ValueTask<int>(value);

			Task<int> task = ExpensiveGetValueAsync();
			return new ValueTask<int>(task);
		}

		public bool TryGetCachedValue(out int value)
		{
			value = 42;
			return true;
		}

		public Task<int> ExpensiveGetValueAsync() => Task.FromResult(0);
		public ValueTask<int> GetValueTaskAsync() => new ValueTask<int>(0);

		public async Task DoNotAwaitTwice()
		{
			ValueTask<int> valueTask = GetValueTaskAsync();

			var result = await valueTask;

			// do something

			// need it again
			// DO NOT DO THIS
			var whatWasThatResultAgain = await valueTask;
		}

		public ValueTask<int> LookUpKeyAsync(string key)
		{
			// DO NOT DO THIS
			if (!s_cache.TryGetValue(key, out var valueTask))
			{
				valueTask = GetValueTaskAsync();
				s_cache.Add(key, valueTask);
			}

			return valueTask;
		}

		static readonly Dictionary<string, ValueTask<int>> s_cache = new Dictionary<string, ValueTask<int>>();

		public void DoNotCallGetAwaiter()
		{
			ValueTask<int> valueTask = GetValueTaskAsync();

			// I need to block
			// DO NOT DO THIS
			var result = valueTask.GetAwaiter().GetResult();

			if (valueTask.IsCompleted)
			{
				// safe to do here
				var awaiter = valueTask.GetAwaiter();
			}
		}

		public async Task DoNotMixAndMatch()
		{
			ValueTask<int> valueTask = GetValueTaskAsync();

			// DO NOT DO THIS
			var task = valueTask.AsTask();
			var result = await valueTask;
			var doubleCheckResult = await task;
		}
	}
}
