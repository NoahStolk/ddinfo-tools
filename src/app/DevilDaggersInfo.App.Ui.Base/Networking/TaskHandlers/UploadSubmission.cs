using DevilDaggersInfo.Api.App.CustomLeaderboards;
using System.Net;
using System.Net.Http.Json;

namespace DevilDaggersInfo.App.Ui.Base.Networking.TaskHandlers;

public static class UploadSubmission
{
	public static async Task<GetUploadResponse> HandleAsync(AddUploadRequest addUploadRequest)
	{
		HttpResponseMessage response = await AsyncHandler.Client.SubmitScore(addUploadRequest);

		if (response.StatusCode != HttpStatusCode.OK)
			throw new HttpRequestException(await response.Content.ReadAsStringAsync(), null, response.StatusCode);

		return await response.Content.ReadFromJsonAsync<GetUploadResponse>() ?? throw new InvalidDataException($"Deserialization error for JSON '{response.Content}'.");
	}
}
