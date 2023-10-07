using DevilDaggersInfo.Web.ApiSpec.App;
using DevilDaggersInfo.Web.ApiSpec.App.CustomLeaderboards;
using DevilDaggersInfo.Web.ApiSpec.App.ProcessMemory;
using DevilDaggersInfo.Web.ApiSpec.App.Spawnsets;
using DevilDaggersInfo.Web.ApiSpec.App.Updates;
using System.Net.Http.Json;

namespace DevilDaggersInfo.App.Networking;

public partial class AppApiHttpClient
{
	public async Task<GetCustomEntryReplayBuffer> GetCustomEntryReplayBufferById(int id)
	{
		return await SendGetRequest<GetCustomEntryReplayBuffer>($"api/app/custom-entries/{id}/replay-buffer");
	}

	public async Task<HttpResponseMessage> SubmitScore(AddUploadRequest uploadRequest)
	{
		return await SendRequest(new HttpMethod("POST"), "api/app/custom-entries/submit", JsonContent.Create(uploadRequest));
	}

	public async Task<List<GetCustomLeaderboardForOverview>> GetCustomLeaderboards(int selectedPlayerId)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(selectedPlayerId), selectedPlayerId },
		};
		return await SendGetRequest<List<GetCustomLeaderboardForOverview>>(BuildUrlWithQuery("api/app/custom-leaderboards/", queryParameters));
	}

	public async Task<GetCustomLeaderboard> GetCustomLeaderboardById(int id)
	{
		return await SendGetRequest<GetCustomLeaderboard>($"api/app/custom-leaderboards/{id}");
	}

	public async Task<GetCustomLeaderboard> GetCustomLeaderboardBySpawnsetHash(byte[] hash)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(hash), Uri.EscapeDataString(Convert.ToBase64String(hash)) },
		};
		return await SendGetRequest<GetCustomLeaderboard>(BuildUrlWithQuery("api/app/custom-leaderboards/by-hash", queryParameters));
	}

	public async Task<HttpResponseMessage> CustomLeaderboardExistsBySpawnsetHash(byte[] hash)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(hash), Uri.EscapeDataString(Convert.ToBase64String(hash)) },
		};
		return await SendRequest(new HttpMethod("HEAD"), BuildUrlWithQuery("api/app/custom-leaderboards/exists", queryParameters));
	}

	public async Task<List<GetCustomLeaderboardAllowedCategory>> GetCustomLeaderboardAllowedCategories()
	{
		return await SendGetRequest<List<GetCustomLeaderboardAllowedCategory>>("api/app/custom-leaderboards/allowed-categories");
	}

	public async Task<GetMarker> GetMarker(AppOperatingSystem appOperatingSystem)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(appOperatingSystem), appOperatingSystem },
		};
		return await SendGetRequest<GetMarker>(BuildUrlWithQuery("api/app/process-memory/marker", queryParameters));
	}

	public async Task<GetSpawnset> GetSpawnsetById(int id)
	{
		return await SendGetRequest<GetSpawnset>($"api/app/spawnsets/{id}");
	}

	public async Task<GetSpawnsetBuffer> GetSpawnsetBufferById(int id)
	{
		return await SendGetRequest<GetSpawnsetBuffer>($"api/app/spawnsets/{id}/buffer");
	}

	public async Task<GetSpawnsetByHash> GetSpawnsetByHash(byte[] hash)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(hash), Uri.EscapeDataString(Convert.ToBase64String(hash)) },
		};
		return await SendGetRequest<GetSpawnsetByHash>(BuildUrlWithQuery("api/app/spawnsets/by-hash", queryParameters));
	}

	public async Task<GetLatestVersion> GetLatest(AppOperatingSystem appOperatingSystem)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(appOperatingSystem), appOperatingSystem },
		};
		return await SendGetRequest<GetLatestVersion>(BuildUrlWithQuery("api/app/updates/latest", queryParameters));
	}

	public async Task<Task> GetLatestFile(AppOperatingSystem appOperatingSystem)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(appOperatingSystem), appOperatingSystem },
		};
		return await SendGetRequest<Task>(BuildUrlWithQuery("api/app/updates/latest-file", queryParameters));
	}
}
