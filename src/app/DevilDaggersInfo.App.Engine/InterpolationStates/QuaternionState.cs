namespace DevilDaggersInfo.App.Engine.InterpolationStates;

public class QuaternionState : AbstractInterpolationState<Quaternion>
{
	public QuaternionState(Quaternion start)
		: base(start)
	{
	}

	public override void PrepareRender()
		=> Render = Quaternion.Lerp(PhysicsPrevious, Physics, EngineNodes.Game.SubFrame);
}
