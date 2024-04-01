// ReSharper disable InconsistentNaming
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

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

	public static void RenderTable<TEvent, TRenderer>(EventType eventType, int eventIndex, int entityId, TEvent @event, EditorReplayModel replay)
		where TEvent : IEventData
		where TRenderer : IEventTypeRenderer<TEvent>
	{
		if (ImGui.BeginTable(EventTypeNames[eventType], TRenderer.ColumnCount, EventTableFlags))
		{
			TRenderer.SetupColumns();

			ImGui.TableHeadersRow();

			ImGui.TableNextRow();
			TRenderer.Render(eventIndex, entityId, @event, replay);

			ImGui.EndTable();
		}
	}

	public static void NextColumnEnum<TEnum>(TEnum value)
		where TEnum : Enum
	{
		ImGui.TableNextColumn();
		ImGui.Text(Inline.Span(value));
	}

	public static void NextColumn<T>(T value, ReadOnlySpan<char> format = default)
		where T : ISpanFormattable
	{
		ImGui.TableNextColumn();
		ImGui.Text(Inline.Span(value, format));
	}

	public static void NextColumnVector3(Vector3 value, ReadOnlySpan<char> format = default)
	{
		ImGui.TableNextColumn();
		ImGui.Text(Inline.Span(value, format));
	}

	public static void NextColumnBool(bool value, ReadOnlySpan<char> trueText, ReadOnlySpan<char> falseText)
	{
		ImGui.TableNextColumn();
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
}
