using DevilDaggersInfo.Core.Common.Extensions;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.GameMemory.Enemies;
using ImGuiNET;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace DevilDaggersInfo.Tools.Ui.MemoryTool;

public static class MemoryToolWindow
{
	private static readonly char[] _survivalHashMd5HexString = new char[16 * 2];
	private static float _recordingTimer;
	private static float _scanInterval = 0.25f;
	private static MainBlock _mainBlock;

	private static readonly List<Thorn> _thorns = [];
	private static readonly List<Spider> _spiders = [];
	private static readonly List<Leviathan> _leviathans = [];
	private static readonly List<Squid> _squids = [];
	private static readonly List<Pede> _pedes = [];
	private static readonly List<Boid> _boids = [];

	public static void Update(float delta)
	{
		_recordingTimer += delta;
		if (_recordingTimer < _scanInterval)
			return;

		_recordingTimer = 0;
		if (!GameMemoryServiceWrapper.Scan() || !Root.GameMemoryService.IsInitialized)
			return;

		_mainBlock = Root.GameMemoryService.MainBlock;

		int thornListLength = _mainBlock.ThornAliveCount;
		int spiderListLength = _mainBlock.Spider1AliveCount + _mainBlock.Spider2AliveCount;
		int leviathanListLength = _mainBlock.LeviathanAliveCount + _mainBlock.OrbAliveCount;
		int squidListLength = _mainBlock.Squid1AliveCount + _mainBlock.Squid2AliveCount + _mainBlock.Squid3AliveCount;
		int pedeListLength = _mainBlock.CentipedeAliveCount + _mainBlock.GigapedeAliveCount + _mainBlock.GhostpedeAliveCount;
		int boidListLength = _mainBlock.Skull1AliveCount + _mainBlock.Skull2AliveCount + _mainBlock.Skull3AliveCount + _mainBlock.Skull4AliveCount + _mainBlock.SpiderlingAliveCount;

		ReadEnemyList(_thorns, thornListLength, 0x002513B0, StructSizes.Thorn, [0x0, 0x28, 0]);
		ReadEnemyList(_spiders, spiderListLength, 0x00251830, StructSizes.Spider, [0x0, 0x28, 0]);
		ReadEnemyList(_leviathans, leviathanListLength, 0x00251590, StructSizes.Leviathan, [0x0, 0x28, 0]);
		ReadEnemyList(_squids, squidListLength, 0x00251890, StructSizes.Squid, [0x0, 0x18, 0]);
		ReadEnemyList(_pedes, pedeListLength, 0x00251470, StructSizes.Pede, [0x0, 0x28, 0]);
		ReadEnemyList(_boids, boidListLength, 0x00251410, StructSizes.Boid, [0x0, 0x20, 0]);
	}

	private static void ReadEnemyList<T>(List<T> list, int count, long address, int size, Span<int> offsets)
		where T : unmanaged
	{
		list.Clear();
		for (int i = 0; i < count; i++)
		{
			list.Add(Read<T>(address, size, offsets));
			offsets[^1] += size;
		}
	}

