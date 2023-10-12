using DevilDaggersInfo.Tools.User.Settings;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ModManager;

public static class ModsDirectoryWindow
{
	public static void Render()
	{
		ImGui.Text("Mods directory");
		ImGui.Text(Inline.Span(UserSettings.ModsDirectory));
	}
}
