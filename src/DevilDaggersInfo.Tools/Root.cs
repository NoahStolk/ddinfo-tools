using DevilDaggersInfo.Tools.Utils;
using Serilog;
using Serilog.Core;

namespace DevilDaggersInfo.Tools;

// TODO: Remove.
[Obsolete("Use StrongInject's dependency injection instead. See Container.cs.")]
internal static class Root
{
	public static Logger Log { get; } = new LoggerConfiguration()
		.WriteTo.File($"ddinfo-{AssemblyUtils.EntryAssemblyVersionString}.log", rollingInterval: RollingInterval.Infinite)
		.CreateLogger();
}
