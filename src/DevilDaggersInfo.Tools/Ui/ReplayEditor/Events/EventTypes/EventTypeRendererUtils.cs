// ReSharper disable InconsistentNaming
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Numerics;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Utils;
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
		[EventType.End] = "End events",
	};

	private static ImGuiTableFlags EventTableFlags => ImGuiTableFlags.Borders | ImGuiTableFlags.NoPadOuterX;

	public static void SetupColumnIndex()
	{
		ImGui.TableSetupColumn("Index", ImGuiTableColumnFlags.WidthFixed, 64);
	}

	public static void SetupColumnEntityId()
	{
		ImGui.TableSetupColumn("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160);
	}

	public static void RenderTable<TEvent, TRenderer>(EventType eventType, IReadOnlyList<(int EventIndex, int EntityId, TEvent Event)> events, EditorReplayModel replay)
		where TEvent : IEventData
		where TRenderer : IEventTypeRenderer<TEvent>
	{
		ImGui.TextColored(GetEventTypeColor(eventType), EventTypeNames[eventType]);

		if (ImGui.BeginTable(EventTypeNames[eventType], TRenderer.ColumnCount, EventTableFlags))
		{
			TRenderer.SetupColumns();

			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int eventIndex, int entityId, TEvent e) = events[i];
				TRenderer.Render(eventIndex, entityId, e, replay);
			}

			ImGui.EndTable();
		}
	}

	public static void RenderInputsTable<TEvent, TRenderer>(ReadOnlySpan<char> tableId, IReadOnlyList<(int EventIndex, int EntityId, TEvent Event)> events, EditorReplayModel replay)
		where TEvent : IEventData
		where TRenderer : IEventTypeRenderer<TEvent>
	{
		if (ImGui.BeginTable(tableId, TRenderer.ColumnCountData, EventTableFlags))
		{
			TRenderer.SetupColumnsData();

			ImGui.TableHeadersRow();

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int eventIndex, int _, TEvent e) = events[i];
				TRenderer.RenderData(eventIndex, e, replay);
			}

			ImGui.EndTable();
		}
	}

	public static void NextColumnEventIndex(int index)
	{
		ImGui.TableNextColumn();
		ImGui.Text(Inline.Span(index));
	}

	public static void NextColumnInputByteEnum<TEnum>(int uniqueId, ReadOnlySpan<char> fieldName, ref TEnum value, IReadOnlyList<TEnum> values, string[] names)
		where TEnum : Enum
	{
		ImGui.TableNextColumn();

		InputByteEnum(uniqueId, fieldName, ref value, values, names);
	}

	public static void InputByteEnum<TEnum>(int uniqueId, ReadOnlySpan<char> fieldName, ref TEnum value, IReadOnlyList<TEnum> values, string[] names)
		where TEnum : Enum
	{
		ImGui.PushItemWidth(-1);
		int intValue = (byte)(object)value;

		int index = 0;
		for (int i = 0; i < values.Count; i++)
		{
			if ((byte)(object)values[i] == intValue)
			{
				index = i;
				break;
			}
		}

		if (ImGui.Combo(EditLabel(fieldName, uniqueId), ref index, names, values.Count))
			value = values[index];

		ImGui.PopItemWidth();
	}

	public static void NextColumnInputInt(int uniqueId, ReadOnlySpan<char> fieldName, ref int value)
	{
		ImGui.TableNextColumn();

		InputInt(uniqueId, fieldName, ref value);
	}

	public static void InputInt(int uniqueId, ReadOnlySpan<char> fieldName, ref int value)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputInt(EditLabel(fieldName, uniqueId), ref value, 0, 0);
		ImGui.PopItemWidth();
	}

	public static void NextColumnInputShort(int uniqueId, ReadOnlySpan<char> fieldName, ref short value)
	{
		ImGui.TableNextColumn();

		InputShort(uniqueId, fieldName, ref value);
	}

	public static unsafe void InputShort(int uniqueId, ReadOnlySpan<char> fieldName, ref short value)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputScalar(EditLabel(fieldName, uniqueId), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value), 0, 0);
		ImGui.PopItemWidth();
	}

	public static void NextColumnInputFloat(int uniqueId, ReadOnlySpan<char> fieldName, ref float value, ReadOnlySpan<char> format = default)
	{
		ImGui.TableNextColumn();

		InputFloat(uniqueId, fieldName, ref value, format);
	}

	public static void InputFloat(int uniqueId, ReadOnlySpan<char> fieldName, ref float value, ReadOnlySpan<char> format = default)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputFloat(EditLabel(fieldName, uniqueId), ref value, 0, 0, format);
		ImGui.PopItemWidth();
	}

	public static void NextColumnInputVector3(int uniqueId, ReadOnlySpan<char> fieldName, ref Vector3 value, ReadOnlySpan<char> format = default)
	{
		ImGui.TableNextColumn();

		InputVector3(uniqueId, fieldName, ref value, format);
	}

	public static void InputVector3(int uniqueId, ReadOnlySpan<char> fieldName, ref Vector3 value, ReadOnlySpan<char> format = default)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputFloat3(EditLabel(fieldName, uniqueId), ref value, format);
		ImGui.PopItemWidth();
	}

	public static void NextColumnInputInt16Vec3(int uniqueId, ReadOnlySpan<char> fieldName, ref Int16Vec3 value)
	{
		ImGui.TableNextColumn();
		InputInt16Vec3(uniqueId, fieldName, ref value);
	}

	public static unsafe void InputInt16Vec3(int uniqueId, ReadOnlySpan<char> fieldName, ref Int16Vec3 value)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value), 3);
		ImGui.PopItemWidth();
	}

	public static void NextColumnInputMatrix3x3(int uniqueId, ReadOnlySpan<char> fieldName, ref Matrix3x3 value, ReadOnlySpan<char> format = default)
	{
		ImGui.TableNextColumn();

		InputMatrix3x3(uniqueId, fieldName, ref value, format);
	}

	public static unsafe void InputMatrix3x3(int uniqueId, ReadOnlySpan<char> fieldName, ref Matrix3x3 value, ReadOnlySpan<char> format = default)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.Float, (IntPtr)Unsafe.AsPointer(ref value), 9, 0, 0, format);
		ImGui.PopItemWidth();
	}

	public static unsafe void InputMatrix3x3Square(int uniqueId, ReadOnlySpan<char> fieldName, ref Matrix3x3 value, ReadOnlySpan<char> format = default)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.Float, (IntPtr)Unsafe.AsPointer(ref value.M11), 3, 0, 0, format);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.Float, (IntPtr)Unsafe.AsPointer(ref value.M21), 3, 0, 0, format);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.Float, (IntPtr)Unsafe.AsPointer(ref value.M31), 3, 0, 0, format);
		ImGui.PopItemWidth();
	}

	public static void NextColumnInputInt16Mat3x3(int uniqueId, ReadOnlySpan<char> fieldName, ref Int16Mat3x3 value)
	{
		ImGui.TableNextColumn();

		InputInt16Mat3x3(uniqueId, fieldName, ref value);
	}

	public static unsafe void InputInt16Mat3x3(int uniqueId, ReadOnlySpan<char> fieldName, ref Int16Mat3x3 value)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value), 9);
		ImGui.PopItemWidth();
	}

	public static unsafe void InputInt16Mat3x3Square(int uniqueId, ReadOnlySpan<char> fieldName, ref Int16Mat3x3 value)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value.M11), 3);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value.M21), 3);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value.M31), 3);
		ImGui.PopItemWidth();
	}

	public static void NextColumnCheckbox(int uniqueId, ReadOnlySpan<char> fieldName, ref bool value, ReadOnlySpan<char> trueText, ReadOnlySpan<char> falseText)
	{
		ImGui.TableNextColumn();
		Checkbox(uniqueId, fieldName, ref value, trueText, falseText);
	}

	public static void Checkbox(int uniqueId, ReadOnlySpan<char> fieldName, ref bool value, ReadOnlySpan<char> trueText, ReadOnlySpan<char> falseText)
	{
		ImGui.Checkbox(EditLabel(fieldName, uniqueId), ref value);
		ImGui.SameLine();
		ImGui.Text(value ? trueText : falseText);
	}

	public static void NextColumnEntityId(EditorReplayModel replay, int entityId)
	{
		EntityType? entityType = replay.GetEntityTypeIncludingNegated(entityId);

		ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);

		ImGui.TableNextColumn();
		ReadOnlySpan<char> label = Inline.Span(entityId);
		float labelWidth = ImGui.CalcTextSize(label).X;
		ImGui.Text(label);
		ImGui.SameLine();

		ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (50 - labelWidth));
		ImGui.Text(" (");
		ImGui.SameLine();
		ImGui.TextColored(entityType.GetColor(), entityType.HasValue ? EnumUtils.EntityTypeShortNames[entityType.Value] : "???");
		ImGui.SameLine();
		ImGui.Text(")");

		ImGui.PopStyleVar();
	}

	public static void NextColumnEditableEntityId(int uniqueId, ReadOnlySpan<char> fieldName, EditorReplayModel replay, ref int entityId)
	{
		ImGui.TableNextColumn();
		EditableEntityId(uniqueId, fieldName, replay, ref entityId);
	}

	public static void EditableEntityId(int uniqueId, ReadOnlySpan<char> fieldName, EditorReplayModel replay, ref int entityId)
	{
		ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);

		ImGui.PushItemWidth(50);
		ImGui.InputInt(EditLabel(fieldName, uniqueId), ref entityId, 0, 0);
		ImGui.PopItemWidth();
		ImGui.SameLine();

		ImGui.Text(" (");
		ImGui.SameLine();
		EntityType? entityType = replay.GetEntityTypeIncludingNegated(entityId);
		ImGui.TextColored(entityType.GetColor(), entityType.HasValue ? EnumUtils.EntityTypeShortNames[entityType.Value] : "???");
		ImGui.SameLine();
		ImGui.Text(")");

		ImGui.PopStyleVar();
	}

	private static ReadOnlySpan<char> EditLabel(ReadOnlySpan<char> label, int uniqueId)
	{
		return Inline.Span($"##{label}{uniqueId}");
	}

	public static Vector4 GetEventTypeColor(EventType eventType)
	{
		return eventType switch
		{
			EventType.BoidSpawn => EnemiesV3_2.Skull4.Color,
			EventType.DaggerSpawn => Color.Purple,
			EventType.End or EventType.Gem => Color.Red,
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
