using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Utils;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;

public static class ReplayTimelineChild
{
	private const float _markerSize = 24;

	private static readonly List<EventType> _shownEventTypes = Enum.GetValues<EventType>().Where(et => et is not EventType.End and not EventType.InitialInputs and not EventType.Inputs).ToList();

	private static readonly Dictionary<EventType, int> _eventTypeRowIndices = [];
	private static readonly Color _lineColorDefault = Color.Gray(0.4f);
	private static readonly Color _lineColorSub = Color.Gray(0.2f);

	private static readonly List<EditorEvent> _selectedEvents = [];
	private static int? _selectedTickIndex;

	private static int GetIndex(EventType eventType)
	{
		if (_eventTypeRowIndices.TryGetValue(eventType, out int index))
			return index;

		index = _shownEventTypes.IndexOf(eventType);
		if (index == -1)
			return -1;

		_eventTypeRowIndices[eventType] = index;
		return index;
	}

	public static void Reset()
	{
		TimelineCache.Clear();
		_selectedEvents.Clear();
		_selectedTickIndex = null;
	}

	public static void Render(EditorReplayModel replay)
	{
		if (TimelineCache.IsEmpty)
			TimelineCache.Build(replay);

		const float markerTextHeight = 32;
		const float scrollBarHeight = 20;
		if (ImGui.BeginChild("TimelineViewChild", new Vector2(0, _shownEventTypes.Count * _markerSize + markerTextHeight + scrollBarHeight)))
		{
			RenderTimeline(replay);
		}

		ImGui.EndChild();

		ReplayTimelineActionsChild.Render();

		ImGui.PushStyleColor(ImGuiCol.ChildBg, Color.Gray(0.08f));
		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(8));
		if (ImGui.BeginChild("SelectedEventsChild", default, ImGuiChildFlags.AlwaysUseWindowPadding))
		{
			if (_selectedTickIndex.HasValue)
			{
				ImGui.PushFont(Root.FontGoetheBold20);
				ImGui.Text(Inline.Span($"Tick {_selectedTickIndex.Value} selected"));
				ImGui.PopFont();

				ReplayTimelineSelectedEventsChild.Render(replay, _selectedEvents, _selectedTickIndex.Value);
			}
		}

		ImGui.EndChild();

