using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Utils;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;

public static class ReplayEventsChild
{
	private static readonly EventCache _eventCache = new();

	private static readonly Dictionary<EventType, bool> _eventTypeEnabled = Enum.GetValues<EventType>().ToDictionary(et => et, _ => true);

	private static int _startTick;
	private static float _targetTime;

	private static bool _showEvents = true;
	private static bool _onlyShowTicksWithEnabledEvents;

	public static void Reset()
	{
		_startTick = 0;
		_targetTime = 0;
	}

	private static void ToggleAll(bool enabled)
	{
		for (int i = 0; i < EnumUtils.EventTypes.Count; i++)
		{
			EventType eventType = EnumUtils.EventTypes[i];
			_eventTypeEnabled[eventType] = enabled;
		}
	}

	public static void Render(ReplayEventsData eventsData, float startTime)
	{
		const int maxTicks = 60;
		const int height = 216;
		const int filteringHeight = 160;

		ImGui.PushStyleColor(ImGuiCol.ChildBg, Color.Gray(0.13f));
		if (ImGui.BeginChild("NavigationAndFilteringWrapper", new(0, height)))
		{
			if (ImGui.BeginChild("TickNavigation", new(448 + 8, height)))
			{
				const int padding = 4;
				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(padding));

				Vector2 iconSize = new(16);
				if (ImGuiImage.ImageButton("Start", Root.InternalResources.ArrowStartTexture.Id, iconSize))
					_startTick = 0;
				ImGui.SameLine();
				if (ImGuiImage.ImageButton("Back", Root.InternalResources.ArrowLeftTexture.Id, iconSize))
					_startTick = Math.Max(0, _startTick - maxTicks);
				ImGui.SameLine();
				if (ImGuiImage.ImageButton("Forward", Root.InternalResources.ArrowRightTexture.Id, iconSize))
					_startTick = Math.Min(eventsData.TickCount - maxTicks, _startTick + maxTicks);
				ImGui.SameLine();
				if (ImGuiImage.ImageButton("End", Root.InternalResources.ArrowEndTexture.Id, iconSize))
					_startTick = eventsData.TickCount - maxTicks;

				ImGui.SameLine();
				ImGui.Text("Go to:");
				ImGui.SameLine();
				ImGui.PushItemWidth(120);

				// TODO: EnterReturnsTrue only works when the value is not the same?
				if (ImGui.InputFloat("##target_time", ref _targetTime, 1, 1, "%.4f", ImGuiInputTextFlags.CharsDecimal | ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AlwaysOverwrite))
					_startTick = TimeUtils.TimeToTick(_targetTime, startTime);

				ImGui.PopItemWidth();

				_startTick = Math.Max(0, Math.Min(_startTick, eventsData.TickCount - maxTicks));
				int endTick = Math.Min(_startTick + maxTicks - 1, eventsData.TickCount);

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(padding));
				ImGui.Text(Inline.Span($"Showing {_startTick} - {endTick} of {eventsData.TickCount} ticks\n{TimeUtils.TickToTime(_startTick, startTime):0.0000} - {TimeUtils.TickToTime(endTick, startTime):0.0000}"));
			}

			ImGui.EndChild(); // TickNavigation

			ImGui.SameLine();

			if (ImGui.BeginChild("EventTypeFiltering", new(0, height)))
			{
				ImGui.Checkbox("Show events", ref _showEvents);
				ImGui.SameLine();
				ImGui.Checkbox("Only show ticks with enabled events", ref _onlyShowTicksWithEnabledEvents);

				ImGui.Separator();

				ImGui.BeginDisabled(!_showEvents);

				const int checkboxesPerRow = 7;
				int rows = (int)Math.Ceiling((float)EnumUtils.EventTypes.Count / checkboxesPerRow);
				for (int i = 0; i < rows; i++)
				{
					if (ImGui.BeginChild(Inline.Span($"EventTypeFiltering{i}"), new(256, filteringHeight)))
					{
						int start = i * checkboxesPerRow;
						for (int j = start; j < Math.Min(EnumUtils.EventTypes.Count, start + checkboxesPerRow); j++)
						{
							EventType eventType = EnumUtils.EventTypes[j];
							bool temp = _eventTypeEnabled[eventType];
							if (ImGui.Checkbox(EventTypeRendererUtils.EventTypeNames[eventType], ref temp))
								_eventTypeEnabled[eventType] = temp;
						}
					}

					ImGui.EndChild();

					if (i < rows - 1)
						ImGui.SameLine();
				}

				ImGui.EndDisabled();

				ImGui.Separator();

				if (ImGui.Button("Enable all"))
					ToggleAll(true);

				ImGui.SameLine();

				if (ImGui.Button("Disable all"))
					ToggleAll(false);
			}

