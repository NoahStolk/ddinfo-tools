using DevilDaggersInfo.DevUtil.DistributeApp.Model;
using DevilDaggersInfo.Web.ApiSpec.Admin.Tools;
using DevilDaggersInfo.Web.ApiSpec.Main.Authentication;
using System.Net;
using System.Net.Http.Json;

namespace DevilDaggersInfo.DevUtil.DistributeApp;

public static class ApiHttpClient
{
	public static async Task<LoginResponse> LoginAsync()
	{
		using HttpRequestMessage loginRequest = new(HttpMethod.Post, "https://devildaggers.info/api/authentication/login");
		loginRequest.Content = JsonContent.Create(AppSettingsModel.Instance.LoginRequest);

		using HttpClient client = new();
		HttpResponseMessage response = await client.SendAsync(loginRequest);
		if (response.StatusCode != HttpStatusCode.OK)
			throw new InvalidOperationException($"Unsuccessful status code from login '{response.StatusCode}'");

		return await response.Content.ReadFromJsonAsync<LoginResponse>() ?? throw new InvalidOperationException("Could not deserialize login response.");
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
			throw new InvalidOperationException($"Unsuccessful status code from upload '{response.StatusCode}'");
	}
}
