using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Engine;

public class KeySubmitter
{
	private float _keyHoldTimer;
	private float _keyHoldSubmitTimer;
	private Keys _lastPressedKey;

	public Keys? GetKey()
	{
		Keys key = Input.GetPressedKey();
		if (key is not (0 or Keys.ShiftLeft or Keys.ShiftRight or Keys.ControlLeft or Keys.ControlRight or Keys.AltLeft or Keys.AltRight))
		{
			_keyHoldTimer = 0;
			_lastPressedKey = key;
			return key;
		}

		if (Input.IsKeyHeld(_lastPressedKey))
		{
			_keyHoldTimer += EngineNodes.Game.Dt;
			_keyHoldSubmitTimer += EngineNodes.Game.Dt;
		}
		else
		{
			_keyHoldTimer = 0;
			_keyHoldSubmitTimer = 0;
		}

		if (_keyHoldTimer > 0.4f && _keyHoldSubmitTimer > 0.03f)
		{
			_keyHoldSubmitTimer = 0;
			return _lastPressedKey;
		}

		return null;
	}
}
