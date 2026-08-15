using DevilDaggersInfo.Tools.Utils;
using Serilog;
using Serilog.Core;

namespace DevilDaggersInfo.Tools;

internal static class StaticLog
{
	public static readonly ILogger Log = CreateLogger();

	public static void Initialize()
	{
		Serilog.Log.Logger = Log;
	}

	private static Logger CreateLogger()
	{
		LoggerConfiguration config = new LoggerConfiguration()
			.WriteTo.File(
				path: $"ddinfo-{AssemblyUtils.EntryAssemblyVersionString}.log",
				rollingInterval: RollingInterval.Infinite);

		return config.CreateLogger();
	}
}
