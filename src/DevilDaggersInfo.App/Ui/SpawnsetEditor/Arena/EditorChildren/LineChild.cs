using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorChildren;

public static class LineChild
{
	private static float _thickness;

	public static float Thickness => _thickness;

	public static void Render()
	{
		ImGui.SliderFloat("Thickness", ref _thickness, 0, 10, "%.1f");
	}
}
