using DevilDaggersInfo.Core.Common.Extensions;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.GameMemory.Enemies.Data;
using ImGuiNET;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace DevilDaggersInfo.Tools.Ui.MemoryTool;

public static class MemoryToolWindow
{
	private static readonly char[] _survivalHashMd5HexString = new char[16 * 2];

	public static void Render()
	{
		if (ImGui.Begin("Main Block"))
		{
			if (Root.GameMemoryService.IsInitialized)
			{
				RenderMainBlockTable();
			}
			else
			{
				ImGui.TextWrapped("Devil Daggers is not running.");
			}
		}

		ImGui.End();

		if (ImGui.Begin("Experimental Memory"))
		{
			ImGui.SliderFloat("Scan interval", ref ExperimentalMemory.ScanInterval, 0.01f, 1f, "%.2f");

			if (Root.GameMemoryService.IsInitialized)
			{
				// byte[] buffer = Root.GameMemoryService.ReadExperimental(0x002513B0, 0x28, [0x0, 0x20]);
				// RenderBuffer(buffer);

				RenderThornsTable();
				RenderSpidersTable();
				RenderLeviathansTable();
				RenderSquidsTable();
				RenderPedeTable();
				RenderBoidsTable();

				if (ImGui.Button("Kill Leviathan"))
					ExperimentalMemory.KillLeviathan();
				if (ImGui.Button("Kill Orb"))
					ExperimentalMemory.KillOrb();
				if (ImGui.Button("Kill All Squids"))
					ExperimentalMemory.KillAllSquids();
			}
			else
			{
				ImGui.TextWrapped("Devil Daggers is not running.");
			}
		}

		ImGui.End();
	}

