using DevilDaggersInfo.Tools.Extensions;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Popups;

internal sealed class Message(string id, Vector2 minSize, string text)
	: Popup(id, minSize)
{
	public override bool Render()
	{
		ImGui.Text(text);

		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();
		return ImGui.Button("OK", new Vector2(120, 0)) || ImGui.IsEnterPressed();
	}
}