		ImGui.PopStyleVar();
		ImGui.PopStyleColor();
	}

	private static void RenderTimeline(EditorReplayModel replay)
	{
		ImGui.PushStyleColor(ImGuiCol.ChildBg, Color.Gray(0.1f));
		const float legendWidth = 160;
		if (ImGui.BeginChild("LegendChild", new Vector2(legendWidth, 0)))
		{
			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			Vector2 origin = ImGui.GetCursorScreenPos();
			for (int i = 0; i < _shownEventTypes.Count; i++)
			{
				EventType eventType = _shownEventTypes[i];
				string name = EnumUtils.EventTypeFriendlyNames[eventType];

				ImGui.SetCursorScreenPos(origin + new Vector2(5, i * _markerSize + 5));
				ImGui.TextColored(eventType.GetColor(), name);

				AddHorizontalLine(drawList, origin, i * _markerSize, legendWidth, _lineColorDefault);
			}

			AddHorizontalLine(drawList, origin, _shownEventTypes.Count * _markerSize, legendWidth, _lineColorDefault);
		}

		ImGui.PopStyleColor();

		ImGui.EndChild();

		ImGui.SameLine();

		if (ImGui.BeginChild("TimelineEditorChild", default, ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar | ImGuiWindowFlags.NoMove))
		{
			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			Vector2 origin = ImGui.GetCursorScreenPos();
			float lineWidth = replay.TickCount * _markerSize;
			for (int i = 0; i < _shownEventTypes.Count; i++)
			{
				AddHorizontalLine(drawList, origin, i * _markerSize, lineWidth, _lineColorSub);

				Vector4 backgroundColor = _shownEventTypes[i].GetColor() with { W = 0.1f };
				drawList.AddRectFilled(origin + new Vector2(0, i * _markerSize), origin + new Vector2(lineWidth, (i + 1) * _markerSize), ImGui.GetColorU32(backgroundColor));
			}

			AddHorizontalLine(drawList, origin, _shownEventTypes.Count * _markerSize, lineWidth, _lineColorSub);

			int startTickIndex = (int)Math.Floor(ImGui.GetScrollX() / _markerSize);
			int endTickIndex = Math.Min((int)Math.Ceiling((ImGui.GetScrollX() + ImGui.GetWindowWidth()) / _markerSize), replay.TickCount);

			// Always render these invisible buttons so the scroll bar is always visible.
			ImGui.SetCursorScreenPos(origin);
			ImGui.InvisibleButton("InvisibleStartMarker", default);
			ImGui.SetCursorScreenPos(origin + new Vector2((replay.TickCount - 1) * _markerSize, 0));
			ImGui.InvisibleButton("InvisibleEndMarker", default);

			for (int i = Math.Max(startTickIndex, 0); i < Math.Min(endTickIndex, replay.TickCount); i++)
			{
				foreach (EventType eventType in _shownEventTypes)
				{
					RenderMarker(replay.StartTime, eventType, origin, i, drawList, TimelineCache.GetEventCountAtTick(i, eventType));
				}
			}

			for (int i = startTickIndex; i < endTickIndex; i++)
			{
				bool showTimeText = i % 5 == 0;
				float height = showTimeText ? _shownEventTypes.Count * _markerSize + 6 : _shownEventTypes.Count * _markerSize;
				Color color = showTimeText ? _lineColorDefault : _lineColorSub;
				AddVerticalLine(drawList, origin, i * _markerSize, height, color);
				if (i == _selectedTickIndex)
					drawList.AddRectFilled(origin + new Vector2(i * _markerSize, 0), origin + new Vector2((i + 1) * _markerSize, _shownEventTypes.Count * _markerSize), ImGui.GetColorU32(new Vector4(1, 1, 1, 0.2f)));

				if (showTimeText)
				{
					Color textColor = i % 60 == 0 ? Color.Yellow : ImGuiUtils.GetColorU32(ImGuiCol.Text);
					ImGui.SetCursorScreenPos(origin + new Vector2(i * _markerSize + 5, _shownEventTypes.Count * _markerSize + 5));
					ImGui.TextColored(textColor, Inline.Span(TimeUtils.TickToTime(i, replay.StartTime), StringFormats.TimeFormat));
					ImGui.SetCursorScreenPos(origin + new Vector2(i * _markerSize + 5, _shownEventTypes.Count * _markerSize + 21));
					ImGui.TextColored(textColor, Inline.Span(i));
				}
			}

			HandleInput(replay, origin);
		}

		ImGui.EndChild();

		static void AddHorizontalLine(ImDrawListPtr drawList, Vector2 origin, float offsetY, float width, Color color)
		{
			drawList.AddLine(origin + new Vector2(0, offsetY), origin + new Vector2(width, offsetY), ImGui.GetColorU32(color));
		}

		static void AddVerticalLine(ImDrawListPtr drawList, Vector2 origin, float offsetX, float height, Color color)
		{
			drawList.AddLine(origin + new Vector2(offsetX, 0), origin + new Vector2(offsetX, height), ImGui.GetColorU32(color));
		}
	}

	private static void RenderMarker(float startTime, EventType eventType, Vector2 origin, int tickIndex, ImDrawListPtr drawList, int eventCount)
	{
		if (eventCount == 0)
			return;

		int eventTypeIndex = GetIndex(eventType);
		Vector2 rectOrigin = origin + new Vector2(tickIndex * _markerSize, eventTypeIndex * _markerSize);
		Vector2 markerSizeVec = new(_markerSize, _markerSize);
		bool isHovering = ImGui.IsMouseHoveringRect(rectOrigin, rectOrigin + markerSizeVec);

		drawList.AddRectFilled(rectOrigin, rectOrigin + markerSizeVec, ImGui.GetColorU32(eventType.GetColor() with { W = isHovering ? 0.7f : 0.4f }));

		float xOffset = eventCount < 10 ? 9 : 5;
		drawList.AddText(rectOrigin + new Vector2(xOffset, 5), 0xffffffff, eventCount > 99 ? Inline.Span("XX") : Inline.Span($"{eventCount}"));

		if (isHovering)
		{
			ImGui.BeginTooltip();
			ImGui.TextColored(eventType.GetColor(), EnumUtils.EventTypeFriendlyNames[eventType]);
			if (ImGui.BeginTable(Inline.Span($"MarkerTooltipTable_{tickIndex}_{EnumUtils.EventTypeNames[eventType]}"), 2, ImGuiTableFlags.Borders))
			{
				ImGui.TableNextColumn();
				ImGui.Text("Event count");

				ImGui.TableNextColumn();
				ImGui.Text(Inline.Span(eventCount));

				ImGui.TableNextColumn();
				ImGui.Text("Time");

				ImGui.TableNextColumn();
				ImGui.Text(Inline.Span(TimeUtils.TickToTime(tickIndex, startTime), StringFormats.TimeFormat));
				ImGui.SameLine();
				ImGui.Text(Inline.Span($"({tickIndex})"));

				ImGui.EndTable();
			}

			ImGui.EndTooltip();
		}
	}

	public static void SelectEvents(EditorReplayModel replay, int tickIndex)
	{
		List<EditorEvent> replayEvents = replay.BoidSpawnEvents
			.Concat(replay.LeviathanSpawnEvents)
			.Concat(replay.PedeSpawnEvents)
			.Concat(replay.SpiderEggSpawnEvents)
			.Concat(replay.SpiderSpawnEvents)
			.Concat(replay.SquidSpawnEvents)
			.Concat(replay.ThornSpawnEvents)
			.Concat(replay.DaggerSpawnEvents)
			.Concat(replay.EntityOrientationEvents)
			.Concat(replay.EntityPositionEvents)
			.Concat(replay.EntityTargetEvents)
			.Concat(replay.GemEvents)
			.Concat(replay.HitEvents)
			.Concat(replay.TransmuteEvents)
			.Where(e => e.TickIndex == tickIndex)
			.ToList();

		_selectedEvents.Clear();
		_selectedEvents.AddRange(replayEvents);
		_selectedTickIndex = tickIndex;
	}

	private static void HandleInput(EditorReplayModel replay, Vector2 origin)
	{
		if (!ImGui.IsWindowHovered())
			return;

		ImGuiIOPtr io = ImGui.GetIO();
		bool left = io.IsKeyDown(ImGuiKey.LeftArrow);
		bool right = io.IsKeyDown(ImGuiKey.RightArrow);
		if (left ^ right)
		{
			float increment = (io.KeyShift ? 3200 : 1200) * io.DeltaTime * (left ? -1 : 1);
			ImGui.SetScrollX(ImGui.GetScrollX() + increment);
		}

		if (ImGui.IsKeyPressed(ImGuiKey.Home))
			ImGui.SetScrollX(0);
		else if (ImGui.IsKeyPressed(ImGuiKey.End))
			ImGui.SetScrollX(replay.TickCount * _markerSize);

		if (io.MouseWheel is < -float.Epsilon or > float.Epsilon)
			ImGui.SetScrollX(ImGui.GetScrollX() - io.MouseWheel * _markerSize * 2.5f);

		bool isClicked = ImGui.IsMouseClicked(ImGuiMouseButton.Left);
		bool isDoubleClicked = ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left);
		if (isClicked || isDoubleClicked)
		{
			HandleClick(replay, isDoubleClicked, origin);
		}
	}

	private static void HandleClick(EditorReplayModel replay, bool isDoubleClicked, Vector2 origin)
	{
		Vector2 mousePos = ImGui.GetMousePos() - origin;
		int tickIndex = (int)Math.Floor(mousePos.X / _markerSize);
		if (tickIndex < 0 || tickIndex >= replay.TickCount)
			return;

		if (isDoubleClicked)
		{
			int eventTypeIndex = (int)Math.Floor(mousePos.Y / _markerSize);
			if (eventTypeIndex < 0 || eventTypeIndex >= _shownEventTypes.Count)
				return;

			EventType eventType = _shownEventTypes[eventTypeIndex];
			IEventData newEventData = eventType switch
			{
				EventType.BoidSpawn => BoidSpawnEventData.CreateDefault(),
				EventType.LeviathanSpawn => LeviathanSpawnEventData.CreateDefault(),
				EventType.PedeSpawn => PedeSpawnEventData.CreateDefault(),
				EventType.SpiderEggSpawn => SpiderEggSpawnEventData.CreateDefault(),
				EventType.SpiderSpawn => SpiderSpawnEventData.CreateDefault(),
				EventType.SquidSpawn => SquidSpawnEventData.CreateDefault(),
				EventType.ThornSpawn => ThornSpawnEventData.CreateDefault(),
				EventType.DaggerSpawn => DaggerSpawnEventData.CreateDefault(),
				EventType.EntityOrientation => EntityOrientationEventData.CreateDefault(),
				EventType.EntityPosition => EntityPositionEventData.CreateDefault(),
				EventType.EntityTarget => EntityTargetEventData.CreateDefault(),
				EventType.Gem => GemEventData.CreateDefault(),
				EventType.Hit => HitEventData.CreateDefault(),
				EventType.Transmute => TransmuteEventData.CreateDefault(),
				EventType.InitialInputs => throw new UnreachableException($"Event type not supported by timeline editor: {eventType}"),
				EventType.Inputs => throw new UnreachableException($"Event type not supported by timeline editor: {eventType}"),
				EventType.End => throw new UnreachableException($"Event type not supported by timeline editor: {eventType}"),
				_ => throw new UnreachableException($"Unknown event data type: {eventType}"),
			};
			replay.AddEvent(tickIndex, newEventData);

			TimelineCache.Clear();
		}

		// Select in case of normal click, but select in case of double-click as well.
		SelectEvents(replay, tickIndex);
	}
}
