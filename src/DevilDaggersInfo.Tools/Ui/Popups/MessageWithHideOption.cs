using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.Popups;

public class MessageWithHideOption : Popup
{
	private readonly string _text;
	private readonly Action<bool> _setDoNotShowAgain;
	private bool _doNotShowAgain;

	public MessageWithHideOption(string id, string text, Action<bool> setDoNotShowAgain, bool doNotShowAgain)
		: base(id)
	{
		_text = text;
		_setDoNotShowAgain = setDoNotShowAgain;
		_doNotShowAgain = doNotShowAgain;
	}

	public override bool Render()
	{
		ImGui.TextWrapped(_text);

		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();

		if (ImGui.Checkbox("Do not show again", ref _doNotShowAgain))
			_setDoNotShowAgain(_doNotShowAgain);

		return ImGui.Button("OK", new(120, 0)) || ImGuiUtils.IsEnterPressed();
	}
}
