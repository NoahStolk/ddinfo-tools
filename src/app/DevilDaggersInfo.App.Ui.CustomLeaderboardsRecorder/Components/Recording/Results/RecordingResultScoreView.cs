using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.Core.Wiki;
using Warp.NET.Content;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.Recording.Results;

public abstract class RecordingResultScoreView : AbstractComponent
{
	protected const int _yStart = 24;
	protected const int _labelHeight = 16;

	protected RecordingResultScoreView(IBounds bounds)
		: base(bounds)
	{
	}

	protected static void AddSpacing(ref int y)
	{
		y += _labelHeight / 2;
	}

	protected void AddIcon(ref int y, Texture texture, Color color)
	{
		const int iconSize = 16;
		RecordingIcon recordingIcon = new(Bounds.CreateNested(4, y, iconSize, iconSize), texture, color) { Depth = Depth + 100 };
		NestingContext.Add(recordingIcon);

		y += iconSize;
	}

	protected void AddDeath(ref int y)
	{
		Label left = new(Bounds.CreateNested(0, y, Bounds.Size.X / 2, 16), "Death", LabelStyles.DefaultLeft) { Depth = Depth + 2 };
		Label right = new(Bounds.CreateNested(Bounds.Size.X / 2, y, Bounds.Size.X / 2, 16), Deaths.GetDeathByLeaderboardType(GameConstants.CurrentVersion, Root.Dependencies.GameMemoryService.MainBlock.DeathType)?.Name ?? "?", LabelStyles.DefaultRight) { Depth = Depth + 2 };
		NestingContext.Add(left);
		NestingContext.Add(right);

		y += _labelHeight;
	}
}
