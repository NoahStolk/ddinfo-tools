using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards;

public sealed class StateChild
{
	private readonly RecordingLogic _recordingLogic;

	public StateChild(RecordingLogic recordingLogic)
	{
		_recordingLogic = recordingLogic;
	}

	public void Render()
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
			ImGui.Text(_recordingLogic.RecordingStateType.ToDisplayString());
			ImGui.TableNextRow();

			ImGui.TableNextColumn();
			ImGui.Text("Spawnset");
			ImGui.TableNextColumn();
			ImGui.Text(SurvivalFileWatcher.SpawnsetName ?? "(unknown)");
			ImGui.TableNextRow();

			ImGui.TableNextColumn();
			ImGui.Text("Last upload");
			ImGui.TableNextColumn();
			ImGui.Text(DateTimeUtils.FormatTimeAgo(_recordingLogic.LastSubmission));
			ImGui.TableNextRow();

			ImGui.EndTable();
		}
	}
}
