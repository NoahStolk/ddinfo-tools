namespace DevilDaggersInfo.App.User.Settings.Model;

public record UserSettingsModel
{
	public string DevilDaggersInstallationDirectory { get; init; } = string.Empty;
	public bool ShowDebugWindow { get; init; }
	public float LookSpeed { get; init; }
	public int FieldOfView { get; init; }

	public static UserSettingsModel Default { get; } = new()
	{
		DevilDaggersInstallationDirectory = string.Empty,
		ShowDebugWindow = false,
		LookSpeed = 20,
		FieldOfView = 90,
	};

	public static float LookSpeedMin => 1;
	public static float LookSpeedMax => 500;
	public static int FieldOfViewMin => 10;
	public static int FieldOfViewMax => 170;

	public UserSettingsModel Sanitize()
	{
		return this with
		{
			LookSpeed = Math.Clamp(LookSpeed, LookSpeedMin, LookSpeedMax),
			FieldOfView = Math.Clamp(FieldOfView, FieldOfViewMin, FieldOfViewMax),
		};
	}
}
