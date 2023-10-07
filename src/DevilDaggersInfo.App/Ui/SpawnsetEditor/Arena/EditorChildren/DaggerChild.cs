using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorChildren;

public static class DaggerChild
{
	private static Vector2 _snap = Vector2.One;

	public static Vector2 Snap => _snap;

	public static void Render()
	{
		ImGui.BeginDisabled(SpawnsetState.Spawnset.GameMode != GameMode.Race);
		ImGui.SliderFloat2("Snap", ref _snap, 0.25f, 2, "%.2f");
		ImGui.EndDisabled();
	}
}
