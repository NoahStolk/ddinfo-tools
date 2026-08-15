using DevilDaggersInfo.Tools.Dialogs;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Encryption;
using DevilDaggersInfo.Tools.Engine;
using DevilDaggersInfo.Tools.Engine.Extensions;
using DevilDaggersInfo.Tools.Engine.Loaders;
using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.GameWindow;
using DevilDaggersInfo.Tools.Platforms;
using DevilDaggersInfo.Tools.Scenes.Rendering;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.AssetEditor;
using DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;
using DevilDaggersInfo.Tools.Ui.Config;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.Leaderboard;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.LeaderboardList;
using DevilDaggersInfo.Tools.Ui.Main;
using DevilDaggersInfo.Tools.Ui.ModManager;
using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.Practice.Main;
using DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis;
using DevilDaggersInfo.Tools.Ui.ReplayEditor;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.Tools.User.Cache;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using Serilog;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using StrongInject;
using Monitor = Silk.NET.GLFW.Monitor;

namespace DevilDaggersInfo.Tools;

// Root
[Register<Application>(Scope.SingleInstance)]

// Engine
[Register<GlfwInput>(Scope.SingleInstance)]
[Register<ShaderLoader>(Scope.SingleInstance)]
[Register<TextureLoader>(Scope.SingleInstance)]
[Register<MeshCache>(Scope.SingleInstance)]
[Register<ResourceManager>(Scope.SingleInstance)]
[Register<FrameCounter>(Scope.SingleInstance)]
[Register<ContentManager>(Scope.SingleInstance)]

// Game Memory
[Register<GameMemoryServiceWrapper>(Scope.SingleInstance)]

// File Watcher
[Register<SurvivalFileWatcher>(Scope.SingleInstance)]

// Interop
[Register<GameInstallationValidator>(Scope.SingleInstance)]

// 3D
[Register<ArenaRenderer>(Scope.SingleInstance)]
[Register<MainScene>(Scope.SingleInstance)]

// UI
[Register<FileStates>(Scope.SingleInstance)]
[Register<PopupManager>(Scope.SingleInstance)]
[Register<Shortcuts>(Scope.SingleInstance)]
[Register<UiLayoutManager>(Scope.SingleInstance)]
[Register<UiRenderer>(Scope.SingleInstance)]
[Register<FontService>(Scope.SingleInstance)]

// Layouts
[Register<ConfigLayout>(Scope.SingleInstance)]

// Windows
[Register<AboutWindow>(Scope.SingleInstance)]
[Register<DebugWindow>(Scope.SingleInstance)]
[Register<MainWindow>(Scope.SingleInstance)]
[Register<UpdateWindow>(Scope.SingleInstance)]

// Spawnset Editor
[Register<SpawnsetEditorMenu>(Scope.SingleInstance)]
[Register<ArenaWindow>(Scope.SingleInstance)]
[Register<SpawnsetEditor3DWindow>(Scope.SingleInstance)]
[Register<SpawnsWindow>(Scope.SingleInstance)]
[Register<HistoryWindow>(Scope.SingleInstance)]
[Register<SettingsWindow>(Scope.SingleInstance)]

[Register<SpawnsetSaver>(Scope.SingleInstance)]

// Mod Manager
[Register<ModsDirectoryWindow>(Scope.SingleInstance)]
[Register<ModPreviewWindow>(Scope.SingleInstance)]
[Register<ModInstallationWindow>(Scope.SingleInstance)]

[Register<ModManagerState>(Scope.SingleInstance)]

// Asset Editor
[Register<ModsDirectoryLogic>(Scope.SingleInstance)]

// Practice
[Register<PracticeWindow>(Scope.SingleInstance)]
[Register<RunAnalysisWindow>(Scope.SingleInstance)]

[Register<PracticeLogic>(Scope.SingleInstance)]

// Custom Leaderboards
[Register<CustomLeaderboardsWindow>(Scope.SingleInstance)]
[Register<CustomLeaderboards3DWindow>(Scope.SingleInstance)]
[Register<LeaderboardListChild>(Scope.SingleInstance)]
[Register<LeaderboardListViewChild>(Scope.SingleInstance)]
[Register<RecordingChild>(Scope.SingleInstance)]
[Register<LeaderboardChild>(Scope.SingleInstance)]
[Register<RecordingLogic>(Scope.SingleInstance)]
[Register<StateChild>(Scope.SingleInstance)]

// Replay Editor
[Register<ReplayEditorMenu>(Scope.SingleInstance)]
[Register<ReplayEventsViewerChild>(Scope.SingleInstance)]
[Register<ReplayEditorWindow>(Scope.SingleInstance)]
[Register<ReplayEntitiesChild>(Scope.SingleInstance)]
[Register<ReplayInputsChild>(Scope.SingleInstance)]
[Register<ReplayEditor3DWindow>(Scope.SingleInstance)]
[Register<LeaderboardReplayBrowser>(Scope.SingleInstance)]
[Register<ReplayTimelineChild>(Scope.SingleInstance)]
[Register<ReplayTimelineSelectedEventsChild>(Scope.SingleInstance)]

