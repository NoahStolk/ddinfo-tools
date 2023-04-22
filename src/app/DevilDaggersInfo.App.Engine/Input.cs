using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Engine;

public static class Input
{
	private const int _maxKeys = 1024;
	private const int _maxButtons = 8;

	private static readonly bool[] _keysPrevious = new bool[_maxKeys];
	private static readonly bool[] _keysCurrent = new bool[_maxKeys];

	private static readonly bool[] _buttonsPrevious = new bool[_maxButtons];
	private static readonly bool[] _buttonsCurrent = new bool[_maxButtons];

	private static double _mouseWheel;

	public static void KeyCallback(Keys key, InputAction inputState)
	{
		if (key >= 0 && (int)key < _maxKeys)
		{
			if (inputState == InputAction.Press)
				SetKeyState(key, true);
			else if (inputState == InputAction.Release)
				SetKeyState(key, false);
		}

		static void SetKeyState(Keys key, bool pressed)
			=> _keysCurrent[(int)key] = pressed;
	}

	public static void ButtonCallback(MouseButton mouseButton, InputAction inputState)
	{
		if (mouseButton >= 0 && (int)mouseButton < _maxButtons)
		{
			if (inputState == InputAction.Press)
				SetButtonState(mouseButton, true);
			else if (inputState == InputAction.Release)
				SetButtonState(mouseButton, false);
		}

		static void SetButtonState(MouseButton mouseButton, bool pressed)
			=> _buttonsCurrent[(int)mouseButton] = pressed;
	}

	#region Keys

	public static Keys GetPressedKey() => Array.Find(Enum.GetValues<Keys>(), k => k > 0 && IsKeyPressed(k));

	public static bool IsKeyHeld(Keys key) => _keysCurrent[(int)key];

	public static bool IsKeyPressed(Keys key)
	{
		int k = (int)key;
		return _keysCurrent[k] && !_keysPrevious[k];
	}

	public static bool IsKeyReleased(Keys key)
	{
		int k = (int)key;
		return !_keysCurrent[k] && _keysPrevious[k];
	}

	public static bool IsCtrlHeld() => IsKeyHeld(Keys.ControlLeft) || IsKeyHeld(Keys.ControlRight);

	public static bool IsShiftHeld() => IsKeyHeld(Keys.ShiftLeft) || IsKeyHeld(Keys.ShiftRight);

	public static bool IsAltHeld() => IsKeyHeld(Keys.ShiftLeft) || IsKeyHeld(Keys.ShiftRight);

	public static bool IsCtrlPressed() => IsKeyPressed(Keys.ControlLeft) || IsKeyPressed(Keys.ControlRight);

	public static bool IsShiftPressed() => IsKeyPressed(Keys.ShiftLeft) || IsKeyPressed(Keys.ShiftRight);

	public static bool IsAltPressed() => IsKeyPressed(Keys.ShiftLeft) || IsKeyPressed(Keys.ShiftRight);

	public static bool IsCtrlReleased() => IsKeyReleased(Keys.ControlLeft) || IsKeyReleased(Keys.ControlRight);

	public static bool IsShiftReleased() => IsKeyReleased(Keys.ShiftLeft) || IsKeyReleased(Keys.ShiftRight);

	public static bool IsAltReleased() => IsKeyReleased(Keys.ShiftLeft) || IsKeyReleased(Keys.ShiftRight);

	#endregion Keys

	#region Buttons

	public static bool IsButtonHeld(MouseButton mouseButton)
		=> _buttonsCurrent[(int)mouseButton];

	public static bool IsButtonPressed(MouseButton mouseButton)
	{
		int b = (int)mouseButton;
		return _buttonsCurrent[b] && !_buttonsPrevious[b];
	}

	public static bool IsButtonReleased(MouseButton mouseButton)
	{
		int b = (int)mouseButton;
		return !_buttonsCurrent[b] && _buttonsPrevious[b];
	}

	#endregion Buttons

	#region Scroll

	public static void MouseWheelCallback(double delta)
		=> _mouseWheel = delta;

	public static int GetScroll()
		=> _mouseWheel > 0 ? 1 : _mouseWheel < 0 ? -1 : 0;

	#endregion Scroll

	public static unsafe Vector2 GetMousePosition()
	{
		Graphics.Glfw.GetCursorPos(Graphics.Window, out double x, out double y);
		return new((float)x, (float)y);
	}

	public static void PostUpdate()
	{
		for (int i = 0; i < _maxKeys; i++)
			_keysPrevious[i] = _keysCurrent[i];
		for (int i = 0; i < _maxButtons; i++)
			_buttonsPrevious[i] = _buttonsCurrent[i];

		_mouseWheel = 0;
	}
}
