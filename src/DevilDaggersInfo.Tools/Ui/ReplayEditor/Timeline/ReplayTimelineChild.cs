using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Utils;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;

public static class ReplayTimelineChild
{
	private const float _markerSize = 24;
	private static readonly Dictionary<EventType, int> _eventTypeRowIndices = [];
	private static readonly Color _lineColorDefault = Color.Gray(0.4f);
	private static readonly Color _lineColorSub = Color.Gray(0.2f);

	private static readonly List<ReplayEvent> _selectedEvents = [];
	private static readonly EventCache _selectedEventDataCache = new();
	private static EventType? _selectedEventType;
	private static int? _selectedTickIndex;

	private static int GetIndex(EventType eventType)
	{
		if (_eventTypeRowIndices.TryGetValue(eventType, out int index))
			return index;

		index = EnumUtils.EventTypes.IndexOf(eventType);
		_eventTypeRowIndices[eventType] = index;
		return index;
	}

	public static void Reset()
	{
		TimelineCache.Clear();
		_selectedEvents.Clear();
		_selectedEventDataCache.Clear();
		_selectedEventType = null;
		_selectedTickIndex = 0;
	}

	public static void Render(ReplayEventsData eventsData, float startTime)
	{
		if (TimelineCache.IsEmpty)
			TimelineCache.Build(eventsData);

		const float markerTextHeight = 32;
		const float scrollBarHeight = 20;
		if (ImGui.BeginChild("TimelineViewChild", new(0, EnumUtils.EventTypes.Count * _markerSize + markerTextHeight + scrollBarHeight)))
		{
			RenderTimeline(eventsData, startTime);
		}

		ImGui.EndChild(); // End TimelineViewChild

		ReplayTimelineActionsChild.Render(eventsData);

		if (ImGui.BeginChild("SelectedEventsChild"))
		{
			RenderSelectedEvents(eventsData);
		}

		ImGui.EndChild(); // End SelectedEventsChild
	}

