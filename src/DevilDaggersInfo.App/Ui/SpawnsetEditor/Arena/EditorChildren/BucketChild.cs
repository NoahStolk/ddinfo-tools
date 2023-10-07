using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorChildren;

public static class BucketChild
{
	private static float _tolerance = 0.1f;
	private static float _voidHeight = -2;

	public static float Tolerance => _tolerance;
	public static float VoidHeight => _voidHeight;

	public static void Render()
	{
		ImGui.SliderFloat("Tolerance", ref _tolerance, 0.01f, 10, "%.2f");
		ImGui.SliderFloat("Void height", ref _voidHeight, -50, 0, "%.1f");
	}
}
