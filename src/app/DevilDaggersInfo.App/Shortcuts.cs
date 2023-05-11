using DevilDaggersInfo.App.Ui;
using DevilDaggersInfo.App.Ui.ReplayEditor;
using DevilDaggersInfo.App.Ui.SpawnsetEditor;
using Silk.NET.Input;

namespace DevilDaggersInfo.App;

public static class Shortcuts
{
	public static void OnKeyPressed(IKeyboard keyboard, Key key, int arg3)
	{
		if (Modals.IsAnyOpen)
			return;

		bool ctrl = keyboard.IsKeyPressed(Key.ControlLeft) || keyboard.IsKeyPressed(Key.ControlRight);

		switch (UiRenderer.Layout)
		{
			case LayoutType.SpawnsetEditor: HandleSpawnsetEditorShortcuts(key, ctrl); break;
			case LayoutType.ReplayEditor: HandleReplayEditorShortcuts(key, ctrl); break;
		}
	}

	private static void HandleSpawnsetEditorShortcuts(Key key, bool ctrl)
	{
		if (key == Key.Escape)
			SpawnsetEditorMenu.Close();

		if (ctrl)
		{
			switch (key)
			{
				case Key.N: SpawnsetEditorMenu.NewSpawnset(); break;
				case Key.O: SpawnsetEditorMenu.OpenSpawnset(); break;
				case Key.S: SpawnsetEditorMenu.SaveSpawnset(); break;
				case Key.R: SpawnsetEditorMenu.ReplaceSpawnset(); break;
			}
		}
	}

	private static void HandleReplayEditorShortcuts(Key key, bool ctrl)
	{
		if (key == Key.Escape)
			ReplayEditorMenu.Close();

		if (ctrl)
		{
			switch (key)
			{
				case Key.N: ReplayEditorMenu.NewReplay(); break;
				case Key.O: ReplayEditorMenu.OpenReplay(); break;
				case Key.S: ReplayEditorMenu.SaveReplay(); break;
			}
		}
	}
}
