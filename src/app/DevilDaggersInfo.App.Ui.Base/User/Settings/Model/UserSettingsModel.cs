namespace DevilDaggersInfo.App.Ui.Base.User.Settings.Model;

// Note: Required properties cause JSON deserialization to fail when the property is missing from the JSON file. After the initial release, we should only add optional properties to this class.
public record UserSettingsModel
{
	public required string DevilDaggersInstallationDirectory { get; init; }

	public required bool ScaleUiToWindow { get; init; }

	public required bool ShowDebugOutput { get; init; }

	public required bool RenderWhileWindowIsInactive { get; init; }

	public required int MaxFps { get; init; }

	public required float LookSpeed { get; init; }

	public required int FieldOfView { get; init; }

	public static UserSettingsModel Default { get; } = new()
	{
		DevilDaggersInstallationDirectory = string.Empty,
		ScaleUiToWindow = true,
		ShowDebugOutput = false,
		RenderWhileWindowIsInactive = true,
		MaxFps = 300,
		LookSpeed = 20,
		FieldOfView = 90,
	};

	public static int MaxFpsMin => 60;
	public static int MaxFpsMax => 300;
	public static float LookSpeedMin => 1;
	public static float LookSpeedMax => 100;
	public static int FieldOfViewMin => 10;
	public static int FieldOfViewMax => 170;
}
