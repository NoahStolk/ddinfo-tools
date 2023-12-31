using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.Tools.User.Settings.Model;

public record UserSettingsPracticeTemplate(string? Name, HandLevel HandLevel, int AdditionalGems, float TimerStart)
{
	public virtual bool Equals(UserSettingsPracticeTemplate? other)
	{
		if (other == null)
			return false;

		const float epsilon = 0.0001f;
		return HandLevel == other.HandLevel && AdditionalGems == other.AdditionalGems && Math.Abs(TimerStart - other.TimerStart) < epsilon;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine((int)HandLevel, AdditionalGems, TimerStart);
	}
}
