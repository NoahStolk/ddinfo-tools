using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.Popups;

public class ErrorMessage : Popup
{
	private readonly string _errorText;

	public ErrorMessage(string id, string errorText)
		: base(id)
	{
		_errorText = errorText;
	}

	public override bool Render()
	{
		ImGui.TextWrapped(_errorText);

		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();
		return ImGui.Button("OK", new(120, 0));
	}
}
