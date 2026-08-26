using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.NativeInterface.Services;
using DevilDaggersInfo.Tools.Utils;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards;

internal sealed class StateChild(RecordingLogic recordingLogic, GameMemoryServiceWrapper gameMemoryServiceWrapper, GameMemoryService gameMemoryService, SurvivalFileWatcher survivalFileWatcher)
{
	public void Render()
	{
		if (!ImGui.BeginTable("StateTable", 2, ImGuiTableFlags.None, new Vector2(288, 80)))
			return;

		ImGui.TableSetupColumn(""u8, ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.NoHeaderLabel, 112);

		ImGui.TableNextColumn();
		ImGui.Text("Memory");
		ImGui.TableNextColumn();
		ImGui.Text(GetMemoryText());
		if (ImGui.IsItemHovered())
			ImGui.SetTooltip(GetMemoryTooltip());

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

	/// <summary>
	/// Where the game's stats block is, or why it is not known. Platforms that fetch a marker offset from the API show
	/// that offset; platforms that search the game's memory for the block show how that search went.
	/// </summary>
	private ReadOnlySpan<byte> GetMemoryText()
	{
		if (gameMemoryService.RequiresMarkerOffset)
			return gameMemoryServiceWrapper.Marker.HasValue ? Inline.Utf8($"0x{gameMemoryServiceWrapper.Marker.Value:X}") : "Waiting..."u8;

		return gameMemoryService.BlockAddressStatus switch
		{
			BlockAddressStatus.Resolved => Inline.Utf8($"0x{gameMemoryService.BlockAddress:X}"),
			BlockAddressStatus.MemoryUnreadable => "No access"u8,
			BlockAddressStatus.BlockNotFound => "Not found"u8,
			_ => "Waiting..."u8,
		};
	}

	private ReadOnlySpan<byte> GetMemoryTooltip()
	{
		if (gameMemoryService.RequiresMarkerOffset)
			return "The offset the DevilDaggers.info API gives for the game's stats block."u8;

		return gameMemoryService.BlockAddressStatus switch
		{
			BlockAddressStatus.Resolved => "The address the game's stats block was found at."u8,
			BlockAddressStatus.MemoryUnreadable => "The game's memory could not be read. Quit ddinfo-tools and start it again under sudo."u8,
			BlockAddressStatus.BlockNotFound => "The game's memory was read, but holds no stats block. Start a run, or check that the game is a version this app knows."u8,
			_ => "Waiting for the game to start."u8,
		};
	}
}
