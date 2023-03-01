using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena.SettingsWrappers;

public class BucketToolSettingsWrapper : AbstractComponent, ISettingsWrapper
{
	public BucketToolSettingsWrapper(IBounds bounds)
		: base(bounds)
	{
		int y = 0;
		ISettingsWrapper wrapper = this;
		wrapper.AddSlider("Tolerance", ref y, f => StateManager.Dispatch(new SetArenaBucketTolerance(f)), 0.01f, 10, 0.01f, StateManager.ArenaBucketState.Tolerance, "0.00");
		wrapper.AddSlider("Void height", ref y, f => StateManager.Dispatch(new SetArenaBucketVoidHeight(f)), -50, 0, 0.1f, StateManager.ArenaBucketState.VoidHeight, "0.0");
	}
}
