using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Scenes;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor;

public static class SpawnsetEditor3DWindow
{
	private static readonly FramebufferData _framebufferData = new();

	private static ArenaScene? _arenaScene;

	public static ArenaScene ArenaScene => _arenaScene ?? throw new InvalidOperationException("Scenes are not initialized.");

	public static void InitializeScene()
	{
		_arenaScene = new(static () => SpawnsetState.Spawnset, false, true);
	}

	public static void Render(float delta)
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Constants.MinWindowSize / 2);
		if (ImGui.Begin("3D Arena Editor"))
		{
			if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
				ImGui.SetWindowFocus();

			float textHeight = ImGui.CalcTextSize(StringResources.SpawnsetEditor3D).Y;
			ImGui.Text(StringResources.SpawnsetEditor3D);

			Vector2 framebufferSize = ImGui.GetWindowSize() - new Vector2(16, 48 + textHeight);
			_framebufferData.ResizeIfNecessary((int)framebufferSize.X, (int)framebufferSize.Y);

			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			ArenaScene.Camera.FramebufferOffset = cursorScreenPos;

			bool isWindowFocused = ImGui.IsWindowFocused();
			bool isMouseOverFramebuffer = ImGui.IsMouseHoveringRect(cursorScreenPos, cursorScreenPos + framebufferSize);
			_framebufferData.RenderArena(isWindowFocused && isMouseOverFramebuffer, isWindowFocused, delta, ArenaScene);

			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			drawList.AddFramebufferImage(_framebufferData, cursorScreenPos, cursorScreenPos + new Vector2(_framebufferData.Width, _framebufferData.Height), isWindowFocused ? Color.White : Color.Gray(0.5f));

			if (ArenaScene.CurrentTick != 0)
			{
				const int padding = 8;
				drawList.AddText(ImGui.GetCursorScreenPos() + new Vector2(padding, padding), ImGui.GetColorU32(Color.Yellow), "(!) Editing is disabled because the shrink preview is active.");
			}

			// Prevent the window from being dragged when clicking on the 3D editor.
			ImGui.InvisibleButton("invisible", new(_framebufferData.Width, _framebufferData.Height));
		}

		ImGui.End(); // End 3D Arena Editor

		ImGui.PopStyleVar();
	}
}
