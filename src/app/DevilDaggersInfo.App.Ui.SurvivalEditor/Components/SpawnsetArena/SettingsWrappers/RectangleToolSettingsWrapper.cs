using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena.SettingsWrappers;

public class RectangleToolSettingsWrapper : AbstractComponent, ISettingsWrapper
{
	public RectangleToolSettingsWrapper(IBounds bounds)
		: base(bounds)
	{
		int y = 0;
		ISettingsWrapper wrapper = this;
		wrapper.AddSlider("Thickness", ref y, f => StateManager.Dispatch(new SetArenaRectangleThickness((int)f)), 1, 10, 1, StateManager.ArenaRectangleState.Thickness, "0");
		wrapper.AddCheckbox("Filled", ref y, b => StateManager.Dispatch(new SetArenaRectangleFilled(b)), StateManager.ArenaRectangleState.Filled);
	}
}
