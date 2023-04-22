using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena.SettingsWrappers;

public class DaggerToolSettingsWrapper : AbstractComponent, ISettingsWrapper
{
	public DaggerToolSettingsWrapper(IBounds bounds)
		: base(bounds)
	{
		int y = 0;
		ISettingsWrapper wrapper = this;
		wrapper.AddSlider("Snap X", ref y, f => StateManager.Dispatch(new SetArenaDaggerSnap(StateManager.ArenaDaggerState.Snap with { X = f })), 0.25f, 2, 0.25f, StateManager.ArenaDaggerState.Snap.X, "0.00");
		wrapper.AddSlider("Snap Y", ref y, f => StateManager.Dispatch(new SetArenaDaggerSnap(StateManager.ArenaDaggerState.Snap with { Y = f })), 0.25f, 2, 0.25f, StateManager.ArenaDaggerState.Snap.Y, "0.00");
	}
}
