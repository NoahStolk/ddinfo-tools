using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui;

public static class Modals
{
	private const string _errorId = "Error";
	private const string _replacedSurvivalFileId = "Successfully replaced current survival file";
	private const string _deletedSurvivalFileId = "Successfully deleted current survival file";

	private static readonly List<ModalData> _modals = new()
	{
		new(_errorId, static () => ImGui.TextWrapped(_errorText)),
		new(_replacedSurvivalFileId, static () => ImGui.Text("The current survival file has been replaced with the current spawnset.")),
		new(_deletedSurvivalFileId, static () => ImGui.Text("The current survival file has been deleted.")),
	};

	private static string? _errorText;

	public static bool IsAnyOpen { get; private set; }

	public static void ShowError(string errorText)
	{
		_errorText = errorText;
		_modals.First(m => m.Id == _errorId).ShouldOpen = true;
	}

	public static void ShowReplacedSurvivalFile()
	{
		_modals.First(m => m.Id == _replacedSurvivalFileId).ShouldOpen = true;
	}

	public static void ShowDeletedSurvivalFile()
	{
		_modals.First(m => m.Id == _deletedSurvivalFileId).ShouldOpen = true;
	}

	public static void Render()
	{
		for (int i = 0; i < _modals.Count; i++)
		{
			ModalData modal = _modals[i];
			if (modal.ShouldOpen)
			{
				ImGui.OpenPopup(modal.Id);
				modal.ShouldOpen = false;
				break;
			}
		}

		IsAnyOpen = false;
		for (int i = 0; i < _modals.Count; i++)
		{
			ModalData modal = _modals[i];
			bool isOpen = RenderModal(modal);
			if (isOpen)
				IsAnyOpen = true;
		}
	}

	private static bool RenderModal(ModalData modal)
	{
		Vector2 center = ImGui.GetMainViewport().GetCenter();
		ImGui.SetNextWindowPos(center, ImGuiCond.Always, new(0.5f, 0.5f));

		bool temp = true;
		if (ImGui.BeginPopupModal(modal.Id, ref temp, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
		{
			modal.RenderAction();

			ImGui.Spacing();
			ImGui.Separator();
			ImGui.Spacing();
			if (ImGui.Button("OK", new(120, 0)))
				ImGui.CloseCurrentPopup();

			ImGui.EndPopup();
		}

		return ImGui.IsPopupOpen(modal.Id);
	}

	private sealed record ModalData(string Id, Action RenderAction)
	{
		public bool ShouldOpen { get; set; }
	}
}
