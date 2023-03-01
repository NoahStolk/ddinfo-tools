namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.States;

public record ArenaPencilState(float Size)
{
	public static ArenaPencilState GetDefault()
	{
		return new(0);
	}
}
