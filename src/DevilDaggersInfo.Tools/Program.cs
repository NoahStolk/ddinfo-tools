using DevilDaggersInfo.Tools;
using DevilDaggersInfo.Tools.Engine;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.User.Cache;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.Utils;
using ImGuiGlfw;
using ImGuiNET;

AppDomain.CurrentDomain.UnhandledException += (_, args) => Root.Log.Fatal(args.ExceptionObject.ToString());

UserSettings.Load();
UserCache.Load();

Graphics.CreateWindow(new($"ddinfo tools {AssemblyUtils.EntryAssemblyVersionString}", UserCache.Model.WindowWidth, UserCache.Model.WindowHeight, false));

Graphics.Gl.ClearColor(0, 0, 0, 1);

// TODO: Configure V-sync.

ImGuiController imGuiController = ConfigureImGui();

Colors.SetColors(Colors.Main);

Graphics.OnChangeWindowSize = (w, h) =>
{
	Graphics.Gl.Viewport(0, 0, (uint)w, (uint)h);
	imGuiController.WindowResized(w, h);

	UserCache.Model = UserCache.Model with
	{
		WindowWidth = w,
		WindowHeight = h,
	};
};

Application app = new(imGuiController);
app.Run();
// app.Destroy();

static ImGuiController ConfigureImGui()
{
	ImGuiController imGuiController = new(Graphics.Gl, Input.GlfwInput, UserCache.Model.WindowWidth, UserCache.Model.WindowHeight);
	imGuiController.CreateDefaultFont();

	ImGuiIOPtr io = ImGui.GetIO();

	// Add the default font first so it is actually used by default.
	io.Fonts.AddFontDefault();

	string fontPath = Path.Combine(AssemblyUtils.InstallationDirectory, "goethebold.ttf");
	Root.FontGoetheBold20 = io.Fonts.AddFontFromFileTTF(fontPath, 20);
	Root.FontGoetheBold30 = io.Fonts.AddFontFromFileTTF(fontPath, 30);
	Root.FontGoetheBold60 = io.Fonts.AddFontFromFileTTF(fontPath, 60);

	ImGuiStylePtr style = ImGui.GetStyle();
	style.ScrollbarSize = 16;
	style.ScrollbarRounding = 0;
	return imGuiController;
}
