using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Events.Interfaces;
using DevilDaggersInfo.Core.Replay.Extensions;
using DevilDaggersInfo.Core.Replay.Numerics;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using ImGuiNET;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public static class EventTypeRendererUtils
{
	public static readonly IReadOnlyDictionary<EventType, string> EventTypeNames = new Dictionary<EventType, string>
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
		[EventType.InitialInputs] = "Initial Inputs events",
		[EventType.Inputs] = "Inputs events",
		[EventType.Death] = "Death events",
		[EventType.End] = "End events",
	};

	private static readonly IReadOnlyDictionary<EntityType, string> _entityTypeShortNames = new Dictionary<EntityType, string>
	{
		[EntityType.Level1Dagger] = "Lvl1",
		[EntityType.Level2Dagger] = "Lvl2",
		[EntityType.Level3Dagger] = "Lvl3",
		[EntityType.Level3HomingDagger] = "Lvl3 Homing",
		[EntityType.Level4Dagger] = "Lvl4",
		[EntityType.Level4HomingDagger] = "Lvl4 Homing",
		[EntityType.Level4HomingSplash] = "Lvl4 Splash",
		[EntityType.Squid1] = "Squid I",
		[EntityType.Squid2] = "Squid II",
		[EntityType.Squid3] = "Squid III",
		[EntityType.Skull1] = "Skull I",
		[EntityType.Skull2] = "Skull II",
		[EntityType.Skull3] = "Skull III",
		[EntityType.Spiderling] = "Spiderling",
		[EntityType.Skull4] = "Skull IV",
		[EntityType.Centipede] = "Centipede",
		[EntityType.Gigapede] = "Gigapede",
		[EntityType.Ghostpede] = "Ghostpede",
		[EntityType.Spider1] = "Spider I",
		[EntityType.Spider2] = "Spider II",
		[EntityType.SpiderEgg] = "Spider Egg",
		[EntityType.Leviathan] = "Leviathan",
		[EntityType.Thorn] = "Thorn",
		[EntityType.Zero] = "Zero",
	};

	private static ImGuiTableFlags EventTableFlags => ImGuiTableFlags.Borders | ImGuiTableFlags.NoPadOuterX;

	private static void SetupColumns(IReadOnlyList<EventColumn> columns)
	{
		for (int i = 0; i < columns.Count; i++)
		{
			EventColumn column = columns[i];
			ImGui.TableSetupColumn(column.Name, column.Flags, column.Width);
		}

		ImGui.TableHeadersRow();
	}

	public static void RenderTable<TEvent, TRenderer>(Vector4 color, EventType eventType, IReadOnlyList<(int Index, TEvent Event)> events, IReadOnlyList<EntityType> entityTypes)
		where TEvent : IEvent
		where TRenderer : IEventTypeRenderer<TEvent>
	{
		ImGui.TextColored(color, EventTypeNames[eventType]);

		if (ImGui.BeginTable(EventTypeNames[eventType], TRenderer.EventColumns.Count, EventTableFlags))
		{
			SetupColumns(TRenderer.EventColumns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, TEvent e) = events[i];
				TRenderer.Render(index, e, entityTypes);
			}

			ImGui.EndTable();
		}
	}

	public static void NextColumnActions(int index)
	{
		ImGui.TableNextColumn();

		ImGui.PushID(Inline.Span($"delete_event_{index}"));
		ImGui.PushStyleColor(ImGuiCol.Button, Color.Red with { A = 159 });
		ImGui.PushStyleColor(ImGuiCol.ButtonActive, Color.Red);
		ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Color.Red with { A = 223 });
		if (ImGui.SmallButton("DEL"))
			FileStates.Replay.Object.EventsData.RemoveEvent(index);
		ImGui.PopStyleColor(3);
		ImGui.PopID();

		if (ImGui.IsItemHovered())
			ImGui.SetTooltip(Inline.Span($"Delete event at index {index}"));

		ImGui.SameLine();
		if (ImGui.SmallButton($"INS##{index}"))
			ImGui.OpenPopup(Inline.Span($"Insert event at index {index}"));

		if (ImGui.IsItemHovered())
			ImGui.SetTooltip(Inline.Span($"Insert event at index {index}"));

		ImGui.SetNextWindowSize(new(512, 512), ImGuiCond.Appearing);
		if (ImGui.BeginPopupModal(Inline.Span($"Insert event at index {index}")))
		{
			IEvent? e = null;
			if (ImGui.Button(nameof(BoidSpawnEvent)))
				e = new BoidSpawnEvent(-1, 1, BoidType.Skull1, Int16Vec3.Zero, Int16Mat3x3.Identity, Vector3.Zero, 0f);
			else if (ImGui.Button(nameof(DaggerSpawnEvent)))
				e = new DaggerSpawnEvent(-1, -1, Int16Vec3.Zero, Int16Mat3x3.Identity, false, DaggerType.Level1);
			else if (ImGui.Button(nameof(DeathEvent)))
				e = new DeathEvent(0);
			else if (ImGui.Button(nameof(EndEvent)))
				e = new EndEvent();
			else if (ImGui.Button(nameof(EntityOrientationEvent)))
				e = new EntityOrientationEvent(0, Int16Mat3x3.Identity);
			else if (ImGui.Button(nameof(EntityPositionEvent)))
				e = new EntityPositionEvent(0, Int16Vec3.Zero);
			else if (ImGui.Button(nameof(EntityTargetEvent)))
				e = new EntityTargetEvent(0, Int16Vec3.Zero);
			else if (ImGui.Button(nameof(GemEvent)))
				e = new GemEvent();
			else if (ImGui.Button(nameof(HitEvent)))
				e = new HitEvent(0, 0, 0);
			else if (ImGui.Button(nameof(InitialInputsEvent)))
				e = new InitialInputsEvent(false, false, false, false, JumpType.None, ShootType.None, ShootType.None, 0, 0, 2);
			else if (ImGui.Button(nameof(InputsEvent)))
				e = new InputsEvent(false, false, false, false, JumpType.None, ShootType.None, ShootType.None, 0, 0);
			else if (ImGui.Button(nameof(LeviathanSpawnEvent)))
				e = new LeviathanSpawnEvent(-1, -1);
			else if (ImGui.Button(nameof(PedeSpawnEvent)))
				e = new PedeSpawnEvent(-1, PedeType.Centipede, -1, Vector3.Zero, Vector3.Zero, Matrix3x3.Identity);
			else if (ImGui.Button(nameof(SpiderEggSpawnEvent)))
				e = new SpiderEggSpawnEvent(-1, 0, Vector3.Zero, Vector3.Zero);
			else if (ImGui.Button(nameof(SpiderSpawnEvent)))
				e = new SpiderSpawnEvent(-1, SpiderType.Spider1, -1, Vector3.Zero);
			else if (ImGui.Button(nameof(SquidSpawnEvent)))
				e = new SquidSpawnEvent(-1, SquidType.Squid1, -1, Vector3.Zero, Vector3.Zero, 0f);
			else if (ImGui.Button(nameof(ThornSpawnEvent)))
				e = new ThornSpawnEvent(-1, -1, Vector3.Zero, 0f);
			else if (ImGui.Button(nameof(TransmuteEvent)))
				e = new TransmuteEvent(-1, Int16Vec3.Zero, Int16Vec3.Zero, Int16Vec3.Zero, Int16Vec3.Zero);

			if (e != null)
			{
				FileStates.Replay.Object.EventsData.InsertEvent(index, e);
				ImGui.CloseCurrentPopup();
			}

			if (ImGui.Button("Cancel"))
				ImGui.CloseCurrentPopup();

			ImGui.EndPopup();
		}
	}

	public static void NextColumnEventIndex(int index)
	{
		ImGui.TableNextColumn();
		ImGui.Text(Inline.Span(index));
	}

	public static void NextColumnInputEnum<TEnum>(int eventIndex, ReadOnlySpan<char> fieldName, ref TEnum value, IReadOnlyList<TEnum> values, string[] names)
		where TEnum : Enum
	{
		ImGui.TableNextColumn();

		ImGui.PushItemWidth(-1);
		int intValue = Convert.ToInt32(value);

		int index = 0;
		for (int i = 0; i < values.Count; i++)
		{
			// TODO: This allocates memory.
			if (Convert.ToInt32(values[i]) == intValue)
			{
				index = i;
				break;
			}
		}

		if (ImGui.Combo(EditLabel(fieldName, eventIndex), ref index, names, values.Count))
			value = values[index];

		ImGui.PopItemWidth();
	}

	public static void NextColumnInputInt(int eventIndex, ReadOnlySpan<char> fieldName, ref int value)
	{
		ImGui.TableNextColumn();

		ImGui.PushItemWidth(-1);
		ImGui.InputInt(EditLabel(fieldName, eventIndex), ref value, 0, 0);
		ImGui.PopItemWidth();
	}

	public static unsafe void NextColumnInputShort(int eventIndex, ReadOnlySpan<char> fieldName, ref short value)
	{
		ImGui.TableNextColumn();

		ImGui.PushItemWidth(-1);
		ImGui.InputScalar(EditLabel(fieldName, eventIndex), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value), 0, 0);
		ImGui.PopItemWidth();
	}

	public static void NextColumnInputFloat(int eventIndex, ReadOnlySpan<char> fieldName, ref float value, ReadOnlySpan<char> format = default)
	{
		ImGui.TableNextColumn();

		ImGui.PushItemWidth(-1);
		ImGui.InputFloat(EditLabel(fieldName, eventIndex), ref value, 0, 0, format);
		ImGui.PopItemWidth();
	}

	public static void NextColumnInputVector3(int eventIndex, ReadOnlySpan<char> fieldName, ref Vector3 value, ReadOnlySpan<char> format = default)
	{
		ImGui.TableNextColumn();

		ImGui.PushItemWidth(-1);
		ImGui.InputFloat3(EditLabel(fieldName, eventIndex), ref value, format);
		ImGui.PopItemWidth();
	}

	public static unsafe void NextColumnInputInt16Vec3(int eventIndex, ReadOnlySpan<char> fieldName, ref Int16Vec3 value)
	{
		ImGui.TableNextColumn();

		ImGui.PushItemWidth(-1);
		ImGui.InputScalarN(EditLabel(fieldName, eventIndex), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value), 3);
		ImGui.PopItemWidth();
	}

	public static unsafe void NextColumnInputMatrix3x3(int eventIndex, ReadOnlySpan<char> fieldName, ref Matrix3x3 value, ReadOnlySpan<char> format = default)
	{
		ImGui.TableNextColumn();

		ImGui.PushItemWidth(-1);
		ImGui.InputScalarN(EditLabel(fieldName, eventIndex), ImGuiDataType.Float, (IntPtr)Unsafe.AsPointer(ref value), 9, 0, 0, format);
		ImGui.PopItemWidth();
	}

	public static unsafe void NextColumnInputInt16Mat3x3(int eventIndex, ReadOnlySpan<char> fieldName, ref Int16Mat3x3 value)
	{
		ImGui.TableNextColumn();

		ImGui.PushItemWidth(-1);
		ImGui.InputScalarN(EditLabel(fieldName, eventIndex), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value), 9);
		ImGui.PopItemWidth();
	}

	public static void NextColumnCheckbox(int eventIndex, ReadOnlySpan<char> fieldName, ref bool value, ReadOnlySpan<char> trueText, ReadOnlySpan<char> falseText)
	{
		ImGui.TableNextColumn();
		ImGui.Checkbox(EditLabel(fieldName, eventIndex), ref value);
		ImGui.SameLine();
		ImGui.Text(value ? trueText : falseText);
	}

	public static void NextColumnEntityId(IReadOnlyList<EntityType> entityTypes, int entityId)
	{
		EntityType? entityType = entityId >= 0 && entityId < entityTypes.Count ? entityTypes[entityId] : null;

		ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);

		ImGui.TableNextColumn();
		ReadOnlySpan<char> label = Inline.Span(entityId);
		float labelWidth = ImGui.CalcTextSize(label).X;
		ImGui.Text(label);
		ImGui.SameLine();

		ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (40 - labelWidth));
		ImGui.Text(" (");
		ImGui.SameLine();
		ImGui.TextColored(entityType.GetColor(), entityType.HasValue ? _entityTypeShortNames[entityType.Value] : "???");
		ImGui.SameLine();
		ImGui.Text(")");

		ImGui.PopStyleVar();
	}

	public static void NextColumnEditableEntityId(int eventIndex, ReadOnlySpan<char> fieldName, IReadOnlyList<EntityType> entityTypes, ref int entityId)
	{
		EntityType? entityType = entityId >= 0 && entityId < entityTypes.Count ? entityTypes[entityId] : null;

		ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);

		ImGui.TableNextColumn();
		ImGui.PushItemWidth(40);
		ImGui.InputInt(EditLabel(fieldName, eventIndex), ref entityId, 0, 0);
		ImGui.PopItemWidth();
		ImGui.SameLine();

		ImGui.Text(" (");
		ImGui.SameLine();
		ImGui.TextColored(entityType.GetColor(), entityType.HasValue ? _entityTypeShortNames[entityType.Value] : "???");
		ImGui.SameLine();
		ImGui.Text(")");

		ImGui.PopStyleVar();
	}

	private static ReadOnlySpan<char> EditLabel(ReadOnlySpan<char> label, int index)
	{
		return Inline.Span($"##{label}{index}");
	}

	public static Vector4 GetEventTypeColor(EventType eventType)
	{
		return eventType switch
		{
			EventType.BoidSpawn => EnemiesV3_2.Skull4.Color,
			EventType.DaggerSpawn => Color.Purple,
			EventType.Death or EventType.End or EventType.Gem => Color.Red,
			EventType.EntityOrientation or EventType.EntityPosition or EventType.EntityTarget => Color.Yellow,
			EventType.Hit => Color.Orange,
			EventType.LeviathanSpawn => EnemiesV3_2.Leviathan.Color,
			EventType.PedeSpawn => EnemiesV3_2.Gigapede.Color,
			EventType.SpiderEggSpawn => EnemiesV3_2.SpiderEgg1.Color,
			EventType.SpiderSpawn => EnemiesV3_2.Spider2.Color,
			EventType.SquidSpawn => EnemiesV3_2.Squid3.Color,
			EventType.ThornSpawn => EnemiesV3_2.Thorn.Color,
			EventType.Transmute => new(0.75f, 0, 0, 1),
			_ => Color.White,
		};
	}
}
