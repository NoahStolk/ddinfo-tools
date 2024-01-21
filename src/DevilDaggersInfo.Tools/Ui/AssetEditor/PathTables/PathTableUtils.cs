using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

public static class PathTableUtils
{
	public static void SetupDefaultColumns()
	{
		ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 160, 0);
		ImGui.TableSetupColumn("Prohibited", ImGuiTableColumnFlags.WidthFixed, 80);
	}

	public static void RenderDefaultColumns(AssetInfo assetInfo)
	{
		ImGui.TableNextColumn();
		ImGui.Text(assetInfo.AssetName);

		ImGui.TableNextColumn();
		if (assetInfo.IsProhibited)
			ImGui.TextColored(Color.Orange, "Prohibited");
		else
			ImGui.TextColored(Color.Green, "OK");
	}
}
