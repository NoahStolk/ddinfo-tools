using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.AssetEditor;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.ReplayEditor;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
using ImGuiGlfw;
using ImGuiNET;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.Tools;

public sealed class Shortcuts
{
	private readonly UiLayoutManager _uiLayoutManager;
	private readonly SpawnsetEditorMenu _spawnsetEditorMenu;
	private readonly AssetEditorMenu _assetEditorMenu;
	private readonly ReplayEditorMenu _replayEditorMenu;

	public Shortcuts(UiLayoutManager uiLayoutManager, SpawnsetEditorMenu spawnsetEditorMenu, AssetEditorMenu assetEditorMenu, ReplayEditorMenu replayEditorMenu)
	{
		_uiLayoutManager = uiLayoutManager;
		_spawnsetEditorMenu = spawnsetEditorMenu;
		_assetEditorMenu = assetEditorMenu;
		_replayEditorMenu = replayEditorMenu;
	}

	public void Handle(ImGuiIOPtr io, GlfwInput glfwInput)
	{
		if (io.WantTextInput)
			return;

		if (PopupManager.IsAnyOpen)
			return;

		bool ctrl = glfwInput.IsKeyDown(Keys.ControlLeft) || glfwInput.IsKeyDown(Keys.ControlRight);
		bool shift = glfwInput.IsKeyDown(Keys.ShiftLeft) || glfwInput.IsKeyDown(Keys.ShiftRight);

		if (glfwInput.IsKeyPressed(Keys.Escape))
		{
			switch (_uiLayoutManager.Layout)
			{
				case LayoutType.SpawnsetEditor: _spawnsetEditorMenu.Close(); break;
				case LayoutType.AssetEditor: _assetEditorMenu.Close(); break;
				case LayoutType.ReplayEditor: _replayEditorMenu.Close(); break;
				case LayoutType.Config: break;
				default: _uiLayoutManager.Layout = LayoutType.Main; break;
			}
		}

		switch (_uiLayoutManager.Layout)
		{
			case LayoutType.SpawnsetEditor: HandleSpawnsetEditorShortcuts(glfwInput, ctrl, shift); break;
			case LayoutType.AssetEditor: HandleAssetEditorShortcuts(glfwInput, ctrl, shift); break;
			case LayoutType.ReplayEditor: HandleReplayEditorShortcuts(glfwInput, ctrl, shift); break;
		}
	}

	private static void HandleSpawnsetEditorShortcuts(GlfwInput glfwInput, bool ctrl, bool shift)
	{
		if (ctrl)
		{
			if (!shift)
			{
				if (glfwInput.IsKeyPressed(Keys.N))
					SpawnsetEditorMenu.NewSpawnset();
				else if (glfwInput.IsKeyPressed(Keys.O))
					SpawnsetEditorMenu.OpenSpawnset();
				else if (glfwInput.IsKeyPressed(Keys.S))
					SpawnsetEditorMenu.SaveSpawnset();
				else if (glfwInput.IsKeyPressed(Keys.R))
					SpawnsetEditorMenu.ReplaceCurrentSpawnset();
				else if (glfwInput.IsKeyPressed(Keys.D))
					SpawnsetEditorMenu.DeleteCurrentSpawnset();
			}
			else
			{
				if (glfwInput.IsKeyPressed(Keys.O))
					SpawnsetEditorMenu.OpenCurrentSpawnset();
				else if (glfwInput.IsKeyPressed(Keys.S))
					SpawnsetEditorMenu.SaveSpawnsetAs();
				else if (glfwInput.IsKeyPressed(Keys.D))
					SpawnsetEditorMenu.OpenDefaultSpawnset();
			}
		}
	}

	private static void HandleAssetEditorShortcuts(GlfwInput glfwInput, bool ctrl, bool shift)
	{
		if (ctrl)
		{
			if (!shift)
			{
				if (glfwInput.IsKeyPressed(Keys.N))
					AssetEditorMenu.NewMod();
				else if (glfwInput.IsKeyPressed(Keys.O))
					AssetEditorMenu.OpenMod();
				else if (glfwInput.IsKeyPressed(Keys.S))
					AssetEditorMenu.SaveMod();
			}
			else
			{
				if (glfwInput.IsKeyPressed(Keys.S))
					AssetEditorMenu.SaveModAs();
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
					_replayEditorMenu.NewReplay();
				else if (glfwInput.IsKeyPressed(Keys.O))
					_replayEditorMenu.OpenReplay();
				else if (glfwInput.IsKeyPressed(Keys.S))
					ReplayEditorMenu.SaveReplay();
				else if (glfwInput.IsKeyPressed(Keys.I))
					ReplayEditorMenu.InjectReplay();
				else if (glfwInput.IsKeyPressed(Keys.G))
					_replayEditorMenu.OpenReplayFromGameMemory();
			}
			else
			{
				if (glfwInput.IsKeyPressed(Keys.O))
					ReplayEditorMenu.OpenLeaderboardReplay();
			}
		}
	}
}
