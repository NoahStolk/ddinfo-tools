using DevilDaggersInfo.Api.App.CustomLeaderboards;

namespace DevilDaggersInfo.App.Networking.TaskHandlers;

public static class FetchCustomEntryReplayById
{
	public static async Task<GetCustomEntryReplayBuffer> HandleAsync(int customEntryId)
	{
		return await AsyncHandler.Client.GetCustomEntryReplayBufferById(customEntryId);
	}
}
