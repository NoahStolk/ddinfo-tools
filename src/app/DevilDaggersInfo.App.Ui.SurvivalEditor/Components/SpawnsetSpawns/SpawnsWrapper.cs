using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Styling;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetSpawns;

public class SpawnsWrapper : AbstractComponent
{
	public SpawnsWrapper(IBounds bounds)
		: base(bounds)
	{
		const int titleHeight = 48;

		Label title = new(bounds.CreateNested(0, 0, bounds.Size.X, titleHeight), "Spawns", LabelStyles.Title);
		SpawnsScrollArea spawnsScrollArea = new(bounds.CreateNested(0, titleHeight, bounds.Size.X, bounds.Size.Y - titleHeight));

		NestingContext.Add(title);
		NestingContext.Add(spawnsScrollArea);
	}
}
