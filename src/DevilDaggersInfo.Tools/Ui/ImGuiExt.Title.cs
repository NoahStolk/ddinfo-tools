using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui;

internal static partial class ImGuiExt
{
	public static void Title(ReadOnlySpan<char> title, ImFontPtr font)
	{
		ImGui.PushFont(font);
		ImGui.Text(title);
		ImGui.PopFont();
	}
}
