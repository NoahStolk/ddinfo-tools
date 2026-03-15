using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.GameWindow;
using DevilDaggersInfo.Tools.Platforms;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using Serilog;
using Serilog.Core;
#if WINDOWS
using DevilDaggersInfo.Tools.NativeInterface.Services.Windows;
#elif LINUX
using DevilDaggersInfo.Tools.NativeInterface.Services.Linux;
#endif

namespace DevilDaggersInfo.Tools;

// TODO: Remove.
[Obsolete]
internal static class Root
{
	private static readonly InvalidOperationException _notInitializedException = new("Root component is not initialized.");

	public static unsafe ImFontPtr FontGoetheBold20
	{
		get => field.NativePtr == (void*)0 ? throw _notInitializedException : field;
		set;
	}

	public static unsafe ImFontPtr FontGoetheBold30
	{
		get => field.NativePtr == (void*)0 ? throw _notInitializedException : field;
		set;
	}

	public static unsafe ImFontPtr FontGoetheBold60
	{
		get => field.NativePtr == (void*)0 ? throw _notInitializedException : field;
		set;
	}

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
