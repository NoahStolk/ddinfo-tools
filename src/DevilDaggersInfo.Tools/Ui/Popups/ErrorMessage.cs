using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Popups;

internal sealed class ErrorMessage(string id, Vector2 minSize, string errorText, string? technicalDetails = null)
	: Popup(id, minSize)
{
	public override bool Render()
	{
		ImGui.TextWrapped(errorText);

		if (technicalDetails != null && ImGui.CollapsingHeader("Technical details"))
		{
			ImGui.TextWrapped(technicalDetails);
		}

		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();
		return ImGui.Button("OK", new Vector2(120, 0));
	}
}
