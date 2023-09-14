using DevilDaggersInfo.App.Ui;
using DevilDaggersInfo.App.Ui.Popups;
using DevilDaggersInfo.App.Ui.ReplayEditor;
using DevilDaggersInfo.App.Ui.SpawnsetEditor;
using Silk.NET.Input;

namespace DevilDaggersInfo.App;

public static class Shortcuts
{
	public static void OnKeyPressed(IKeyboard keyboard, Key key)
	{
		if (PopupManager.IsAnyOpen)
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

		if (ctrl)
		{
			if (!shift)
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
			else
			{
				Action? action = key switch
				{
					Key.O => SpawnsetEditorMenu.OpenCurrentSpawnset,
					Key.S => SpawnsetEditorMenu.SaveSpawnsetAs,
					Key.D => SpawnsetEditorMenu.OpenDefaultSpawnset,
					_ => null,
				};
				action?.Invoke();
			}
		}
	}

	private static void HandleReplayEditorShortcuts(Key key, bool ctrl, bool shift)
	{
		if (key == Key.Escape)
			ReplayEditorMenu.Close();

		if (ctrl)
		{
			if (!shift)
			{
				Action? action = key switch
				{
					Key.N => ReplayEditorMenu.NewReplay,
					Key.O => ReplayEditorMenu.OpenReplay,
					Key.S => ReplayEditorMenu.SaveReplay,
					Key.I => ReplayEditorMenu.InjectReplay,
					Key.G => ReplayEditorMenu.OpenReplayFromGameMemory,
					_ => null,
				};
				action?.Invoke();
			}
			else
			{
				Action? action = key switch
				{
					Key.O => ReplayEditorMenu.OpenLeaderboardReplay,
					_ => null,
				};
				action?.Invoke();
			}
		}
	}
}
