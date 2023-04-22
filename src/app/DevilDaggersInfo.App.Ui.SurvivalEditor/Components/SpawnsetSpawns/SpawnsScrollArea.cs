using DevilDaggersInfo.App.Engine;
using DevilDaggersInfo.App.Engine.Extensions;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.App.Ui.SurvivalEditor.EditorSpawns;
using Silk.NET.GLFW;
using System.Collections.Immutable;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetSpawns;

public class SpawnsScrollArea : ScrollArea
{
	public const int SpawnEntryHeight = 16;

	private readonly List<SpawnEntry> _spawnComponents = new();

	private int _currentIndex;

	public SpawnsScrollArea(IBounds bounds)
		: base(bounds, 96, 16, ScrollAreaStyles.Default)
	{
		StateManager.Subscribe<LoadSpawnset>(SetSpawns);
		StateManager.Subscribe<SetSpawnsetHistoryIndex>(SetSpawns);

		StateManager.Subscribe<UpdateHandLevel>(SetSpawns);
		StateManager.Subscribe<UpdateAdditionalGems>(SetSpawns);
		StateManager.Subscribe<UpdateTimerStart>(SetSpawns);

		StateManager.Subscribe<AddSpawn>(SetSpawns);
		StateManager.Subscribe<EditSpawns>(SetSpawns);
		StateManager.Subscribe<InsertSpawn>(SetSpawns);
		StateManager.Subscribe<DeleteSpawns>(SetSpawns);

		StateManager.Subscribe<AddSpawn>(ScrollToEnd);
		StateManager.Subscribe<EditSpawns>(ScrollToFirstSelectedIndex);
		StateManager.Subscribe<InsertSpawn>(ScrollToInsertedSpawn);
	}

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		bool ctrl = Input.IsCtrlHeld();
		bool shift = Input.IsShiftHeld();

		// TODO: Only do this when the component is focused.
		if (ctrl && Input.IsKeyPressed(Keys.A))
			StateManager.Dispatch(new SetSpawnSelections(_spawnComponents.ConvertAll(sp => sp.Index)));

		// TODO: Only do this when the component is focused.
		if (ctrl && Input.IsKeyPressed(Keys.D))
			StateManager.Dispatch(new SetSpawnSelections(new()));

		// TODO: Only do this when the component is focused.
		if (Input.IsKeyPressed(Keys.Delete))
		{
			StateManager.Dispatch(new DeleteSpawns(StateManager.SpawnsetState.Spawnset.Spawns.Where((_, i) => !StateManager.SpawnEditorState.SelectedIndices.Contains(i)).ToImmutableArray()));
			StateManager.Dispatch(new SetSpawnSelections(new()));
		}

		// TODO: Fix this. Right now you can click on File > Save and it will deselect the selected spawns.
		bool hoverWithoutBlock = ContentBounds.Contains(MouseUiContext.MousePosition.RoundToVector2Int32() - scrollOffset);
		if (!Input.IsButtonPressed(MouseButton.Left) || !hoverWithoutBlock)
			return;

		if (shift)
		{
			HashSet<int> newSelectedIndices = StateManager.SpawnEditorState.SelectedIndices.ToHashSet();

			int endIndex = _spawnComponents.Find(sc => sc.Hover)?.Index ?? _spawnComponents.Count - 1;
			int start = Math.Clamp(Math.Min(_currentIndex, endIndex), 0, _spawnComponents.Count - 1);
			int end = Math.Clamp(Math.Max(_currentIndex, endIndex), 0, _spawnComponents.Count - 1);
			for (int i = start; i <= end; i++)
				newSelectedIndices.Add(i);

			StateManager.Dispatch(new SetSpawnSelections(newSelectedIndices.ToList()));
		}
		else if (ctrl)
		{
			HashSet<int> newSelectedIndices = StateManager.SpawnEditorState.SelectedIndices.ToHashSet();

			SpawnEntry? selectedSpawn = _spawnComponents.Find(sc => sc.Hover);

			if (selectedSpawn != null)
			{
				if (newSelectedIndices.Contains(selectedSpawn.Index))
					newSelectedIndices.Remove(selectedSpawn.Index);
				else
					newSelectedIndices.Add(selectedSpawn.Index);

				_currentIndex = selectedSpawn.Index;
			}

			StateManager.Dispatch(new SetSpawnSelections(newSelectedIndices.ToList()));
		}
		else
		{
			HashSet<int> oldSelectedIndices = StateManager.SpawnEditorState.SelectedIndices.ToHashSet();
			HashSet<int> newSelectedIndices = new();

			SpawnEntry? selectedSpawn = _spawnComponents.Find(sc => sc.Hover);

			if (selectedSpawn != null)
			{
				if (!oldSelectedIndices.Contains(selectedSpawn.Index))
					newSelectedIndices.Add(selectedSpawn.Index);

				_currentIndex = selectedSpawn.Index;
			}

			StateManager.Dispatch(new SetSpawnSelections(newSelectedIndices.ToList()));
		}
	}

	private void ScrollToEnd()
	{
		int index = _spawnComponents.Count - 1;
		ScheduleScrollTarget(index * SpawnEntryHeight, (index + 1) * SpawnEntryHeight);
	}

	private void ScrollToFirstSelectedIndex()
	{
		if (StateManager.SpawnEditorState.SelectedIndices.Count == 0)
			throw new InvalidOperationException("No spawn selected.");

		int index = StateManager.SpawnEditorState.SelectedIndices.Min();
		ScheduleScrollTarget(index * SpawnEntryHeight, (index + 1) * SpawnEntryHeight);
	}

	private void ScrollToInsertedSpawn()
	{
		// TODO: Scroll to inserted spawn index.
	}

	private void SetSpawns()
	{
		foreach (SpawnEntry component in _spawnComponents)
			NestingContext.Remove(component);

		_spawnComponents.Clear();

		int i = 0;
		foreach (SpawnUiEntry spawn in EditSpawnContext.GetFrom(StateManager.SpawnsetState.Spawnset))
		{
			SpawnEntry spawnEntry = new(Bounds.CreateNested(0, i++ * SpawnEntryHeight, ContentBounds.Size.X, SpawnEntryHeight), spawn);
			_spawnComponents.Add(spawnEntry);
		}

		foreach (SpawnEntry component in _spawnComponents)
			NestingContext.Add(component);
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		Root.Game.RectangleRenderer.Schedule(Bounds.Size, Bounds.Center, Depth, Color.Black);
	}
}
