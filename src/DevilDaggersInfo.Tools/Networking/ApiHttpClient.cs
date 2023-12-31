using DevilDaggersInfo.Tools.JsonSerializerContexts;
using DevilDaggersInfo.Web.ApiSpec.Tools;
using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using DevilDaggersInfo.Web.ApiSpec.Tools.ProcessMemory;
using DevilDaggersInfo.Web.ApiSpec.Tools.Spawnsets;
using DevilDaggersInfo.Web.ApiSpec.Tools.Updates;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

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

	private async Task<T> SendGetRequest<T>(string url, JsonTypeInfo<T> jsonTypeInfo)
	{
		using HttpResponseMessage response = await SendRequest(HttpMethod.Get, url);
		if (response.StatusCode != HttpStatusCode.OK)
			throw new HttpRequestException(await response.Content.ReadAsStringAsync(), null, response.StatusCode);

		try
		{
			return await response.Content.ReadFromJsonAsync(jsonTypeInfo) ?? throw new JsonException("JSON deserialization returned null.");
		}
		catch (JsonException ex)
		{
			string json = await response.Content.ReadAsStringAsync();
			throw new InvalidDataException($"Deserialization error when requesting data from endpoint '{url}'. JSON:\n{json}", ex);
		}
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
		return await SendGetRequest($"api/app/custom-entries/{id}/replay-buffer", ApiModelsContext.Default.GetCustomEntryReplayBuffer);
	}

	public async Task<HttpResponseMessage> SubmitScore(AddUploadRequest uploadRequest)
	{
		return await SendRequest(HttpMethod.Post, "api/app/custom-entries/submit", JsonContent.Create(uploadRequest, ApiModelsContext.Default.AddUploadRequest));
	}

	public async Task<List<GetCustomLeaderboardForOverview>> GetCustomLeaderboards(int selectedPlayerId)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(selectedPlayerId), selectedPlayerId },
		};
		return await SendGetRequest(BuildUrlWithQuery("api/app/custom-leaderboards/", queryParameters), ApiModelsContext.Default.ListGetCustomLeaderboardForOverview);
	}

	public async Task<GetCustomLeaderboard> GetCustomLeaderboardById(int id)
	{
		return await SendGetRequest($"api/app/custom-leaderboards/{id}", ApiModelsContext.Default.GetCustomLeaderboard);
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
		return await SendGetRequest("api/app/custom-leaderboards/allowed-categories", ApiModelsContext.Default.ListGetCustomLeaderboardAllowedCategory);
	}

	public async Task<GetMarker> GetMarker(AppOperatingSystem appOperatingSystem)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(appOperatingSystem), appOperatingSystem },
		};
		return await SendGetRequest(BuildUrlWithQuery("api/app/process-memory/marker", queryParameters), ApiModelsContext.Default.GetMarker);
	}

	public async Task<GetSpawnset> GetSpawnsetById(int id)
	{
		return await SendGetRequest($"api/app/spawnsets/{id}", ApiModelsContext.Default.GetSpawnset);
	}

	public async Task<GetSpawnsetByHash> GetSpawnsetByHash(byte[] hash)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(hash), Uri.EscapeDataString(Convert.ToBase64String(hash)) },
		};
		return await SendGetRequest(BuildUrlWithQuery("api/app/spawnsets/by-hash", queryParameters), ApiModelsContext.Default.GetSpawnsetByHash);
	}

	public async Task<GetLatestVersion> GetLatest(AppOperatingSystem appOperatingSystem)
	{
		Dictionary<string, object?> queryParameters = new()
		{
			{ nameof(appOperatingSystem), appOperatingSystem },
		};
		return await SendGetRequest(BuildUrlWithQuery("api/app/updates/latest", queryParameters), ApiModelsContext.Default.GetLatestVersion);
	}
}