// Asset Editor
[Register<AssetEditorMenu>(Scope.SingleInstance)]
[Register<AssetEditorWindow>(Scope.SingleInstance)]
[Register<CompileModWindow>(Scope.SingleInstance)]
[Register<ExtractModWindow>(Scope.SingleInstance)]
[Register<AssetPathsChild>(Scope.SingleInstance)]
[Register<AudioPathsTable>(Scope.SingleInstance)]
[Register<MeshPathsTable>(Scope.SingleInstance)]
[Register<ObjectBindingPathsTable>(Scope.SingleInstance)]
[Register<ShaderPathsTable>(Scope.SingleInstance)]
[Register<TexturePathsTable>(Scope.SingleInstance)]
internal sealed partial class Container : IContainer<Application>
{
	[Factory(Scope.SingleInstance)]
	private static ILogger CreateLogger()
	{
		ILogger logger = StaticLog.Log;
		Log.Logger = logger;
		return logger;
	}

	[Factory(Scope.SingleInstance)]
	private static UserSettings CreateUserSettings(ILogger logger)
	{
		UserSettings userSettings = new(logger);
		userSettings.Load();
		return userSettings;
	}

	[Factory(Scope.SingleInstance)]
	private static UserCache CreateUserCache(ILogger logger)
	{
		UserCache userCache = new(logger);
		userCache.Load();
		return userCache;
	}

	[Factory(Scope.SingleInstance)]
	private static Glfw GetGlfw()
	{
		Glfw glfw = Glfw.GetApi();
		glfw.Init();
		glfw.CheckError();

		glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
		glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
		glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
		glfw.WindowHint(WindowHintBool.Focused, true);
		glfw.WindowHint(WindowHintBool.Resizable, true);
#if DEBUG
		glfw.WindowHint(WindowHintBool.OpenGLDebugContext, true);
#endif
		glfw.CheckError();

		return glfw;
	}

	/// <remarks>
	/// The <paramref name="window"/> parameter is unused, but it forces the window (and therefore the current GL context)
	/// to exist before the entry points are resolved. <c>glfwGetProcAddress</c> is only guaranteed to work with a current
	/// context; it happens to work without one on GLX, but not on WGL or EGL.
	/// </remarks>
	[Factory(Scope.SingleInstance)]
	private static unsafe GL GetGl(Glfw glfw, WindowHandle* window, ILogger logger)
	{
		_ = window;

		GL gl = GL.GetApi(glfw.GetProcAddress);

#if DEBUG
		// A debug context is requested in GetGlfw. Without a callback installed, the driver's diagnostics are discarded,
		// which is how spec violations end up only being noticed as visual glitches on stricter drivers.
		gl.Enable(EnableCap.DebugOutput);
		gl.Enable(EnableCap.DebugOutputSynchronous);
		gl.DebugMessageCallback(
			callback: (source, type, _, severity, length, message, _) => LogGlDebugMessage(logger, source, type, severity, length, message),
			userParam: null);
#endif

		return gl;
	}

#if DEBUG
	private static void LogGlDebugMessage(ILogger logger, GLEnum source, GLEnum type, GLEnum severity, int length, nint message)
	{
		string text = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(message, length);
		switch (severity)
		{
			case GLEnum.DebugSeverityHigh: logger.Error("GL {Type} ({Source}): {Message}", type, source, text); break;
			case GLEnum.DebugSeverityMedium: logger.Warning("GL {Type} ({Source}): {Message}", type, source, text); break;
			case GLEnum.DebugSeverityLow: logger.Information("GL {Type} ({Source}): {Message}", type, source, text); break;
			default: logger.Debug("GL {Type} ({Source}): {Message}", type, source, text); break;
		}
	}
#endif

	[Factory(Scope.SingleInstance)]
	private static unsafe ImGuiController CreateImGuiController(
		Glfw glfw,
		GL gl,
		WindowHandle* window,
		GlfwInput glfwInput,
		ShaderLoader shaderLoader,
		FontService fontService,
		UserSettings userSettings,
		UserCache userCache)
	{
		ImGuiController imGuiController = new(gl, glfwInput, shaderLoader, userCache.Model.WindowWidth, userCache.Model.WindowHeight);

		Colors.SetColors(Colors.Main);

		ImGuiIOPtr io = ImGui.GetIO();

		// Load imgui.ini.
		io.NativePtr->IniFilename = null;
		userSettings.LoadImGuiIni();

		// Add the default font first so it is actually used by default.
		io.Fonts.AddFontDefault();

		// Determine DPI scale from framebuffer/window ratio (e.g. 3.0 on Wayland with 300% scaling at 4K).
		glfw.GetFramebufferSize(window, out int fbWidth, out int _);
		glfw.GetWindowSize(window, out int winWidth, out int _);
		float dpiScale = winWidth > 0 ? (float)fbWidth / winWidth : 1f;

		fontService.Load(dpiScale);

		imGuiController.CreateDefaultFont();

		// Configure style.
		ImGuiStylePtr style = ImGui.GetStyle();
		style.ScrollbarSize = 16;
		style.ScrollbarRounding = 0;

		return imGuiController;
	}

