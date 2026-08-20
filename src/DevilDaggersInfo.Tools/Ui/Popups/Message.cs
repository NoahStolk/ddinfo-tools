using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Popups;

internal sealed class Message(string id, string text) : Popup(id)
{
	public override bool Render()
	{
		ImGui.Text(text);

		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();
		return ImGui.Button("OK", new Vector2(120, 0)) || ImGuiUtils.IsEnterPressed();
	}
}
