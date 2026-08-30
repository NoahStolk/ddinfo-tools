using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.Utils;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

/// <summary>
/// Renders the raw contents of the game's <see cref="MainBlock" /> for debugging purposes.
/// </summary>
internal sealed class MainBlockChild(GameMemoryServiceWrapper gameMemoryServiceWrapper, GameMemoryService gameMemoryService)
{
	private ScanResult _scanResult;

	private enum ScanResult
	{
		NotScanned,
		MarkerUnavailable,
		GameNotRunning,
		Success,
	}

	public void Render()
	{
		if (ImGui.Button("Scan"u8))
			_scanResult = Scan();

		ImGui.SameLine();

		switch (_scanResult)
		{
			case ScanResult.NotScanned:
				ImGui.TextColored(Color.Gray(0.4f), "Not scanned"u8);
				return;
			case ScanResult.MarkerUnavailable:
				ImGui.TextColored(Color.Orange, "The marker is not available yet; try again in a moment"u8);
				return;
			case ScanResult.GameNotRunning:
				ImGui.TextColored(Color.Red, "Devil Daggers is not running"u8);
				return;
			default:
				ImGui.TextColored(Color.Green, "Scanned"u8);
				break;
		}

		RenderTable(gameMemoryService.MainBlock);
	}

	private ScanResult Scan()
	{
		// The wrapper fetches the marker if needed, and initializes the process before scanning.
		if (!gameMemoryServiceWrapper.Scan())
			return ScanResult.MarkerUnavailable;

		return gameMemoryService.IsInitialized ? ScanResult.Success : ScanResult.GameNotRunning;
	}

