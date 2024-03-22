using DevilDaggersInfo.Tools;
using DevilDaggersInfo.Tools.Engine;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.User.Cache;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.Utils;
using ImGuiGlfw;
using ImGuiNET;
using Silk.NET.GLFW;
using System.Diagnostics;

AppDomain.CurrentDomain.UnhandledException += (_, args) => Root.Log.Fatal(args.ExceptionObject.ToString());

UserSettings.Load();
UserCache.Load();

Graphics.CreateWindow($"ddinfo tools {AssemblyUtils.EntryAssemblyVersionString}", UserCache.Model.WindowWidth, UserCache.Model.WindowHeight, UserCache.Model.WindowIsMaximized);
Graphics.SetWindowSizeLimits((int)Constants.MinWindowSize.X, (int)Constants.MinWindowSize.Y, -1, -1);

Graphics.Gl.ClearColor(0, 0, 0, 1);

ImGuiController imGuiController = ConfigureImGui();

Colors.SetColors(Colors.Main);

Graphics.OnChangeWindowSize = (w, h) =>
{
	bool isMaximized;
	unsafe
	{
		isMaximized = Graphics.Glfw.GetWindowAttrib(Graphics.Window, WindowAttributeGetter.Maximized);
	}

	Graphics.Gl.Viewport(0, 0, (uint)w, (uint)h);
	imGuiController.WindowResized(w, h);

	UserCache.Model = UserCache.Model with
	{
		WindowWidth = w,
		WindowHeight = h,
		WindowIsMaximized = isMaximized,
	};
};

unsafe
{
	// Always invoke this in case of incorrect cache.
	Graphics.Glfw.GetWindowSize(Graphics.Window, out int width, out int height);
	Graphics.OnChangeWindowSize.Invoke(width, height);
}

Application app = new(imGuiController);
app.Run();

static unsafe ImGuiController ConfigureImGui()
{
	ImGuiController imGuiController = new(Graphics.Gl, Input.GlfwInput, UserCache.Model.WindowWidth, UserCache.Model.WindowHeight);

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
