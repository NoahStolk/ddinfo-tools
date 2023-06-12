using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui;

public static partial class ImGuiExt
{
	public static void Hyperlink(string url, string? text = null)
	{
		Vector4 hyperlinkColor = new(0, 0.625f, 1, 1);
		Vector4 hyperlinkHoverColor = new(0.25f, 0.875f, 1, 0.25f);

		ImGui.PushStyleColor(ImGuiCol.Text, hyperlinkColor);
		ImGui.PushStyleColor(ImGuiCol.Button, default(Vector4));
		ImGui.PushStyleColor(ImGuiCol.ButtonHovered, hyperlinkHoverColor);
		ImGui.PushStyleColor(ImGuiCol.ButtonActive, default(Vector4));
		ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);

		if (ImGui.SmallButton(text ?? url))
			Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });

		ImGui.PopStyleColor(4);
		ImGui.PopStyleVar();
	}
}
