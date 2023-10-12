using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ModManager;

public static class ModManagerWindow
{
	public static void Render()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(1280, 768));
		if (ImGui.Begin("Mod Manager", ImGuiWindowFlags.NoCollapse))
		{
			ImGui.PopStyleVar();

			if (ImGui.BeginTabBar("mod_manager"))
			{
				if (ImGui.BeginTabItem("Browse mods online"))
				{
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Mods installation"))
				{
					ImGui.EndTabItem();
				}

				ImGui.EndTabBar();
			}
		}
		else
		{
			ImGui.PopStyleVar();
		}

		ImGui.End(); // End Practice

		if (ImGui.IsKeyPressed(ImGuiKey.Escape))
			UiRenderer.Layout = LayoutType.Main;
	}
}
