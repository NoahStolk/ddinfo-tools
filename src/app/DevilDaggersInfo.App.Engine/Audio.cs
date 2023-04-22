using DevilDaggersInfo.App.Engine.Maths.Numerics;
using Silk.NET.OpenAL;

namespace DevilDaggersInfo.App.Engine;

public static class Audio
{
	private static AL? _al;
	private static ALContext? _alc;
	private static unsafe Device* _device;
	private static unsafe Context* _context;

	public static AL Al => _al ?? throw new InvalidOperationException("Audio engine is not initialized.");

	public static Vector3 ListenerPosition
	{
		get
		{
			Al.GetListenerProperty(ListenerVector3.Position, out Vector3 listenerPosition);
			return listenerPosition;
		}
		set => Al.SetListenerProperty(ListenerVector3.Position, value);
	}

	public static Vector3 ListenerVelocity
	{
		get
		{
			Al.GetListenerProperty(ListenerVector3.Velocity, out Vector3 listenerVelocity);
			return listenerVelocity;
		}
		set => Al.SetListenerProperty(ListenerVector3.Velocity, value);
	}

	public static unsafe Orientation ListenerOrientation
	{
		get
		{
			float[] values = new float[6];
			fixed (float* v = &values[0])
				Al.GetListenerProperty(ListenerFloatArray.Orientation, v);

			return new()
			{
				At = new() { X = values[0], Y = values[1], Z = values[2] },
				Up = new() { X = values[3], Y = values[4], Z = values[5] },
			};
		}
		set
		{
			float[] values = { value.At.X, value.At.Y, value.At.Z, value.Up.X, value.Up.Y, value.Up.Z };
			fixed (float* v = &values[0])
				Al.SetListenerProperty(ListenerFloatArray.Orientation, v);
		}
	}

	public static unsafe void Initialize()
	{
		_alc = ALContext.GetApi();
		_al = AL.GetApi();
		_device = _alc.OpenDevice(string.Empty);
		if (_device == null)
			throw new InvalidOperationException("Could not create audio device.");

		_context = _alc.CreateContext(_device, null);
		_alc.MakeContextCurrent(_context);

		_al.GetError();

		ListenerOrientation = new()
		{
			At = new() { X = 0.0f, Y = 0.0f, Z = 1.0f },
			Up = new() { X = 0.0f, Y = 1.0f, Z = 0.0f },
		};
	}

	public static unsafe void Destroy()
	{
		if (_alc == null || _al == null)
			throw new InvalidOperationException("Cannot destroy null ALContext and AL.");

		_alc.DestroyContext(_context);
		_alc.CloseDevice(_device);
		_al.Dispose();
		_alc.Dispose();
	}
}
