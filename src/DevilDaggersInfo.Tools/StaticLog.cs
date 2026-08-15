using DevilDaggersInfo.Tools.Utils;
using Serilog;
using Serilog.Core;

namespace DevilDaggersInfo.Tools;

/// <summary>
/// The one logger instance that exists before the DI container does, so that <see cref="Program"/> can log fatal
/// exceptions thrown while constructing or disposing the container. Everything else takes an <see cref="ILogger"/>
/// through the constructor; see <see cref="Container"/>.
/// </summary>
internal static class StaticLog
{
	public static readonly ILogger Log = CreateLogger();

	private static Logger CreateLogger()
	{
		LoggerConfiguration config = new LoggerConfiguration()
			.WriteTo.File(
				path: $"ddinfo-{AssemblyUtils.EntryAssemblyVersionString}.log",
				rollingInterval: RollingInterval.Infinite);

		return config.CreateLogger();
	}
}
