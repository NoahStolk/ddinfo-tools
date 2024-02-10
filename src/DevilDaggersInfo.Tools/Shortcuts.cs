using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.AssetEditor;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.ReplayEditor;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
using ImGuiGlfw;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.Tools;

public static class Shortcuts
{
	// TODO: Call from GLFW callback.
	public static void OnKeyPressed(GlfwInput glfwInput, Keys key)
	{
		if (PopupManager.IsAnyOpen)
			return;

		bool ctrl = glfwInput.IsKeyPressed(Keys.ControlLeft) || glfwInput.IsKeyPressed(Keys.ControlRight);
		bool shift = glfwInput.IsKeyPressed(Keys.ShiftLeft) || glfwInput.IsKeyPressed(Keys.ShiftRight);

		if (key == Keys.Escape)
		{
			switch (UiRenderer.Layout)
			{
				case LayoutType.SpawnsetEditor: SpawnsetEditorMenu.Close(); break;
				case LayoutType.AssetEditor: AssetEditorMenu.Close(); break;
				case LayoutType.ReplayEditor: ReplayEditorMenu.Close(); break;
				case LayoutType.Config: break;
				default: UiRenderer.Layout = LayoutType.Main; break;
			}
		}

		switch (UiRenderer.Layout)
		{
			case LayoutType.SpawnsetEditor: HandleSpawnsetEditorShortcuts(key, ctrl, shift); break;
			case LayoutType.AssetEditor: HandleAssetEditorShortcuts(key, ctrl, shift); break;
			case LayoutType.ReplayEditor: HandleReplayEditorShortcuts(key, ctrl, shift); break;
		}
	}

	private static void HandleSpawnsetEditorShortcuts(Keys key, bool ctrl, bool shift)
	{
		if (ctrl)
		{
			if (!shift)
			{
				Action? action = key switch
				{
					Keys.N => SpawnsetEditorMenu.NewSpawnset,
					Keys.O => SpawnsetEditorMenu.OpenSpawnset,
					Keys.S => SpawnsetEditorMenu.SaveSpawnset,
					Keys.R => SpawnsetEditorMenu.ReplaceCurrentSpawnset,
					Keys.D => SpawnsetEditorMenu.DeleteCurrentSpawnset,
					_ => null,
				};
				action?.Invoke();
			}
			else
			{
				Action? action = key switch
				{
					Keys.O => SpawnsetEditorMenu.OpenCurrentSpawnset,
					Keys.S => SpawnsetEditorMenu.SaveSpawnsetAs,
					Keys.D => SpawnsetEditorMenu.OpenDefaultSpawnset,
					_ => null,
				};
				action?.Invoke();
			}
		}
	}

	private static void HandleAssetEditorShortcuts(Keys key, bool ctrl, bool shift)
	{
		if (ctrl)
		{
			if (!shift)
			{
				Action? action = key switch
				{
					Keys.N => AssetEditorMenu.NewMod,
					Keys.O => AssetEditorMenu.OpenMod,
					Keys.S => AssetEditorMenu.SaveMod,
					_ => null,
				};
				action?.Invoke();
			}
			else
			{
				Action? action = key switch
				{
					Keys.S => AssetEditorMenu.SaveModAs,
					_ => null,
				};
				action?.Invoke();
			}
		}
	}

	private static void HandleReplayEditorShortcuts(Keys key, bool ctrl, bool shift)
	{
		if (ctrl)
		{
			if (!shift)
			{
				Action? action = key switch
				{
					Keys.N => ReplayEditorMenu.NewReplay,
					Keys.O => ReplayEditorMenu.OpenReplay,
					Keys.S => ReplayEditorMenu.SaveReplay,
					Keys.I => ReplayEditorMenu.InjectReplay,
					Keys.G => ReplayEditorMenu.OpenReplayFromGameMemory,
					_ => null,
				};
				action?.Invoke();
			}
			else
			{
				Action? action = key switch
				{
					Keys.O => ReplayEditorMenu.OpenLeaderboardReplay,
					_ => null,
				};
				action?.Invoke();
			}
		}
	}
}
