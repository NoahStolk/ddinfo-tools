using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.ReplayEditor.Events.EventTypes;
using DevilDaggersInfo.App.Ui.ReplayEditor.Utils;
using DevilDaggersInfo.App.Utils;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Events.Interfaces;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Events;

public static class ReplayEventsChild
{
	private static readonly EventCache _eventCache = new();

	private static readonly List<EventColumn> _columnsBoidSpawn = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Spawner Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Type", ImGuiTableColumnFlags.None, 128),
		new("Position", ImGuiTableColumnFlags.None, 196),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("Speed", ImGuiTableColumnFlags.None, 128),
	};

	private static readonly List<EventColumn> _columnsLeviathanSpawn = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("?", ImGuiTableColumnFlags.None, 128),
	};

	private static readonly List<EventColumn> _columnsPedeSpawn = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Type", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("Position", ImGuiTableColumnFlags.None, 196),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("Orientation", ImGuiTableColumnFlags.None, 196),
	};

	private static readonly List<EventColumn> _columnsSpiderEggSpawn = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Spawner Entity Id", ImGuiTableColumnFlags.None, 196),
		new("Position", ImGuiTableColumnFlags.None, 196),
		new("Target Position", ImGuiTableColumnFlags.None, 196),
	};

	private static readonly List<EventColumn> _columnsSpiderSpawn = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Type", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("Position", ImGuiTableColumnFlags.None, 196),
	};

	private static readonly List<EventColumn> _columnsSquidSpawn = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Type", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("Position", ImGuiTableColumnFlags.None, 196),
		new("Direction", ImGuiTableColumnFlags.None, 196),
		new("Rotation", ImGuiTableColumnFlags.None, 196),
	};

	private static readonly List<EventColumn> _columnsThornSpawn = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Type", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("Position", ImGuiTableColumnFlags.None, 196),
	};

	private static readonly List<EventColumn> _columnsDaggerSpawn = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Type", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("Position", ImGuiTableColumnFlags.None, 196),
		new("Orientation", ImGuiTableColumnFlags.None, 196),
		new("Shot / Rapid", ImGuiTableColumnFlags.None, 128),
	};

	private static readonly List<EventColumn> _columnsEntityOrientation = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Orientation", ImGuiTableColumnFlags.None, 196),
	};

	private static readonly List<EventColumn> _columnsEntityPosition = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Position", ImGuiTableColumnFlags.None, 196),
	};

	private static readonly List<EventColumn> _columnsEntityTarget = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Target Position", ImGuiTableColumnFlags.None, 196),
	};

	private static readonly List<EventColumn> _columnsGem = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
	};

	private static readonly List<EventColumn> _columnsHit = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id A", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Entity Id B", ImGuiTableColumnFlags.WidthFixed, 160),
		new("User Data", ImGuiTableColumnFlags.WidthFixed, 128),
	};

	private static readonly List<EventColumn> _columnsTransmute = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
	};

	private static readonly List<EventColumn> _columnsDeath = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Death Type", ImGuiTableColumnFlags.WidthFixed, 160),
	};

	private static readonly List<EventColumn> _columnsEnd = new()
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
	};

	private static readonly Dictionary<EventType, bool> _eventTypeEnabled = Enum.GetValues<EventType>().ToDictionary(et => et, _ => true);

	private static int _startTick;
	private static float _targetTime;

	private static bool _showEvents = true;
	private static bool _showTicksWithoutEvents = true;

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

		ImGui.Text("Go to:");
		ImGui.PushItemWidth(120);
		if (ImGui.InputFloat("##target_time", ref _targetTime))
			_startTick = TimeUtils.TimeToTick(_targetTime, startTime);

		ImGui.PopItemWidth();

		_startTick = Math.Max(0, Math.Min(_startTick, eventsData.TickCount - maxTicks));

		int endTick = Math.Min(_startTick + maxTicks - 1, eventsData.TickCount);
		ImGui.Text(UnsafeSpan.Get($"Showing {_startTick} - {endTick} of {eventsData.TickCount} ticks\n{TimeUtils.TickToTime(_startTick, startTime):0.0000} - {TimeUtils.TickToTime(endTick, startTime):0.0000}"));

		ImGui.Checkbox("Show events", ref _showEvents);
		ImGui.SameLine();
		ImGui.Checkbox("Show ticks without events", ref _showTicksWithoutEvents);

		ImGui.Separator();

		ImGui.BeginDisabled(!_showEvents);
		for (int i = 0; i < EnumUtils.EventTypes.Count; i++)
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

		if (ImGui.BeginChild("ReplayEventsChild", new(0, 0)))
		{
			if (ImGui.BeginTable("ReplayEventsTable", 3, ImGuiTableFlags.BordersInnerH))
			{
				ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.None, 56);
				ImGui.TableSetupColumn("Inputs", ImGuiTableColumnFlags.None, 128);
				ImGui.TableSetupColumn("Events", ImGuiTableColumnFlags.None, 384);
				ImGui.TableHeadersRow();

				for (int i = _startTick; i < Math.Min(_startTick + maxTicks, eventsData.TickCount); i++)
				{
					int offset = eventsData.EventOffsetsPerTick[i];
					int count = eventsData.EventOffsetsPerTick[i + 1] - offset;

					IInputsEvent? inputsEvent = null;
					_eventCache.Clear();
					for (int j = offset; j < offset + count; j++)
					{
						IEvent @event = eventsData.Events[j];
						if (@event is IInputsEvent ie)
							inputsEvent = ie;
						else
							_eventCache.Add(j, @event);
					}

					if (!_showTicksWithoutEvents && _eventCache.Count == 0)
						continue;

					ImGui.TableNextRow();

					ImGui.TableNextColumn();
					ImGui.Text(UnsafeSpan.Get($"{TimeUtils.TickToTime(i, startTime):0.0000} ({i})"));
					ImGui.TableNextColumn();
					if (inputsEvent != null)
						RenderInputsEvent(inputsEvent);
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
					RenderEvents<BoidSpawnEvent, BoidSpawnEvents>(EventType.BoidSpawn, _eventCache.BoidSpawnEvents, eventsData.EntityTypes, _columnsBoidSpawn);
					RenderEvents<LeviathanSpawnEvent, LeviathanSpawnEvents>(EventType.LeviathanSpawn, _eventCache.LeviathanSpawnEvents, eventsData.EntityTypes, _columnsLeviathanSpawn);
					RenderEvents<PedeSpawnEvent, PedeSpawnEvents>(EventType.PedeSpawn, _eventCache.PedeSpawnEvents, eventsData.EntityTypes, _columnsPedeSpawn);
					RenderEvents<SpiderEggSpawnEvent, SpiderEggSpawnEvents>(EventType.SpiderEggSpawn, _eventCache.SpiderEggSpawnEvents, eventsData.EntityTypes, _columnsSpiderEggSpawn);
					RenderEvents<SpiderSpawnEvent, SpiderSpawnEvents>(EventType.SpiderSpawn, _eventCache.SpiderSpawnEvents, eventsData.EntityTypes, _columnsSpiderSpawn);
					RenderEvents<SquidSpawnEvent, SquidSpawnEvents>(EventType.SquidSpawn, _eventCache.SquidSpawnEvents, eventsData.EntityTypes, _columnsSquidSpawn);
					RenderEvents<ThornSpawnEvent, ThornSpawnEvents>(EventType.ThornSpawn, _eventCache.ThornSpawnEvents, eventsData.EntityTypes, _columnsThornSpawn);

					// Other events
					RenderEvents<DaggerSpawnEvent, DaggerSpawnEvents>(EventType.DaggerSpawn, _eventCache.DaggerSpawnEvents, eventsData.EntityTypes, _columnsDaggerSpawn);
					RenderEvents<EntityOrientationEvent, EntityOrientationEvents>(EventType.EntityOrientation, _eventCache.EntityOrientationEvents, eventsData.EntityTypes, _columnsEntityOrientation);
					RenderEvents<EntityPositionEvent, EntityPositionEvents>(EventType.EntityPosition, _eventCache.EntityPositionEvents, eventsData.EntityTypes, _columnsEntityPosition);
					RenderEvents<EntityTargetEvent, EntityTargetEvents>(EventType.EntityTarget, _eventCache.EntityTargetEvents, eventsData.EntityTypes, _columnsEntityTarget);
					RenderEvents<GemEvent, GemEvents>(EventType.Gem, _eventCache.GemEvents, eventsData.EntityTypes, _columnsGem);
					RenderEvents<HitEvent, HitEvents>(EventType.Hit, _eventCache.HitEvents, eventsData.EntityTypes, _columnsHit);
					RenderEvents<TransmuteEvent, TransmuteEvents>(EventType.Transmute, _eventCache.TransmuteEvents, eventsData.EntityTypes, _columnsTransmute);

					// Final events
					RenderEvent<DeathEvent, DeathEvents>(_eventCache.DeathEvents, eventsData.EntityTypes, _columnsDeath);
					RenderEvent<EndEvent, EndEvents>(_eventCache.EndEvents, eventsData.EntityTypes, _columnsEnd);
				}

				ImGui.EndTable();
			}
		}

		ImGui.EndChild(); // ReplayEventsChild
	}

	private static void RenderInputsEvent(IInputsEvent ie)
	{
		ImGui.TextColored(ie.Forward ? Color.Red : Color.White, "W");
		ImGui.SameLine();
		ImGui.TextColored(ie.Left ? Color.Red : Color.White, "A");
		ImGui.SameLine();
		ImGui.TextColored(ie.Backward ? Color.Red : Color.White, "S");
		ImGui.SameLine();
		ImGui.TextColored(ie.Right ? Color.Red : Color.White, "D");
		ImGui.SameLine();
		ImGui.TextColored(GetJumpTypeColor(ie.Jump), "[Space]");
		ImGui.SameLine();
		ImGui.TextColored(GetShootTypeColor(ie.Shoot), "[LMB]");
		ImGui.SameLine();
		ImGui.TextColored(GetShootTypeColor(ie.ShootHoming), "[RMB]");
		ImGui.SameLine();
		ImGui.TextColored(ie.MouseX == 0 ? Color.White : Color.Red, UnsafeSpan.Get($"X:{ie.MouseX}"));
		ImGui.SameLine();
		ImGui.TextColored(ie.MouseY == 0 ? Color.White : Color.Red, UnsafeSpan.Get($"Y:{ie.MouseY}"));

		if (ie is InitialInputsEvent initial)
			ImGui.TextColored(Color.White, UnsafeSpan.Get($"Look Speed: {initial.LookSpeed}"));

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