			ImGui.EndChild(); // EventTypeFiltering
		}

		ImGui.EndChild(); // NavigationAndFilteringWrapper

		ImGui.PopStyleColor();

		if (ImGui.BeginChild("ReplayEventsChild", new(0, 0)))
		{
			RenderEventsTable(eventsData, startTime, maxTicks);
		}

		ImGui.EndChild(); // ReplayEventsChild
	}

	private static void RenderEventsTable(ReplayEventsData eventsData, float startTime, int maxTicks)
	{
		if (!ImGui.BeginTable("ReplayEventsTable", 2, ImGuiTableFlags.BordersInnerH))
			return;

		ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("Events", ImGuiTableColumnFlags.None, 384);
		ImGui.TableHeadersRow();

		for (int i = _startTick; i < Math.Min(_startTick + maxTicks, eventsData.TickCount); i++)
		{
			int offset = eventsData.EventOffsetsPerTick[i];
			int count = eventsData.EventOffsetsPerTick[i + 1] - offset;

			_eventCache.Clear();
			bool showTick = !_onlyShowTicksWithEnabledEvents;
			for (int j = offset; j < offset + count; j++)
			{
				ReplayEvent replayEvent = eventsData.Events[j];
				_eventCache.Add(j, replayEvent);
				if (!showTick)
				{
					EventType? eventType = replayEvent.GetEventType();
					if (eventType.HasValue && _eventTypeEnabled[eventType.Value])
						showTick = true;
				}
			}

			if (!showTick)
				continue;

			ImGui.TableNextRow();

			ImGui.TableNextColumn();
			ImGui.Text(Inline.Span($"{TimeUtils.TickToTime(i, startTime):0.0000} ({i})"));

			ImGui.TableNextColumn();

			if (!_showEvents)
				continue;

			static void RenderEvents<TEvent, TRenderer>(
				EventType eventType,
				IReadOnlyList<(int EventIndex, int EntityId, TEvent Event)> events,
				ReplayEventsData replayEventsData)
				where TEvent : IEventData
				where TRenderer : IEventTypeRenderer<TEvent>
			{
				if (_eventTypeEnabled[eventType] && events.Count > 0)
					EventTypeRendererUtils.RenderTable<TEvent, TRenderer>(EventTypeRendererUtils.GetEventTypeColor(eventType), eventType, events, replayEventsData);
			}

			// Enemy spawn events
			RenderEvents<BoidSpawnEventData, BoidSpawnEvents>(EventType.BoidSpawn, _eventCache.BoidSpawnEvents, eventsData);
			RenderEvents<LeviathanSpawnEventData, LeviathanSpawnEvents>(EventType.LeviathanSpawn, _eventCache.LeviathanSpawnEvents, eventsData);
			RenderEvents<PedeSpawnEventData, PedeSpawnEvents>(EventType.PedeSpawn, _eventCache.PedeSpawnEvents, eventsData);
			RenderEvents<SpiderEggSpawnEventData, SpiderEggSpawnEvents>(EventType.SpiderEggSpawn, _eventCache.SpiderEggSpawnEvents, eventsData);
			RenderEvents<SpiderSpawnEventData, SpiderSpawnEvents>(EventType.SpiderSpawn, _eventCache.SpiderSpawnEvents, eventsData);
			RenderEvents<SquidSpawnEventData, SquidSpawnEvents>(EventType.SquidSpawn, _eventCache.SquidSpawnEvents, eventsData);
			RenderEvents<ThornSpawnEventData, ThornSpawnEvents>(EventType.ThornSpawn, _eventCache.ThornSpawnEvents, eventsData);

			// Other events
			RenderEvents<DaggerSpawnEventData, DaggerSpawnEvents>(EventType.DaggerSpawn, _eventCache.DaggerSpawnEvents, eventsData);
			RenderEvents<EntityOrientationEventData, EntityOrientationEvents>(EventType.EntityOrientation, _eventCache.EntityOrientationEvents, eventsData);
			RenderEvents<EntityPositionEventData, EntityPositionEvents>(EventType.EntityPosition, _eventCache.EntityPositionEvents, eventsData);
			RenderEvents<EntityTargetEventData, EntityTargetEvents>(EventType.EntityTarget, _eventCache.EntityTargetEvents, eventsData);
			RenderEvents<GemEventData, GemEvents>(EventType.Gem, _eventCache.GemEvents, eventsData);
			RenderEvents<HitEventData, HitEvents>(EventType.Hit, _eventCache.HitEvents, eventsData);
			RenderEvents<TransmuteEventData, TransmuteEvents>(EventType.Transmute, _eventCache.TransmuteEvents, eventsData);

			// Final events
			RenderEvents<InitialInputsEventData, InitialInputsEvents>(EventType.InitialInputs, _eventCache.InitialInputsEvents, eventsData);
			RenderEvents<InputsEventData, InputsEvents>(EventType.Inputs, _eventCache.InputsEvents, eventsData);
			RenderEvents<EndEventData, EndEvents>(EventType.End, _eventCache.EndEvents, eventsData);
		}

		ImGui.EndTable();
	}
}