	private static void RenderSelectedEvents(ReplayEventsData replayEventsData)
	{
		if (_selectedEventType == null || _selectedEvents.Count == 0)
		{
			ImGui.Text("No events selected");
			return;
		}

		switch (_selectedEventType)
		{
			case EventType.BoidSpawn: EventTypeRendererUtils.RenderTable<BoidSpawnEventData, BoidSpawnEvents>(_selectedEventType.Value, _selectedEventDataCache.BoidSpawnEvents, replayEventsData); break;
			case EventType.LeviathanSpawn: EventTypeRendererUtils.RenderTable<LeviathanSpawnEventData, LeviathanSpawnEvents>(_selectedEventType.Value, _selectedEventDataCache.LeviathanSpawnEvents, replayEventsData); break;
			case EventType.PedeSpawn: EventTypeRendererUtils.RenderTable<PedeSpawnEventData, PedeSpawnEvents>(_selectedEventType.Value, _selectedEventDataCache.PedeSpawnEvents, replayEventsData); break;
			case EventType.SpiderEggSpawn: EventTypeRendererUtils.RenderTable<SpiderEggSpawnEventData, SpiderEggSpawnEvents>(_selectedEventType.Value, _selectedEventDataCache.SpiderEggSpawnEvents, replayEventsData); break;
			case EventType.SpiderSpawn: EventTypeRendererUtils.RenderTable<SpiderSpawnEventData, SpiderSpawnEvents>(_selectedEventType.Value, _selectedEventDataCache.SpiderSpawnEvents, replayEventsData); break;
			case EventType.SquidSpawn: EventTypeRendererUtils.RenderTable<SquidSpawnEventData, SquidSpawnEvents>(_selectedEventType.Value, _selectedEventDataCache.SquidSpawnEvents, replayEventsData); break;
			case EventType.ThornSpawn: EventTypeRendererUtils.RenderTable<ThornSpawnEventData, ThornSpawnEvents>(_selectedEventType.Value, _selectedEventDataCache.ThornSpawnEvents, replayEventsData); break;
			case EventType.DaggerSpawn: EventTypeRendererUtils.RenderTable<DaggerSpawnEventData, DaggerSpawnEvents>(_selectedEventType.Value, _selectedEventDataCache.DaggerSpawnEvents, replayEventsData); break;
			case EventType.EntityOrientation: EventTypeRendererUtils.RenderTable<EntityOrientationEventData, EntityOrientationEvents>(_selectedEventType.Value, _selectedEventDataCache.EntityOrientationEvents, replayEventsData); break;
			case EventType.EntityPosition: EventTypeRendererUtils.RenderTable<EntityPositionEventData, EntityPositionEvents>(_selectedEventType.Value, _selectedEventDataCache.EntityPositionEvents, replayEventsData); break;
			case EventType.EntityTarget: EventTypeRendererUtils.RenderTable<EntityTargetEventData, EntityTargetEvents>(_selectedEventType.Value, _selectedEventDataCache.EntityTargetEvents, replayEventsData); break;
			case EventType.Gem: EventTypeRendererUtils.RenderTable<GemEventData, GemEvents>(_selectedEventType.Value, _selectedEventDataCache.GemEvents, replayEventsData); break;
			case EventType.Hit: EventTypeRendererUtils.RenderTable<HitEventData, HitEvents>(_selectedEventType.Value, _selectedEventDataCache.HitEvents, replayEventsData); break;
			case EventType.Transmute: EventTypeRendererUtils.RenderTable<TransmuteEventData, TransmuteEvents>(_selectedEventType.Value, _selectedEventDataCache.TransmuteEvents, replayEventsData); break;
			case EventType.InitialInputs: EventTypeRendererUtils.RenderTable<InitialInputsEventData, InitialInputsEvents>(_selectedEventType.Value, _selectedEventDataCache.InitialInputsEvents, replayEventsData); break;
			case EventType.Inputs: EventTypeRendererUtils.RenderTable<InputsEventData, InputsEvents>(_selectedEventType.Value, _selectedEventDataCache.InputsEvents, replayEventsData); break;
			case EventType.End: EventTypeRendererUtils.RenderTable<EndEventData, EndEvents>(_selectedEventType.Value, _selectedEventDataCache.EndEvents, replayEventsData); break;
			default: throw new UnreachableException($"Unknown event type: {_selectedEventType}");
		}
	}

