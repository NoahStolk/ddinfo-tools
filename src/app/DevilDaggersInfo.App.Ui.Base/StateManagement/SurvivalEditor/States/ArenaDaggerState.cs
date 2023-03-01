namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.States;

public record ArenaDaggerState(Vector2 Snap)
{
	public static ArenaDaggerState GetDefault()
	{
		return new(Vector2.One);
	}
}
