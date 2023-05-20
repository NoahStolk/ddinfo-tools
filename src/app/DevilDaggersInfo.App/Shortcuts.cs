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
		bool shift = keyboard.IsKeyPressed(Key.ShiftLeft) || keyboard.IsKeyPressed(Key.ShiftRight);

		switch (UiRenderer.Layout)
		{
			case LayoutType.SpawnsetEditor: HandleSpawnsetEditorShortcuts(key, ctrl, shift); break;
			case LayoutType.ReplayEditor: HandleReplayEditorShortcuts(key, ctrl, shift); break;
		}
	}

	private static void HandleSpawnsetEditorShortcuts(Key key, bool ctrl, bool shift)
	{
		if (key == Key.Escape)
		{
			SpawnsetEditorMenu.Close();
			return;
		}

		if (ctrl && !shift)
		{
			Action? action = key switch
			{
				Key.N => SpawnsetEditorMenu.NewSpawnset,
				Key.O => SpawnsetEditorMenu.OpenSpawnset,
				Key.S => SpawnsetEditorMenu.SaveSpawnset,
				Key.R => SpawnsetEditorMenu.ReplaceCurrentSpawnset,
				Key.D => SpawnsetEditorMenu.DeleteCurrentSpawnset,
				_ => null,
			};
			action?.Invoke();
		}

		if (ctrl && shift)
		{
			Action? action = key switch
			{
				Key.O => SpawnsetEditorMenu.OpenCurrentSpawnset,
				Key.D => SpawnsetEditorMenu.OpenDefaultSpawnset,
				_ => null,
			};
			action?.Invoke();
		}
	}

	private static void HandleReplayEditorShortcuts(Key key, bool ctrl, bool shift)
	{
		if (key == Key.Escape)
			ReplayEditorMenu.Close();

		if (ctrl && !shift)
		{
			Action? action = key switch
			{
				Key.N => ReplayEditorMenu.NewReplay,
				Key.O => ReplayEditorMenu.OpenReplay,
				Key.S => ReplayEditorMenu.SaveReplay,
				_ => null,
			};
			action?.Invoke();
		}
	}
}