	private static void RenderTimeline(ReplayEventsData eventsData, float startTime)
	{
		ImGui.PushStyleColor(ImGuiCol.ChildBg, Color.Gray(0.1f));
		const float legendWidth = 160;
		if (ImGui.BeginChild("LegendChild", new(legendWidth, 0)))
		{
			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			Vector2 origin = ImGui.GetCursorScreenPos();
			for (int i = 0; i < EnumUtils.EventTypes.Count; i++)
			{
				EventType eventType = EnumUtils.EventTypes[i];
				string name = EnumUtils.EventTypeFriendlyNames[eventType];

				ImGui.SetCursorScreenPos(origin + new Vector2(5, i * _markerSize + 5));
				ImGui.TextColored(EventTypeRendererUtils.GetEventTypeColor(eventType), name);

				AddHorizontalLine(drawList, origin, i * _markerSize, legendWidth, _lineColorDefault);
			}

			AddHorizontalLine(drawList, origin, EnumUtils.EventTypes.Count * _markerSize, legendWidth, _lineColorDefault);
		}

		ImGui.PopStyleColor();

		ImGui.EndChild(); // End LegendChild

		ImGui.SameLine();

		if (ImGui.BeginChild("TimelineEditorChild", default, ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar))
		{
			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			Vector2 origin = ImGui.GetCursorScreenPos();
			float lineWidth = eventsData.TickCount * _markerSize;
			for (int i = 0; i < EnumUtils.EventTypes.Count; i++)
			{
				AddHorizontalLine(drawList, origin, i * _markerSize, lineWidth, _lineColorDefault);

				bool isSelected = _selectedEventType == EnumUtils.EventTypes[i];
				Vector4 backgroundColor = EventTypeRendererUtils.GetEventTypeColor(EnumUtils.EventTypes[i]) with { W = isSelected ? 0.3f : 0.1f };
				drawList.AddRectFilled(origin + new Vector2(0, i * _markerSize), origin + new Vector2(lineWidth, (i + 1) * _markerSize), ImGui.GetColorU32(backgroundColor));
			}

			AddHorizontalLine(drawList, origin, EnumUtils.EventTypes.Count * _markerSize, lineWidth, _lineColorDefault);

			int startTickIndex = (int)Math.Floor(ImGui.GetScrollX() / _markerSize);
			int endTickIndex = Math.Min((int)Math.Ceiling((ImGui.GetScrollX() + ImGui.GetWindowWidth()) / _markerSize), eventsData.TickCount);

			// Always render these invisible buttons so the scroll bar is always visible.
			ImGui.SetCursorScreenPos(origin);
			ImGui.InvisibleButton("InvisibleStartMarker", new(_markerSize, _markerSize));
			ImGui.SetCursorScreenPos(origin + new Vector2((eventsData.TickCount - 1) * _markerSize, 0));
			ImGui.InvisibleButton("InvisibleEndMarker", new(_markerSize, _markerSize));

			for (int i = Math.Max(startTickIndex, 0); i < Math.Min(endTickIndex, TimelineCache.TickData.Count); i++)
			{
				Dictionary<EventType, List<(ReplayEvent Event, int EventIndex)>> tickData = TimelineCache.TickData[i];
				foreach ((EventType eventType, List<(ReplayEvent Event, int EventIndex)> replayEvents) in tickData)
				{
					int eventTypeIndex = GetIndex(eventType);
					ImGui.SetCursorScreenPos(origin + new Vector2(i * _markerSize, eventTypeIndex * _markerSize));
					ImGui.PushStyleColor(ImGuiCol.Button, EventTypeRendererUtils.GetEventTypeColor(eventType) with { W = 0.4f });
					string typeName = EnumUtils.EventTypeNames[eventType];
					if (ImGui.Button(replayEvents.Count > 99 ? Inline.Span($"XX##_{i}_{typeName}") : Inline.Span($"{replayEvents.Count}##_{i}_{typeName}"), new(_markerSize, _markerSize)))
						SelectEvents(replayEvents, eventType, i);

					if (ImGui.IsItemHovered())
					{
						ImGui.BeginTooltip();
						ImGui.TextColored(EventTypeRendererUtils.GetEventTypeColor(eventType), EnumUtils.EventTypeFriendlyNames[eventType]);
						if (ImGui.BeginTable(Inline.Span($"MarkerTooltipTable_{i}_{EnumUtils.EventTypeNames[eventType]}"), 2, ImGuiTableFlags.Borders))
						{
							ImGui.TableNextColumn();
							ImGui.Text("Event count");

							ImGui.TableNextColumn();
							ImGui.Text(Inline.Span(replayEvents.Count));

							ImGui.TableNextColumn();
							ImGui.Text("Time");

							ImGui.TableNextColumn();
							ImGui.Text(Inline.Span(TimeUtils.TickToTime(i, startTime), StringFormats.TimeFormat));
							ImGui.SameLine();
							ImGui.Text(Inline.Span($"({i})"));

							ImGui.EndTable();
						}

						ImGui.EndTooltip();
					}

					ImGui.PopStyleColor();
				}
			}

			for (int i = startTickIndex; i < endTickIndex; i++)
			{
				bool showTimeText = i % 5 == 0;
				float height = showTimeText ? EnumUtils.EventTypes.Count * _markerSize + 6 : EnumUtils.EventTypes.Count * _markerSize;
				Color color = showTimeText ? _lineColorDefault : _lineColorSub;
				AddVerticalLine(drawList, origin, i * _markerSize, height, color);
				if (i == _selectedTickIndex)
					drawList.AddRectFilled(origin + new Vector2(i * _markerSize, 0), origin + new Vector2((i + 1) * _markerSize, EnumUtils.EventTypes.Count * _markerSize), ImGui.GetColorU32(new Vector4(1, 1, 1, 0.2f)));

				if (showTimeText)
				{
#pragma warning disable S2583 // False positive
					Color textColor = i % 60 == 0 ? Color.Yellow : ImGuiUtils.GetColorU32(ImGuiCol.Text);
#pragma warning restore S2583 // False positive
					ImGui.SetCursorScreenPos(origin + new Vector2(i * _markerSize + 5, EnumUtils.EventTypes.Count * _markerSize + 5));
					ImGui.TextColored(textColor, Inline.Span(TimeUtils.TickToTime(i, startTime), StringFormats.TimeFormat));
					ImGui.SetCursorScreenPos(origin + new Vector2(i * _markerSize + 5, EnumUtils.EventTypes.Count * _markerSize + 21));
					ImGui.TextColored(textColor, Inline.Span(i));
				}
			}

			HandleInput(origin);
		}

		ImGui.EndChild(); // End TimelineEditorChild

		static void AddHorizontalLine(ImDrawListPtr drawList, Vector2 origin, float offsetY, float width, Color color)
		{
			drawList.AddLine(origin + new Vector2(0, offsetY), origin + new Vector2(width, offsetY), ImGui.GetColorU32(color));
		}

		static void AddVerticalLine(ImDrawListPtr drawList, Vector2 origin, float offsetX, float height, Color color)
		{
			drawList.AddLine(origin + new Vector2(offsetX, 0), origin + new Vector2(offsetX, height), ImGui.GetColorU32(color));
		}
	}

