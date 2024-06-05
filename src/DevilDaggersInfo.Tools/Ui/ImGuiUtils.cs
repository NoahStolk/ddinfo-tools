using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
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
		SetNextWindowMinSize(new Vector2(minSizeX, minSizeY));
	}

	public static void SetNextWindowMinSize(Vector2 minSize)
	{
		ImGui.SetNextWindowSizeConstraints(minSize, _maxValue);
	}

	public static Color GetColorU32(ImGuiCol color)
	{
		uint i = ImGui.GetColorU32(color);
		byte r = (byte)(i >> 00 & 0xFF);
		byte g = (byte)(i >> 08 & 0xFF);
		byte b = (byte)(i >> 16 & 0xFF);
		byte a = (byte)(i >> 24 & 0xFF);
		return new Color(r, g, b, a);
	}
}
