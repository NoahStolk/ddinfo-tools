namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.States;

public record ArenaLineState(float Thickness)
{
	public static ArenaLineState GetDefault()
	{
		return new(0);
	}
}
