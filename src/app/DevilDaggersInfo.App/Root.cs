using DevilDaggersInfo.App.Core.GameMemory;
using DevilDaggersInfo.App.GameWindow;
using DevilDaggersInfo.App.Platforms;
using DevilDaggersInfo.App.Utils;
using ImGuiNET;
using Serilog;
using Serilog.Core;
#if WINDOWS
using DevilDaggersInfo.App.Core.NativeInterface.Services.Windows;
#elif LINUX
using DevilDaggersInfo.App.Core.NativeInterface.Services.Linux;
#endif
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace DevilDaggersInfo.App;

public static class Root
{
	private static readonly InvalidOperationException _notInitializedException = new("Root component is not initialized.");

	private static InternalResources? _internalResources;
	private static GameResources? _gameResources;
	private static GL? _gl;
	private static IWindow? _window;
	private static Application? _application;
	private static ImFontPtr _fontGoetheBold20;
	private static ImFontPtr _fontGoetheBold30;
	private static ImFontPtr _fontGoetheBold60;

	/// <summary>
	/// Holds the internal resources, such as shaders and icons.
	/// </summary>
	public static InternalResources InternalResources
	{
		get => _internalResources ?? throw _notInitializedException;
		set => _internalResources = value;
	}

	/// <summary>
	/// Holds the game resources, such as the tile texture and dagger mesh.
	/// </summary>
	public static GameResources GameResources
	{
		get => _gameResources ?? throw _notInitializedException;
		set => _gameResources = value;
	}

	public static GL Gl
	{
		get => _gl ?? throw _notInitializedException;
		set => _gl = value;
	}

	public static IWindow Window
	{
		get => _window ?? throw _notInitializedException;
		set => _window = value;
	}

	public static Application Application
	{
		get => _application ?? throw _notInitializedException;
		set => _application = value;
	}

	public static unsafe ImFontPtr FontGoetheBold20
	{
		get => _fontGoetheBold20.NativePtr == (void*)0 ? throw _notInitializedException : _fontGoetheBold20;
		set => _fontGoetheBold20 = value;
	}

	public static unsafe ImFontPtr FontGoetheBold30
	{
		get => _fontGoetheBold30.NativePtr == (void*)0 ? throw _notInitializedException : _fontGoetheBold30;
		set => _fontGoetheBold30 = value;
	}

	public static unsafe ImFontPtr FontGoetheBold60
	{
		get => _fontGoetheBold60.NativePtr == (void*)0 ? throw _notInitializedException : _fontGoetheBold60;
		set => _fontGoetheBold60 = value;
	}

	public static IMouse? Mouse { get; set; }
	public static IKeyboard? Keyboard { get; set; }
	public static Logger Log { get; } = new LoggerConfiguration()
		.WriteTo.File($"ddinfo-{AssemblyUtils.EntryAssemblyVersion}.log", rollingInterval: RollingInterval.Infinite)
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
