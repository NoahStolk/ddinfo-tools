namespace DevilDaggersInfo.App.Engine.InterpolationStates;

public class Vector3State : AbstractInterpolationState<Vector3>
{
	public Vector3State(Vector3 start)
		: base(start)
	{
	}

	public override void PrepareRender()
		=> Render = Vector3.Lerp(PhysicsPrevious, Physics, EngineNodes.Game.SubFrame);
}
