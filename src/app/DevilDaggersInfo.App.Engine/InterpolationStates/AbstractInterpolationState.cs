namespace DevilDaggersInfo.App.Engine.InterpolationStates;

public abstract class AbstractInterpolationState<TState>
	where TState : struct, IEquatable<TState>
{
	protected AbstractInterpolationState(TState start)
	{
		Start = start;
		PhysicsPrevious = start;
		Physics = start;
		Render = start;
	}

	public TState Start { get; }
	public TState PhysicsPrevious { get; private set; }
	public TState Physics { get; set; }
	public TState Render { get; protected set; }

	public void PrepareUpdate()
		=> PhysicsPrevious = Physics;

	public abstract void PrepareRender();
}
