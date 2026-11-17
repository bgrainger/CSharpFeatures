using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CSharp15;

// Microsoft.Extensions.*.Abstractions, Options, and Primitives are now in the base shared framework

internal partial class Net11Extensions
{
	public void ReadConfiguration(IConfiguration configuration)
	{
		// library code can now accept IConfiguration without a NuGet reference
		var hostName = configuration["Backend:HostName"];

		var backend = configuration.GetSection("Backend");
		var port = int.Parse(backend["Port"] ?? "443");

		var connectionString = configuration.GetConnectionString("Orders"); // reads "ConnectionStrings:Orders"

		// configuration keys are paths separated by ':'
		Console.WriteLine(ConfigurationPath.Combine("Backend", "Port")); // Backend:Port
		Console.WriteLine(ConfigurationPath.GetSectionKey("Backend:Port")); // Port

		// ConfigurationBinder.Bind(this IConfiguration, object) still requires Microsoft.Extensions.Configuration.Binder NuGet package
	}

	public void GenericLoggerMessage(ILogger logger)
	{
		// [LoggerMessage] source generator supports generic methods (e.g., to avoid boxing enums)
		PacketReceived(logger, DayOfWeek.Tuesday, 42);
	}

	[LoggerMessage(LogLevel.Information, "Received {Code} ({Length} bytes)")]
	private static partial void PacketReceived<TCode>(ILogger logger, TCode code, int length)
		where TCode : struct, Enum;
}
