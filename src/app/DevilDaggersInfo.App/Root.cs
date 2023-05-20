using DevilDaggersInfo.App.Core.GameMemory;
using DevilDaggersInfo.App.Platforms;
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
	private static readonly InvalidOperationException _alreadyInitializedException = new("Root component is already initialized.");

	private static InternalResources? _internalResources;
	private static GameResources? _gameResources;
	private static GL? _gl;
	private static IWindow? _window;
	private static Application? _application;

	/// <summary>
	/// Holds the internal resources, such as shaders and icons.
	/// </summary>
	public static InternalResources InternalResources
	{
		get => _internalResources ?? throw _notInitializedException;
		set
		{
			if (_internalResources != null)
				throw _alreadyInitializedException;

			_internalResources = value;
		}
	}

	/// <summary>
	/// Holds the game resources, such as the tile texture and dagger mesh.
	/// </summary>
	public static GameResources GameResources
	{
		get => _gameResources ?? throw _notInitializedException;
		set
		{
			if (_gameResources != null)
				throw _alreadyInitializedException;

			_gameResources = value;
		}
	}

	public static GL Gl
	{
		get => _gl ?? throw _notInitializedException;
		set
		{
			if (_gl != null)
				throw _alreadyInitializedException;

			_gl = value;
		}
	}

	public static IWindow Window
	{
		get => _window ?? throw _notInitializedException;
		set
		{
			if (_window != null)
				throw _alreadyInitializedException;

			_window = value;
		}
	}

	public static Application Application
	{
		get => _application ?? throw _notInitializedException;
		set
		{
			if (_application != null)
				throw _alreadyInitializedException;

			_application = value;
		}
	}

	public static IMouse? Mouse { get; set; }
	public static IKeyboard? Keyboard { get; set; }
	public static Logger Log { get; } = new LoggerConfiguration()
		.WriteTo.File($"ddinfo-{VersionUtils.EntryAssemblyVersion}.log", rollingInterval: RollingInterval.Infinite)
		.CreateLogger();

#if WINDOWS
	public static IPlatformSpecificValues PlatformSpecificValues { get; } = new WindowsValues();
	public static GameMemoryService GameMemoryService { get; } = new(new WindowsMemoryService());
#elif LINUX
	public static IPlatformSpecificValues PlatformSpecificValues { get; } = new LinuxValues();
	public static GameMemoryService GameMemoryService { get; } = new(new LinuxMemoryService());
#endif
}
