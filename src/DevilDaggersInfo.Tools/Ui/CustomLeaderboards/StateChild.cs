using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards;

public static class StateChild
{
	public static void Render()
	{
		if (ImGui.BeginTable("StateTable", 2, ImGuiTableFlags.None, new Vector2(288, 80)))
		{
			ImGui.TableSetupColumn(null, ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.NoHeaderLabel, 112);

			ImGui.TableNextColumn();
			ImGui.Text("Memory");
			ImGui.TableNextColumn();
			ImGui.Text(GameMemoryServiceWrapper.Marker.HasValue ? Inline.Span($"0x{GameMemoryServiceWrapper.Marker.Value:X}") : "Waiting...");
			ImGui.TableNextRow();

			ImGui.TableNextColumn();
			ImGui.Text("State");
			ImGui.TableNextColumn();
			ImGui.Text(RecordingLogic.RecordingStateType.ToDisplayString());
			ImGui.TableNextRow();

			ImGui.TableNextColumn();
			ImGui.Text("Spawnset");
			ImGui.TableNextColumn();
			ImGui.Text(SurvivalFileWatcher.SpawnsetName ?? "(unknown)");
			ImGui.TableNextRow();

			ImGui.TableNextColumn();
			ImGui.Text("Last upload");
			ImGui.TableNextColumn();
			ImGui.Text(DateTimeUtils.FormatTimeAgo(RecordingLogic.LastSubmission));
			ImGui.TableNextRow();

			ImGui.EndTable();
		}
	}
}
