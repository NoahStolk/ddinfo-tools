using DevilDaggersInfo.App.Engine.Maths;

namespace DevilDaggersInfo.App.Engine.InterpolationStates;

public class FloatState : AbstractInterpolationState<float>
{
	public FloatState(float start)
		: base(start)
	{
	}

	public override void PrepareRender()
		=> Render = MathUtils.Lerp(PhysicsPrevious, Physics, EngineNodes.Game.SubFrame);
}
