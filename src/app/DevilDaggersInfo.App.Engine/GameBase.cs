using DevilDaggersInfo.App.Engine.Debugging;

namespace DevilDaggersInfo.App.Engine;

public abstract class GameBase
{
	private const int _ticksInSecond = 10000000;
	private const float _maxMainDelta = 0.25f;

	private readonly Stopwatch _updateLoopTimer = new();

	private DateTime _currentTime = DateTime.Now;
	private float _accumulator;

	private float _updateRate;
	private float _mainLoopRate;

	private float _updateLength;
	private float _mainLoopLength;

	private int _currentSecond;
	private int _updates;
	private int _renders;

	protected GameBase()
	{
		UpdateRate = 60;
		MainLoopRate = 300;
	}

	public float UpdateRate
	{
		get => _updateRate;
		set
		{
			_updateRate = value;
			_updateLength = 1 / _updateRate;
		}
	}

	public float MainLoopRate
	{
		get => _mainLoopRate;
		set
		{
			_mainLoopRate = value;
			_mainLoopLength = 1 / _mainLoopRate;
		}
	}

	/// <summary>
	/// Represents the delta time in seconds.
	/// </summary>
	public float Dt { get; private set; }

	/// <summary>
	/// Represents the total elapsed time in seconds.
	/// </summary>
	public float Tt { get; private set; }

	public float SubFrame { get; private set; }

	public float TimeMultiplier { get; set; } = 1;

	public float AudioVolume { get; set; } = 1;

	protected int Tps { get; private set; }
	protected int Fps { get; private set; }

	public unsafe void Run()
	{
		while (!Graphics.Glfw.WindowShouldClose(Graphics.Window))
		{
			DateTime expectedNextFrame = DateTime.Now + TimeSpan.FromSeconds(_mainLoopLength);
			Main();

			while (DateTime.Now < expectedNextFrame)
				Thread.Yield();
		}

		Graphics.Glfw.Terminate();
	}

	private unsafe void Main()
	{
		DateTime now = DateTime.Now;

		if (_currentSecond != now.Second)
		{
			Tps = _updates;
			Fps = _renders;
			_updates = 0;
			_renders = 0;
			_currentSecond = now.Second;
		}

		float frameTime = (now - _currentTime).Ticks / (float)_ticksInSecond;
		if (frameTime > _maxMainDelta)
			frameTime = _maxMainDelta;

		_currentTime = now;

		_accumulator += frameTime;

		Dt = _updateLength * TimeMultiplier;

		Graphics.Glfw.PollEvents();

		while (_accumulator >= _updateLength)
		{
			_updateLoopTimer.Restart();

			DebugStack.Update(Dt);

			_updates++;
			Update();

			Input.PostUpdate();

			Tt += Dt;
			_accumulator -= _updateLength;
		}

		SubFrame = _updateLoopTimer.Elapsed.Ticks / (float)_ticksInSecond * UpdateRate;

		PrepareRender();

		_renders++;
		Render();

		Graphics.Glfw.SwapBuffers(Graphics.Window);
		DebugStack.EndRender();
	}

	protected abstract void Update();

	protected abstract void PrepareRender();

	protected abstract void Render();
}
