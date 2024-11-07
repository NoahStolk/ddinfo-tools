using DevilDaggersInfo.Tools.Ui;

namespace DevilDaggersInfo.Tools;

public sealed class UiLayoutManager
{
	private LayoutType _layout;

	public LayoutType Layout
	{
		get => _layout;
		set
		{
			_layout = value;
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
