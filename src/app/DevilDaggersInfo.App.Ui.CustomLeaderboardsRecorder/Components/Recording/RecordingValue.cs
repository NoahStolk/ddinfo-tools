using DevilDaggersInfo.App.Core.GameMemory;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using Warp.NET.InterpolationStates;
using Warp.NET.Text;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.Recording;

public class RecordingValue : AbstractComponent
{
	private readonly string _labelText;
	private readonly FloatState _intensity = new(0);

	private string _value = string.Empty;
	private Color _color;
	private readonly Func<MainBlock, string> _valueGetter;
	private readonly Func<MainBlock, Color>? _colorGetter;

	public RecordingValue(IBounds bounds, string labelText, Color color, Func<MainBlock, string> valueGetter, Func<MainBlock, Color>? colorGetter)
		: base(bounds)
	{
		_labelText = labelText;
		_color = color;
		_valueGetter = valueGetter;
		_colorGetter = colorGetter;
	}

	public void SetState()
	{
		string value = _valueGetter(Root.Dependencies.GameMemoryService.MainBlock);
		Color? color = _colorGetter?.Invoke(Root.Dependencies.GameMemoryService.MainBlock);
		UpdateValue(value, color);
	}

	private void UpdateValue(string value, Color? color = null)
	{
		if (_value == value)
			return;

		_value = value;
		if (color.HasValue)
			_color = color.Value;

		_intensity.Physics = 1;
	}

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		_intensity.PrepareUpdate();

		_intensity.Physics = Math.Max(0, _intensity.Physics - Root.Game.Dt);
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		_intensity.PrepareRender();

		const int padding = 4;

		Root.Game.MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, scrollOffset + Bounds.TopLeft + new Vector2i<int>(padding), Depth + 2, Color.White, _labelText, TextAlign.Left);
		Root.Game.MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, scrollOffset + Bounds.TopLeft + new Vector2i<int>(Bounds.Size.X - padding, padding), Depth + 2, Color.Lerp(Color.White, _color, _intensity.Render), _value, TextAlign.Right);
	}
}
