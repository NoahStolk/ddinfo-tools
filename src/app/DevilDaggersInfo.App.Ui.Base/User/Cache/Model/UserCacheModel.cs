namespace DevilDaggersInfo.App.Ui.Base.User.Cache.Model;

// Note: Required properties cause JSON deserialization to fail when the property is missing from the JSON file. After the initial release, we should only add optional properties to this class.
public record UserCacheModel
{
	public required int PlayerId { get; init; }

	public static UserCacheModel Default { get; } = new()
	{
		PlayerId = 0,
	};
}
