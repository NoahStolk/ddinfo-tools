using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.Styling;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena;

public class ShrinkSlider : Slider
{
	public ShrinkSlider(IBounds bounds, Action<float> onChange, bool applyInstantly, float min, float max, float step, float defaultValue, SliderStyle sliderStyle)
		: base(bounds, onChange, applyInstantly, min, max, step, defaultValue, sliderStyle)
	{
	}

	protected override void RenderHighlighters(Vector2i<int> scrollOffset)
	{
		RenderHighlighter(scrollOffset, GetPercentage(CurrentValue), GlobalColors.ShrinkCurrent);
		RenderHighlighter(scrollOffset, GetPercentage(StateManager.SpawnsetState.Spawnset.GetShrinkEndTime()), GlobalColors.ShrinkEnd);
	}
}
