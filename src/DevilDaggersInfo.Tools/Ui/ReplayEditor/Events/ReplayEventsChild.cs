using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Events.Interfaces;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
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
				if (ImGuiImage.ImageButton("Start", Root.InternalResources.ArrowStartTexture.Handle, iconSize))
					_startTick = 0;
				ImGui.SameLine();
				if (ImGuiImage.ImageButton("Back", Root.InternalResources.ArrowLeftTexture.Handle, iconSize))
					_startTick = Math.Max(0, _startTick - maxTicks);
				ImGui.SameLine();
				if (ImGuiImage.ImageButton("Forward", Root.InternalResources.ArrowRightTexture.Handle, iconSize))
					_startTick = Math.Min(eventsData.TickCount - maxTicks, _startTick + maxTicks);
				ImGui.SameLine();
				if (ImGuiImage.ImageButton("End", Root.InternalResources.ArrowEndTexture.Handle, iconSize))
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

				if (ImGui.BeginChild("EventTypeFilteringLeft", new(256, filteringHeight)))
				{
					for (int i = 0; i < EnumUtils.EventTypes.Count / 2; i++)
						EventTypeFilterCheckbox(i);
				}

				ImGui.EndChild(); // EventTypeFilteringLeft

				ImGui.SameLine();

				if (ImGui.BeginChild("EventTypeFilteringRight", new(256, filteringHeight)))
				{
					for (int i = EnumUtils.EventTypes.Count / 2; i < EnumUtils.EventTypes.Count; i++)
						EventTypeFilterCheckbox(i);
				}

				ImGui.EndChild(); // EventTypeFilteringRight

				static void EventTypeFilterCheckbox(int i)
				{
					EventType eventType = EnumUtils.EventTypes[i];
					bool temp = _eventTypeEnabled[eventType];
					if (ImGui.Checkbox(EventTypeRendererUtils.EventTypeNames[eventType], ref temp))
						_eventTypeEnabled[eventType] = temp;
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
		if (!ImGui.BeginTable("ReplayEventsTable", 3, ImGuiTableFlags.BordersInnerH))
			return;

		ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("Inputs", ImGuiTableColumnFlags.WidthFixed, 320);
		ImGui.TableSetupColumn("Events", ImGuiTableColumnFlags.None, 384);
		ImGui.TableHeadersRow();

		for (int i = _startTick; i < Math.Min(_startTick + maxTicks, eventsData.TickCount); i++)
		{
			int offset = eventsData.EventOffsetsPerTick[i];
			int count = eventsData.EventOffsetsPerTick[i + 1] - offset;

			// TODO: Refactor using discriminated unions if they ever get added to C#.
			InputsEvent? inputsE = null;
			InitialInputsEvent? initInputsE = null;
			_eventCache.Clear();
			bool showTick = !_onlyShowTicksWithEnabledEvents;
			for (int j = offset; j < offset + count; j++)
			{
				IEvent @event = eventsData.Events[j];
				if (@event is InputsEvent ie)
				{
					inputsE = ie;
				}
				else if (@event is InitialInputsEvent iie)
				{
					initInputsE = iie;
				}
				else
				{
					_eventCache.Add(j, @event);

					if (!showTick)
					{
						EventType? eventType = @event switch
						{
							BoidSpawnEvent => EventType.BoidSpawn,
							LeviathanSpawnEvent => EventType.LeviathanSpawn,
							PedeSpawnEvent => EventType.PedeSpawn,
							SpiderEggSpawnEvent => EventType.SpiderEggSpawn,
							SpiderSpawnEvent => EventType.SpiderSpawn,
							SquidSpawnEvent => EventType.SquidSpawn,
							ThornSpawnEvent => EventType.ThornSpawn,
							DaggerSpawnEvent => EventType.DaggerSpawn,
							EntityOrientationEvent => EventType.EntityOrientation,
							EntityPositionEvent => EventType.EntityPosition,
							EntityTargetEvent => EventType.EntityTarget,
							GemEvent => EventType.Gem,
							HitEvent => EventType.Hit,
							TransmuteEvent => EventType.Transmute,
							_ => null,
						};
						if (eventType.HasValue && _eventTypeEnabled[eventType.Value])
							showTick = true;
					}
				}
			}

			if (!showTick)
				continue;

			ImGui.TableNextRow();

			ImGui.TableNextColumn();
			ImGui.Text(Inline.Span($"{TimeUtils.TickToTime(i, startTime):0.0000} ({i})"));
			ImGui.TableNextColumn();
			if (inputsE != null)
				RenderInputsEvent(inputsE.Value.Left, inputsE.Value.Right, inputsE.Value.Forward, inputsE.Value.Backward, inputsE.Value.Jump, inputsE.Value.Shoot, inputsE.Value.ShootHoming, inputsE.Value.MouseX, inputsE.Value.MouseY, null);
			else if (initInputsE != null)
				RenderInputsEvent(initInputsE.Value.Left, initInputsE.Value.Right, initInputsE.Value.Forward, initInputsE.Value.Backward, initInputsE.Value.Jump, initInputsE.Value.Shoot, initInputsE.Value.ShootHoming, initInputsE.Value.MouseX, initInputsE.Value.MouseY, initInputsE.Value.LookSpeed);
			else
				ImGui.Text("End of inputs");

			ImGui.TableNextColumn();

			if (!_showEvents)
				continue;

			static void RenderEvents<TEvent, TRenderer>(
				EventType eventType,
				IReadOnlyList<(int Index, TEvent Event)> events,
				IReadOnlyList<EntityType> entityTypes,
				IReadOnlyList<EventColumn> columns)
				where TEvent : IEvent
				where TRenderer : IEventTypeRenderer<TEvent>
			{
				if (_eventTypeEnabled[eventType] && events.Count > 0)
					TRenderer.Render(events, entityTypes, columns);
			}

			static void RenderEvent<TEvent, TRenderer>(
				IReadOnlyList<(int Index, TEvent Event)> events,
				IReadOnlyList<EntityType> entityTypes,
				IReadOnlyList<EventColumn> columns)
				where TEvent : IEvent
				where TRenderer : IEventTypeRenderer<TEvent>
			{
				if (events.Count > 0)
					TRenderer.Render(events, entityTypes, columns);
			}

			// Enemy spawn events
			RenderEvents<BoidSpawnEvent, BoidSpawnEvents>(EventType.BoidSpawn, _eventCache.BoidSpawnEvents, eventsData.EntityTypes, EventColumns.ColumnsBoidSpawn);
			RenderEvents<LeviathanSpawnEvent, LeviathanSpawnEvents>(EventType.LeviathanSpawn, _eventCache.LeviathanSpawnEvents, eventsData.EntityTypes, EventColumns.ColumnsLeviathanSpawn);
			RenderEvents<PedeSpawnEvent, PedeSpawnEvents>(EventType.PedeSpawn, _eventCache.PedeSpawnEvents, eventsData.EntityTypes, EventColumns.ColumnsPedeSpawn);
			RenderEvents<SpiderEggSpawnEvent, SpiderEggSpawnEvents>(EventType.SpiderEggSpawn, _eventCache.SpiderEggSpawnEvents, eventsData.EntityTypes, EventColumns.ColumnsSpiderEggSpawn);
			RenderEvents<SpiderSpawnEvent, SpiderSpawnEvents>(EventType.SpiderSpawn, _eventCache.SpiderSpawnEvents, eventsData.EntityTypes, EventColumns.ColumnsSpiderSpawn);
			RenderEvents<SquidSpawnEvent, SquidSpawnEvents>(EventType.SquidSpawn, _eventCache.SquidSpawnEvents, eventsData.EntityTypes, EventColumns.ColumnsSquidSpawn);
			RenderEvents<ThornSpawnEvent, ThornSpawnEvents>(EventType.ThornSpawn, _eventCache.ThornSpawnEvents, eventsData.EntityTypes, EventColumns.ColumnsThornSpawn);

			// Other events
			RenderEvents<DaggerSpawnEvent, DaggerSpawnEvents>(EventType.DaggerSpawn, _eventCache.DaggerSpawnEvents, eventsData.EntityTypes, EventColumns.ColumnsDaggerSpawn);
			RenderEvents<EntityOrientationEvent, EntityOrientationEvents>(EventType.EntityOrientation, _eventCache.EntityOrientationEvents, eventsData.EntityTypes, EventColumns.ColumnsEntityOrientation);
			RenderEvents<EntityPositionEvent, EntityPositionEvents>(EventType.EntityPosition, _eventCache.EntityPositionEvents, eventsData.EntityTypes, EventColumns.ColumnsEntityPosition);
			RenderEvents<EntityTargetEvent, EntityTargetEvents>(EventType.EntityTarget, _eventCache.EntityTargetEvents, eventsData.EntityTypes, EventColumns.ColumnsEntityTarget);
			RenderEvents<GemEvent, GemEvents>(EventType.Gem, _eventCache.GemEvents, eventsData.EntityTypes, EventColumns.ColumnsGem);
			RenderEvents<HitEvent, HitEvents>(EventType.Hit, _eventCache.HitEvents, eventsData.EntityTypes, EventColumns.ColumnsHit);
			RenderEvents<TransmuteEvent, TransmuteEvents>(EventType.Transmute, _eventCache.TransmuteEvents, eventsData.EntityTypes, EventColumns.ColumnsTransmute);

			// Final events
			RenderEvent<DeathEvent, DeathEvents>(_eventCache.DeathEvents, eventsData.EntityTypes, EventColumns.ColumnsDeath);
			RenderEvent<EndEvent, EndEvents>(_eventCache.EndEvents, eventsData.EntityTypes, EventColumns.ColumnsEnd);
		}

		ImGui.EndTable();
	}

	private static void RenderInputsEvent(
		bool left,
		bool right,
		bool forward,
		bool backward,
		JumpType jump,
		ShootType shoot,
		ShootType shootHoming,
		short mouseX,
		short mouseY,
		float? lookSpeed)
	{
		ImGui.TextColored(forward ? Color.Red : Color.White, "W");
		ImGui.SameLine();
		ImGui.TextColored(left ? Color.Red : Color.White, "A");
		ImGui.SameLine();
		ImGui.TextColored(backward ? Color.Red : Color.White, "S");
		ImGui.SameLine();
		ImGui.TextColored(right ? Color.Red : Color.White, "D");
		ImGui.SameLine();
		ImGui.TextColored(GetJumpTypeColor(jump), "[Space]");
		ImGui.SameLine();
		ImGui.TextColored(GetShootTypeColor(shoot), "[LMB]");
		ImGui.SameLine();
		ImGui.TextColored(GetShootTypeColor(shootHoming), "[RMB]");
		ImGui.SameLine();
		ImGui.TextColored(mouseX == 0 ? Color.White : Color.Red, Inline.Span($"X:{mouseX}"));
		ImGui.SameLine();
		ImGui.TextColored(mouseY == 0 ? Color.White : Color.Red, Inline.Span($"Y:{mouseY}"));

		if (lookSpeed.HasValue)
			ImGui.TextColored(Color.White, Inline.Span($"Look Speed: {lookSpeed.Value}"));

		static Color GetJumpTypeColor(JumpType jumpType) => jumpType switch
		{
			JumpType.Hold => Color.Orange,
			JumpType.StartedPress => Color.Red,
			_ => Color.White,
		};

		static Color GetShootTypeColor(ShootType shootType) => shootType switch
		{
			ShootType.Hold => Color.Orange,
			ShootType.Release => Color.Red,
			_ => Color.White,
		};
	}
}
