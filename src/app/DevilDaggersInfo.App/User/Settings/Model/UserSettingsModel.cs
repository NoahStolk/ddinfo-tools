namespace DevilDaggersInfo.App.User.Settings.Model;

public record UserSettingsModel
{
	public string DevilDaggersInstallationDirectory { get; init; } = string.Empty;
	public bool ShowDebugWindow { get; init; }
	public int MaxFps { get; init; }
	public bool VerticalSync { get; init; }
	public float LookSpeed { get; init; }
	public int FieldOfView { get; init; }

	public static UserSettingsModel Default { get; } = new()
	{
		DevilDaggersInstallationDirectory = string.Empty,
		ShowDebugWindow = false,
		MaxFps = 300,
		VerticalSync = false,
		LookSpeed = 20,
		FieldOfView = 90,
	};

	public static int MaxFpsMin => 30;
	public static int MaxFpsMax => 300;
	public static float LookSpeedMin => 1;
	public static float LookSpeedMax => 100;
	public static int FieldOfViewMin => 10;
	public static int FieldOfViewMax => 170;

	public UserSettingsModel Sanitize()
	{
		return this with
		{
			MaxFps = Math.Clamp(MaxFps, MaxFpsMin, MaxFpsMax),
			LookSpeed = Math.Clamp(LookSpeed, LookSpeedMin, LookSpeedMax),
			FieldOfView = Math.Clamp(FieldOfView, FieldOfViewMin, FieldOfViewMax),
		};
	}
}
