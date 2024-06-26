using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Popups;

public class Message : Popup
{
	private readonly string _text;

	public Message(string id, string text)
		: base(id)
	{
		_text = text;
	}

	public override bool Render()
	{
		ImGui.Text(_text);

		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();
		return ImGui.Button("OK", new Vector2(120, 0)) || ImGuiUtils.IsEnterPressed();
	}
}
