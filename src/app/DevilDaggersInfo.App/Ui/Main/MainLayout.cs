using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Scenes;
using DevilDaggersInfo.App.Ui.CustomLeaderboards.LeaderboardList;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;
using Silk.NET.OpenGL;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.Main;

public static class MainLayout
{
	private static readonly int _centerX = (int)Constants.LayoutSize.X / 2;

	private static readonly string _version = VersionUtils.EntryAssemblyVersion;

	private static readonly SpawnsetBinary _mainMenuSpawnset = SpawnsetBinary.CreateDefault();

	private static ArenaScene? _mainMenuScene;

	public static void InitializeScene()
	{
		_mainMenuScene = new(static () => _mainMenuSpawnset, true, false);
		_mainMenuScene.AddSkull4();
	}

	public static void Render(float delta, out bool shouldClose)
	{
		shouldClose = false;

		Vector2 center = ImGui.GetMainViewport().GetCenter();
		ImGui.SetNextWindowPos(center, ImGuiCond.Always, new(0.5f, 0.5f));
		ImGui.SetNextWindowSize(Constants.LayoutSize);

		ImGui.Begin("Main Menu", Constants.LayoutFlags);

		TextAt(StringResources.MainMenu, 8, 8);
		// TextAt("DDINFO", _centerX, 64, new(1, 0, 0, 1), true); // TODO: font size 64
		// TextAt("TOOLS", _centerX, 128, new(1, 0.5f, 0, 1), true); // TODO: font size 32
		// TextAt(_version, _centerX, 176, new(1, 0.8f, 0, 1), true); // TODO: font size 24
		TextAt("Devil Daggers is created by Sorath", _centerX, 708, default, true);
		TextAt("DevilDaggers.info is created by Noah Stolk", _centerX, 724, default, true);

		const byte buttonAlpha = 191;

		if (MainButtonAt(0, 0, Colors.SpawnsetEditor with { A = buttonAlpha }, "Spawnset Editor (wip)"))
		{
			UiRenderer.Layout = LayoutType.SpawnsetEditor;
			Colors.SetSpawnsetEditorColors();
		}

		MainButtonAt(1, 0, Colors.AssetEditor with { A = buttonAlpha }, "Asset Editor (todo)");
		if (MainButtonAt(2, 0, Colors.ReplayEditor with { A = buttonAlpha }, "Replay Editor (wip)"))
		{
			UiRenderer.Layout = LayoutType.ReplayEditor;
			Colors.SetReplayEditorColors();
		}

		if (MainButtonAt(0, 1, Colors.CustomLeaderboards with { A = buttonAlpha }, "Custom Leaderboards"))
		{
			UiRenderer.Layout = LayoutType.CustomLeaderboards;
			Colors.SetCustomLeaderboardsColors();
			LeaderboardListChild.LoadAll();
		}

		MainButtonAt(1, 1, Colors.Practice with { A = buttonAlpha }, "Practice (todo)");
		MainButtonAt(2, 1, Colors.ModManager with { A = buttonAlpha }, "Mod Manager (todo)");

		Color gray = new(96, 96, 96, buttonAlpha);
		if (MainButtonAt(0, 2, gray, "Configuration"))
			UiRenderer.Layout = LayoutType.Config;

		if (MainButtonAt(1, 2, gray, "Settings"))
			UiRenderer.ShowSettings();

		if (MainButtonAt(2, 2, gray, "Exit"))
			shouldClose = true;

		Vector4 hyperlinkColor = new(0, 0.625f, 1, 1);
		Vector4 hyperlinkHoverColor = new(0.25f, 0.875f, 1, 0.25f);

		ImGui.PushStyleColor(ImGuiCol.Text, hyperlinkColor);
		ImGui.PushStyleColor(ImGuiCol.Button, default(Vector4));
		ImGui.PushStyleColor(ImGuiCol.ButtonHovered, hyperlinkHoverColor);
		ImGui.PushStyleColor(ImGuiCol.ButtonActive, default(Vector4));

		const string homePage = "https://devildaggers.info/";
		const string toolsPage = "https://devildaggers.info/tools";

		ImGui.SetCursorPos(new(_centerX - ImGui.CalcTextSize(homePage).X / 2, 740));
		if (ImGui.Button(homePage))
			Process.Start(new ProcessStartInfo(homePage) { UseShellExecute = true });

		ImGui.SetCursorPos(new(4, 156));
		if (ImGui.Button(toolsPage))
			Process.Start(new ProcessStartInfo(toolsPage) { UseShellExecute = true });

		ImGui.PopStyleColor(4);

		ImGui.End();

		RenderScene(delta);
	}

	private static void TextAt(string text, int x, int y, Vector4 color = default, bool center = false)
	{
		if (center)
		{
			float textSize = ImGui.CalcTextSize(text).X;
			ImGui.SetCursorPos(new(x - textSize / 2, y));
		}
		else
		{
			ImGui.SetCursorPos(new(x, y));
		}

		if (color == default)
			ImGui.Text(text);
		else
			ImGui.TextColored(color, text);
	}

	private static bool MainButtonAt(int x, int y, Color color, string text)
	{
		int xPos = x switch
		{
			0 => _centerX - 96 - 384,
			1 => _centerX - 96,
			_ => _centerX - 96 + 384,
		};
		int yPos = y * 128 + 256;

		ImGui.PushStyleColor(ImGuiCol.Button, color);
		ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color + new Vector4(0, 0, 0, 0.2f));
		ImGui.PushStyleColor(ImGuiCol.ButtonActive, color + new Vector4(0, 0, 0, 0.3f));
		ImGui.PushStyleColor(ImGuiCol.Border, color with { A = 255 });

		ImGui.SetCursorPos(new(xPos, yPos));
		ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 4);
		bool value = ImGui.Button(text, new(192, 96));
		ImGui.PopStyleVar();

		ImGui.PopStyleColor(4);

		return value;
	}

	private static void RenderScene(float delta)
	{
		_mainMenuScene?.Update(false, false, delta);

		Root.Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

		int framebufferWidth = Root.Window.Size.X;
		int framebufferHeight = Root.Window.Size.Y;

		// Keep track of the original viewport so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		Root.Gl.GetInteger(GLEnum.Viewport, originalViewport);
		Root.Gl.Viewport(0, 0, (uint)framebufferWidth, (uint)framebufferHeight);

		Root.Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		Root.Gl.Enable(EnableCap.DepthTest);
		Root.Gl.Enable(EnableCap.Blend);
		Root.Gl.Enable(EnableCap.CullFace);
		Root.Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		_mainMenuScene?.Render(framebufferWidth, framebufferHeight);

		Root.Gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		Root.Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
