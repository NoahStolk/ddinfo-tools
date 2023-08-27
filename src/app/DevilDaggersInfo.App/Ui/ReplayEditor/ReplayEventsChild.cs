using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Utils;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Events.Interfaces;
using DevilDaggersInfo.Core.Replay.Extensions;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor;

public static class ReplayEventsChild
{
	private static readonly EventCache _eventCache = new();

	private static readonly IReadOnlyDictionary<EventType, string> _eventTypeNames = new Dictionary<EventType, string>
	{
		[EventType.BoidSpawn] = "Boid Spawn events",
		[EventType.LeviathanSpawn] = "Leviathan Spawn events",
		[EventType.PedeSpawn] = "Pede Spawn events",
		[EventType.SpiderEggSpawn] = "Spider Egg Spawn events",
		[EventType.SpiderSpawn] = "Spider Spawn events",
		[EventType.SquidSpawn] = "Squid Spawn events",
		[EventType.ThornSpawn] = "Thorn Spawn events",
		[EventType.DaggerSpawn] = "Dagger Spawn events",
		[EventType.EntityOrientation] = "Entity Orientation events",
		[EventType.EntityPosition] = "Entity Position events",
		[EventType.EntityTarget] = "Entity Target events",
		[EventType.Gem] = "Gem events",
		[EventType.Hit] = "Hit events",
		[EventType.Transmute] = "Transmute events",
	};

	private static readonly Dictionary<EventType, bool> _eventTypeEnabled = Enum.GetValues<EventType>().ToDictionary(et => et, _ => true);

	private static int _startTick;

	private static bool _showEvents = true;
	private static bool _showTicksWithoutEvents = true;

	private enum EventType
	{
		BoidSpawn,
		LeviathanSpawn,
		PedeSpawn,
		SpiderEggSpawn,
		SpiderSpawn,
		SquidSpawn,
		ThornSpawn,
		DaggerSpawn,
		EntityOrientation,
		EntityPosition,
		EntityTarget,
		Gem,
		Hit,
		Transmute,
	}

	private static ImGuiTableFlags EventTableFlags => ImGuiTableFlags.Borders | ImGuiTableFlags.NoPadOuterX;
	private static ImGuiTableColumnFlags EventTableColumnFlags => ImGuiTableColumnFlags.None;

	public static void Reset()
	{
		_startTick = 0;
	}

	private static void ToggleAll(bool enabled)
	{
		foreach (EventType eventType in Enum.GetValues<EventType>())
			_eventTypeEnabled[eventType] = enabled;
	}

	public static void Render(ReplayEventsData eventsData)
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

		ImGui.Text(UnsafeSpan.Get($"Showing {_startTick} - {_startTick + maxTicks - 1} of {eventsData.TickCount}"));

		ImGui.Checkbox("Show events", ref _showEvents);
		ImGui.SameLine();
		ImGui.Checkbox("Show ticks without events", ref _showTicksWithoutEvents);

		ImGui.Separator();

		ImGui.BeginDisabled(!_showEvents);
		foreach (EventType eventType in Enum.GetValues<EventType>())
		{
			bool temp = _eventTypeEnabled[eventType];
			if (ImGui.Checkbox(_eventTypeNames[eventType], ref temp))
				_eventTypeEnabled[eventType] = temp;
		}

		ImGui.EndDisabled();

		ImGui.Separator();

		if (ImGui.Button("Enable all"))
			ToggleAll(true);

		ImGui.SameLine();

		if (ImGui.Button("Disable all"))
			ToggleAll(false);

