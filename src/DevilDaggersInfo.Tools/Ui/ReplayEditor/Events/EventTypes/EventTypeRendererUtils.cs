// ReSharper disable InconsistentNaming
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Numerics;
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

	// Used in old editor.
	public static void RenderTable<TEvent, TRenderer>(EventType eventType, IReadOnlyList<(int EventIndex, int EntityId, TEvent Event)> events, EditorReplayModel replay)
		where TEvent : IEventData
		where TRenderer : IEventTypeRenderer<TEvent>
	{
		ImGui.TextColored(eventType.GetColor(), EventTypeNames[eventType]);

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

	public static void NextColumnEventIndex(int index)
	{
		ImGui.TableNextColumn();
		ImGui.Text(Inline.Span(index));
	}

	[Obsolete]
	public static void NextColumnInputByteEnum<TEnum>(int uniqueId, ReadOnlySpan<char> fieldName, ref TEnum value, IReadOnlyList<TEnum> values, string[] names)
		where TEnum : Enum
	{
		ImGui.TableNextColumn();

		InputByteEnum(uniqueId, fieldName, ref value, values, names);
	}

	[Obsolete]
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

	[Obsolete]
	public static void NextColumnInputInt(int uniqueId, ReadOnlySpan<char> fieldName, ref int value)
	{
		ImGui.TableNextColumn();

		InputInt(uniqueId, fieldName, ref value);
	}

	[Obsolete]
	public static void InputInt(int uniqueId, ReadOnlySpan<char> fieldName, ref int value)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputInt(EditLabel(fieldName, uniqueId), ref value, 0, 0);
		ImGui.PopItemWidth();
	}

	[Obsolete]
	public static void NextColumnInputShort(int uniqueId, ReadOnlySpan<char> fieldName, ref short value)
	{
		ImGui.TableNextColumn();

		InputShort(uniqueId, fieldName, ref value);
	}

	[Obsolete]
	private static unsafe void InputShort(int uniqueId, ReadOnlySpan<char> fieldName, ref short value)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputScalar(EditLabel(fieldName, uniqueId), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value), 0, 0);
		ImGui.PopItemWidth();
	}

	[Obsolete]
	public static void NextColumnInputFloat(int uniqueId, ReadOnlySpan<char> fieldName, ref float value, ReadOnlySpan<char> format = default)
	{
		ImGui.TableNextColumn();

		InputFloat(uniqueId, fieldName, ref value, format);
	}

	[Obsolete]
	public static void InputFloat(int uniqueId, ReadOnlySpan<char> fieldName, ref float value, ReadOnlySpan<char> format = default)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputFloat(EditLabel(fieldName, uniqueId), ref value, 0, 0, format);
		ImGui.PopItemWidth();
	}

	[Obsolete]
	public static void NextColumnInputVector3(int uniqueId, ReadOnlySpan<char> fieldName, ref Vector3 value, ReadOnlySpan<char> format = default)
	{
		ImGui.TableNextColumn();

		InputVector3(uniqueId, fieldName, ref value, format);
	}

	[Obsolete]
	public static void InputVector3(int uniqueId, ReadOnlySpan<char> fieldName, ref Vector3 value, ReadOnlySpan<char> format = default)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputFloat3(EditLabel(fieldName, uniqueId), ref value, format);
		ImGui.PopItemWidth();
	}

	[Obsolete]
	public static void NextColumnInputInt16Vec3(int uniqueId, ReadOnlySpan<char> fieldName, ref Int16Vec3 value)
	{
		ImGui.TableNextColumn();
		InputInt16Vec3(uniqueId, fieldName, ref value);
	}

	[Obsolete]
	public static unsafe void InputInt16Vec3(int uniqueId, ReadOnlySpan<char> fieldName, ref Int16Vec3 value)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value), 3);
		ImGui.PopItemWidth();
	}

	[Obsolete]
	public static void NextColumnInputMatrix3x3(int uniqueId, ReadOnlySpan<char> fieldName, ref Matrix3x3 value, ReadOnlySpan<char> format = default)
	{
		ImGui.TableNextColumn();

		InputMatrix3x3(uniqueId, fieldName, ref value, format);
	}

	[Obsolete]
	private static unsafe void InputMatrix3x3(int uniqueId, ReadOnlySpan<char> fieldName, ref Matrix3x3 value, ReadOnlySpan<char> format = default)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.Float, (IntPtr)Unsafe.AsPointer(ref value), 9, 0, 0, format);
		ImGui.PopItemWidth();
	}

	[Obsolete]
	public static void NextColumnInputInt16Mat3x3(int uniqueId, ReadOnlySpan<char> fieldName, ref Int16Mat3x3 value)
	{
		ImGui.TableNextColumn();

		InputInt16Mat3x3(uniqueId, fieldName, ref value);
	}

	[Obsolete]
	private static unsafe void InputInt16Mat3x3(int uniqueId, ReadOnlySpan<char> fieldName, ref Int16Mat3x3 value)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value), 9);
		ImGui.PopItemWidth();
	}

	[Obsolete]
	public static void NextColumnCheckbox(int uniqueId, ReadOnlySpan<char> fieldName, ref bool value, ReadOnlySpan<char> trueText, ReadOnlySpan<char> falseText)
	{
		ImGui.TableNextColumn();
		Checkbox(uniqueId, fieldName, ref value, trueText, falseText);
	}

	[Obsolete]
	public static void Checkbox(int uniqueId, ReadOnlySpan<char> fieldName, ref bool value, ReadOnlySpan<char> trueText, ReadOnlySpan<char> falseText)
	{
		ImGui.Checkbox(EditLabel(fieldName, uniqueId), ref value);
		ImGui.SameLine();
		ImGui.Text(value ? trueText : falseText);
	}

	public static void NextColumnEntityId(EditorReplayModel replay, int entityId)
	{
		EntityType? entityType = replay.Cache.GetEntityTypeIncludingNegated(entityId);

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

	private static ReadOnlySpan<char> EditLabel(ReadOnlySpan<char> label, int uniqueId)
	{
		return Inline.Span($"##{label}{uniqueId}");
	}
}
