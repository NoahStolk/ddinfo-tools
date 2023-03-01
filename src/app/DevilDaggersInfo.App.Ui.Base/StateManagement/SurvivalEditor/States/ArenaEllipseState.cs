namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.States;

public record ArenaEllipseState(int Thickness, bool Filled)
{
	public static ArenaEllipseState GetDefault()
	{
		return new(1, true);
	}
}
