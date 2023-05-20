using DevilDaggersInfo.Core.Versioning;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui;

public static class Modals
{
	private const string _updateId = "Update available";
	private const string _errorId = "Error";
	private const string _replacedSurvivalFileId = "Successfully replaced current survival file";
	private const string _deletedSurvivalFileId = "Successfully deleted current survival file";

	private static readonly List<ModalData> _modals = new()
	{
		new(_updateId, () => ImGui.Text($"Version {_availableVersion} is available. Re-run the launcher to install it.")),
		new(_errorId, () => ImGui.TextWrapped(_errorText)),
		new(_replacedSurvivalFileId, () => ImGui.Text("The current survival file has been replaced with the current spawnset.")),
		new(_deletedSurvivalFileId, () => ImGui.Text("The current survival file has been deleted.")),
	};

	private static AppVersion? _availableVersion;
	private static string? _errorText;

	public static bool IsAnyOpen { get; private set; }

	public static void ShowUpdateAvailable(AppVersion availableVersion)
	{
		_availableVersion = availableVersion;
		_modals.First(m => m.Id == _updateId).ShouldOpen = true;
	}

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
		ModalData? modalToOpen = _modals.Find(kvp => kvp.ShouldOpen);
		if (modalToOpen != null)
		{
			ImGui.OpenPopup(modalToOpen.Id);
			modalToOpen.ShouldOpen = false;
		}

		IsAnyOpen = false;
		foreach (ModalData modal in _modals)
		{
			bool isOpen = RenderModal(modal.Id, modal.RenderAction);
			if (isOpen)
				IsAnyOpen = true;
		}
	}

	private static bool RenderModal(string modalId, Action renderAction)
	{
		Vector2 center = ImGui.GetMainViewport().GetCenter();
		ImGui.SetNextWindowPos(center, ImGuiCond.Always, new(0.5f, 0.5f));

		bool temp = true;
		if (ImGui.BeginPopupModal(modalId, ref temp, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
		{
			renderAction();

			ImGui.Spacing();
			ImGui.Separator();
			ImGui.Spacing();
			if (ImGui.Button("OK", new(120, 0)))
				ImGui.CloseCurrentPopup();

			ImGui.EndPopup();
		}

		return ImGui.IsPopupOpen(modalId);
	}

	private sealed record ModalData(string Id, Action RenderAction)
	{
		public bool ShouldOpen { get; set; }
	}
}
