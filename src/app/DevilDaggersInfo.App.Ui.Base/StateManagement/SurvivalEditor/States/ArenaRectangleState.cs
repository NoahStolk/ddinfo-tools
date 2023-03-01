namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.States;

public record ArenaRectangleState(int Thickness, bool Filled)
{
	public static ArenaRectangleState GetDefault()
	{
		return new(1, true);
	}
}
