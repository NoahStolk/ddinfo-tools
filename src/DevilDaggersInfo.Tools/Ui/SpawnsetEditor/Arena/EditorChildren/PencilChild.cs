using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorChildren;

public static class PencilChild
{
	private static float _size;

	public static float Size => _size;

	public static void Render()
	{
		ImGui.SliderFloat("Size", ref _size, 0, 10, "%.1f");
	}
}