	[Factory(Scope.SingleInstance)]
	private static unsafe WindowHandle* CreateWindow(Glfw glfw, GlfwInput glfwInput, UserCache userCache)
	{
		WindowHandle* window = glfw.CreateWindow(userCache.Model.WindowWidth, userCache.Model.WindowHeight, $"ddinfo tools {AssemblyUtils.EntryAssemblyVersionString}", null, null);
		glfw.CheckError();
		if (window == null)
			throw new InvalidOperationException("Could not create window. Window pointer was null.");

		glfw.SetCursorPosCallback(window, (_, x, y) => glfwInput.CursorPosCallback(x, y));
		glfw.SetScrollCallback(window, (_, _, y) => glfwInput.MouseWheelCallback(y));
		glfw.SetMouseButtonCallback(window, (_, button, state, _) => glfwInput.MouseButtonCallback(button, state));
		glfw.SetKeyCallback(window, (_, keys, _, state, _) => glfwInput.KeyCallback(keys, state));
		glfw.SetCharCallback(window, (_, codepoint) => glfwInput.CharCallback(codepoint));

		if (userCache.Model.WindowIsMaximized)
		{
			glfw.MaximizeWindow(window);
		}
		else
		{
			Monitor* primaryMonitor = glfw.GetPrimaryMonitor();
			int primaryMonitorWidth, primaryMonitorHeight;
			if (primaryMonitor != null)
				glfw.GetMonitorWorkarea(primaryMonitor, out _, out _, out primaryMonitorWidth, out primaryMonitorHeight);
			else
				(primaryMonitorWidth, primaryMonitorHeight) = (1024, 768);

			glfw.SetWindowPos(window, (primaryMonitorWidth - userCache.Model.WindowWidth) / 2, (primaryMonitorHeight - userCache.Model.WindowHeight) / 2);
		}

		glfw.MakeContextCurrent(window);

		// Turn VSync on. There's really no need to turn it off for this application.
		glfw.SwapInterval(1);

		glfw.SetWindowSizeLimits(window, (int)Constants.MinWindowSize.X, (int)Constants.MinWindowSize.Y, -1, -1);

		return window;
	}

	[Factory(Scope.SingleInstance)]
	private static IEncryptionService CreateEncryptionService(ILogger logger)
	{
		EncryptionService? encryptionService = EncryptionService.TryCreate();
		if (encryptionService != null)
			return encryptionService;

		logger.Error("Could not create the encryption service. The encryption.ini resource is missing or incomplete.");
		return new DummyEncryptionService();
	}

	[Factory(Scope.SingleInstance)]
	private static INativeFileDialog CreateNativeFileDialog()
	{
		return IsWayland() ? new NativeFileDialogWayland() : new NativeFileDialog();

		static bool IsWayland()
		{
			if (!OperatingSystem.IsLinux())
				return false;

			string? session = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE");
			if (string.Equals(session, "wayland", StringComparison.OrdinalIgnoreCase))
				return true;

			// Fallback used by many compositors
			return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY"));
		}
	}

	[Factory(Scope.SingleInstance)]
	private static GameMemoryService CreateGameMemoryService(ILogger logger)
	{
#if WINDOWS
		return new GameMemoryService(new NativeInterface.Services.Windows.WindowsMemoryService());
#elif LINUX
		return new GameMemoryService(new NativeInterface.Services.Linux.LinuxMemoryService(logger));
#endif
	}

	[Factory(Scope.SingleInstance)]
	private static GameWindowService CreateGameWindowService()
	{
#if WINDOWS
		return new GameWindowService(new NativeInterface.Services.Windows.WindowsWindowingService());
#elif LINUX
		return new GameWindowService(new NativeInterface.Services.Linux.LinuxWindowingService());
#endif
	}

	[Factory(Scope.SingleInstance)]
#pragma warning disable CA1859 // Change return type of method 'CreatePlatformSpecificValues' from 'DevilDaggersInfo.Tools.Platforms.IPlatformSpecificValues' to 'DevilDaggersInfo.Tools.Platforms.LinuxValues' for improved performance.
	private static IPlatformSpecificValues CreatePlatformSpecificValues()
#pragma warning restore CA1859
	{
#if WINDOWS
		return new WindowsValues();
#elif LINUX
		return new LinuxValues();
#endif
	}
}
