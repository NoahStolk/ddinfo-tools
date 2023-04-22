using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.Extensions;
using DevilDaggersInfo.Core.Wiki.Objects;
using DevilDaggersInfo.Types.Core.Spawnsets;

namespace DevilDaggersInfo.App.Ui.Base.Styling;

public static class ButtonStyles
{
	static ButtonStyles()
	{
		foreach (EnemyType enemyType in Enum.GetValues<EnemyType>())
		{
			Enemy? enemy = enemyType.GetEnemy();
			Color enemyColor = enemy?.Color.ToWarpColor() ?? Color.Gray(0.75f);
			Color hoverBackgroundColor = Color.Lerp(enemyColor, Color.White, 0.5f);

			Enemies[enemyType] = new(enemyColor, Color.Black, hoverBackgroundColor, 1);
			SelectedEnemies[enemyType] = new(enemyColor, Color.White, hoverBackgroundColor, 1);
		}

		foreach (HandLevel handLevel in Enum.GetValues<HandLevel>())
		{
			Color handColor = handLevel.GetColor();
			HandLevels[handLevel] = new(Color.Black, handColor, Color.Gray(0.6f), 1);
			SelectedHandLevels[handLevel] = new(handColor, handColor, Color.Gray(0.6f), 1);
		}
	}

	public static ButtonStyle Borderless { get; } = new(Color.Invisible, Color.Invisible, Color.Invisible, 0);
	public static ButtonStyle Default { get; } = new(Color.Black, Color.White, Color.Gray(0.5f), 1);
	public static ButtonStyle NavigationButton { get; } = new(Color.Black, Color.Gray(0.75f), Color.Gray(0.5f), 1);

	public static Dictionary<EnemyType, ButtonStyle> Enemies { get; } = new();
	public static Dictionary<EnemyType, ButtonStyle> SelectedEnemies { get; } = new();

	public static Dictionary<HandLevel, ButtonStyle> HandLevels { get; } = new();
	public static Dictionary<HandLevel, ButtonStyle> SelectedHandLevels { get; } = new();
}
