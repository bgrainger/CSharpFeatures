using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CSharp15;

internal partial class Net11
{
	public void BeforeNet11()
	{
		// the classic boilerplate to run a process and capture its output
		var startInfo = new ProcessStartInfo("git", ["status", "--porcelain"])
		{
			RedirectStandardOutput = true,
			RedirectStandardError = true,
		};
		using var process = Process.Start(startInfo)!;

		// must read both streams concurrently, or risk a deadlock when a pipe buffer fills up
		var errorTask = process.StandardError.ReadToEndAsync();
		var output = process.StandardOutput.ReadToEnd();
		var error = errorTask.GetAwaiter().GetResult();
		process.WaitForExit();

		Console.WriteLine($"Exit code {process.ExitCode}: {output}{error}");
	}

	public async Task RunAndCaptureText()
	{
		// .NET 11
		ProcessTextOutput result = await Process.RunAndCaptureTextAsync("git", ["status", "--porcelain"]);

		Console.WriteLine(result.StandardOutput);
		Console.WriteLine(result.StandardError);
		Console.WriteLine(result.ExitStatus.ExitCode); // 0

		// synchronous, with an optional timeout
		result = Process.RunAndCaptureText("dotnet", ["--version"], TimeSpan.FromSeconds(30));
		Console.WriteLine(result.StandardOutput); // 11.0.100
	}

	public async Task Run()
	{
		// run without capturing output (it goes to this console) and get the exit status
		ProcessExitStatus status = await Process.RunAsync("dotnet", ["--list-sdks"]);
		Console.WriteLine(status.ExitCode); // 0

		// cancellation kills the process
		using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
		status = await Process.RunAsync(new ProcessStartInfo("ping", ["-n", "10", "localhost"]), cts.Token);
		Console.WriteLine(status.Canceled); // True
	}

	public async Task ReadAllLines()
	{
		using var process = Process.Start(new ProcessStartInfo("git", ["fetch", "--verbose"])
		{
			RedirectStandardOutput = true,
			RedirectStandardError = true,
		})!;

		// interleaved lines from stdout and stderr, as they're written
		await foreach (ProcessOutputLine line in process.ReadAllLinesAsync())
			Console.WriteLine($"{(line.StandardError ? "ERR" : "OUT")}: {line.Content}");
	}

	public void ReadAllText()
	{
		using var process = Process.Start(new ProcessStartInfo("git", ["log", "-1", "--oneline"])
		{
			RedirectStandardOutput = true,
			RedirectStandardError = true,
		})!;

		// reads both streams to the end
		var (output, error) = process.ReadAllText(TimeSpan.FromSeconds(10));
		Console.WriteLine(output); // 1e78699 Fix typo in attribute name (#2)
	}

	public void StartAndForget()
	{
		// for when you don't intend to wait: returns the process ID, with no Process (or handle) to dispose
		int processId = Process.StartAndForget("notepad.exe", []);
	}

	public async Task Signals()
	{
		if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
		{
			using var process = Process.Start("sleep", "30");
			process.Signal(PosixSignal.SIGTERM);

			// ProcessExitStatus distinguishes a normal exit from termination by a signal (or cancellation)
			ProcessExitStatus status = await process.WaitForExitStatusAsync();
			Console.WriteLine(status.Signal); // SIGTERM
		}
	}

	public void HandleControl()
	{
		// choose exactly which handles the child inherits (instead of all inheritable handles)
		// and redirect stdin/stdout/stderr to handles you already have open
		using var log = File.OpenHandle("output.log", FileMode.Create, FileAccess.Write);
		var startInfo = new ProcessStartInfo("dotnet", ["--info"])
		{
			StandardOutputHandle = log,
			StandardErrorHandle = log,
		};
		Process.Run(startInfo);
	}
}
