using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorChildren;

public static class EllipseChild
{
	private static int _thickness = 1;
	private static bool _filled = true;

	public static int Thickness => _thickness;
	public static bool Filled => _filled;

	public static void Render()
	{
		ImGui.SliderInt("Thickness", ref _thickness, 1, 10);
		ImGui.Checkbox("Filled", ref _filled);
	}
}
