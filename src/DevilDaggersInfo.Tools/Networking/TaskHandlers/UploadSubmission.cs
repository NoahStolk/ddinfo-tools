using DevilDaggersInfo.Tools.JsonSerializerContexts;
using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace DevilDaggersInfo.Tools.Networking.TaskHandlers;

public static class UploadSubmission
{
	public static async Task<GetUploadResponse> HandleAsync(AddUploadRequest addUploadRequest)
	{
		HttpResponseMessage response = await AsyncHandler.Client.SubmitScore(addUploadRequest);

		if (response.StatusCode != HttpStatusCode.OK)
			throw new HttpRequestException(await response.Content.ReadAsStringAsync(), null, response.StatusCode);

		try
		{
			return await response.Content.ReadFromJsonAsync(ApiModelsContext.Default.GetUploadResponse) ?? throw new JsonException("JSON deserialization returned null.");
		}
		catch (JsonException ex)
		{
			string json = await response.Content.ReadAsStringAsync();
			throw new InvalidDataException($"Deserialization error when reading score submission response. JSON:\n{json}", ex);
		}
	}
}
