using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using DevilDaggersInfo.Types.Core.Replays;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Components;

public class InputsVisualizer : AbstractComponent
{
	private readonly InputVisualizer _left;
	private readonly InputVisualizer _right;
	private readonly InputVisualizer _up;
	private readonly InputVisualizer _down;
	private readonly InputVisualizer _space;
	private readonly InputVisualizer _lmb;
	private readonly InputVisualizer _rmb;
	private readonly MouseVisualizer _mouse;

	public InputsVisualizer(IBounds bounds)
		: base(bounds)
	{
		_left = new(bounds.CreateNested(0, 64, 64, 64), "A");
		_right = new(bounds.CreateNested(128, 64, 64, 64), "D");
		_up = new(bounds.CreateNested(64, 0, 64, 64), "W");
		_down = new(bounds.CreateNested(64, 64, 64, 64), "S");
		_space = new(bounds.CreateNested(0, 128, 192, 64), "Space");
		_lmb = new(bounds.CreateNested(192, 0, 64, 64), "LMB");
		_rmb = new(bounds.CreateNested(256, 0, 64, 64), "RMB");
		_mouse = new(bounds.CreateNested(192, 64, 128, 128));

		NestingContext.Add(_left);
		NestingContext.Add(_right);
		NestingContext.Add(_up);
		NestingContext.Add(_down);
		NestingContext.Add(_space);
		NestingContext.Add(_lmb);
		NestingContext.Add(_rmb);
		NestingContext.Add(_mouse);
	}

	public void SetInputs(PlayerInputSnapshot snapshot)
	{
		_left.IsEnabled = snapshot.Left;
		_right.IsEnabled = snapshot.Right;
		_up.IsEnabled = snapshot.Forward;
		_down.IsEnabled = snapshot.Backward;
		_space.IsEnabled = snapshot.Jump is JumpType.StartedPress or JumpType.Hold;
		_lmb.IsEnabled = snapshot.Shoot is ShootType.Hold;
		_rmb.IsEnabled = snapshot.ShootHoming is ShootType.Hold;
		_mouse.MouseX = snapshot.MouseX;
		_mouse.MouseY = snapshot.MouseY;
	}
}