	public static void Render()
	{
		if (ImGui.Begin("Main Block"))
		{
			RenderMainBlockTable();
		}

		ImGui.End();

		if (ImGui.Begin("Experimental Memory"))
		{
			ImGui.SliderFloat("Scan interval", ref _scanInterval, 0.01f, 1f, "%.2f");

			if (Root.GameMemoryService.IsInitialized)
			{
				RenderThornsTable();
				RenderSpidersTable();
				RenderLeviathansTable();
				RenderSquidsTable();
				RenderPedeTable();
				RenderBoidsTable();

				if (ImGui.Button("Kill Levi"))
				{
					for (int i = 0; i < 6; i++)
						Root.GameMemoryService.WriteExperimental(0x00251590, [0x0, 0x28, 0x84 + i * 56], 0);
				}

				if (ImGui.Button("Kill Orb"))
					Root.GameMemoryService.WriteExperimental(0x00251590, [0x0, 0x28, 0x84 + 6 * 56 + 8], 0);

				if (ImGui.Button("Kill All Squids"))
				{
					for (int i = 0; i < Root.GameMemoryService.MainBlock.Squid1AliveCount + Root.GameMemoryService.MainBlock.Squid2AliveCount + Root.GameMemoryService.MainBlock.Squid3AliveCount; i++)
					{
						Root.GameMemoryService.WriteExperimental(0x00251890, [0x0, 0x18, i * StructSizes.Squid + 148], 0);
						Root.GameMemoryService.WriteExperimental(0x00251890, [0x0, 0x18, i * StructSizes.Squid + 152], 0);
						Root.GameMemoryService.WriteExperimental(0x00251890, [0x0, 0x18, i * StructSizes.Squid + 156], 0);
					}
				}
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
		if (_thorns.Count == 0)
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

			for (int i = 0; i < _thorns.Count; i++)
			{
				Thorn thorn = _thorns[i];

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
		if (_spiders.Count == 0)
			return;

		ImGui.SeparatorText("Spiders");

		if (ImGui.BeginTable("Spiders", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable))
		{
			ImGui.TableSetupColumn("HP");
			ImGui.TableHeadersRow();

			for (int i = 0; i < _spiders.Count; i++)
			{
				Spider spider = _spiders[i];

				ImGui.TableNextRow();
				NextColumnText(Inline.Span(spider.Hp));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderLeviathansTable()
	{
		if (_leviathans.Count == 0)
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

			for (int i = 0; i < _leviathans.Count; i++)
			{
				Leviathan leviathan = _leviathans[i];

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
		if (_squids.Count == 0)
			return;

		ImGui.SeparatorText("Squids");

		if (ImGui.BeginTable("Squids", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable))
		{
			ImGui.TableSetupColumn("Type");
			ImGui.TableSetupColumn("HP 1");
			ImGui.TableSetupColumn("HP 2");
			ImGui.TableSetupColumn("HP 3");
			ImGui.TableHeadersRow();

			for (int i = 0; i < _squids.Count; i++)
			{
				Squid squid = _squids[i];

				ImGui.TableNextRow();
				NextColumnText(Inline.Span(squid.Type));
				NextColumnText(Inline.Span(squid.HpNode1));
				NextColumnTextOptional(Inline.Span(squid.HpNode2), squid.Type is SquidType.Squid2 or SquidType.Squid3);
				NextColumnTextOptional(Inline.Span(squid.HpNode3), squid.Type == SquidType.Squid3);
			}

			ImGui.EndTable();
		}
	}

	private static void RenderPedeTable()
	{
		if (_pedes.Count == 0)
			return;

		ImGui.SeparatorText("Pedes");

		if (ImGui.BeginTable("Pedes", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable))
		{
			ImGui.TableSetupColumn("Segment HP");
			ImGui.TableHeadersRow();

			for (int i = 0; i < _pedes.Count; i++)
			{
				Pede pede = _pedes[i];

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
		if (_boids.Count == 0)
			return;

		ImGui.SeparatorText("Boids");

		if (ImGui.BeginTable("Boids", 9, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable))
		{
			ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("HP", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Spawner Id", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 192);
			ImGui.TableSetupColumn("Velocity", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableSetupColumn("Speed", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Rotation", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableSetupColumn("Unknown Vec3", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableSetupColumn("Timer", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableHeadersRow();

			for (int i = 0; i < _boids.Count; i++)
			{
				const string floatFormat = "+00.00;-00.00;+00.00";

				Boid boid = _boids[i];

				ImGui.TableNextRow();
				NextColumnText(Inline.Span(boid.Type));
				NextColumnText(Inline.Span(boid.Hp));
				NextColumnText(Inline.Span(boid.SpawnerId));
				NextColumnText(Inline.Span(boid.Position, floatFormat));
				NextColumnText(Inline.Span(boid.Velocity, floatFormat));
				NextColumnText(Inline.Span(boid.Speed, floatFormat));
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
			}

			ImGui.EndTable();
		}
	}

	private static T Read<T>(long address, int size, Span<int> offsets)
		where T : unmanaged
	{
		byte[] buffer = Root.GameMemoryService.ReadExperimental(address, size, offsets);
		return MemoryMarshal.Read<T>(buffer);
	}

	private static void RenderMainBlockTable()
	{
		if (ImGui.BeginTable("MainBlockTable", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable))
		{
			ImGui.TableSetupColumn("Name");
			ImGui.TableSetupColumn("Value");
			ImGui.TableHeadersRow();

			for (int i = 0; i < 16; i++)
				_mainBlock.SurvivalHashMd5[i].TryFormat(_survivalHashMd5HexString.AsSpan()[(i * 2)..], out _, "X2");

			Column("Process base address", Inline.Span($"0x{Root.GameMemoryService.ProcessBaseAddress:X}"));
			Column("Marker offset", Inline.Span($"0x{Root.GameMemoryService.DdstatsMarkerOffset:X}"));

			Column("Marker", _mainBlock.Marker);
			Column("Format version", Inline.Span(_mainBlock.FormatVersion));
			Column("Player ID", Inline.Span(_mainBlock.PlayerId));
			Column("Player name", _mainBlock.PlayerName);
			Column("Time", Inline.Span(_mainBlock.Time));
			Column("Gems collected", Inline.Span(_mainBlock.GemsCollected));
			Column("Enemies killed", Inline.Span(_mainBlock.EnemiesKilled));
			Column("Daggers fired", Inline.Span(_mainBlock.DaggersFired));
			Column("Daggers hit", Inline.Span(_mainBlock.DaggersHit));
			Column("Enemies alive", Inline.Span(_mainBlock.EnemiesAlive));
			Column("Level gems", Inline.Span(_mainBlock.LevelGems));
			Column("Homing stored", Inline.Span(_mainBlock.HomingStored));
			Column("Gems despawned", Inline.Span(_mainBlock.GemsDespawned));
			Column("Gems eaten", Inline.Span(_mainBlock.GemsEaten));
			Column("Gems total", Inline.Span(_mainBlock.GemsTotal));
			Column("Homing eaten", Inline.Span(_mainBlock.HomingEaten));

			Column("Skull 1 alive count", Inline.Span(_mainBlock.Skull1AliveCount));
			Column("Skull 2 alive count", Inline.Span(_mainBlock.Skull2AliveCount));
			Column("Skull 3 alive count", Inline.Span(_mainBlock.Skull3AliveCount));
			Column("Spiderling alive count", Inline.Span(_mainBlock.SpiderlingAliveCount));
			Column("Skull 4 alive count", Inline.Span(_mainBlock.Skull4AliveCount));
			Column("Squid 1 alive count", Inline.Span(_mainBlock.Squid1AliveCount));
			Column("Squid 2 alive count", Inline.Span(_mainBlock.Squid2AliveCount));
			Column("Squid 3 alive count", Inline.Span(_mainBlock.Squid3AliveCount));
			Column("Centipede alive count", Inline.Span(_mainBlock.CentipedeAliveCount));
			Column("Gigapede alive count", Inline.Span(_mainBlock.GigapedeAliveCount));
			Column("Spider 1 alive count", Inline.Span(_mainBlock.Spider1AliveCount));
			Column("Spider 2 alive count", Inline.Span(_mainBlock.Spider2AliveCount));
			Column("Leviathan alive count", Inline.Span(_mainBlock.LeviathanAliveCount));
			Column("Orb alive count", Inline.Span(_mainBlock.OrbAliveCount));
			Column("Thorn alive count", Inline.Span(_mainBlock.ThornAliveCount));
			Column("Ghostpede alive count", Inline.Span(_mainBlock.GhostpedeAliveCount));
			Column("Spider egg alive count", Inline.Span(_mainBlock.SpiderEggAliveCount));

			Column("Skull 1 kill count", Inline.Span(_mainBlock.Skull1KillCount));
			Column("Skull 2 kill count", Inline.Span(_mainBlock.Skull2KillCount));
			Column("Skull 3 kill count", Inline.Span(_mainBlock.Skull3KillCount));
			Column("Spiderling kill count", Inline.Span(_mainBlock.SpiderlingKillCount));
			Column("Skull 4 kill count", Inline.Span(_mainBlock.Skull4KillCount));
			Column("Squid 1 kill count", Inline.Span(_mainBlock.Squid1KillCount));
			Column("Squid 2 kill count", Inline.Span(_mainBlock.Squid2KillCount));
			Column("Squid 3 kill count", Inline.Span(_mainBlock.Squid3KillCount));
			Column("Centipede kill count", Inline.Span(_mainBlock.CentipedeKillCount));
			Column("Gigapede kill count", Inline.Span(_mainBlock.GigapedeKillCount));
			Column("Spider 1 kill count", Inline.Span(_mainBlock.Spider1KillCount));
			Column("Spider 2 kill count", Inline.Span(_mainBlock.Spider2KillCount));
			Column("Leviathan kill count", Inline.Span(_mainBlock.LeviathanKillCount));
			Column("Orb kill count", Inline.Span(_mainBlock.OrbKillCount));
			Column("Thorn kill count", Inline.Span(_mainBlock.ThornKillCount));
			Column("Ghostpede kill count", Inline.Span(_mainBlock.GhostpedeKillCount));
			Column("Spider egg kill count", Inline.Span(_mainBlock.SpiderEggKillCount));

			Column("Is player alive", _mainBlock.IsPlayerAlive ? "True" : "False");
			Column("Is replay", _mainBlock.IsReplay ? "True" : "False");
			Column("Death type", Inline.Span(_mainBlock.DeathType));
			Column("Is in game", _mainBlock.IsInGame ? "True" : "False");

			Column("Replay player ID", Inline.Span(_mainBlock.ReplayPlayerId));
			Column("Replay player name", _mainBlock.ReplayPlayerName);

			Column("Survival hash MD5", _survivalHashMd5HexString);

			Column("Level up time 2", Inline.Span(_mainBlock.LevelUpTime2));
			Column("Level up time 3", Inline.Span(_mainBlock.LevelUpTime3));
			Column("Level up time 4", Inline.Span(_mainBlock.LevelUpTime4));

			Column("Leviathan down time", Inline.Span(_mainBlock.LeviathanDownTime));
			Column("Orb down time", Inline.Span(_mainBlock.OrbDownTime));

			Column("Status", Inline.Span(_mainBlock.Status));

			Column("Homing max", Inline.Span(_mainBlock.HomingMax));
			Column("Homing max time", Inline.Span(_mainBlock.HomingMaxTime));
			Column("Enemies alive max", Inline.Span(_mainBlock.EnemiesAliveMax));
			Column("Enemies alive max time", Inline.Span(_mainBlock.EnemiesAliveMaxTime));
			Column("Max time", Inline.Span(_mainBlock.MaxTime));
			Column("Stats base", Inline.Span($"0x{_mainBlock.StatsBase:X}"));
			Column("Stats count", Inline.Span(_mainBlock.StatsCount));
			Column("Stats loaded", _mainBlock.StatsLoaded ? "True" : "False");

			Column("Start hand level", Inline.Span(_mainBlock.StartHandLevel));
			Column("Start additional gems", Inline.Span(_mainBlock.StartAdditionalGems));
			Column("Start timer", Inline.Span(_mainBlock.StartTimer));
			Column("Prohibited mods", _mainBlock.ProhibitedMods ? "True" : "False");
			Column("Replay base", Inline.Span($"0x{_mainBlock.ReplayBase:X}"));
			Column("Replay length", Inline.Span(_mainBlock.ReplayLength));
			Column("Play replay from memory", _mainBlock.PlayReplayFromMemory ? "True" : "False");
			Column("Game mode", Inline.Span(_mainBlock.GameMode));
			Column("Time attack or race finished", _mainBlock.TimeAttackOrRaceFinished ? "True" : "False");

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
