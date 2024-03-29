using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Popups;

public static class PopupManager
{
	private static readonly List<Popup> _openPopups = [];

	public static bool IsAnyOpen => _openPopups.Count > 0;

	public static void ShowError(string errorText, Exception? exception)
	{
		ShowError(errorText, exception?.Message);
	}

	public static void ShowError(string errorText, string? technicalDetails = null)
	{
		_openPopups.Add(new ErrorMessage($"Error##{DateTime.UtcNow.Ticks}", errorText, technicalDetails));
	}

	public static void ShowMessage(string title, string text)
	{
		_openPopups.Add(new Message(title, text));
	}

	public static void ShowMessageWithHideOption(string title, string text, bool doNotShowAgain, Action<bool> setDoNotShowAgain)
	{
		if (doNotShowAgain)
			return;

		_openPopups.Add(new MessageWithHideOption(title, text, setDoNotShowAgain, doNotShowAgain));
	}

	public static void ShowSaveSpawnsetPrompt(Action action)
	{
		ShowQuestion(
			"Save spawnset?",
			"Do you want to save the current spawnset?",
			() =>
			{
				SpawnsetEditorMenu.SaveSpawnset();
				action();
			},
			action);
	}

	public static void ShowQuestion(string title, string text, Action onConfirm, Action onDeny)
	{
		_openPopups.Add(new Question(title, text, onConfirm, onDeny));
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
		ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new(0.5f, 0.5f));

		ImGui.SetNextWindowSizeConstraints(new(192, 128), new(float.MaxValue, float.MaxValue));
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
