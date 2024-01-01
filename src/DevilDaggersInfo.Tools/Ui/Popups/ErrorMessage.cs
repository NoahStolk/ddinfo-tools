using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.Popups;

public class ErrorMessage : Popup
{
	private readonly string _errorText;
	private readonly string? _technicalDetails;

	public ErrorMessage(string id, string errorText, string? technicalDetails = null)
		: base(id)
	{
		_errorText = errorText;
		_technicalDetails = technicalDetails;
	}

	public override bool Render()
	{
		ImGui.TextWrapped(_errorText);

		if (_technicalDetails != null && ImGui.CollapsingHeader("Technical details"))
		{
			ImGui.TextWrapped(_technicalDetails);
		}

		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();
		return ImGui.Button("OK", new(120, 0));
	}
}
