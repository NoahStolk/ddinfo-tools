using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace DevilDaggersInfo.Tools.Networking;

internal static class HttpClientExtensions
{
	extension(HttpClient client)
	{
		public async Task<HttpResponseMessage> SendRequest(HttpMethod httpMethod, string url, HttpContent? content = null)
		{
			using HttpRequestMessage request = new();
			request.RequestUri = new Uri(url, UriKind.Relative);
			request.Method = httpMethod;
			request.Content = content;

			return await client.SendAsync(request);
		}

		public async Task<T> SendGetRequest<T>(string url, JsonTypeInfo<T> jsonTypeInfo)
		{
			using HttpResponseMessage response = await client.SendRequest(HttpMethod.Get, url);
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
	}
}
