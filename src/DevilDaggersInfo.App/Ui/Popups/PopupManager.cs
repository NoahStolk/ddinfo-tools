using DevilDaggersInfo.App.Ui.SpawnsetEditor;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.Popups;

public static class PopupManager
{
	private static readonly List<Popup> _openPopups = new();

	public static bool IsAnyOpen => _openPopups.Count > 0;

	public static void ShowError(string errorText)
	{
		_openPopups.Add(new ErrorMessage("Error", errorText));
	}

	public static void ShowMessage(string title, string text)
	{
		_openPopups.Add(new Message(title, text));
	}

	public static void ShowSaveSpawnsetPrompt(Action action)
	{
		_openPopups.Add(new Question(
			"Save spawnset?",
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
