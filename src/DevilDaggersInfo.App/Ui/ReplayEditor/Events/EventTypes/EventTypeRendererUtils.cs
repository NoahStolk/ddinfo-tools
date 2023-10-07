using DevilDaggersInfo.App.Utils;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Extensions;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Events.EventTypes;

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

	public static void NextColumnText(ReadOnlySpan<char> text)
	{
		ImGui.TableNextColumn();
		ImGui.Text(text);
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
}
