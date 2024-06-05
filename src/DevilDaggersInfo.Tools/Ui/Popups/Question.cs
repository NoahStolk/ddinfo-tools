using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Popups;

public class Question : Popup
{
	private readonly string _text;
	private readonly Action _onConfirm;
	private readonly Action _onDeny;

	public Question(string id, string text, Action onConfirm, Action onDeny)
		: base(id)
	{
		_text = text;
		_onConfirm = onConfirm;
		_onDeny = onDeny;
	}

	public override bool Render()
	{
		ImGui.Text(_text);

		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();

		bool shouldExit = false;

		if (ImGui.Button("Yes", new Vector2(120, 0)))
		{
			_onConfirm();
			shouldExit = true;
		}

		ImGui.SameLine();

		if (ImGui.Button("No", new Vector2(120, 0)))
		{
			_onDeny();
			shouldExit = true;
		}

		ImGui.SameLine();

		if (ImGui.Button("Cancel", new Vector2(120, 0)))
			shouldExit = true;

		return shouldExit;
	}
}
