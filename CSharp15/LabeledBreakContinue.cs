namespace CSharp15;

internal class LabeledBreakContinue
{
	public (int Row, int Column)? FindFirstNegativeWithFlag(int[][] grid)
	{
		(int Row, int Column)? result = null;

		// Boolean flag that's checked at every level
		bool found = false;
		for (int row = 0; row < grid.Length && !found; row++)
		{
			for (int column = 0; column < grid[row].Length; column++)
			{
				if (grid[row][column] < 0)
				{
					result = (row, column);
					found = true;
					break;
				}
			}
		}

		return result;
	}

	public (int Row, int Column)? FindFirstNegativeWithGoto(int[][] grid)
	{
		(int Row, int Column)? result = null;

		for (int row = 0; row < grid.Length; row++)
		{
			for (int column = 0; column < grid[row].Length; column++)
			{
				if (grid[row][column] < 0)
				{
					result = (row, column);
					goto done;
				}
			}
		}

	done:
		return result;
	}

	public (int Row, int Column)? FindFirstNegativeWithReturn(int[][] grid)
	{
		for (int row = 0; row < grid.Length; row++)
		{
			for (int column = 0; column < grid[row].Length; column++)
			{
				if (grid[row][column] < 0)
				{
					return (row, column);
				}
			}
		}

		return default;
	}


	public (int Row, int Column)? FindFirstNegative(int[][] grid)
	{
		(int Row, int Column)? result = null;

		// C# 15: 'break label' exits the labeled loop
		rows: for (int row = 0; row < grid.Length; row++)
		{
			for (int column = 0; column < grid[row].Length; column++)
			{
				if (grid[row][column] < 0)
				{
					result = (row, column);
					break rows;
				}
			}
		}

		return result;
	}

	public IEnumerable<string> ContinueOuter(IEnumerable<string> lines, IReadOnlyList<string> forbiddenWords)
	{
		// 'continue label' starts the next iteration of the labeled loop
		lines: foreach (var line in lines)
		{
			foreach (var word in forbiddenWords)
			{
				if (line.Contains(word, StringComparison.OrdinalIgnoreCase))
					continue lines;
			}

			yield return line;
		}
	}

	public int BreakOutOfLoopFromSwitch(IEnumerable<string> tokens)
	{
		int sum = 0;

		tokens: foreach (var token in tokens)
		{
			switch (token)
			{
			case "end":
				break tokens;

			case "skip":
				continue tokens;

			default:
				sum += int.Parse(token);
				break;
			}

			Console.WriteLine("after switch");
		}

		return sum;
	}
}
