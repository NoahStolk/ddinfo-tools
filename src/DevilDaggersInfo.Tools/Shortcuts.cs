using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.AssetEditor;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.ReplayEditor;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
using ImGuiNET;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.Tools;

internal sealed class Shortcuts(
	UiLayoutManager uiLayoutManager,
	SpawnsetEditorMenu spawnsetEditorMenu,
	AssetEditorMenu assetEditorMenu,
	ReplayEditorMenu replayEditorMenu,
	PopupManager popupManager,
	FileStates fileStates)
{
	public void Handle(ImGuiIOPtr io, GlfwInput glfwInput)
	{
		if (io.WantTextInput)
			return;

		if (popupManager.IsAnyOpen)
			return;

		bool ctrl = glfwInput.IsKeyDown(Keys.ControlLeft) || glfwInput.IsKeyDown(Keys.ControlRight);
		bool shift = glfwInput.IsKeyDown(Keys.ShiftLeft) || glfwInput.IsKeyDown(Keys.ShiftRight);

		if (glfwInput.IsKeyPressed(Keys.Escape))
		{
			switch (uiLayoutManager.Layout)
			{
				case LayoutType.SpawnsetEditor: spawnsetEditorMenu.Close(); break;
				case LayoutType.AssetEditor: assetEditorMenu.Close(); break;
				case LayoutType.ReplayEditor: replayEditorMenu.Close(); break;
				case LayoutType.Config: break;
				default: uiLayoutManager.Layout = LayoutType.Main; break;
			}
		}

		switch (uiLayoutManager.Layout)
		{
			case LayoutType.SpawnsetEditor: HandleSpawnsetEditorShortcuts(glfwInput, ctrl, shift); break;
			case LayoutType.AssetEditor: HandleAssetEditorShortcuts(glfwInput, ctrl, shift); break;
			case LayoutType.ReplayEditor: HandleReplayEditorShortcuts(glfwInput, ctrl, shift); break;
		}
	}

	private void HandleSpawnsetEditorShortcuts(GlfwInput glfwInput, bool ctrl, bool shift)
	{
		if (ctrl)
		{
			if (!shift)
			{
				if (glfwInput.IsKeyPressed(Keys.N))
					spawnsetEditorMenu.NewSpawnset();
				else if (glfwInput.IsKeyPressed(Keys.O))
					spawnsetEditorMenu.OpenSpawnset();
				else if (glfwInput.IsKeyPressed(Keys.S))
					fileStates.SaveSpawnset();
				else if (glfwInput.IsKeyPressed(Keys.R))
					spawnsetEditorMenu.ReplaceCurrentSpawnset();
				else if (glfwInput.IsKeyPressed(Keys.D))
					spawnsetEditorMenu.DeleteCurrentSpawnset();
			}
			else
			{
				if (glfwInput.IsKeyPressed(Keys.O))
					spawnsetEditorMenu.OpenCurrentSpawnset();
				else if (glfwInput.IsKeyPressed(Keys.S))
					fileStates.SaveSpawnsetAs();
				else if (glfwInput.IsKeyPressed(Keys.D))
					spawnsetEditorMenu.OpenDefaultSpawnset();
			}
		}
	}

	private void HandleAssetEditorShortcuts(GlfwInput glfwInput, bool ctrl, bool shift)
	{
		if (ctrl)
		{
			if (!shift)
			{
				if (glfwInput.IsKeyPressed(Keys.N))
					assetEditorMenu.NewMod();
				else if (glfwInput.IsKeyPressed(Keys.O))
					assetEditorMenu.OpenMod();
				else if (glfwInput.IsKeyPressed(Keys.S))
					assetEditorMenu.SaveMod();
			}
			else
			{
				if (glfwInput.IsKeyPressed(Keys.S))
					assetEditorMenu.SaveModAs();
			}
		}
	}

	private void HandleReplayEditorShortcuts(GlfwInput glfwInput, bool ctrl, bool shift)
	{
		if (ctrl)
		{
			if (!shift)
			{
				if (glfwInput.IsKeyPressed(Keys.N))
					replayEditorMenu.NewReplay();
				else if (glfwInput.IsKeyPressed(Keys.O))
					replayEditorMenu.OpenReplay();
				else if (glfwInput.IsKeyPressed(Keys.S))
					replayEditorMenu.SaveReplay();
				else if (glfwInput.IsKeyPressed(Keys.I))
					replayEditorMenu.InjectReplay();
				else if (glfwInput.IsKeyPressed(Keys.G))
					replayEditorMenu.OpenReplayFromGameMemory();
			}
			else
			{
				if (glfwInput.IsKeyPressed(Keys.O))
					replayEditorMenu.OpenLeaderboardReplay();
			}
		}
	}
}
