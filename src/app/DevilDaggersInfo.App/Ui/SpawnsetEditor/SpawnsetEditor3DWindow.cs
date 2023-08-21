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
			float textHeight = ImGui.CalcTextSize(StringResources.SpawnsetEditor3D).Y;

			Vector2 framebufferSize = ImGui.GetWindowSize() - new Vector2(32, 48 + textHeight);
			_framebufferData.ResizeIfNecessary((int)framebufferSize.X, (int)framebufferSize.Y);

			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos() + new Vector2(0, textHeight);
			ArenaScene.Camera.FramebufferOffset = cursorScreenPos;

			_framebufferData.RenderArena(delta, ArenaScene);

			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			drawList.AddFramebufferImage(_framebufferData, cursorScreenPos, cursorScreenPos + new Vector2(_framebufferData.Width, _framebufferData.Height));

			ImGui.Text(StringResources.SpawnsetEditor3D);

			ImGui.End();
		}

		ImGui.PopStyleVar();
	}
}
