using System.Net;
using System.Net.Http.Json;

namespace DevilDaggersInfo.App.Ui.Base.Networking;

public partial class AppApiHttpClient
{
	private readonly HttpClient _client;

	public AppApiHttpClient(HttpClient client)
	{
		_client = client;
	}

	private async Task<HttpResponseMessage> SendRequest(HttpMethod httpMethod, string url, JsonContent? body = null)
	{
		using HttpRequestMessage request = new()
		{
			RequestUri = new(url, UriKind.Relative),
			Method = httpMethod,
			Content = body,
		};

		return await _client.SendAsync(request);
	}

	private async Task<T> SendGetRequest<T>(string url)
	{
		using HttpResponseMessage response = await SendRequest(HttpMethod.Get, url);
		if (response.StatusCode != HttpStatusCode.OK)
			throw new HttpRequestException(await response.Content.ReadAsStringAsync(), null, response.StatusCode);

		return await response.Content.ReadFromJsonAsync<T>() ?? throw new InvalidDataException($"Deserialization error in {url} for JSON '{response.Content}'.");
	}
}
