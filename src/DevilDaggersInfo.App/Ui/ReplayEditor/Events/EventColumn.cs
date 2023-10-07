using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Events;

public sealed record EventColumn(string Name, ImGuiTableColumnFlags Flags, float Width);
