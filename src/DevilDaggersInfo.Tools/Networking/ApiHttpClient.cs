using DevilDaggersInfo.Web.ApiSpec.Tools;
using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using DevilDaggersInfo.Web.ApiSpec.Tools.ProcessMemory;
using DevilDaggersInfo.Web.ApiSpec.Tools.Spawnsets;
using DevilDaggersInfo.Web.ApiSpec.Tools.Updates;
using System.Net;
using System.Net.Http.Json;

namespace DevilDaggersInfo.Tools.Networking;

public class ApiHttpClient
{
	private readonly HttpClient _client;

	public ApiHttpClient(HttpClient client)
	{
		_client = client;
	}

	private async Task<HttpResponseMessage> SendRequest(HttpMethod httpMethod, string url, JsonContent? body = null)
	{
		using HttpRequestMessage request = new();
		request.RequestUri = new(url, UriKind.Relative);
		request.Method = httpMethod;
		request.Content = body;

		return await _client.SendAsync(request);
	}

	private async Task<T> SendGetRequest<T>(string url)
	{
		using HttpResponseMessage response = await SendRequest(HttpMethod.Get, url);
		if (response.StatusCode != HttpStatusCode.OK)
			throw new HttpRequestException(await response.Content.ReadAsStringAsync(), null, response.StatusCode);

		return await response.Content.ReadFromJsonAsync<T>() ?? throw new InvalidDataException($"Deserialization error in {url} for JSON '{response.Content}'.");
	}

	private static string BuildUrlWithQuery(string baseUrl, Dictionary<string, object?> queryParameters)
	{
		if (queryParameters.Count == 0)
			return baseUrl;

		string queryParameterString = string.Join('&', queryParameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
		return $"{baseUrl.TrimEnd('/')}?{queryParameterString}";
	}

	public async Task<GetCustomEntryReplayBuffer> GetCustomEntryReplayBufferById(int id)
	{
		return await SendGetRequest<GetCustomEntryReplayBuffer>($"api/app/custom-entries/{id}/replay-buffer");
	}

	public async Task<HttpResponseMessage> SubmitScore(AddUploadRequest uploadRequest)
	{
		return await SendRequest(HttpMethod.Post, "api/app/custom-entries/submit", JsonContent.Create(uploadRequest));
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
		return await SendRequest(HttpMethod.Head, BuildUrlWithQuery("api/app/custom-leaderboards/exists", queryParameters));
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