	private static void RenderTable(MainBlock mainBlock)
	{
		if (ImGui.BeginChild("MainBlockTableWrapper", new Vector2(0, 512)) && ImGui.BeginTable("MainBlockTable", 3, ImGuiTableFlags.ScrollY | ImGuiTableFlags.RowBg))
		{
			ImGui.TableSetupColumn("Field", ImGuiTableColumnFlags.WidthFixed, 224);
			ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.WidthStretch);

			ImGui.TableSetupScrollFreeze(0, 1);
			ImGui.TableHeadersRow();

			GroupRow("Header"u8);
			Row("Marker"u8, "string"u8, mainBlock.Marker);
			Row("FormatVersion"u8, "int"u8, mainBlock.FormatVersion);

			GroupRow("Player"u8);
			Row("PlayerId"u8, "int"u8, mainBlock.PlayerId);
			Row("PlayerName"u8, "string"u8, mainBlock.PlayerName);

			GroupRow("Run"u8);
			Row("Time"u8, "float"u8, mainBlock.Time);
			Row("GemsCollected"u8, "int"u8, mainBlock.GemsCollected);
			Row("EnemiesKilled"u8, "int"u8, mainBlock.EnemiesKilled);
			Row("DaggersFired"u8, "int"u8, mainBlock.DaggersFired);
			Row("DaggersHit"u8, "int"u8, mainBlock.DaggersHit);
			Row("EnemiesAlive"u8, "int"u8, mainBlock.EnemiesAlive);
			Row("LevelGems"u8, "int"u8, mainBlock.LevelGems);
			Row("HomingStored"u8, "int"u8, mainBlock.HomingStored);
			Row("GemsDespawned"u8, "int"u8, mainBlock.GemsDespawned);
			Row("GemsEaten"u8, "int"u8, mainBlock.GemsEaten);
			Row("GemsTotal"u8, "int"u8, mainBlock.GemsTotal);
			Row("HomingEaten"u8, "int"u8, mainBlock.HomingEaten);

			GroupRow("Alive counts"u8);
			Row("Skull1AliveCount"u8, "short"u8, mainBlock.Skull1AliveCount);
			Row("Skull2AliveCount"u8, "short"u8, mainBlock.Skull2AliveCount);
			Row("Skull3AliveCount"u8, "short"u8, mainBlock.Skull3AliveCount);
			Row("SpiderlingAliveCount"u8, "short"u8, mainBlock.SpiderlingAliveCount);
			Row("Skull4AliveCount"u8, "short"u8, mainBlock.Skull4AliveCount);
			Row("Squid1AliveCount"u8, "short"u8, mainBlock.Squid1AliveCount);
			Row("Squid2AliveCount"u8, "short"u8, mainBlock.Squid2AliveCount);
			Row("Squid3AliveCount"u8, "short"u8, mainBlock.Squid3AliveCount);
			Row("CentipedeAliveCount"u8, "short"u8, mainBlock.CentipedeAliveCount);
			Row("GigapedeAliveCount"u8, "short"u8, mainBlock.GigapedeAliveCount);
			Row("Spider1AliveCount"u8, "short"u8, mainBlock.Spider1AliveCount);
			Row("Spider2AliveCount"u8, "short"u8, mainBlock.Spider2AliveCount);
			Row("LeviathanAliveCount"u8, "short"u8, mainBlock.LeviathanAliveCount);
			Row("OrbAliveCount"u8, "short"u8, mainBlock.OrbAliveCount);
			Row("ThornAliveCount"u8, "short"u8, mainBlock.ThornAliveCount);
			Row("GhostpedeAliveCount"u8, "short"u8, mainBlock.GhostpedeAliveCount);
			Row("SpiderEggAliveCount"u8, "short"u8, mainBlock.SpiderEggAliveCount);

			GroupRow("Kill counts"u8);
			Row("Skull1KillCount"u8, "short"u8, mainBlock.Skull1KillCount);
			Row("Skull2KillCount"u8, "short"u8, mainBlock.Skull2KillCount);
			Row("Skull3KillCount"u8, "short"u8, mainBlock.Skull3KillCount);
			Row("SpiderlingKillCount"u8, "short"u8, mainBlock.SpiderlingKillCount);
			Row("Skull4KillCount"u8, "short"u8, mainBlock.Skull4KillCount);
			Row("Squid1KillCount"u8, "short"u8, mainBlock.Squid1KillCount);
			Row("Squid2KillCount"u8, "short"u8, mainBlock.Squid2KillCount);
			Row("Squid3KillCount"u8, "short"u8, mainBlock.Squid3KillCount);
			Row("CentipedeKillCount"u8, "short"u8, mainBlock.CentipedeKillCount);
			Row("GigapedeKillCount"u8, "short"u8, mainBlock.GigapedeKillCount);
			Row("Spider1KillCount"u8, "short"u8, mainBlock.Spider1KillCount);
			Row("Spider2KillCount"u8, "short"u8, mainBlock.Spider2KillCount);
			Row("LeviathanKillCount"u8, "short"u8, mainBlock.LeviathanKillCount);
			Row("OrbKillCount"u8, "short"u8, mainBlock.OrbKillCount);
			Row("ThornKillCount"u8, "short"u8, mainBlock.ThornKillCount);
			Row("GhostpedeKillCount"u8, "short"u8, mainBlock.GhostpedeKillCount);
			Row("SpiderEggKillCount"u8, "short"u8, mainBlock.SpiderEggKillCount);

			GroupRow("State"u8);
			Row("IsPlayerAlive"u8, "bool"u8, mainBlock.IsPlayerAlive);
			Row("IsReplay"u8, "bool"u8, mainBlock.IsReplay);
			Row("DeathType"u8, "byte"u8, DeathTypeUtils.InterpretDeathType(mainBlock.DeathType));
			Row("IsInGame"u8, "bool"u8, mainBlock.IsInGame);

			GroupRow("Replay player"u8);
			Row("ReplayPlayerId"u8, "int"u8, mainBlock.ReplayPlayerId);
			Row("ReplayPlayerName"u8, "string"u8, mainBlock.ReplayPlayerName);

			GroupRow("Spawnset"u8);
			Row("SurvivalHashMd5"u8, "byte[16]"u8, FormatHex(mainBlock.SurvivalHashMd5));

			GroupRow("Times"u8);
			Row("LevelUpTime2"u8, "float"u8, mainBlock.LevelUpTime2);
			Row("LevelUpTime3"u8, "float"u8, mainBlock.LevelUpTime3);
			Row("LevelUpTime4"u8, "float"u8, mainBlock.LevelUpTime4);
			Row("LeviathanDownTime"u8, "float"u8, mainBlock.LeviathanDownTime);
			Row("OrbDownTime"u8, "float"u8, mainBlock.OrbDownTime);

			GroupRow("Status"u8);
			Row("Status"u8, "int"u8, ((GameStatus)mainBlock.Status).ToDisplayString());

			GroupRow("Maxima"u8);
			Row("HomingMax"u8, "int"u8, mainBlock.HomingMax);
			Row("HomingMaxTime"u8, "float"u8, mainBlock.HomingMaxTime);
			Row("EnemiesAliveMax"u8, "int"u8, mainBlock.EnemiesAliveMax);
			Row("EnemiesAliveMaxTime"u8, "float"u8, mainBlock.EnemiesAliveMaxTime);
			Row("MaxTime"u8, "float"u8, mainBlock.MaxTime);

			GroupRow("Stats"u8);
			Row("StatsBase"u8, "long"u8, mainBlock.StatsBase);
			Row("StatsCount"u8, "int"u8, mainBlock.StatsCount);
			Row("StatsLoaded"u8, "bool"u8, mainBlock.StatsLoaded);

			GroupRow("Start"u8);
			Row("StartHandLevel"u8, "int"u8, mainBlock.StartHandLevel);
			Row("StartAdditionalGems"u8, "int"u8, mainBlock.StartAdditionalGems);
			Row("StartTimer"u8, "float"u8, mainBlock.StartTimer);
			Row("ProhibitedMods"u8, "bool"u8, mainBlock.ProhibitedMods);

			GroupRow("Replay"u8);
			Row("ReplayBase"u8, "long"u8, mainBlock.ReplayBase);
			Row("ReplayLength"u8, "int"u8, mainBlock.ReplayLength);
			Row("PlayReplayFromMemory"u8, "bool"u8, mainBlock.PlayReplayFromMemory);

			GroupRow("Game mode"u8);
			Row("GameMode"u8, "byte"u8, ((GameMode)mainBlock.GameMode).ToDisplayString());
			Row("TimeAttackOrRaceFinished"u8, "bool"u8, mainBlock.TimeAttackOrRaceFinished);

			ImGui.EndTable();
		}

