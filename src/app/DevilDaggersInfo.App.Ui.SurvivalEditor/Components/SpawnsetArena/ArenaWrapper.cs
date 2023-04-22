using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.Core.Spawnset;
using StateManager = DevilDaggersInfo.App.Ui.Base.StateManagement.StateManager;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena;

public class ArenaWrapper : AbstractComponent
{
	private readonly ShrinkSlider _shrinkSlider;
	private readonly Arena _arena;

	public ArenaWrapper(IBounds bounds)
		: base(bounds)
	{
		const int titleHeight = 48;
		const int arenaSize = Arena.TileSize * SpawnsetBinary.ArenaDimensionMax;
		const int sliderHeight = 16;

		_arena = new(bounds.CreateNested(0, titleHeight, arenaSize, arenaSize));

		Label title = new(bounds.CreateNested(0, 0, arenaSize, titleHeight), "Arena", LabelStyles.Title);
		ArenaHeightButtons arenaHeightButtons = new(bounds.CreateNested(arenaSize + 8, titleHeight, 80, 320));
		_shrinkSlider = new(bounds.CreateNested(0, titleHeight + arenaSize, arenaSize, sliderHeight), _arena.SetShrinkCurrent, true, 0, StateManager.SpawnsetState.Spawnset.GetSliderMaxSeconds(), 0.001f, 0, SliderStyles.Default);
		ArenaToolsWrapper arenaToolsWrapper = new(bounds.CreateNested(0, titleHeight + arenaSize + sliderHeight, 304, 480));

		NestingContext.Add(title);
		NestingContext.Add(_arena);
		NestingContext.Add(arenaHeightButtons);
		NestingContext.Add(_shrinkSlider);
		NestingContext.Add(arenaToolsWrapper);

		StateManager.Subscribe<LoadSpawnset>(SetSliderAndShrinkValues);
		StateManager.Subscribe<SetSpawnsetHistoryIndex>(SetSliderAndShrinkValues);
		StateManager.Subscribe<UpdateArena>(SetSliderAndShrinkValues);
		StateManager.Subscribe<UpdateShrinkStart>(SetSliderAndShrinkValues);
		StateManager.Subscribe<UpdateShrinkEnd>(SetSliderAndShrinkValues);
		StateManager.Subscribe<UpdateShrinkRate>(SetSliderAndShrinkValues);
	}

	private void SetSliderAndShrinkValues()
	{
		_shrinkSlider.Max = StateManager.SpawnsetState.Spawnset.GetSliderMaxSeconds();
		_shrinkSlider.CurrentValue = Math.Clamp(_shrinkSlider.CurrentValue, 0, _shrinkSlider.Max);
		_arena.SetShrinkCurrent(_shrinkSlider.CurrentValue);
	}
}
