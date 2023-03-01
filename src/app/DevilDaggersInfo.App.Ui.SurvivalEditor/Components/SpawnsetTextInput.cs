using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components;

public class SpawnsetTextInput : TextInput
{
	public SpawnsetTextInput(
		IBounds bounds,
		bool isNumeric,
		Action<string>? onEnter,
		Action<string>? onDeselect,
		Action<string>? onInput,
		TextInputStyle textInputStyle)
		: base(bounds, isNumeric, onEnter, onDeselect, onInput, textInputStyle)
	{
	}

	public void SetTextIfDeselected(string value)
	{
		if (!IsSelected)
			KeyboardInput.SetText(value);
	}
}