		ImGui.EndChild();
	}

	private static void GroupRow(ReadOnlySpan<byte> name)
	{
		ImGui.TableNextRow();
		ImGui.TableNextColumn();
		ImGui.TextColored(Color.Aqua, name);
	}

	private static void Row(ReadOnlySpan<byte> name, ReadOnlySpan<byte> type, ReadOnlySpan<byte> value)
	{
		ImGui.TableNextRow();

		ImGui.TableNextColumn();
		ImGui.Text(name);

		ImGui.TableNextColumn();
		ImGui.TextColored(Color.Gray(0.5f), type);

		ImGui.TableNextColumn();
		ImGui.Text(value);
	}

	private static void Row<T>(ReadOnlySpan<byte> name, ReadOnlySpan<byte> type, T value)
		where T : IUtf8SpanFormattable
	{
		Row(name, type, Inline.Utf8(value));
	}

	private static void Row(ReadOnlySpan<byte> name, ReadOnlySpan<byte> type, bool value)
	{
		Row(name, type, value ? "True"u8 : "False"u8);
	}

	private static void Row(ReadOnlySpan<byte> name, ReadOnlySpan<byte> type, string value)
	{
		Row(name, type, Inline.Utf8(value.AsSpan()));
	}

	private static ReadOnlySpan<byte> FormatHex(byte[] bytes)
	{
		int bytesWritten = 0;
		for (int i = 0; i < bytes.Length; i++)
			Inline.Write(ref bytesWritten, bytes[i], "X2");

		return Inline.Terminate(bytesWritten);
	}
}
