using DevilDaggersInfo.Tools.Networking;
using DevilDaggersInfo.Tools.Networking.TaskHandlers;
using DevilDaggersInfo.Tools.User.Cache;

namespace DevilDaggersInfo.Tools.User.Stats;

internal sealed class UserStats(UserCache userCache)
{
	public FetchPlayerStats.EntryResponse? PlayerStats { get; private set; }

	public DateTime? LastFetchedUtc { get; private set; }

	public void Update()
	{
		if (userCache.Model.PlayerId <= 0)
			return;

		const int intervalInMinutes = 10;
		if (LastFetchedUtc > DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(intervalInMinutes)))
			return;

		LastFetchedUtc = DateTime.UtcNow;
		AsyncHandler.Run(
			p => p.Match(
				s => PlayerStats = s,
				_ => { }),
			() => FetchPlayerStats.HandleAsync(userCache.Model.PlayerId));
	}
}
