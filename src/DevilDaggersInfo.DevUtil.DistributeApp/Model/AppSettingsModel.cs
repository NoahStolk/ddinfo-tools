using DevilDaggersInfo.Web.ApiSpec.Main.Authentication;
using System.Text.Json;

namespace DevilDaggersInfo.DevUtil.DistributeApp.Model;

public record AppSettingsModel(LoginRequest LoginRequest, EncryptionModel Encryption)
{
	private static AppSettingsModel? _instance;

	public static AppSettingsModel Instance => _instance ??= JsonSerializer.Deserialize<AppSettingsModel>(File.ReadAllText("appsettings.json")) ?? throw new InvalidOperationException("Could not deserialize appsettings.json.");
}
