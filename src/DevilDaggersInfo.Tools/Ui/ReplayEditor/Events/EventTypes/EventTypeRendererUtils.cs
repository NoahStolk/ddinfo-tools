using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Events.Interfaces;
using DevilDaggersInfo.Core.Replay.Extensions;
using DevilDaggersInfo.Core.Replay.Numerics;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
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
		[EventType.Death] = "Death events",
		[EventType.End] = "End events",
	};

	public static ImGuiTableFlags EventTableFlags => ImGuiTableFlags.Borders | ImGuiTableFlags.NoPadOuterX;

	public static void SetupColumns(IReadOnlyList<EventColumn> columns)
	{
		for (int i = 0; i < columns.Count; i++)
		{
			EventColumn column = columns[i];
			ImGui.TableSetupColumn(column.Name, column.Flags, column.Width);
		}

		ImGui.TableHeadersRow();
	}

	public static void RenderTable<TEvent, TRenderer>(Vector4 color, EventType eventType, IReadOnlyList<(int Index, TEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
		where TEvent : IEvent
		where TRenderer : IEventTypeRenderer<TEvent>
	{
		ImGui.TextColored(color, EventTypeNames[eventType]);

		if (ImGui.BeginTable(EventTypeNames[eventType], columns.Count, EventTableFlags))
		{
			SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, TEvent e) = events[i];
				TRenderer.Render(index, e, entityTypes);
			}

			ImGui.EndTable();
		}
	}

	public static void NextColumnText(ReadOnlySpan<char> text)
	{
		ImGui.TableNextColumn();
		ImGui.Text(text);
	}

	public static void NextColumnInputInt(int eventIndex, ReadOnlySpan<char> fieldName, ref int value)
	{
		ImGui.TableNextColumn();

		ImGui.PushItemWidth(-1);
		ImGui.InputInt(EditLabel(fieldName, eventIndex), ref value, 0, 0);
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

	public static unsafe void NextColumnInputMatrix3x3(int eventIndex, ReadOnlySpan<char> fieldName, ref Matrix3x3 value)
	{
		ImGui.TableNextColumn();

		ImGui.PushItemWidth(-1);
		ImGui.InputScalarN(EditLabel(fieldName, eventIndex), ImGuiDataType.Float, (IntPtr)Unsafe.AsPointer(ref value), 9);
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

	public static void EntityColumn(IReadOnlyList<EntityType> entityTypes, int entityId)
	{
		EntityType? entityType = entityId >= 0 && entityId < entityTypes.Count ? entityTypes[entityId] : null;

		ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);

		ImGui.TableNextColumn();
		ImGui.Text(Inline.Span(entityId));
		ImGui.SameLine();
		ImGui.Text(" (");
		ImGui.SameLine();
		ImGui.TextColored(entityType.GetColor(), entityType.HasValue ? EnumUtils.EntityTypeNames[entityType.Value] : "???");
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
