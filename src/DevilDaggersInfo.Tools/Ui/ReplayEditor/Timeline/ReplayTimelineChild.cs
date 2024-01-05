using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
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
		if (ImGui.BeginChild("TimelineViewChild", new(0, EnumUtils.EventTypeNames.Count * _markerSize + 20)))
		{
			RenderTimeline(eventsData, startTime);
		}

		ImGui.EndChild(); // End TimelineChild
	}

	private static void RenderTimeline(ReplayEventsData eventsData, float startTime)
	{
		ImGui.PushStyleColor(ImGuiCol.ChildBg, Color.Gray(0.1f));
		if (ImGui.BeginChild("LegendChild", new(128, 0)))
		{
			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			Vector2 origin = ImGui.GetCursorScreenPos();
			for (int i = 0; i < EnumUtils.EventTypeNames.Count; i++)
			{
				EventType eventType = EnumUtils.EventTypes[i];
				string name = EnumUtils.EventTypeNames[eventType];

				ImGui.SetCursorScreenPos(origin + new Vector2(0, i * _markerSize + 5));
				ImGui.Text(name);

				AddHorizontalLine(drawList, origin, i * _markerSize, 128, _lineColorDefault);
			}

			AddHorizontalLine(drawList, origin, EnumUtils.EventTypeNames.Count * _markerSize, 128, _lineColorDefault);
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
				AddVerticalLine(drawList, origin, i * _markerSize, EnumUtils.EventTypeNames.Count * _markerSize, i % 5 == 0 ? _lineColorDefault : _lineColorSub);
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
