using DevilDaggersInfo.Tools.JsonSerializerContexts;
using DevilDaggersInfo.Web.ApiSpec.Tools;
using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using DevilDaggersInfo.Web.ApiSpec.Tools.ProcessMemory;
using DevilDaggersInfo.Web.ApiSpec.Tools.Spawnsets;
using System.Net.Http.Json;

namespace DevilDaggersInfo.Tools.Networking;

internal sealed class DevilDaggersInfoClient(HttpClient client)
{
	public async Task<GetCustomEntryReplayBuffer> GetCustomEntryReplayBufferById(int id)
	{
		return await client.SendGetRequest($"api/app/custom-entries/{id}/replay-buffer", ApiModelsContext.Default.GetCustomEntryReplayBuffer);
	}

	public async Task<HttpResponseMessage> SubmitScore(AddUploadRequest uploadRequest)
	{
		return await client.SendRequest(HttpMethod.Post, "api/app/custom-entries/submit", JsonContent.Create(uploadRequest, ApiModelsContext.Default.AddUploadRequest));
	}

	public async Task<List<GetCustomLeaderboardForOverview>> GetCustomLeaderboards(int selectedPlayerId)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(selectedPlayerId), selectedPlayerId },
		};
		return await client.SendGetRequest(UrlUtils.BuildUrlWithQuery("api/app/custom-leaderboards/", queryParameters), ApiModelsContext.Default.ListGetCustomLeaderboardForOverview);
	}

	public async Task<GetCustomLeaderboard> GetCustomLeaderboardById(int id)
	{
		return await client.SendGetRequest($"api/app/custom-leaderboards/{id}", ApiModelsContext.Default.GetCustomLeaderboard);
	}

	public async Task<HttpResponseMessage> CustomLeaderboardExistsBySpawnsetHash(byte[] hash)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(hash), Uri.EscapeDataString(Convert.ToBase64String(hash)) },
		};
		return await client.SendRequest(HttpMethod.Head, UrlUtils.BuildUrlWithQuery("api/app/custom-leaderboards/exists", queryParameters));
	}

	public async Task<List<GetCustomLeaderboardAllowedCategory>> GetCustomLeaderboardAllowedCategories()
	{
		return await client.SendGetRequest("api/app/custom-leaderboards/allowed-categories", ApiModelsContext.Default.ListGetCustomLeaderboardAllowedCategory);
	}

	public async Task<GetMarker> GetMarker(AppOperatingSystem appOperatingSystem)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(appOperatingSystem), appOperatingSystem },
		};
		return await client.SendGetRequest(UrlUtils.BuildUrlWithQuery("api/app/process-memory/marker", queryParameters), ApiModelsContext.Default.GetMarker);
	}

	public async Task<GetSpawnset> GetSpawnsetById(int id)
	{
		return await client.SendGetRequest($"api/app/spawnsets/{id}", ApiModelsContext.Default.GetSpawnset);
	}

	public async Task<GetSpawnsetByHash> GetSpawnsetByHash(byte[] hash)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(hash), Uri.EscapeDataString(Convert.ToBase64String(hash)) },
		};
		return await client.SendGetRequest(UrlUtils.BuildUrlWithQuery("api/app/spawnsets/by-hash", queryParameters), ApiModelsContext.Default.GetSpawnsetByHash);
	}
}
