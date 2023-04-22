using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena.SettingsWrappers;

public class LineToolSettingsWrapper : AbstractComponent, ISettingsWrapper
{
	public LineToolSettingsWrapper(IBounds bounds)
		: base(bounds)
	{
		int y = 0;
		ISettingsWrapper wrapper = this;
		wrapper.AddSlider("Thickness", ref y, f => StateManager.Dispatch(new SetArenaLineThickness(f)), 0, 10, 0.1f, StateManager.ArenaLineState.Thickness, "0.0");
	}
}