	private static void SelectEvents(List<(ReplayEvent Event, int EventIndex)> replayEvents, EventType eventType, int tickIndex)
	{
		_selectedEvents.Clear();
		_selectedEvents.AddRange(replayEvents.Select(t => t.Event));
		_selectedEventType = eventType;
		_selectedTickIndex = tickIndex;

		_selectedEventDataCache.Clear();
		foreach ((ReplayEvent replayEvent, int eventIndex) in replayEvents)
			_selectedEventDataCache.Add(eventIndex, replayEvent);
	}

	private static void HandleInput(Vector2 origin)
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
			ImGui.SetScrollX(TimelineCache.TickData.Count * _markerSize);

		if (io.MouseWheel != 0)
			ImGui.SetScrollX(ImGui.GetScrollX() - io.MouseWheel * _markerSize * 2.5f);

		if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
		{
			Vector2 mousePos = ImGui.GetMousePos() - origin;

			// TODO: Fix:
			// - Tick index is not always correct.
			// - Adding boid spawn event to 0th tick doesn't work.
			int tickIndex = (int)Math.Floor(mousePos.X / _markerSize);
			int eventTypeIndex = Math.Clamp((int)Math.Floor(mousePos.Y / _markerSize), 0, EnumUtils.EventTypes.Count - 1);
			EventType eventType = EnumUtils.EventTypes[eventTypeIndex];

			if (tickIndex >= 0 && tickIndex < FileStates.Replay.Object.EventsData.EventOffsetsPerTick.Count)
			{
				int eventIndex = FileStates.Replay.Object.EventsData.EventOffsetsPerTick[tickIndex];
				IEventData eventData = eventType switch
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
					EventType.InitialInputs => InitialInputsEventData.CreateDefault(),
					EventType.Inputs => InputsEventData.CreateDefault(),
					EventType.End => EndEventData.CreateDefault(),
					_ => throw new UnreachableException($"Unknown event type: {eventType}"),
				};

				FileStates.Replay.Object.EventsData.InsertEvent(eventIndex, eventData);
				TimelineCache.Clear();
			}
		}
	}
}
