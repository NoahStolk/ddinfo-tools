using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Utils;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena;

public class ArenaHeightButtons : AbstractComponent
{
	private const int _arenaButtonSize = 20;

	public ArenaHeightButtons(IBounds bounds)
		: base(bounds)
	{
		Span<float> heights = stackalloc float[] { -1000, -1.1f, -1.01f, -1, -0.8f, -0.6f, -0.4f, -0.2f };
		for (int i = 0; i < heights.Length; i++)
		{
			float height = heights[i];
			int offsetX = i % 4 * _arenaButtonSize;
			int offsetY = i / 4 * _arenaButtonSize;
			AddHeightButton(height, offsetX, offsetY);
		}

		for (int i = 0; i < 16; i++)
		{
			int offsetX = i % 4 * _arenaButtonSize;
			int offsetY = i / 4 * _arenaButtonSize;
			AddHeightButton(i, offsetX, offsetY + _arenaButtonSize * 2 + _arenaButtonSize / 2);
		}

		for (int i = 16; i < 32; i++)
		{
			int offsetX = i % 4 * _arenaButtonSize;
			int offsetY = i / 4 * _arenaButtonSize;
			AddHeightButton(16 + (i - 16) * 2, offsetX, offsetY + _arenaButtonSize * 3);
		}
	}

	private void AddHeightButton(float height, int offsetX, int offsetY)
	{
		Color heightColor = TileUtils.GetColorFromHeight(height);

		ButtonStyle buttonStyle = new(heightColor, Color.Black, Color.Lerp(heightColor, Color.White, 0.75f), 1);
		HeightButton button = new(Bounds.CreateNested(offsetX, offsetY, _arenaButtonSize, _arenaButtonSize), () => StateManager.Dispatch(new SetArenaSelectedHeight(height)), buttonStyle, height);
		NestingContext.Add(button);
	}
}
