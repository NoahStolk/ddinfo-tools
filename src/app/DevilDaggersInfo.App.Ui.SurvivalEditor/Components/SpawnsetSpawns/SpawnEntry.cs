using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Extensions;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.App.Ui.SurvivalEditor.EditorSpawns;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Core.Wiki.Objects;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetSpawns;

public class SpawnEntry : AbstractComponent
{
	private readonly SpawnUiEntry _spawnUiEntry;
	private readonly Enemy? _enemy;
	private readonly Color _enemyColor;

	public SpawnEntry(IBounds bounds, SpawnUiEntry spawnUiEntry)
		: base(bounds)
	{
		_spawnUiEntry = spawnUiEntry;
		_enemy = spawnUiEntry.EnemyType.GetEnemy();
		_enemyColor = _enemy == null ? Color.Gray(0.75f) : _enemy.Color.ToWarpColor();

		Index = spawnUiEntry.Index;
	}

	public bool Hover { get; private set; }
	public int Index { get; }

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		Hover = MouseUiContext.Contains(scrollOffset, Bounds);
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		bool isSelected = StateManager.SpawnEditorState.SelectedIndices.Contains(Index);
		Color background = (isSelected, Hover) switch
		{
			(true, true) => GlobalColors.EntrySelectHover,
			(true, false) => GlobalColors.EntrySelect,
			(false, true) => GlobalColors.EntryHover,
			_ => Color.Invisible,
		};
		if (background != Color.Invisible)
			Root.Game.RectangleRenderer.Schedule(Bounds.Size, scrollOffset + Bounds.Center, Depth, background);

		RenderText(new PixelBounds(Bounds.X1, Bounds.Y1, 96, SpawnsScrollArea.SpawnEntryHeight), _enemyColor, _enemy?.Name ?? "Empty", TextAlign.Left);
		RenderText(new PixelBounds(Bounds.X1 + 96, Bounds.Y1, 96, SpawnsScrollArea.SpawnEntryHeight), Color.White, _spawnUiEntry.Delay.ToString("0.0000"), TextAlign.Right);
		RenderText(new PixelBounds(Bounds.X1 + 192, Bounds.Y1, 96, SpawnsScrollArea.SpawnEntryHeight), Color.White, _spawnUiEntry.Seconds.ToString("0.0000"), TextAlign.Right);
		RenderText(new PixelBounds(Bounds.X1 + 288, Bounds.Y1, 48, SpawnsScrollArea.SpawnEntryHeight), Color.White, NoFarmGemsString(_spawnUiEntry.NoFarmGems), TextAlign.Right);
		RenderText(new PixelBounds(Bounds.X1 + 336, Bounds.Y1, 48, SpawnsScrollArea.SpawnEntryHeight), GetColorFromHand(_spawnUiEntry.GemState.HandLevel), _spawnUiEntry.GemState.Value.ToString(), TextAlign.Right);

		void RenderText(IBounds bounds, Color textColor, string text, TextAlign textAlign)
		{
			int padding = (int)MathF.Round((bounds.Y2 - bounds.Y1) / 4f);
			Vector2i<int> textPosition = textAlign switch
			{
				TextAlign.Middle => new Vector2i<int>(bounds.X1 + bounds.X2, bounds.Y1 + bounds.Y2) / 2,
				TextAlign.Left => new(bounds.X1 + padding, bounds.Y1 + padding),
				TextAlign.Right => new(bounds.X2 - padding, bounds.Y1 + padding),
				_ => throw new InvalidOperationException("Invalid text align."),
			};

			Root.Game.MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, scrollOffset + textPosition, Depth + 2, textColor, text, textAlign);
		}

		Color GetColorFromHand(HandLevel handLevel) => handLevel switch
		{
			HandLevel.Level3 => UpgradeColors.Level3.ToWarpColor(),
			HandLevel.Level4 => UpgradeColors.Level4.ToWarpColor(),
			_ => Color.Red,
		};

		static string NoFarmGemsString(int gems) => gems == 0 ? "-" : $"+{gems}";
	}
}
