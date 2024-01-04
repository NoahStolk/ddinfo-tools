using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

public static class ImGuiUtils
{
	private static readonly Vector2 _maxValue = new(float.MaxValue, float.MaxValue);

	public static bool IsEnterPressed()
	{
		return ImGui.IsKeyPressed(ImGuiKey.Enter) || ImGui.IsKeyPressed(ImGuiKey.KeypadEnter);
	}

	public static void SetNextWindowMinSize(float minSizeX, float minSizeY)
	{
		SetNextWindowMinSize(new(minSizeX, minSizeY));
	}

	public static void SetNextWindowMinSize(Vector2 minSize)
	{
		ImGui.SetNextWindowSizeConstraints(minSize, _maxValue);
	}
}
