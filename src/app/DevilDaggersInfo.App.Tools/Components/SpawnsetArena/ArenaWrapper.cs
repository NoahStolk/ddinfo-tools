using DevilDaggersInfo.App.Tools.Enums;
using DevilDaggersInfo.App.Tools.States;
using Warp.Ui;
using Warp.Ui.Components;

namespace DevilDaggersInfo.App.Tools.Components.SpawnsetArena;

public class ArenaWrapper : AbstractComponent
{
	private const int _arenaButtonSize = 32;

	public ArenaWrapper(Rectangle metric)
		: base(metric)
	{
		Arena arena = new(default, 12);
		NestingContext.Add(arena);

		int buttonsOffsetX = arena.Metric.Size.X + 16;

		int offset = 0;
		foreach (float height in new [] { -1000, -1.1f, -1.01f, -1, -0.8f, -0.6f, -0.4f, -0.2f })
			AddHeightButton(height, offset++ * 32, 0);

		for (int i = 0; i < 64; i++)
		{
			int offsetX = i % 8 * _arenaButtonSize;
			int offsetY = i / 8 * _arenaButtonSize;
			AddHeightButton(i, offsetX, offsetY + 64);
		}

		AddToolButton(0, 480, ArenaTool.Pencil, "P");
		AddToolButton(32, 480, ArenaTool.Line, "L");
		AddToolButton(64, 480, ArenaTool.Rectangle, "R");
		AddToolButton(96, 480, ArenaTool.Bucket, "B");

		void AddHeightButton(float height, int offsetX, int offsetY)
		{
			Button.HeightButton button = new(Rectangle.At(buttonsOffsetX + offsetX, offsetY, _arenaButtonSize, _arenaButtonSize), () => StateManager.SetArenaSelectedHeight(height), height);
			NestingContext.Add(button);
		}

		void AddToolButton(int offsetX, int offsetY, ArenaTool arenaTool, string text)
		{
			Button.ArenaButton button = new(Rectangle.At(buttonsOffsetX + offsetX, offsetY, _arenaButtonSize, _arenaButtonSize), () => StateManager.SetArenaTool(arenaTool), Color.Yellow, Color.Black, text, false);
			NestingContext.Add(button);
		}
	}
}
