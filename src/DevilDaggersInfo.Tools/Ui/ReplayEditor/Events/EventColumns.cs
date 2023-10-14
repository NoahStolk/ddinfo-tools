using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;

public static class EventColumns
{
	public static IReadOnlyList<EventColumn> ColumnsBoidSpawn { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Spawner Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Type", ImGuiTableColumnFlags.None, 128),
		new("Position", ImGuiTableColumnFlags.None, 128),
		new("Orientation", ImGuiTableColumnFlags.None, 196),
		new("Velocity", ImGuiTableColumnFlags.None, 128),
		new("Speed", ImGuiTableColumnFlags.None, 128),
	};

	public static IReadOnlyList<EventColumn> ColumnsLeviathanSpawn { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("?", ImGuiTableColumnFlags.None, 128),
	};

	public static IReadOnlyList<EventColumn> ColumnsPedeSpawn { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Type", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("Position", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("Orientation", ImGuiTableColumnFlags.None, 128),
	};

	public static IReadOnlyList<EventColumn> ColumnsSpiderEggSpawn { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Spawner Entity Id", ImGuiTableColumnFlags.None, 196),
		new("Position", ImGuiTableColumnFlags.None, 128),
		new("Target Position", ImGuiTableColumnFlags.None, 128),
	};

	public static IReadOnlyList<EventColumn> ColumnsSpiderSpawn { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Type", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("Position", ImGuiTableColumnFlags.None, 128),
	};

	public static IReadOnlyList<EventColumn> ColumnsSquidSpawn { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Type", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("Position", ImGuiTableColumnFlags.None, 128),
		new("Direction", ImGuiTableColumnFlags.None, 128),
		new("Rotation", ImGuiTableColumnFlags.None, 128),
	};

	public static IReadOnlyList<EventColumn> ColumnsThornSpawn { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("Position", ImGuiTableColumnFlags.None, 128),
		new("Rotation", ImGuiTableColumnFlags.None, 128),
	};

	public static IReadOnlyList<EventColumn> ColumnsDaggerSpawn { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Type", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("Position", ImGuiTableColumnFlags.None, 128),
		new("Orientation", ImGuiTableColumnFlags.None, 196),
		new("Shot / Rapid", ImGuiTableColumnFlags.None, 128),
	};

	public static IReadOnlyList<EventColumn> ColumnsEntityOrientation { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Orientation", ImGuiTableColumnFlags.None, 196),
	};

	public static IReadOnlyList<EventColumn> ColumnsEntityPosition { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Position", ImGuiTableColumnFlags.None, 128),
	};

	public static IReadOnlyList<EventColumn> ColumnsEntityTarget { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Target Position", ImGuiTableColumnFlags.None, 128),
	};

	public static IReadOnlyList<EventColumn> ColumnsGem { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
	};

	public static IReadOnlyList<EventColumn> ColumnsHit { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id A", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Entity Id B", ImGuiTableColumnFlags.WidthFixed, 160),
		new("User Data", ImGuiTableColumnFlags.WidthFixed, 128),
	};

	public static IReadOnlyList<EventColumn> ColumnsTransmute { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
	};

	public static IReadOnlyList<EventColumn> ColumnsDeath { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Death Type", ImGuiTableColumnFlags.WidthFixed, 160),
	};

	public static IReadOnlyList<EventColumn> ColumnsEnd { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
	};
}
