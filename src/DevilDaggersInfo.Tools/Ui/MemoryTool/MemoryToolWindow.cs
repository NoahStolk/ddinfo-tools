using DevilDaggersInfo.Core.Common.Extensions;
using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.GameMemory.Enemies;
using ImGuiNET;

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
				const int thornStructSize = 2288;
				int thornOffset = 0;
				for (int i = 0; i < Root.GameMemoryService.MainBlock.ThornAliveCount; i++)
				{
					Thorn thorn;
					//do
					{
						thorn = Root.GameMemoryService.ReadExperimental<Thorn>(0x002513B0, thornStructSize, [0x0, 0x28, thornOffset]);
						thornOffset += thornStructSize;
					}
					//while (thorn.Hp == 0); // Dead Thorns aren't cleared up from game memory immediately, so skip them until they get cleared up.

					RenderThorn(thorn);
					RenderExperimentalBuffer("Thorn Struct Test", 0x002513B0, 128, [0x0, 0x28, thornOffset]);
				}

				const int spider2StructSize = 100; // TODO: Find the correct size.
				int spider2Offset = 0;
				for (int i = 0; i < Root.GameMemoryService.MainBlock.Spider2AliveCount; i++)
				{
					Spider2 spider2 = Root.GameMemoryService.ReadExperimental<Spider2>(0x00251830, spider2StructSize, [0x0, 0x28, 0xE4 + spider2Offset]);
					spider2Offset += spider2StructSize;

					RenderSpider2(spider2);
					RenderExperimentalBuffer("Spider2 Struct Test", 0x00251830, 128, [0x0, 0x28, 0xE4 + spider2Offset]);
				}
			}
			else
			{
				ImGui.TextWrapped("Devil Daggers is not running.");
			}
		}

		ImGui.End();
	}

	private static void RenderThorn(Thorn thorn)
	{
		ImGui.SeparatorText("Thorn");
		ImGui.Text(Inline.Span($"State: {thorn.State}"));
		ImGui.Text(Inline.Span($"IsAlive: {(thorn.IsAlive == 1 ? "True" : "False")}"));
		ImGui.Text(Inline.Span($"StateTimer: {thorn.StateTimer:0.00}"));
		ImGui.Text(Inline.Span($"HP: {thorn.Hp}"));
		ImGui.Text(Inline.Span($"Position: {Inline.Span(thorn.Position, "0.00")}"));
		ImGui.Text(Inline.Span($"Rotation: {thorn.Rotation:0.00}"));
		ImGui.Text(Inline.Span($"Unknown1: {thorn.Unknown1:0.00}"));
		ImGui.Text(Inline.Span($"Unknown2: {thorn.Unknown2:0.00}"));
		ImGui.Text(Inline.Span($"Unknown3: {thorn.Unknown3:0.00}"));
		ImGui.Text(Inline.Span($"Unknown4: {thorn.Unknown4:0.00}"));
		ImGui.Text(Inline.Span($"Unknown5: {thorn.Unknown5:0.00}"));
		ImGui.Text(Inline.Span($"Unknown6: {thorn.Unknown6:0.00}"));
		ImGui.Text(Inline.Span($"Unknown7: {thorn.Unknown7:0.00}"));
		ImGui.Text(Inline.Span($"Unknown8: {thorn.Unknown8:0.00}"));
		ImGui.Text(Inline.Span($"Unknown9: {thorn.Unknown9:0.00}"));
		ImGui.Text(Inline.Span($"Unknown10: {thorn.Unknown10:0.00}"));
		ImGui.Text(Inline.Span($"Unknown11: {thorn.Unknown11:0.00}"));
	}

	private static void RenderSpider2(Spider2 spider2)
	{
		ImGui.SeparatorText("Spider2");
		ImGui.Text(Inline.Span($"HP: {spider2.Hp}"));
	}

	private static void RenderExperimentalBuffer(ReadOnlySpan<char> name, long address, int size, int[] offsets)
	{
		byte[]? buffer = Root.GameMemoryService.ReadBufferExperimental(address, size, offsets);
		if (buffer == null)
		{
			ImGui.Text("NULL");
		}
		else
		{
			string hexString = InsertStrings(buffer.ByteArrayToHexString(), 8, " ");
			ImGui.TextWrapped($"{name} ({buffer.Length}):\n{hexString}");
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
