using System.Threading.Tasks;

namespace CSharp8Nuget
{
	public class ValueTaskDemo
	{
		public async ValueTask Method()
		{
			await Task.Delay(100);
		}

		public ValueTask<int> MoreEfficient()
		{
			if (TryGetCachedValue(out var value))
				return new ValueTask<int>(value);

			Task<int> task = ExpensiveGetValueAsync();
			return new ValueTask<int>(task);
		}

		public async ValueTask<int> EfficientUsingAsync()
		{
			if (TryGetCachedValue(out var value))
				return value;

			value = await ExpensiveGetValueAsync();
			return value;
		}

		public bool TryGetCachedValue(out int value)
		{
			value = 42;
			return true;
		}

		public Task<int> ExpensiveGetValueAsync() => Task.FromResult(0);
	}
}