		Color rowOdd = Color.Gray(0.1f);
		Color rowEven = Color.Gray(0.05f);
		if (ImGui.BeginTable("ReplayEventsTable", 4, ImGuiTableFlags.None))
		{
			ImGui.TableSetupColumn("Tick", ImGuiTableColumnFlags.None, 32);
			ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.None, 32);
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

				ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, ImGui.GetColorU32(i % 2 == 0 ? rowEven : rowOdd));

				ImGui.TableNextColumn();
				ImGui.Text(UnsafeSpan.Get(i));
				ImGui.TableNextColumn();
				ImGui.Text(UnsafeSpan.Get(i / 60f, StringFormats.TimeFormat));
				ImGui.TableNextColumn();
				if (inputsEvent != null)
					RenderInputsEvent(inputsEvent);
				else
					ImGui.Text("ERROR: no inputs");

				ImGui.TableNextColumn();

				if (!_showEvents)
					continue;

				// Enemy spawn events
				if (_eventTypeEnabled[EventType.BoidSpawn] && _eventCache.BoidSpawnEvents.Count > 0)
					RenderBoidSpawnEvents(_eventCache.BoidSpawnEvents, eventsData.EntityTypes);

				if (_eventTypeEnabled[EventType.LeviathanSpawn] && _eventCache.LeviathanSpawnEvents.Count > 0)
					RenderLeviathanSpawnEvents(_eventCache.LeviathanSpawnEvents, eventsData.EntityTypes);

				if (_eventTypeEnabled[EventType.PedeSpawn] && _eventCache.PedeSpawnEvents.Count > 0)
					RenderPedeSpawnEvents(_eventCache.PedeSpawnEvents, eventsData.EntityTypes);

				if (_eventTypeEnabled[EventType.SpiderEggSpawn] && _eventCache.SpiderEggSpawnEvents.Count > 0)
					RenderSpiderEggSpawnEvents(_eventCache.SpiderEggSpawnEvents, eventsData.EntityTypes);

				if (_eventTypeEnabled[EventType.SpiderSpawn] && _eventCache.SpiderSpawnEvents.Count > 0)
					RenderSpiderSpawnEvents(_eventCache.SpiderSpawnEvents, eventsData.EntityTypes);

				if (_eventTypeEnabled[EventType.SquidSpawn] && _eventCache.SquidSpawnEvents.Count > 0)
					RenderSquidSpawnEvents(_eventCache.SquidSpawnEvents, eventsData.EntityTypes);

				if (_eventTypeEnabled[EventType.ThornSpawn] && _eventCache.ThornSpawnEvents.Count > 0)
					RenderThornSpawnEvents(_eventCache.ThornSpawnEvents, eventsData.EntityTypes);

				// Other events
				if (_eventTypeEnabled[EventType.DaggerSpawn] && _eventCache.DaggerSpawnEvents.Count > 0)
					RenderDaggerSpawnEvents(_eventCache.DaggerSpawnEvents, eventsData.EntityTypes);

				if (_eventTypeEnabled[EventType.EntityOrientation] && _eventCache.EntityOrientationEvents.Count > 0)
					RenderEntityOrientationEvents(_eventCache.EntityOrientationEvents, eventsData.EntityTypes);

				if (_eventTypeEnabled[EventType.EntityPosition] && _eventCache.EntityPositionEvents.Count > 0)
					RenderEntityPositionEvents(_eventCache.EntityPositionEvents, eventsData.EntityTypes);

				if (_eventTypeEnabled[EventType.EntityTarget] && _eventCache.EntityTargetEvents.Count > 0)
					RenderEntityTargetEvents(_eventCache.EntityTargetEvents, eventsData.EntityTypes);

				if (_eventTypeEnabled[EventType.Gem] && _eventCache.GemEvents.Count > 0)
					RenderGemEvents(_eventCache.GemEvents);

				if (_eventTypeEnabled[EventType.Hit] && _eventCache.HitEvents.Count > 0)
					RenderHitEvents(_eventCache.HitEvents, eventsData.EntityTypes);

				if (_eventTypeEnabled[EventType.Transmute] && _eventCache.TransmuteEvents.Count > 0)
					RenderTransmuteEvents(_eventCache.TransmuteEvents, eventsData.EntityTypes);

				// Final events
				if (_eventCache.DeathEvents.Count > 0)
					RenderDeathEvents(_eventCache.DeathEvents);

				if (_eventCache.EndEvents.Count > 0)
					RenderEndEvents(_eventCache.EndEvents);
			}

			ImGui.EndTable();
		}
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
		ImGui.TextColored(ie.MouseX == 0 ? Color.White : Color.Red, UnsafeSpan.Get($"X: {ie.MouseX}"));
		ImGui.SameLine();
		ImGui.TextColored(ie.MouseY == 0 ? Color.White : Color.Red, UnsafeSpan.Get($"X: {ie.MouseY}"));

		if (ie is InitialInputsEvent initial)
			ImGui.TextColored(Color.White, UnsafeSpan.Get($"Look Speed: {initial.LookSpeed}"));
	}

	#region Enemies

	private static void RenderBoidSpawnEvents(IReadOnlyList<(int Index, BoidSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes)
	{
		ImGui.TextColored(EnemiesV3_2.Skull4.Color, _eventTypeNames[EventType.BoidSpawn]);

		if (ImGui.BeginTable(_eventTypeNames[EventType.BoidSpawn], 10, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Spawner Entity Id", EventTableColumnFlags, 196);
			ImGui.TableSetupColumn("Type", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Position", EventTableColumnFlags, 196);
			ImGui.TableSetupColumn("?", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("?", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("?", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("?", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Speed", EventTableColumnFlags, 128);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, BoidSpawnEvent e) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
				EntityColumn(entityTypes, e.EntityId);
				EntityColumn(entityTypes, e.SpawnerEntityId);
				NextColumnText(GetBoidTypeText(e.BoidType));
				NextColumnText(UnsafeSpan.Get(e.Position));
				NextColumnText(UnsafeSpan.Get(e.A));
				NextColumnText(UnsafeSpan.Get(e.B));
				NextColumnText(UnsafeSpan.Get(e.C));
				NextColumnText(UnsafeSpan.Get(e.D));
				NextColumnText(UnsafeSpan.Get(e.Speed, "0.00"));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderLeviathanSpawnEvents(IReadOnlyList<(int Index, LeviathanSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes)
	{
		ImGui.TextColored(EnemiesV3_2.Leviathan.Color, _eventTypeNames[EventType.LeviathanSpawn]);

		if (ImGui.BeginTable(_eventTypeNames[EventType.LeviathanSpawn], 3, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("?", EventTableColumnFlags, 128);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, LeviathanSpawnEvent e) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
				EntityColumn(entityTypes, e.EntityId);
				NextColumnText(UnsafeSpan.Get(e.A));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderPedeSpawnEvents(IReadOnlyList<(int Index, PedeSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes)
	{
		ImGui.TextColored(EnemiesV3_2.Gigapede.Color, _eventTypeNames[EventType.PedeSpawn]);

		if (ImGui.BeginTable(_eventTypeNames[EventType.PedeSpawn], 7, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Type", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("?", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Position", EventTableColumnFlags, 196);
			ImGui.TableSetupColumn("?", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Orientation", EventTableColumnFlags, 196);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, PedeSpawnEvent e) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
				EntityColumn(entityTypes, e.EntityId);
				NextColumnText(GetPedeTypeText(e.PedeType));
				NextColumnText(UnsafeSpan.Get(e.A));
				NextColumnText(UnsafeSpan.Get(e.Position, "0.00"));
				NextColumnText(UnsafeSpan.Get(e.B));
				NextColumnText(UnsafeSpan.Get(e.Orientation));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderSpiderEggSpawnEvents(IReadOnlyList<(int Index, SpiderEggSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes)
	{
		ImGui.TextColored(EnemiesV3_2.SpiderEgg1.Color, _eventTypeNames[EventType.SpiderEggSpawn]);

		if (ImGui.BeginTable(_eventTypeNames[EventType.SpiderEggSpawn], 5, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Spawner Entity Id", EventTableColumnFlags, 196);
			ImGui.TableSetupColumn("Position", EventTableColumnFlags, 196);
			ImGui.TableSetupColumn("Target Position", EventTableColumnFlags, 196);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, SpiderEggSpawnEvent e) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
				EntityColumn(entityTypes, e.EntityId);
				EntityColumn(entityTypes, e.SpawnerEntityId);
				NextColumnText(UnsafeSpan.Get(e.Position, "0.00"));
				NextColumnText(UnsafeSpan.Get(e.TargetPosition, "0.00"));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderSpiderSpawnEvents(IReadOnlyList<(int Index, SpiderSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes)
	{
		ImGui.TextColored(EnemiesV3_2.Spider2.Color, _eventTypeNames[EventType.SpiderSpawn]);

		if (ImGui.BeginTable(_eventTypeNames[EventType.SpiderSpawn], 5, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Type", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("?", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Position", EventTableColumnFlags, 196);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, SpiderSpawnEvent e) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
				EntityColumn(entityTypes, e.EntityId);
				NextColumnText(GetSpiderTypeText(e.SpiderType));
				NextColumnText(UnsafeSpan.Get(e.A));
				NextColumnText(UnsafeSpan.Get(e.Position, "0.00"));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderSquidSpawnEvents(IReadOnlyList<(int Index, SquidSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes)
	{
		ImGui.TextColored(EnemiesV3_2.Squid3.Color, _eventTypeNames[EventType.SquidSpawn]);

		if (ImGui.BeginTable(_eventTypeNames[EventType.SquidSpawn], 6, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Type", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Position", EventTableColumnFlags, 192);
			ImGui.TableSetupColumn("Direction", EventTableColumnFlags, 192);
			ImGui.TableSetupColumn("Rotation", EventTableColumnFlags, 128);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, SquidSpawnEvent e) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
				EntityColumn(entityTypes, e.EntityId);
				NextColumnText(GetSquidTypeText(e.SquidType));
				NextColumnText(UnsafeSpan.Get(e.Position, "0.00"));
				NextColumnText(UnsafeSpan.Get(e.Direction, "0.00"));
				NextColumnText(UnsafeSpan.Get(e.RotationInRadians, "0.00"));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderThornSpawnEvents(IReadOnlyList<(int Index, ThornSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes)
	{
		ImGui.TextColored(EnemiesV3_2.Thorn.Color, _eventTypeNames[EventType.ThornSpawn]);

		if (ImGui.BeginTable(_eventTypeNames[EventType.ThornSpawn], 5, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("?", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Position", EventTableColumnFlags, 192);
			ImGui.TableSetupColumn("Rotation", EventTableColumnFlags, 128);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, ThornSpawnEvent e) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
				EntityColumn(entityTypes, e.EntityId);
				NextColumnText(UnsafeSpan.Get(e.A));
				NextColumnText(UnsafeSpan.Get(e.Position, "0.00"));
				NextColumnText(UnsafeSpan.Get(e.RotationInRadians, "0.00"));
			}

			ImGui.EndTable();
		}
	}

	#endregion Enemies

	private static void RenderDaggerSpawnEvents(IReadOnlyList<(int Index, DaggerSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes)
	{
		ImGui.TextColored(Color.Purple, _eventTypeNames[EventType.DaggerSpawn]);

		if (ImGui.BeginTable(_eventTypeNames[EventType.DaggerSpawn], 7, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id", EventTableColumnFlags, 192);
			ImGui.TableSetupColumn("Type", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("?", EventTableColumnFlags, 32);
			ImGui.TableSetupColumn("Position", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Orientation", EventTableColumnFlags, 192);
			ImGui.TableSetupColumn("Shot / Rapid", EventTableColumnFlags, 96);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, DaggerSpawnEvent e) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
				EntityColumn(entityTypes, e.EntityId);
				NextColumnText(EnumUtils.DaggerTypeNames[e.DaggerType]);
				NextColumnText(UnsafeSpan.Get(e.A));
				NextColumnText(UnsafeSpan.Get(e.Position));
				NextColumnText(UnsafeSpan.Get(e.Orientation));
				NextColumnText(e.IsShot ? "Shot" : "Rapid");
			}

			ImGui.EndTable();
		}
	}

	private static void RenderEntityOrientationEvents(IReadOnlyList<(int Index, EntityOrientationEvent Event)> events, IReadOnlyList<EntityType> entityTypes)
	{
		ImGui.TextColored(Color.Yellow, _eventTypeNames[EventType.EntityOrientation]);

		if (ImGui.BeginTable(_eventTypeNames[EventType.EntityOrientation], 3, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Orientation", EventTableColumnFlags, 192);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, EntityOrientationEvent e) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
				EntityColumn(entityTypes, e.EntityId);
				NextColumnText(UnsafeSpan.Get(e.Orientation));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderEntityPositionEvents(IReadOnlyList<(int Index, EntityPositionEvent Event)> events, IReadOnlyList<EntityType> entityTypes)
	{
		ImGui.TextColored(Color.Yellow, _eventTypeNames[EventType.EntityPosition]);

		if (ImGui.BeginTable(_eventTypeNames[EventType.EntityPosition], 3, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Position", EventTableColumnFlags, 192);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, EntityPositionEvent e) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
				EntityColumn(entityTypes, e.EntityId);
				NextColumnText(UnsafeSpan.Get(e.Position));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderEntityTargetEvents(IReadOnlyList<(int Index, EntityTargetEvent Event)> events, IReadOnlyList<EntityType> entityTypes)
	{
		ImGui.TextColored(Color.Yellow, _eventTypeNames[EventType.EntityTarget]);

		if (ImGui.BeginTable(_eventTypeNames[EventType.EntityTarget], 3, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Target Position", EventTableColumnFlags, 192);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, EntityTargetEvent e) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
				EntityColumn(entityTypes, e.EntityId);
				NextColumnText(UnsafeSpan.Get(e.TargetPosition));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderGemEvents(IReadOnlyList<(int Index, GemEvent Event)> events)
	{
		ImGui.TextColored(Color.Yellow, _eventTypeNames[EventType.Gem]);

		if (ImGui.BeginTable(_eventTypeNames[EventType.Gem], 1, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, _) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderHitEvents(IReadOnlyList<(int Index, HitEvent Event)> events, IReadOnlyList<EntityType> entityTypes)
	{
		ImGui.TextColored(Color.Orange, _eventTypeNames[EventType.Hit]);

		if (ImGui.BeginTable(_eventTypeNames[EventType.Hit], 4, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id A", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Entity Id B", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("User Data", EventTableColumnFlags, 128);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, HitEvent e) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
				EntityColumn(entityTypes, e.EntityIdA);
				EntityColumn(entityTypes, e.EntityIdB);
				NextColumnText(UnsafeSpan.Get(e.UserData));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderTransmuteEvents(IReadOnlyList<(int Index, TransmuteEvent Event)> events, IReadOnlyList<EntityType> entityTypes)
	{
		ImGui.TextColored(Color.Yellow, _eventTypeNames[EventType.Transmute]);

		if (ImGui.BeginTable(_eventTypeNames[EventType.Transmute], 6, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("?", EventTableColumnFlags, 192);
			ImGui.TableSetupColumn("?", EventTableColumnFlags, 192);
			ImGui.TableSetupColumn("?", EventTableColumnFlags, 192);
			ImGui.TableSetupColumn("?", EventTableColumnFlags, 192);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, TransmuteEvent e) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
				EntityColumn(entityTypes, e.EntityId);
				NextColumnText(UnsafeSpan.Get(e.A));
				NextColumnText(UnsafeSpan.Get(e.B));
				NextColumnText(UnsafeSpan.Get(e.C));
				NextColumnText(UnsafeSpan.Get(e.D));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderDeathEvents(IReadOnlyList<(int Index, DeathEvent Event)> events)
	{
		ImGui.TextColored(Color.Red, "Death");

		if (ImGui.BeginTable("Death", 2, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Death Type", EventTableColumnFlags, 192);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, DeathEvent e) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
				NextColumnText(UnsafeSpan.Get(Deaths.GetDeathByType(GameConstants.CurrentVersion, (byte)e.DeathType)?.Name ?? "???"));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderEndEvents(IReadOnlyList<(int Index, EndEvent Event)> events)
	{
		ImGui.TextColored(Color.Red, "End");

		if (ImGui.BeginTable("End", 1, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, _) = events[i];
				NextColumnText(UnsafeSpan.Get(index));
			}

			ImGui.EndTable();
		}
	}

	private static void NextColumnText(ReadOnlySpan<char> text)
	{
		ImGui.TableNextColumn();
		ImGui.Text(text);
	}

	private static void EntityColumn(IReadOnlyList<EntityType> entityTypes, int entityId)
	{
		EntityType? entityType = entityId >= 0 && entityId < entityTypes.Count ? entityTypes[entityId] : null;

		ImGui.TableNextColumn();
		ImGui.Text(UnsafeSpan.Get(entityId));
		ImGui.SameLine();
		ImGui.Text("(");
		ImGui.SameLine();
		ImGui.TextColored(entityType.GetColor(), entityType.HasValue ? EnumUtils.EntityTypeNames[entityType.Value] : "???");
		ImGui.SameLine();
		ImGui.Text(")");
	}

	private static ReadOnlySpan<char> GetBoidTypeText(BoidType boidType) => boidType switch
	{
		BoidType.Skull1 => "Skull1",
		BoidType.Skull2 => "Skull2",
		BoidType.Skull3 => "Skull3",
		BoidType.Skull4 => "Skull4",
		BoidType.Spiderling => "Spiderling",
		_ => throw new UnreachableException(),
	};

	private static ReadOnlySpan<char> GetPedeTypeText(PedeType pedeType) => pedeType switch
	{
		PedeType.Centipede => "Centipede",
		PedeType.Gigapede => "Gigapede",
		PedeType.Ghostpede => "Ghostpede",
		_ => throw new UnreachableException(),
	};

	private static ReadOnlySpan<char> GetSpiderTypeText(SpiderType spiderType) => spiderType switch
	{
		SpiderType.Spider1 => "Spider1",
		SpiderType.Spider2 => "Spider2",
		_ => throw new UnreachableException(),
	};

	private static ReadOnlySpan<char> GetSquidTypeText(SquidType squidType) => squidType switch
	{
		SquidType.Squid1 => "Squid1",
		SquidType.Squid2 => "Squid2",
		SquidType.Squid3 => "Squid3",
		_ => throw new UnreachableException(),
	};

	private static Color GetJumpTypeColor(JumpType jumpType) => jumpType switch
	{
		JumpType.Hold => Color.Orange,
		JumpType.StartedPress => Color.Red,
		_ => Color.White,
	};

	private static Color GetShootTypeColor(ShootType shootType) => shootType switch
	{
		ShootType.Hold => Color.Orange,
		ShootType.Release => Color.Red,
		_ => Color.White,
	};
}
