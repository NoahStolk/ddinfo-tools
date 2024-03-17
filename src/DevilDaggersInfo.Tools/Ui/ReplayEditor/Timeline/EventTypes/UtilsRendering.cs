using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline.EventTypes;

public static class UtilsRendering
{
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

	public static void InputInt(int uniqueId, ReadOnlySpan<char> fieldName, ref int value)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputInt(EditLabel(fieldName, uniqueId), ref value, 0, 0);
		ImGui.PopItemWidth();
	}

	public static unsafe void InputShort(int uniqueId, ReadOnlySpan<char> fieldName, ref short value)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputScalar(EditLabel(fieldName, uniqueId), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value), 0, 0);
		ImGui.PopItemWidth();
	}

	public static void InputFloat(int uniqueId, ReadOnlySpan<char> fieldName, ref float value, ReadOnlySpan<char> format = default)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputFloat(EditLabel(fieldName, uniqueId), ref value, 0, 0, format);
		ImGui.PopItemWidth();
	}

	public static void InputVector3(int uniqueId, ReadOnlySpan<char> fieldName, ref Vector3 value, ReadOnlySpan<char> format = default)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputFloat3(EditLabel(fieldName, uniqueId), ref value, format);
		ImGui.PopItemWidth();
	}

	public static unsafe void InputInt16Vec3(int uniqueId, ReadOnlySpan<char> fieldName, ref Int16Vec3 value)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value), 3);
		ImGui.PopItemWidth();
	}

	// ReSharper disable once InconsistentNaming
	public static unsafe void InputMatrix3x3Square(int uniqueId, ReadOnlySpan<char> fieldName, ref Matrix3x3 value, ReadOnlySpan<char> format = default)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.Float, (IntPtr)Unsafe.AsPointer(ref value.M11), 3, 0, 0, format);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.Float, (IntPtr)Unsafe.AsPointer(ref value.M21), 3, 0, 0, format);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.Float, (IntPtr)Unsafe.AsPointer(ref value.M31), 3, 0, 0, format);
		ImGui.PopItemWidth();
	}

	// ReSharper disable once InconsistentNaming
	public static unsafe void InputInt16Mat3x3Square(int uniqueId, ReadOnlySpan<char> fieldName, ref Int16Mat3x3 value)
	{
		ImGui.PushItemWidth(-1);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value.M11), 3);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value.M21), 3);
		ImGui.InputScalarN(EditLabel(fieldName, uniqueId), ImGuiDataType.S16, (IntPtr)Unsafe.AsPointer(ref value.M31), 3);
		ImGui.PopItemWidth();
	}

	public static void Checkbox(int uniqueId, ReadOnlySpan<char> fieldName, ref bool value, ReadOnlySpan<char> trueText, ReadOnlySpan<char> falseText)
	{
		ImGui.Checkbox(EditLabel(fieldName, uniqueId), ref value);
		ImGui.SameLine();
		ImGui.Text(value ? trueText : falseText);
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
		EntityType? entityType = replay.Cache.GetEntityTypeIncludingNegated(entityId);
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
