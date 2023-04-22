using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena.SettingsWrappers;

public class PencilToolSettingsWrapper : AbstractComponent, ISettingsWrapper
{
	public PencilToolSettingsWrapper(IBounds bounds)
		: base(bounds)
	{
		int y = 0;
		ISettingsWrapper wrapper = this;
		wrapper.AddSlider("Size", ref y, f => StateManager.Dispatch(new SetArenaPencilSize(f)), 0, 10, 0.1f, StateManager.ArenaPencilState.Size, "0.0");
	}
}
