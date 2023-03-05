using DevilDaggersInfo.App.Ui.Base.User.Cache;
using DevilDaggersInfo.App.Ui.Base.User.Cache.Model;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;

public record SetCurrentPlayerId(int CurrentPlayerId) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		UserCache.Model = new UserCacheModel
		{
			PlayerId = CurrentPlayerId,
		};
	}
}
