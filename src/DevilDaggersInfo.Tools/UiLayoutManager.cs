using DevilDaggersInfo.Tools.Ui;

namespace DevilDaggersInfo.Tools;

internal sealed class UiLayoutManager
{
	public LayoutType Layout
	{
		get;
		set
		{
			field = value;
			Colors.SetColors(value switch
			{
				LayoutType.Config or LayoutType.Main => Colors.Main,
				LayoutType.SpawnsetEditor => Colors.SpawnsetEditor,
				LayoutType.AssetEditor => Colors.AssetEditor,
				LayoutType.ReplayEditor => Colors.ReplayEditor,
				LayoutType.CustomLeaderboards => Colors.CustomLeaderboards,
				LayoutType.Practice => Colors.Practice,
				LayoutType.ModManager => Colors.ModManager,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
			});
		}
	}
}
