using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Rendering.Text;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Extensions;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetHistory;

public class HistoryScrollArea : ScrollArea
{
	private readonly List<AbstractComponent> _historyComponents = new();

	public HistoryScrollArea(IBounds bounds)
		: base(bounds, 96, 16, ScrollAreaStyles.Default)
	{
		StateManager.Subscribe<AddHistory>(SetContent);
		StateManager.Subscribe<ClearHistory>(SetContent);
		StateManager.Subscribe<SetSpawnsetHistoryIndex>(SetContent);

		StateManager.Subscribe<SetLayout>(SetContent);
	}

	private void SetContent()
	{
		// When changes are being made in the 3D editor, AddHistory events are fired. However, we don't want to update the history list in that case, because the component isn't active, meaning that all of these updates will build up and cause a huge performance drop when exiting the 3D editor.
		if (StateManager.LayoutState.CurrentLayout != Root.Dependencies.SurvivalEditorMainLayout)
			return;

		foreach (AbstractComponent component in _historyComponents)
			NestingContext.Remove(component);

		_historyComponents.Clear();

		const int historyEntryHeight = 16;
		for (int i = 0; i < StateManager.SpawnsetHistoryState.History.Count; i++)
		{
			SpawnsetHistoryEntry historyEntry = StateManager.SpawnsetHistoryState.History[i];
			bool isActive = i == StateManager.SpawnsetHistoryState.CurrentIndex;
			Color colorBackground = historyEntry.EditType.GetColor();
			Color colorBackgroundActive = colorBackground.Intensify(32);
			Color hoverBackgroundColor = colorBackground.Intensify(64);
			ButtonStyle buttonStyle = new(isActive ? colorBackgroundActive : colorBackground, isActive ? Color.White : Color.Black, hoverBackgroundColor, 1);
			TextButtonStyle textButtonStyle = new(Color.White, TextAlign.Left, FontSize.H12);
			int index = i;
			TextButton button = new(Bounds.CreateNested(0, i * historyEntryHeight, ContentBounds.Size.X, historyEntryHeight), () => StateManager.Dispatch(new SetSpawnsetHistoryIndex(index)), buttonStyle, textButtonStyle, historyEntry.EditType.GetChange())
			{
				Depth = Depth + 1,
			};
			_historyComponents.Add(button);
		}

		foreach (AbstractComponent component in _historyComponents)
			NestingContext.Add(component);

		int scrollIndex = StateManager.SpawnsetHistoryState.CurrentIndex;
		ScheduleScrollTarget(scrollIndex * historyEntryHeight, (scrollIndex + 1) * historyEntryHeight);
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		Root.Game.RectangleRenderer.Schedule(Bounds.Size, Bounds.Center, Depth, Color.Black);
	}
}
