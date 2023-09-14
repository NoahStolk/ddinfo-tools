using DevilDaggersInfo.App.Ui.SpawnsetEditor;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.Popups;

public static class PopupManager
{
	private const string _errorId = "Error";
	private const string _replacedSurvivalFileId = "Successfully replaced current survival file";
	private const string _deletedSurvivalFileId = "Successfully deleted current survival file";
	private const string _saveSpawnsetId = "Save spawnset?";

	private static readonly List<Popup> _openPopups = new();

	public static bool IsAnyOpen => _openPopups.Count > 0;

	public static void ShowError(string errorText)
	{
		_openPopups.Add(new ErrorMessage(_errorId, errorText));
	}

	public static void ShowReplacedSurvivalFile()
	{
		_openPopups.Add(new Message(_replacedSurvivalFileId, "The current survival file has been replaced with the current spawnset."));
	}

	public static void ShowDeletedSurvivalFile()
	{
		_openPopups.Add(new Message(_deletedSurvivalFileId, "The current survival file has been deleted."));
	}

	public static void ShowSaveSpawnsetPrompt(Action action)
	{
		_openPopups.Add(new Question(
			_saveSpawnsetId,
			"Do you want to save the current spawnset?",
			() =>
			{
				SpawnsetEditorMenu.SaveSpawnset();
				action();
			},
			action));
	}

	public static void Render()
	{
		// We remove popups from the list during rendering, so we need to iterate backwards.
		for (int i = _openPopups.Count - 1; i >= 0; i--)
		{
			Popup popup = _openPopups[i];
			if (!popup.HasOpened)
			{
				ImGui.OpenPopup(popup.Id);
				popup.HasOpened = false;
			}

			RenderModal(popup);
		}
	}

	private static void RenderModal(Popup popup)
	{
		Vector2 center = ImGui.GetMainViewport().GetCenter();
		ImGui.SetNextWindowPos(center, ImGuiCond.Always, new(0.5f, 0.5f));

		if (ImGui.BeginPopupModal(popup.Id))
		{
			if (popup.Render())
			{
				ImGui.CloseCurrentPopup();
				_openPopups.Remove(popup);
			}

			ImGui.EndPopup();
		}
	}
}
