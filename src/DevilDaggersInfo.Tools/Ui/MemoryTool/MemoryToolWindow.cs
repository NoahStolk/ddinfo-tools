using DevilDaggersInfo.Core.Common.Extensions;
using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.GameMemory.Enemies;
using ImGuiNET;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.Ui.MemoryTool;

public static class MemoryToolWindow
{
	private static readonly char[] _mainBufferHexString = new char[GameMemoryService.MainBufferSize * 2];
	private static readonly char[] _survivalHashMd5HexString = new char[16 * 2];
	private static float _recordingTimer;
	private static MainBlock _mainBlock;

	public static void Update(float delta)
	{
		_recordingTimer += delta;
		if (_recordingTimer < 0.5f)
			return;

		_recordingTimer = 0;
		if (!GameMemoryServiceWrapper.Scan() || !Root.GameMemoryService.IsInitialized)
			return;

		_mainBlock = Root.GameMemoryService.MainBlock;
	}

	public static void Render()
	{
		if (ImGui.Begin("Memory Tool"))
		{
			if (Root.GameMemoryService.IsInitialized)
			{
				RenderBuffer();
				RenderMainBlockTable();

				ImGui.SeparatorText("EXPERIMENTAL");

				// Alternative address: 0x00251350, [0x0, 0x198, 0x38, 0x28, 0x0]
				int thornOffset = 0;
				for (int i = 0; i < Root.GameMemoryService.MainBlock.ThornAliveCount; i++)
				{
					//do
					{
						RenderExperimentalBuffer<Thorn>(Inline.Span($"Thorn {i}"), 0x002513B0, StructSizes.Thorn, [0x0, 0x28, thornOffset]);
						thornOffset += StructSizes.Thorn;
					}
					//while (thorn.Hp == 0); // Dead Thorns aren't cleared up from game memory immediately, so skip them until they get cleared up.
				}

				int spiderOffset = 0;
				for (int i = 0; i < Root.GameMemoryService.MainBlock.Spider1AliveCount + Root.GameMemoryService.MainBlock.Spider2AliveCount; i++)
				{
					RenderExperimentalBuffer<Spider>(Inline.Span($"Spider {i}"), 0x00251830, StructSizes.Spider, [0x0, 0x28, spiderOffset]);
					spiderOffset += StructSizes.Spider;
				}

				int leviathanOffset = 0;
				for (int i = 0; i < Root.GameMemoryService.MainBlock.LeviathanAliveCount + Root.GameMemoryService.MainBlock.OrbAliveCount; i++)
				{
					RenderExperimentalBuffer<Leviathan>("Leviathan", 0x00251590, StructSizes.Leviathan, [0x0, 0x28, leviathanOffset]);
					leviathanOffset += StructSizes.Leviathan;
				}

				int squidOffset = 0;
				for (int i = 0; i < Root.GameMemoryService.MainBlock.Squid1AliveCount + Root.GameMemoryService.MainBlock.Squid2AliveCount + Root.GameMemoryService.MainBlock.Squid3AliveCount; i++)
				{
					RenderExperimentalBuffer<Squid>("Squid", 0x00251890, StructSizes.Squid, [0x0, 0x18, 0x94 + squidOffset]);
					squidOffset += StructSizes.Squid;
				}

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
						Root.GameMemoryService.WriteExperimental(0x00251890, [0x0, 0x18, 0x94 + i * StructSizes.Squid + 0], 0);
						Root.GameMemoryService.WriteExperimental(0x00251890, [0x0, 0x18, 0x94 + i * StructSizes.Squid + 4], 0);
						Root.GameMemoryService.WriteExperimental(0x00251890, [0x0, 0x18, 0x94 + i * StructSizes.Squid + 8], 0);
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

	private static void RenderExperimentalBuffer<T>(ReadOnlySpan<char> name, long address, int size, int[] offsets)
		where T : unmanaged
	{
		if (ImGui.CollapsingHeader(name))
		{
			byte[] buffer = Root.GameMemoryService.ReadExperimental(address, size, offsets);

			T value = MemoryMarshal.Read<T>(buffer);
			FieldInfo[] fields = value.GetType().GetFields();
			foreach (FieldInfo field in fields)
			{
				string fieldValue = field.GetValue(value)?.ToString() ?? "null";
				ImGui.Text($"{field.Name}: {fieldValue}");
			}

			ImGui.TextWrapped(InsertStrings(buffer.ByteArrayToHexString(), 8, " "));
		}
	}

	private static string InsertStrings(string s, int insertEvery, string insert)
	{
		char[] ins = s.ToCharArray();
		char[] inserts = insert.ToCharArray();
		int insertLength = inserts.Length;
		int length = s.Length + s.Length / insertEvery * insert.Length;
		if (ins.Length % insertEvery == 0)
			length -= insert.Length;

		char[] outs = new char[length];
		long di = 0;
		long si = 0;
		while (si < s.Length - insertEvery)
		{
			Array.Copy(ins, si, outs, di, insertEvery);
			si += insertEvery;
			di += insertEvery;
			Array.Copy(inserts, 0, outs, di, insertLength);
			di += insertLength;
		}

		Array.Copy(ins, si, outs, di, ins.Length - si);
		return new(outs);
	}

	private static void RenderMainBlockTable()
	{
		if (ImGui.CollapsingHeader("Main block"))
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
	}

	private static void Column(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
	{
		ImGui.TableNextColumn();
		ImGui.Text(left);

		ImGui.TableNextColumn();
		ImGui.Text(right);
	}

	private static void RenderBuffer()
	{
		for (int i = 0; i < _mainBlock.Buffer.Length; i++)
			_mainBlock.Buffer[i].TryFormat(_mainBufferHexString.AsSpan()[(i * 2)..], out _, "X2");

		ImGui.TextWrapped(_mainBufferHexString);
	}
}
