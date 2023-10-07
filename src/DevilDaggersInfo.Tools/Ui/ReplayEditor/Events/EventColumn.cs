using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;

public sealed record EventColumn(string Name, ImGuiTableColumnFlags Flags, float Width);
