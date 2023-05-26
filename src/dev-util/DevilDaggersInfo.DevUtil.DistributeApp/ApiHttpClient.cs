using DevilDaggersInfo.Api.Admin.Tools;
using DevilDaggersInfo.Api.Main.Authentication;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace DevilDaggersInfo.Tool.DistributeApp;

public static class ApiHttpClient
{
	public static async Task<LoginResponse> LoginAsync()
	{
		using HttpRequestMessage loginRequest = new(HttpMethod.Post, "https://devildaggers.info/api/authentication/login");
		loginRequest.Content = JsonContent.Create(JsonSerializer.Deserialize<LoginRequest>(await File.ReadAllTextAsync("appsettings.json")));

		using HttpClient client = new();
		HttpResponseMessage response = await client.SendAsync(loginRequest);
		if (response.StatusCode != HttpStatusCode.OK)
			throw new($"Unsuccessful status code from login '{response.StatusCode}'");

		return await response.Content.ReadFromJsonAsync<LoginResponse>() ?? throw new("Could not deserialize login response.");
	}

	public static async Task UploadAsync(string projectName, string projectVersion, ToolBuildType toolBuildType, ToolPublishMethod toolPublishMethod, string outputZipFilePath, string loginToken)
	{
		AddDistribution addDistribution = new()
		{
			Name = projectName,
			Version = projectVersion,
			BuildType = toolBuildType,
			PublishMethod = toolPublishMethod,
			ZipFileContents = await File.ReadAllBytesAsync(outputZipFilePath),
			UpdateVersion = true,
			UpdateRequiredVersion = false,
		};

		using HttpRequestMessage uploadRequest = new(HttpMethod.Post, "https://devildaggers.info/api/admin/tools");
		uploadRequest.Content = JsonContent.Create(addDistribution);
		uploadRequest.Headers.Authorization = new("Bearer", loginToken);

		using HttpClient client = new();
		HttpResponseMessage response = await client.SendAsync(uploadRequest);
		if (response.StatusCode != HttpStatusCode.OK)
			throw new($"Unsuccessful status code from upload '{response.StatusCode}'");
	}
}
