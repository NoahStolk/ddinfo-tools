using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards;

internal sealed class StateChild(RecordingLogic recordingLogic, GameMemoryServiceWrapper gameMemoryServiceWrapper, SurvivalFileWatcher survivalFileWatcher)
{
	public void Render()
	{
		if (ImGui.BeginTable("StateTable", 2, ImGuiTableFlags.None, new Vector2(288, 80)))
		{
			ImGui.TableSetupColumn(null, ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.NoHeaderLabel, 112);

			ImGui.TableNextColumn();
			ImGui.Text("Memory");
			ImGui.TableNextColumn();
			ImGui.Text(gameMemoryServiceWrapper.Marker.HasValue ? Inline.Span($"0x{gameMemoryServiceWrapper.Marker.Value:X}") : "Waiting...");
			ImGui.TableNextRow();

			ImGui.TableNextColumn();
			ImGui.Text("State");
			ImGui.TableNextColumn();
			ImGui.Text(recordingLogic.RecordingStateType.ToDisplayString());
			ImGui.TableNextRow();

			ImGui.TableNextColumn();
			ImGui.Text("Spawnset");
			ImGui.TableNextColumn();
			ImGui.Text(survivalFileWatcher.SpawnsetName ?? "(unknown)");
			ImGui.TableNextRow();

			ImGui.TableNextColumn();
			ImGui.Text("Last upload");
			ImGui.TableNextColumn();
			ImGui.Text(DateTimeUtils.FormatTimeAgo(recordingLogic.LastSubmission));
			ImGui.TableNextRow();

			ImGui.EndTable();
		}
	}
}
