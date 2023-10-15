using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;

public sealed record EventColumn(string Name, ImGuiTableColumnFlags Flags, float Width)
{
	public static EventColumn Actions { get; } = new("Actions", ImGuiTableColumnFlags.WidthFixed, 64);
	public static EventColumn Index { get; } = new("Index", ImGuiTableColumnFlags.WidthFixed, 64);
	public static EventColumn EntityId { get; } = new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160);
}
