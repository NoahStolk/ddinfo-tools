using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Popups;

internal sealed class Question(string id, string text, Action onConfirm, Action onDeny)
	: Popup(id)
{
	public override bool Render()
	{
		ImGui.Text(text);

		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();

		bool shouldExit = false;

		if (ImGui.Button("Yes", new Vector2(120, 0)))
		{
			onConfirm();
			shouldExit = true;
		}

		ImGui.SameLine();

		if (ImGui.Button("No", new Vector2(120, 0)))
		{
			onDeny();
			shouldExit = true;
		}

		ImGui.SameLine();

		if (ImGui.Button("Cancel", new Vector2(120, 0)))
			shouldExit = true;

		return shouldExit;
	}
}
