using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
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
		const float markerTextHeight = 32;
		const float scrollBarHeight = 20;
		if (ImGui.BeginChild("TimelineViewChild", new(0, EnumUtils.EventTypeNames.Count * _markerSize + markerTextHeight + scrollBarHeight)))
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
			for (int i = 0; i < EnumUtils.EventTypeNames.Count; i++)
			{
				EventType eventType = EnumUtils.EventTypes[i];
				string name = EnumUtils.EventTypeNames[eventType];

				ImGui.SetCursorScreenPos(origin + new Vector2(5, i * _markerSize + 5));
				ImGui.Text(name);

				AddHorizontalLine(drawList, origin, i * _markerSize, legendWidth, _lineColorDefault);
			}

			AddHorizontalLine(drawList, origin, EnumUtils.EventTypeNames.Count * _markerSize, legendWidth, _lineColorDefault);
		}

		ImGui.PopStyleColor();

		ImGui.EndChild(); // End LegendChild

		ImGui.SameLine();

		if (ImGui.BeginChild("TimelineEditorChild", default, ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar))
		{
			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			Vector2 origin = ImGui.GetCursorScreenPos();
			float lineWidth = eventsData.TickCount * _markerSize;
			for (int i = 0; i < EnumUtils.EventTypeNames.Count; i++)
				AddHorizontalLine(drawList, origin, i * _markerSize, lineWidth, _lineColorDefault);

			AddHorizontalLine(drawList, origin, EnumUtils.EventTypeNames.Count * _markerSize, lineWidth, _lineColorDefault);

			int frameIndex = 0;
			for (int i = 0; i < eventsData.Events.Count; i++)
			{
				ReplayEvent replayEvent = eventsData.Events[i];
				EventType? eventType = replayEvent.GetEventType();
				if (eventType is null)
					continue;

				int eventTypeIndex = GetIndex(eventType.Value);
				ImGui.SetCursorScreenPos(origin + new Vector2(frameIndex * _markerSize, eventTypeIndex * _markerSize));
				ImGui.Button("1", new(_markerSize, _markerSize));

				if (eventType is EventType.InitialInputs or EventType.Inputs)
					frameIndex++;
			}

			for (int i = 0; i < eventsData.TickCount + 1; i++)
			{
				bool showTimeText = i % 5 == 0;
				float height = showTimeText ? EnumUtils.EventTypeNames.Count * _markerSize + 6 : EnumUtils.EventTypeNames.Count * _markerSize;
				Color color = showTimeText ? _lineColorDefault : _lineColorSub;
				AddVerticalLine(drawList, origin, i * _markerSize, height, color);
				if (showTimeText)
				{
#pragma warning disable S2583 // False positive
					Color textColor = i % 60 == 0 ? Color.Yellow : ImGuiUtils.GetColorU32(ImGuiCol.Text);
#pragma warning restore S2583 // False positive
					ImGui.SetCursorScreenPos(origin + new Vector2(i * _markerSize + 5, EnumUtils.EventTypeNames.Count * _markerSize + 5));
					ImGui.TextColored(textColor, Inline.Span(TimeUtils.TickToTime(i, startTime), StringFormats.TimeFormat));
					ImGui.SetCursorScreenPos(origin + new Vector2(i * _markerSize + 5, EnumUtils.EventTypeNames.Count * _markerSize + 21));
					ImGui.TextColored(textColor, Inline.Span(i));
				}
			}
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
}
