using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Popups;

internal sealed class MessageWithHideOption(string id, string text, Action<bool> setDoNotShowAgain, bool doNotShowAgain)
	: Popup(id)
{
	private bool _doNotShowAgain = doNotShowAgain;

	public override bool Render()
	{
		ImGui.TextWrapped(text);

		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();

		if (ImGui.Checkbox("Do not show again", ref _doNotShowAgain))
			setDoNotShowAgain(_doNotShowAgain);

		return ImGui.Button("OK", new Vector2(120, 0)) || ImGuiUtils.IsEnterPressed();
	}
}
