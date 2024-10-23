using DevilDaggersInfo.Tools.Engine.Extensions;
using DevilDaggersInfo.Tools.Engine.Loaders;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.AssetEditor;
using DevilDaggersInfo.Tools.Ui.Config;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.Leaderboard;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.LeaderboardList;
using DevilDaggersInfo.Tools.Ui.Main;
using DevilDaggersInfo.Tools.Ui.Practice.Main;
using DevilDaggersInfo.Tools.Ui.ReplayEditor;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena;
using DevilDaggersInfo.Tools.User.Cache;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.Utils;
using ImGuiGlfw;
using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using StrongInject;
using System.Diagnostics;
using Monitor = Silk.NET.GLFW.Monitor;

namespace DevilDaggersInfo.Tools;

// Root
[Register<Application>(Scope.SingleInstance)]

// Engine
[Register<GlfwInput>(Scope.SingleInstance)]
[Register<ShaderLoader>(Scope.SingleInstance)]
[Register<TextureLoader>(Scope.SingleInstance)]
[Register<ResourceManager>(Scope.SingleInstance)]
[Register<FrameCounter>(Scope.SingleInstance)]

// Interop
[Register<GameInstallationValidator>(Scope.SingleInstance)]

// 3D
[Register<MainScene>(Scope.SingleInstance)]

// UI
[Register<Shortcuts>(Scope.SingleInstance)]
[Register<UiLayoutManager>(Scope.SingleInstance)]
[Register<UiRenderer>(Scope.SingleInstance)]

// Layouts
[Register<ConfigLayout>(Scope.SingleInstance)]

// Menus
[Register<AssetEditorMenu>(Scope.SingleInstance)]
[Register<ReplayEditorMenu>(Scope.SingleInstance)]
[Register<SpawnsetEditorMenu>(Scope.SingleInstance)]

// Windows
[Register<AboutWindow>(Scope.SingleInstance)]
[Register<DebugWindow>(Scope.SingleInstance)]
[Register<MainWindow>(Scope.SingleInstance)]

// Spawnset Editor
[Register<ArenaWindow>(Scope.SingleInstance)]
[Register<SpawnsetEditor3DWindow>(Scope.SingleInstance)]

// Practice
[Register<PracticeWindow>(Scope.SingleInstance)]

// Custom Leaderboards
[Register<CustomLeaderboardsWindow>(Scope.SingleInstance)]
[Register<CustomLeaderboards3DWindow>(Scope.SingleInstance)]
[Register<LeaderboardListChild>(Scope.SingleInstance)]
[Register<LeaderboardListViewChild>(Scope.SingleInstance)]
[Register<RecordingChild>(Scope.SingleInstance)]
[Register<LeaderboardChild>(Scope.SingleInstance)]
#pragma warning disable S3881 // "IDisposable" should be implemented correctly. The source generator already implements IDisposable correctly.
public sealed partial class Container : IContainer<Application>
#pragma warning restore S3881
{
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
		glfw.CheckError();

		return glfw;
	}

	[Factory(Scope.SingleInstance)]
	private static GL GetGl(Glfw glfw)
	{
		GL gl = GL.GetApi(glfw.GetProcAddress);
		gl.ClearColor(0, 0, 0, 1);
		return gl;
	}

	[Factory(Scope.SingleInstance)]
	private static unsafe ImGuiController CreateImGuiController(GL gl, GlfwInput glfwInput)
	{
		// TODO: Inject UserCache.
		ImGuiController imGuiController = new(gl, glfwInput, UserCache.Model.WindowWidth, UserCache.Model.WindowHeight);

		ImGuiIOPtr io = ImGui.GetIO();

		// Load imgui.ini.
		io.NativePtr->IniFilename = null;
		UserSettings.LoadImGuiIni();

		// Add the default font first so it is actually used by default.
		io.Fonts.AddFontDefault();

		// Load custom fonts.
		string fontPath = Path.Combine(AssemblyUtils.InstallationDirectory, "goethebold.ttf");
		Debug.Assert(File.Exists(fontPath), $"Font file not found: {fontPath}");
		Root.FontGoetheBold20 = io.Fonts.AddFontFromFileTTF(fontPath, 20);
		Root.FontGoetheBold30 = io.Fonts.AddFontFromFileTTF(fontPath, 30);
		Root.FontGoetheBold60 = io.Fonts.AddFontFromFileTTF(fontPath, 60);

		imGuiController.CreateDefaultFont();

		// Configure style.
		ImGuiStylePtr style = ImGui.GetStyle();
		style.ScrollbarSize = 16;
		style.ScrollbarRounding = 0;

		return imGuiController;
	}

	[Factory(Scope.SingleInstance)]
	private static unsafe WindowHandle* CreateWindow(GL gl, Glfw glfw, GlfwInput glfwInput, ImGuiController imGuiController)
	{
		// TODO: Inject UserCache.
		WindowHandle* window = glfw.CreateWindow(UserCache.Model.WindowWidth, UserCache.Model.WindowHeight, $"ddinfo tools {AssemblyUtils.EntryAssemblyVersionString}", null, null);
		glfw.CheckError();
		if (window == null)
			throw new InvalidOperationException("Could not create window.");

		glfw.SetFramebufferSizeCallback(window, (_, w, h) =>
		{
			bool isMaximized = glfw.GetWindowAttrib(window, WindowAttributeGetter.Maximized);

			gl.Viewport(0, 0, (uint)w, (uint)h);
			imGuiController.WindowResized(w, h);

			UserCache.Model = UserCache.Model with
			{
				WindowWidth = w,
				WindowHeight = h,
				WindowIsMaximized = isMaximized,
			};
		});
		glfw.SetCursorPosCallback(window, (_, x, y) => glfwInput.CursorPosCallback(x, y));
		glfw.SetScrollCallback(window, (_, _, y) => glfwInput.MouseWheelCallback(y));
		glfw.SetMouseButtonCallback(window, (_, button, state, _) => glfwInput.MouseButtonCallback(button, state));
		glfw.SetKeyCallback(window, (_, keys, _, state, _) => glfwInput.KeyCallback(keys, state));
		glfw.SetCharCallback(window, (_, codepoint) => glfwInput.CharCallback(codepoint));

		if (UserCache.Model.WindowIsMaximized)
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

			glfw.SetWindowPos(window, (primaryMonitorWidth - UserCache.Model.WindowWidth) / 2, (primaryMonitorHeight - UserCache.Model.WindowHeight) / 2);
		}

		glfw.MakeContextCurrent(window);

		// Turn VSync on. There's really no need to turn it off for this application.
		glfw.SwapInterval(1);

		glfw.SetWindowSizeLimits(window, (int)Constants.MinWindowSize.X, (int)Constants.MinWindowSize.Y, -1, -1);

		return window;
	}
}
