using ImGuiNET;

namespace DevilDaggersInfo.Tools.Utils;

public static class ImGuiUtils
{
	public static bool IsEnterPressed()
	{
		return ImGui.IsKeyPressed(ImGuiKey.Enter) || ImGui.IsKeyPressed(ImGuiKey.KeypadEnter);
	}
}
