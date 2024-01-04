using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ModManager;

public static class ModManagerWindow
{
	public static void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(1280, 768);
		if (ImGui.Begin("Mod Manager", ImGuiWindowFlags.NoCollapse))
		{
			if (ImGui.BeginTabBar("mod_manager"))
			{
				if (ImGui.BeginTabItem("Mods Folder"))
				{
					ModsDirectoryWindow.Render();
					ModPreviewWindow.Render();
					ModInstallationWindow.Render();
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Download New Mods"))
				{
					ImGui.Text("Not implemented yet.");
					ImGui.EndTabItem();
				}

				ImGui.EndTabBar();
			}
		}

		ImGui.End(); // End Mod Manager

		if (ImGui.IsKeyPressed(ImGuiKey.Escape))
			UiRenderer.Layout = LayoutType.Main;
	}
}
