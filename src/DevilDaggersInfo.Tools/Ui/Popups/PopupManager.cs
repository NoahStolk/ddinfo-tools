using DevilDaggersInfo.Tools.Networking;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Popups;

public static class PopupManager
{
	private static readonly List<Popup> _popups = [];

	public static bool IsAnyOpen
	{
		get
		{
			for (int i = 0; i < _popups.Count; i++)
			{
				if (_popups[i].HasOpened)
					return true;
			}

			return false;
		}
	}

	public static IReadOnlyList<Popup> Popups => _popups;

	public static void ShowError(string errorText, ApiError? apiError)
	{
		ShowError(errorText, apiError?.Message + Environment.NewLine + apiError?.Exception?.Message);
	}

	public static void ShowError(string errorText, Exception? exception)
	{
		ShowError(errorText, exception?.Message);
	}

	public static void ShowError(string errorText, string? technicalDetails = null)
	{
		_popups.Add(new ErrorMessage("Error", errorText, technicalDetails));
	}

	public static void ShowMessage(string title, string text)
	{
		_popups.Add(new Message(title, text));
	}

	public static void ShowMessageWithHideOption(string title, string text, bool doNotShowAgain, Action<bool> setDoNotShowAgain)
	{
		if (doNotShowAgain)
			return;

		_popups.Add(new MessageWithHideOption(title, text, setDoNotShowAgain, doNotShowAgain));
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
		_popups.Add(new Question(title, text, onConfirm, onDeny));
	}

	public static void Render()
	{
		// Render all open popups (there should be only one at a time).
		// We remove popups from the list during rendering, so we need to iterate backwards.
		bool isAnyOpen = false;
		for (int i = _popups.Count - 1; i >= 0; i--)
		{
			Popup popup = _popups[i];
			if (popup.HasOpened)
			{
				RenderModal(popup);
				isAnyOpen = true;

				// TODO: Might need to break here if we want to prevent rendering multiple popups at once (but this should never happen).
			}
		}

		if (!isAnyOpen)
		{
			// IF there are popups left, and there are currently no popups open, open the remaining popups in order of addition.
			for (int i = 0; i < _popups.Count; i++)
			{
				Popup popup = _popups[i];
				if (!popup.HasOpened)
				{
					ImGui.OpenPopup(popup.Id);
					popup.HasOpened = true;
					break;
				}
			}
		}
	}

	private static void RenderModal(Popup popup)
	{
		Vector2 center = ImGui.GetMainViewport().GetCenter();
		ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));

		ImGui.SetNextWindowSizeConstraints(new Vector2(192, 128), new Vector2(float.MaxValue, float.MaxValue));
		if (ImGui.BeginPopupModal(popup.Id))
		{
			if (popup.Render())
			{
				ImGui.CloseCurrentPopup();
				_popups.Remove(popup);
			}

			ImGui.EndPopup();
		}
	}
}