	private static void RenderThornsTable()
	{
		if (ExperimentalMemory.Thorns.Count == 0)
			return;

		ImGui.SeparatorText("Thorns");

		if (ImGui.BeginTable("Thorns", 6, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable))
		{
			ImGui.TableSetupColumn("State");
			ImGui.TableSetupColumn("IsAlive");
			ImGui.TableSetupColumn("StateTimer");
			ImGui.TableSetupColumn("HP");
			ImGui.TableSetupColumn("Position");
			ImGui.TableSetupColumn("Rotation");
			ImGui.TableHeadersRow();

			for (int i = 0; i < ExperimentalMemory.Thorns.Count; i++)
			{
				Thorn thorn = ExperimentalMemory.Thorns[i];

				ImGui.TableNextRow();
				NextColumnText(Inline.Span(thorn.State));
				NextColumnText(Inline.Span(thorn.IsAlive));
				NextColumnText(Inline.Span(thorn.StateTimer));
				NextColumnText(Inline.Span(thorn.Hp));
				NextColumnText(Inline.Span(thorn.Position));
				NextColumnText(Inline.Span(thorn.Rotation));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderSpidersTable()
	{
		if (ExperimentalMemory.Spiders.Count == 0)
			return;

		ImGui.SeparatorText("Spiders");

		if (ImGui.BeginTable("Spiders", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable))
		{
			ImGui.TableSetupColumn("HP");
			ImGui.TableHeadersRow();

			for (int i = 0; i < ExperimentalMemory.Spiders.Count; i++)
			{
				Spider spider = ExperimentalMemory.Spiders[i];

				ImGui.TableNextRow();
				NextColumnText(Inline.Span(spider.Hp));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderLeviathansTable()
	{
		if (ExperimentalMemory.Leviathans.Count == 0)
			return;

		ImGui.SeparatorText("Leviathans");

		if (ImGui.BeginTable("Leviathans", 7, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable))
		{
			ImGui.TableSetupColumn("HP 1");
			ImGui.TableSetupColumn("HP 2");
			ImGui.TableSetupColumn("HP 3");
			ImGui.TableSetupColumn("HP 4");
			ImGui.TableSetupColumn("HP 5");
			ImGui.TableSetupColumn("HP 6");
			ImGui.TableSetupColumn("HP Orb");
			ImGui.TableHeadersRow();

			for (int i = 0; i < ExperimentalMemory.Leviathans.Count; i++)
			{
				Leviathan leviathan = ExperimentalMemory.Leviathans[i];

				ImGui.TableNextRow();
				NextColumnText(Inline.Span(leviathan.NodeHp1));
				NextColumnText(Inline.Span(leviathan.NodeHp2));
				NextColumnText(Inline.Span(leviathan.NodeHp3));
				NextColumnText(Inline.Span(leviathan.NodeHp4));
				NextColumnText(Inline.Span(leviathan.NodeHp5));
				NextColumnText(Inline.Span(leviathan.NodeHp6));
				NextColumnText(Inline.Span(leviathan.OrbHp));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderSquidsTable()
	{
		if (ExperimentalMemory.Squids.Count == 0)
			return;

		ImGui.SeparatorText("Squids");

		if (ImGui.BeginTable("Squids", 5, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable))
		{
			ImGui.TableSetupColumn("Type");
			ImGui.TableSetupColumn("HP");
			ImGui.TableSetupColumn("Timer");
			ImGui.TableSetupColumn("Position");
			ImGui.TableSetupColumn("Gush Countdown");

			ImGui.TableHeadersRow();

			for (int i = 0; i < ExperimentalMemory.Squids.Count; i++)
			{
				Squid squid = ExperimentalMemory.Squids[i];

				ImGui.TableNextRow();
				NextColumnText(Inline.Span(squid.Type));
				NextColumnText(Inline.Span(squid.NodeHp1));
				if (squid.Type is SquidType.Squid2 or SquidType.Squid3)
				{
					ImGui.SameLine();
					ImGui.Text(Inline.Span(squid.NodeHp2));
					if (squid.Type == SquidType.Squid3)
					{
						ImGui.SameLine();
						ImGui.Text(Inline.Span(squid.NodeHp3));
					}
				}

				NextColumnText(Inline.Span(squid.Timer, "0.00"));
				NextColumnText(Inline.Span(squid.Position, "0.00"));
				NextColumnText(Inline.Span(squid.GushCountDown, "0.00"));

				if (ImGui.CollapsingHeader(Inline.Span($"Show buffer##{i}")))
				{
					byte[] bytes = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref squid, 1)).ToArray();
					RenderBuffer(bytes);
				}
			}

			ImGui.EndTable();
		}
	}

	private static void RenderPedeTable()
	{
		if (ExperimentalMemory.Pedes.Count == 0)
			return;

		ImGui.SeparatorText("Pedes");

		if (ImGui.BeginTable("Pedes", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable))
		{
			ImGui.TableSetupColumn("Segment HP");
			ImGui.TableHeadersRow();

			for (int i = 0; i < ExperimentalMemory.Pedes.Count; i++)
			{
				Pede pede = ExperimentalMemory.Pedes[i];

				ImGui.TableNextRow();

				Span<PedeSegment> spanOf50 = MemoryMarshal.CreateSpan(ref Unsafe.As<PedeSegments, PedeSegment>(ref pede.Segments), 50);
				StringBuilder sb = new();
				for (int j = 0; j < 50; j++)
					sb.Append($"{spanOf50[j].Hp} ");
				NextColumnText(sb.ToString());
			}

			ImGui.EndTable();
		}
	}

	private static void RenderBoidsTable()
	{
		if (ExperimentalMemory.Boids.Count == 0)
			return;

		ImGui.SeparatorText("Boids");

		if (ImGui.BeginTable("Boids", 9, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable))
		{
			ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("HP", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Spawner Id", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 192);
			ImGui.TableSetupColumn("Velocity", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableSetupColumn("Base Speed", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Rotation", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableSetupColumn("Unknown Vec3", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableSetupColumn("Timer", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableHeadersRow();

			for (int i = 0; i < ExperimentalMemory.Boids.Count; i++)
			{
				const string floatFormat = "+00.00;-00.00;+00.00";

				Boid boid = ExperimentalMemory.Boids[i];

				ImGui.TableNextRow();
				NextColumnText(Inline.Span(boid.Type));
				NextColumnText(Inline.Span(boid.Hp));
				NextColumnText(Inline.Span(boid.SpawnerId));
				NextColumnText(Inline.Span(boid.Position, floatFormat));
				NextColumnText(Inline.Span(boid.Velocity, floatFormat));
				NextColumnText(Inline.Span(boid.BaseSpeed, floatFormat));
				NextColumnText(Inline.Span(boid.Rotation, floatFormat));

				// Get span of floats
				Span<float> spanOfFloats = MemoryMarshal.CreateSpan(ref Unsafe.As<Matrix4x4, float>(ref boid.Floats), 16);
				StringBuilder sb = new();
				for (int j = 0; j < 16; j++)
					sb.Append($"{spanOfFloats[j].ToString(floatFormat)} ");
				NextColumnText(sb.ToString());

				NextColumnText(Inline.Span(boid.Timer, floatFormat));

				if (ImGui.CollapsingHeader(Inline.Span($"Show buffer##{i}")))
				{
					byte[] bytes = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref boid, 1)).ToArray();
					RenderBuffer(bytes);
				}
			}

			ImGui.EndTable();
		}
	}

	private static void RenderBuffer(byte[] bytes)
	{
		for (int j = 0; j < bytes.Length; j += 4)
		{
			Span<byte> span = bytes[j..(j + 4)].AsSpan();
			ImGui.TextWrapped(span.ToArray().ByteArrayToHexString());
			if (ImGui.IsItemHovered())
			{
				ImGui.SetTooltip($"""
				                  Offset:  0x{j:X} ({j})
				                  Integer: {BitConverter.ToInt32(bytes, j)}
				                  Float:   {BitConverter.ToSingle(bytes, j)}
				                  """);
			}

			if (j == 0 || j % 64 != 0)
				ImGui.SameLine();
		}
	}

	private static void RenderMainBlockTable()
	{
		if (ImGui.BeginTable("MainBlockTable", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable))
		{
			ImGui.TableSetupColumn("Name");
			ImGui.TableSetupColumn("Value");
			ImGui.TableHeadersRow();

			MainBlock mainBlock = Root.GameMemoryService.MainBlock;
			for (int i = 0; i < 16; i++)
				mainBlock.SurvivalHashMd5[i].TryFormat(_survivalHashMd5HexString.AsSpan()[(i * 2)..], out _, "X2");

			Column("Process base address", Inline.Span($"0x{Root.GameMemoryService.ProcessBaseAddress:X}"));
			Column("Marker offset", Inline.Span($"0x{Root.GameMemoryService.DdstatsMarkerOffset:X}"));

			Column("Marker", mainBlock.Marker);
			Column("Format version", Inline.Span(mainBlock.FormatVersion));
			Column("Player ID", Inline.Span(mainBlock.PlayerId));
			Column("Player name", mainBlock.PlayerName);
			Column("Time", Inline.Span(mainBlock.Time));
			Column("Gems collected", Inline.Span(mainBlock.GemsCollected));
			Column("Enemies killed", Inline.Span(mainBlock.EnemiesKilled));
			Column("Daggers fired", Inline.Span(mainBlock.DaggersFired));
			Column("Daggers hit", Inline.Span(mainBlock.DaggersHit));
			Column("Enemies alive", Inline.Span(mainBlock.EnemiesAlive));
			Column("Level gems", Inline.Span(mainBlock.LevelGems));
			Column("Homing stored", Inline.Span(mainBlock.HomingStored));
			Column("Gems despawned", Inline.Span(mainBlock.GemsDespawned));
			Column("Gems eaten", Inline.Span(mainBlock.GemsEaten));
			Column("Gems total", Inline.Span(mainBlock.GemsTotal));
			Column("Homing eaten", Inline.Span(mainBlock.HomingEaten));

			Column("Skull 1 alive count", Inline.Span(mainBlock.Skull1AliveCount));
			Column("Skull 2 alive count", Inline.Span(mainBlock.Skull2AliveCount));
			Column("Skull 3 alive count", Inline.Span(mainBlock.Skull3AliveCount));
			Column("Spiderling alive count", Inline.Span(mainBlock.SpiderlingAliveCount));
			Column("Skull 4 alive count", Inline.Span(mainBlock.Skull4AliveCount));
			Column("Squid 1 alive count", Inline.Span(mainBlock.Squid1AliveCount));
			Column("Squid 2 alive count", Inline.Span(mainBlock.Squid2AliveCount));
			Column("Squid 3 alive count", Inline.Span(mainBlock.Squid3AliveCount));
			Column("Centipede alive count", Inline.Span(mainBlock.CentipedeAliveCount));
			Column("Gigapede alive count", Inline.Span(mainBlock.GigapedeAliveCount));
			Column("Spider 1 alive count", Inline.Span(mainBlock.Spider1AliveCount));
			Column("Spider 2 alive count", Inline.Span(mainBlock.Spider2AliveCount));
			Column("Leviathan alive count", Inline.Span(mainBlock.LeviathanAliveCount));
			Column("Orb alive count", Inline.Span(mainBlock.OrbAliveCount));
			Column("Thorn alive count", Inline.Span(mainBlock.ThornAliveCount));
			Column("Ghostpede alive count", Inline.Span(mainBlock.GhostpedeAliveCount));
			Column("Spider egg alive count", Inline.Span(mainBlock.SpiderEggAliveCount));

			Column("Skull 1 kill count", Inline.Span(mainBlock.Skull1KillCount));
			Column("Skull 2 kill count", Inline.Span(mainBlock.Skull2KillCount));
			Column("Skull 3 kill count", Inline.Span(mainBlock.Skull3KillCount));
			Column("Spiderling kill count", Inline.Span(mainBlock.SpiderlingKillCount));
			Column("Skull 4 kill count", Inline.Span(mainBlock.Skull4KillCount));
			Column("Squid 1 kill count", Inline.Span(mainBlock.Squid1KillCount));
			Column("Squid 2 kill count", Inline.Span(mainBlock.Squid2KillCount));
			Column("Squid 3 kill count", Inline.Span(mainBlock.Squid3KillCount));
			Column("Centipede kill count", Inline.Span(mainBlock.CentipedeKillCount));
			Column("Gigapede kill count", Inline.Span(mainBlock.GigapedeKillCount));
			Column("Spider 1 kill count", Inline.Span(mainBlock.Spider1KillCount));
			Column("Spider 2 kill count", Inline.Span(mainBlock.Spider2KillCount));
			Column("Leviathan kill count", Inline.Span(mainBlock.LeviathanKillCount));
			Column("Orb kill count", Inline.Span(mainBlock.OrbKillCount));
			Column("Thorn kill count", Inline.Span(mainBlock.ThornKillCount));
			Column("Ghostpede kill count", Inline.Span(mainBlock.GhostpedeKillCount));
			Column("Spider egg kill count", Inline.Span(mainBlock.SpiderEggKillCount));

			Column("Is player alive", mainBlock.IsPlayerAlive ? "True" : "False");
			Column("Is replay", mainBlock.IsReplay ? "True" : "False");
			Column("Death type", Inline.Span(mainBlock.DeathType));
			Column("Is in game", mainBlock.IsInGame ? "True" : "False");

			Column("Replay player ID", Inline.Span(mainBlock.ReplayPlayerId));
			Column("Replay player name", mainBlock.ReplayPlayerName);

			Column("Survival hash MD5", _survivalHashMd5HexString);

			Column("Level up time 2", Inline.Span(mainBlock.LevelUpTime2));
			Column("Level up time 3", Inline.Span(mainBlock.LevelUpTime3));
			Column("Level up time 4", Inline.Span(mainBlock.LevelUpTime4));

			Column("Leviathan down time", Inline.Span(mainBlock.LeviathanDownTime));
			Column("Orb down time", Inline.Span(mainBlock.OrbDownTime));

			Column("Status", Inline.Span(mainBlock.Status));

			Column("Homing max", Inline.Span(mainBlock.HomingMax));
			Column("Homing max time", Inline.Span(mainBlock.HomingMaxTime));
			Column("Enemies alive max", Inline.Span(mainBlock.EnemiesAliveMax));
			Column("Enemies alive max time", Inline.Span(mainBlock.EnemiesAliveMaxTime));
			Column("Max time", Inline.Span(mainBlock.MaxTime));
			Column("Stats base", Inline.Span($"0x{mainBlock.StatsBase:X}"));
			Column("Stats count", Inline.Span(mainBlock.StatsCount));
			Column("Stats loaded", mainBlock.StatsLoaded ? "True" : "False");

			Column("Start hand level", Inline.Span(mainBlock.StartHandLevel));
			Column("Start additional gems", Inline.Span(mainBlock.StartAdditionalGems));
			Column("Start timer", Inline.Span(mainBlock.StartTimer));
			Column("Prohibited mods", mainBlock.ProhibitedMods ? "True" : "False");
			Column("Replay base", Inline.Span($"0x{mainBlock.ReplayBase:X}"));
			Column("Replay length", Inline.Span(mainBlock.ReplayLength));
			Column("Play replay from memory", mainBlock.PlayReplayFromMemory ? "True" : "False");
			Column("Game mode", Inline.Span(mainBlock.GameMode));
			Column("Time attack or race finished", mainBlock.TimeAttackOrRaceFinished ? "True" : "False");

			ImGui.EndTable();
		}
	}

	private static void Column(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
	{
		NextColumnText(left);
		NextColumnText(right);
	}

	private static void NextColumnText(ReadOnlySpan<char> text)
	{
		ImGui.TableNextColumn();
		ImGui.Text(text);
	}

	private static void NextColumnTextOptional(ReadOnlySpan<char> text, bool condition)
	{
		ImGui.TableNextColumn();
		ImGui.TextColored(condition ? Color.White : Color.Gray(0.4f), condition ? text : "-");
	}
}
