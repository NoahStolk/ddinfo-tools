using DevilDaggersInfo.Core.Common;
using System.Text;

namespace DevilDaggersInfo.Tools.Networking.TaskHandlers;

internal sealed class FetchPlayerStats
{
	public static async Task<EntryResponse> HandleAsync(int id)
	{
		byte[] data = await Clients.DevilDaggers.GetPlayerStats(id);
		return ParseGetEntryById(data);
	}

	private static EntryResponse ParseGetEntryById(byte[] response)
	{
		using MemoryStream ms = new(response);
		using BinaryReader br = new(ms);

		br.BaseStream.Seek(19, SeekOrigin.Begin);

		return ReadEntry(br);
	}

	private static EntryResponse ReadEntry(BinaryReader br)
	{
		short usernameLength = br.ReadInt16();
		string username = Encoding.UTF8.GetString(br.ReadBytes(usernameLength));
		int rank = br.ReadInt32();
		int id = br.ReadInt32();

		br.BaseStream.Seek(4, SeekOrigin.Current);
		int time = br.ReadInt32();
		int kills = br.ReadInt32();
		int daggersFired = br.ReadInt32();
		int daggersHit = br.ReadInt32();
		int gems = br.ReadInt32();
		int deathType = br.ReadInt32();
		ulong deathsTotal = br.ReadUInt64();
		ulong killsTotal = br.ReadUInt64();
		ulong daggersFiredTotal = br.ReadUInt64();
		ulong timeTotal = br.ReadUInt64();
		ulong gemsTotal = br.ReadUInt64();
		ulong daggersHitTotal = br.ReadUInt64();

		return new EntryResponse(
			Rank: rank,
			Id: id,
			Username: username,
			Time: GameTime.FromGameUnits(time),
			Kills: kills,
			Gems: gems,
			DeathType: deathType,
			DaggersHit: daggersHit,
			DaggersFired: daggersFired,
			TimeTotal: GameTime.FromGameUnits(timeTotal),
			KillsTotal: killsTotal,
			GemsTotal: gemsTotal,
			DeathsTotal: deathsTotal,
			DaggersHitTotal: daggersHitTotal,
			DaggersFiredTotal: daggersFiredTotal);
	}

	internal sealed record EntryResponse(
		int Rank,
		int Id,
		string Username,
		GameTime Time,
		int Kills,
		int Gems,
		int DeathType,
		int DaggersHit,
		int DaggersFired,
		GameTime TimeTotal,
		ulong KillsTotal,
		ulong GemsTotal,
		ulong DeathsTotal,
		ulong DaggersHitTotal,
		ulong DaggersFiredTotal);
}
