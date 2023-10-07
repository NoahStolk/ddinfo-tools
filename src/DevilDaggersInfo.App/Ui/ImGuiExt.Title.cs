using ImGuiNET;

namespace DevilDaggersInfo.App.Ui;

public static partial class ImGuiExt
{
	public static unsafe void Title(ReadOnlySpan<char> title, ImFontPtr font = default)
	{
		if (font == (void*)0)
			font = Root.FontGoetheBold30;
		ImGui.PushFont(font);
		ImGui.Text(title);
		ImGui.PopFont();
	}
}
