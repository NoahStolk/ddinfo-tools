namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.States;

public record ArenaBucketState(float Tolerance, float VoidHeight)
{
	public static ArenaBucketState GetDefault()
	{
		return new(0.1f, -2);
	}
}
