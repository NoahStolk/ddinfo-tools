using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Utils;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;

public static class ReplayTimelineChild
{
	private const float _markerSize = 24;
	private static readonly Dictionary<EventType, int> _indices = [];
	private static readonly Color _lineColorDefault = Color.Gray(0.4f);
	private static readonly Color _lineColorSub = Color.Gray(0.2f);

	private static int GetIndex(EventType eventType)
	{
		if (_indices.TryGetValue(eventType, out int index))
			return index;

		index = EnumUtils.EventTypes.IndexOf(eventType);
		_indices[eventType] = index;
		return index;
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

				Vector4 backgroundColor = EventTypeRendererUtils.GetEventTypeColor(EnumUtils.EventTypes[i]) with { W = 0.1f };
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

			for (int i = Math.Max(startTickIndex, 0); i < Math.Min(endTickIndex, TimelineCache.Markers.Count); i++)
			{
				TickMarkers tickMarkers = TimelineCache.Markers[i];
				foreach ((EventType eventType, List<ReplayEvent> replayEvents) in tickMarkers.ReplayEvents)
				{
					int eventTypeIndex = GetIndex(eventType);
					ImGui.SetCursorScreenPos(origin + new Vector2(i * _markerSize, eventTypeIndex * _markerSize));
					ImGui.PushStyleColor(ImGuiCol.Button, EventTypeRendererUtils.GetEventTypeColor(eventType) with { W = 0.4f });
					ImGui.Button(replayEvents.Count > 99 ? "XX" : Inline.Span(replayEvents.Count), new(_markerSize, _markerSize));
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

			HandleInput();
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

	private static void HandleInput()
	{
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
			ImGui.SetScrollX(TimelineCache.Markers.Count * _markerSize);

		if (io.MouseWheel != 0)
			ImGui.SetScrollX(ImGui.GetScrollX() - io.MouseWheel * _markerSize * 2.5f);
	}
}
