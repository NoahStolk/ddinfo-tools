using ImGuiNET;

namespace DevilDaggersInfo.App.Ui;

public static partial class ImGuiExt
{
	public static void Title(string title)
	{
		ImGui.PushFont(Root.FontGoetheBold30);
		ImGui.Text(title);
		ImGui.PopFont();
	}
}
