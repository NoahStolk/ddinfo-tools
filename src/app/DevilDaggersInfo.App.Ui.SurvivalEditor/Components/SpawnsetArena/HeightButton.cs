// ReSharper disable SpecifyACultureInStringConversionExplicitly
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena;

public class HeightButton : Button
{
	private readonly float _height;

	public HeightButton(IBounds bounds, Action onClick, ButtonStyle buttonStyle, float height)
		: base(bounds, onClick, buttonStyle)
	{
		_height = height;
	}

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		if (Hover)
			Root.Game.TooltipContext = new(_height.ToString(), Color.White, Color.Black, TextAlign.Left);
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		if (MathF.Abs(StateManager.ArenaEditorState.SelectedHeight - _height) < 0.001f)
			Root.Game.RectangleRenderer.Schedule(Bounds.Size, Bounds.Center, Depth + 1, Color.White);
	}
}
