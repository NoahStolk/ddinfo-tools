using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.Tools.User.Settings.Model;

public record UserSettingsModel
{
	public string DevilDaggersInstallationDirectory { get; init; } = string.Empty;
	public bool ShowDebug { get; init; }
	public float LookSpeed { get; init; }
	public int FieldOfView { get; init; }
	public IReadOnlyList<UserSettingsPracticeTemplate> PracticeTemplates { get; init; } = new List<UserSettingsPracticeTemplate>();
	public bool DoNotShowAgainPracticeSpawnsetApplied { get; init; }
	public bool DoNotShowAgainPracticeSpawnsetDeleted { get; init; }

	public static UserSettingsModel Default { get; } = new()
	{
		DevilDaggersInstallationDirectory = string.Empty,
		ShowDebug = false,
		LookSpeed = 20,
		FieldOfView = 90,
		PracticeTemplates = new List<UserSettingsPracticeTemplate>(),
		DoNotShowAgainPracticeSpawnsetApplied = false,
		DoNotShowAgainPracticeSpawnsetDeleted = false,
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
			PracticeTemplates = PracticeTemplates
				.Select(pt => pt with
				{
					HandLevel = Enum.IsDefined(pt.HandLevel) ? pt.HandLevel : HandLevel.Level1,
				})
				.ToList(),
		};
	}
}
