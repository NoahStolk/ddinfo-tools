using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.Styling;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena.SettingsWrappers;

public interface ISettingsWrapper
{
	static readonly CheckboxStyle CheckboxStyle = new(6, 4, 4);

	IBounds Bounds { get; }

	NestingContext NestingContext { get; }

	public void AddSlider(string labelText, ref int y, Action<float> onChange, float min, float max, float step, float defaultValue, string format)
	{
		int halfWidth = Bounds.Size.X / 2;
		Label label = new(Bounds.CreateNested(0, y, halfWidth, 16), labelText, LabelStyles.DefaultLeft);
		Slider slider = new(Bounds.CreateNested(0 + halfWidth, y, halfWidth, 16), onChange, false, min, max, step, defaultValue, SliderStyles.Default with { ValueFormat = format });
		NestingContext.Add(label);
		NestingContext.Add(slider);
		y += 16;
	}

	public void AddCheckbox(string labelText, ref int y, Action<bool> onChange, bool defaultValue)
	{
		int halfWidth = Bounds.Size.X / 2;
		Label label = new(Bounds.CreateNested(0, y, halfWidth, 16), labelText, LabelStyles.DefaultLeft);
		Checkbox checkbox = new(Bounds.CreateNested(0 + halfWidth, y, 16, 16), onChange, CheckboxStyle) { CurrentValue = defaultValue };
		NestingContext.Add(label);
		NestingContext.Add(checkbox);
		y += 16;
	}
}
