using Silk.NET.GLFW;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

internal sealed class GlfwInput
{
	private readonly Dictionary<MouseButton, InputAction> _mouseButtons = new();
	private readonly List<MouseButton> _mouseButtonsChanged = [];

	private readonly Dictionary<Keys, InputAction> _keys = [];
	private readonly List<Keys> _keysChanged = [];

	private readonly List<uint> _charsPressed = [];

	public Vector2 CursorPosition { get; private set; }
	public float MouseWheelY { get; private set; }

	public IReadOnlyList<MouseButton> MouseButtonsChanged => _mouseButtonsChanged;
	public IReadOnlyList<Keys> KeysChanged => _keysChanged;
	public IReadOnlyList<uint> CharsPressed => _charsPressed;

	#region Callbacks

	public void CursorPosCallback(double x, double y)
	{
		CursorPosition = new Vector2((float)x, (float)y);
	}

	public void MouseWheelCallback(double deltaY)
	{
		MouseWheelY = (float)deltaY;
	}

	public void MouseButtonCallback(MouseButton button, InputAction state)
	{
		_mouseButtonsChanged.Add(button);
		_mouseButtons[button] = state;
	}

	public void KeyCallback(Keys key, InputAction state)
	{
		_keysChanged.Add(key);
		_keys[key] = state;
	}

	public void CharCallback(uint codepoint)
	{
		_charsPressed.Add(codepoint);
	}

	#endregion Callbacks

	public bool IsMouseButtonDown(MouseButton button)
	{
		return _mouseButtons.TryGetValue(button, out InputAction inputAction) && inputAction == InputAction.Press;
	}

	public bool IsMouseButtonPressed(MouseButton button)
	{
		return _mouseButtonsChanged.Contains(button) && IsMouseButtonDown(button);
	}

	public bool IsMouseButtonReleased(MouseButton button)
	{
		return _mouseButtonsChanged.Contains(button) && !IsMouseButtonDown(button);
	}

	public bool IsKeyDown(Keys key)
	{
		return _keys.TryGetValue(key, out InputAction inputAction) && inputAction is InputAction.Press or InputAction.Repeat;
	}

	public bool IsKeyRepeating(Keys key)
	{
		return _keys.TryGetValue(key, out InputAction inputAction) && inputAction == InputAction.Repeat;
	}

	public bool IsKeyPressed(Keys key)
	{
		return _keysChanged.Contains(key) && _keys.TryGetValue(key, out InputAction inputAction) && inputAction == InputAction.Press;
	}

	public bool IsKeyReleased(Keys key)
	{
		return _keysChanged.Contains(key) && _keys.TryGetValue(key, out InputAction inputAction) && inputAction == InputAction.Release;
	}

	public void EndFrame()
	{
		_mouseButtonsChanged.Clear();
		_keysChanged.Clear();
		_charsPressed.Clear();
		MouseWheelY = 0;
	}
}
