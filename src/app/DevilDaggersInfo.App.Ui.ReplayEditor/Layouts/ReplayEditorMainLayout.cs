using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.ReplayEditor.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.App.Ui.ReplayEditor.Components;
using DevilDaggersInfo.App.Ui.ReplayEditor.Utils;
using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.GLFW;
using Warp.NET;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Layouts;

public class ReplayEditorMainLayout : Layout, IExtendedLayout
{
	public ReplayEditorMainLayout()
	{
		Menu menu = new(new PixelBounds(0, 0, 1024, 768));
		TextButton button3d = new(new PixelBounds(64, 64, 128, 32), LoadReplay3d, ButtonStyles.Default, TextButtonStyles.DefaultMiddle, "3D");

		NestingContext.Add(menu);
		NestingContext.Add(button3d);

		void LoadReplay3d()
		{
			StateManager.Dispatch(new SetLayout(Root.Dependencies.ReplayEditor3dLayout));
			StateManager.Dispatch(new BuildReplayScene());
		}
	}

	public void Update()
	{
		if (!Input.IsCtrlHeld())
			return;

		if (Input.IsKeyPressed(Keys.N))
			StateManager.Dispatch(new LoadSpawnset("(untitled)", SpawnsetBinary.CreateDefault()));
		else if (Input.IsKeyPressed(Keys.O))
			ReplayFileUtils.OpenReplay();
		else if (Input.IsKeyPressed(Keys.S))
			ReplayFileUtils.SaveReplay();
	}

	public void Render3d()
	{
	}

	public void Render()
	{
		Vector2i<int> windowSize = new(CurrentWindowState.Width, CurrentWindowState.Height);
		Root.Game.RectangleRenderer.Schedule(windowSize, windowSize / 2, -100, Color.Gray(0.1f));
	}
}
