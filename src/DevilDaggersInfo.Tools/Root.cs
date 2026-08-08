using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.GameWindow;
using DevilDaggersInfo.Tools.Platforms;
using DevilDaggersInfo.Tools.Utils;
using Serilog;
using Serilog.Core;
#if WINDOWS
using DevilDaggersInfo.Tools.NativeInterface.Services.Windows;
#elif LINUX
using DevilDaggersInfo.Tools.NativeInterface.Services.Linux;
#endif

namespace DevilDaggersInfo.Tools;

// TODO: Remove.
[Obsolete("Use StrongInject's dependency injection instead. See Container.cs.")]
internal static class Root
{
	public static Logger Log { get; } = new LoggerConfiguration()
		.WriteTo.File($"ddinfo-{AssemblyUtils.EntryAssemblyVersionString}.log", rollingInterval: RollingInterval.Infinite)
		.CreateLogger();

#if WINDOWS
	public static IPlatformSpecificValues PlatformSpecificValues { get; } = new WindowsValues();
	public static GameMemoryService GameMemoryService { get; } = new(new WindowsMemoryService());
	public static GameWindowService GameWindowService { get; } = new(new WindowsWindowingService());
#elif LINUX
	public static IPlatformSpecificValues PlatformSpecificValues { get; } = new LinuxValues();
	public static GameMemoryService GameMemoryService { get; } = new(new LinuxMemoryService());
	public static GameWindowService GameWindowService { get; } = new(new LinuxWindowingService());
#endif
}
