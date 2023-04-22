using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Ui.Components;

namespace DevilDaggersInfo.App.Engine.Ui;

public class NestingContext
{
	/// <summary>
	/// Components are ordered by depth (ascending).
	/// We update them in reverse order, so components in front of other components are updated first.
	/// We render them in normal order, so components in front of other components are rendered last.
	/// </summary>
	private List<AbstractComponent> _orderedComponents = new();

	/// <summary>
	/// Holds component updates which were processed during the current update, meaning we need to block them so duplicates will be processed during the next update.
	/// </summary>
	private readonly HashSet<QueuedComponentUpdate> _blockedUpdates = new();

	/// <summary>
	/// Component updates which will be applied during the next update. This list is processed in reverse order.
	/// </summary>
	private readonly List<QueuedComponentUpdate> _queuedComponentsUpdates = new();

	public NestingContext(IBounds bounds)
	{
		Bounds = bounds;
	}

	public IReadOnlyList<AbstractComponent> OrderedComponents => _orderedComponents;

	public Vector2i<int> ScrollOffset { get; set; }

	public IBounds Bounds { get; }

	public Action? OnUpdateQueue { get; set; }

	public void Add(AbstractComponent component)
	{
		_queuedComponentsUpdates.Insert(0, new(component, true));
	}

	public void Remove(AbstractComponent component)
	{
		_queuedComponentsUpdates.Insert(0, new(component, false));
	}

	private void UpdateQueue()
	{
		if (_queuedComponentsUpdates.Count == 0)
			return;

		for (int i = _queuedComponentsUpdates.Count - 1; i >= 0; i--)
		{
			QueuedComponentUpdate queuedComponentUpdate = _queuedComponentsUpdates[i];

			// Skip this component if we already processed it.
			// This entry will be processed next time (because we don't drop it).
			// This way we can add and remove the same component multiple times during a single update, without breaking the UI.
			if (_blockedUpdates.Contains(queuedComponentUpdate))
				continue;

			if (queuedComponentUpdate.Add)
			{
				// Only add the component if it doesn't already exist.
				// Otherwise we would get corrupted state.
				if (!_orderedComponents.Contains(queuedComponentUpdate.Component))
				{
					_orderedComponents.Add(queuedComponentUpdate.Component);
					_blockedUpdates.Add(queuedComponentUpdate);
				}
			}
			else
			{
				_orderedComponents.Remove(queuedComponentUpdate.Component);
				_blockedUpdates.Add(queuedComponentUpdate);
			}

			// Always drop this entry, even if we didn't process it because the component already exists.
			_queuedComponentsUpdates.Remove(queuedComponentUpdate);
		}

		_blockedUpdates.Clear();

		// TODO: Don't allocate new memory.
		_orderedComponents = _orderedComponents.OrderBy(c => c.Depth).ToList();

		OnUpdateQueue?.Invoke();
	}

	public void Update(Vector2i<int> scrollOffset)
	{
		UpdateQueue();

		for (int i = _orderedComponents.Count - 1; i >= 0; i--)
		{
			AbstractComponent component = _orderedComponents[i];
			if (Overlaps(component))
				component.Update(scrollOffset + ScrollOffset);
		}
	}

	public void Render(Vector2i<int> scrollOffset)
	{
		for (int i = 0; i < _orderedComponents.Count; i++)
		{
			AbstractComponent component = _orderedComponents[i];
			if (Overlaps(component))
				component.Render(scrollOffset + ScrollOffset);
		}
	}

	private bool Overlaps(AbstractComponent component)
	{
		if (!component.IsActive)
			return false;

		(int x1, int y1, int x2, int y2) = component.Bounds.Move(ScrollOffset.X, ScrollOffset.Y);
		return Bounds.Overlaps(x1, y1, x2, y2);
	}

	private sealed record QueuedComponentUpdate(AbstractComponent Component, bool Add);
}
