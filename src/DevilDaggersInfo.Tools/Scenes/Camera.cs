using DevilDaggersInfo.Tools.Ui;
using Silk.NET.GLFW;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes;

internal sealed unsafe class Camera(
	Glfw glfw,
	GlfwInput glfwInput,
	WindowHandle* window,
	float maxZoom = 250,
	float zoomSpeed = 1,
	float panSpeed = 0.0005f,
	float moveSpeed = 15f)
{
	private const MouseButton _lookButton = MouseButton.Right;
	private const MouseButton _panButton = MouseButton.Middle;

	private CameraMode _mode = CameraMode.None;
	private Vector2 _lastCursorPosition;
	private float _yaw = MathF.PI * 0.25f;
	private float _pitch = -0.5f;

	public float MaxZoom = maxZoom;
	public float ZoomSpeed = zoomSpeed;
	public float PanSpeed = panSpeed;
	public float MoveSpeed = moveSpeed;

	public Vector3 Position = new(0, 4, -4);
	public Vector3 Target = Vector3.Zero;
	public float FieldOfView = 75;
	public float Zoom = 4;

	private enum CameraMode
	{
		None,
		Look,
		Pan,
	}

	public Quaternion Rotation => Quaternion.CreateFromYawPitchRoll(_yaw, -_pitch, 0);

	public void SetMenuCamera(float totalTime)
	{
		const float distance = 10;
		const float height = 7;
		const float speed = 0.7f;

		float time = totalTime * speed;
		Position.X = MathF.Sin(time) * distance;
		Position.Y = height;
		Position.Z = MathF.Cos(time) * distance;

		Target = new Vector3(0, 4, 0);
	}

	public void Update(float dt, bool activateControls)
	{
		HandleMouse(activateControls);

		if (activateControls)
			HandleKeyboard(dt);

		Position = Target + Vector3.Transform(new Vector3(0, 0, -Zoom), Rotation);
	}

	private static bool IsZero(float value)
	{
		return value is < float.Epsilon and > -float.Epsilon;
	}

	private void HandleMouse(bool activateControls)
	{
		// Update Zoom (unaffected by interaction mode)
		float scroll = glfwInput.MouseWheelY;
		if (activateControls && !IsZero(scroll) && !glfwInput.IsKeyDown(Keys.ControlLeft) && !glfwInput.IsKeyDown(Keys.ControlRight))
			Zoom = Math.Clamp(Zoom - scroll * ZoomSpeed, 1, MaxZoom);

		Vector2 cursor = glfwInput.CursorPosition;

		switch (_mode)
		{
			case CameraMode.None:
				// Activate only if cursor is inside the viewport
				if (activateControls && (glfwInput.IsMouseButtonDown(_lookButton) || glfwInput.IsMouseButtonDown(_panButton)))
				{
					glfw.SetInputMode(window, CursorStateAttribute.Cursor, CursorModeValue.CursorDisabled);
					_mode = glfwInput.IsMouseButtonDown(_lookButton) ? CameraMode.Look : CameraMode.Pan;
					_lastCursorPosition = cursor;
				}

				break;

			case CameraMode.Look:
			case CameraMode.Pan:
				if (!glfwInput.IsMouseButtonDown(_lookButton) && !glfwInput.IsMouseButtonDown(_panButton))
				{
					glfw.SetInputMode(window, CursorStateAttribute.Cursor, CursorModeValue.CursorNormal);
					_mode = CameraMode.None;
					break;
				}

				Vector2 delta = cursor - _lastCursorPosition;
				_lastCursorPosition = cursor;

				if (_mode == CameraMode.Look)
					UpdateLook(delta);
				else
					UpdatePan(delta);
				break;
		}
	}

	private void UpdateLook(Vector2 delta)
	{
		const float lookSpeed = 0.002f;
		_yaw -= delta.X * lookSpeed;
		_pitch -= delta.Y * lookSpeed;
		_pitch = Math.Clamp(_pitch, -MathF.PI * 0.4999f, MathF.PI * 0.4999f);
	}

	private void UpdatePan(Vector2 delta)
	{
		float multiplier = PanSpeed * Zoom;
		Target -= Vector3.Transform(new Vector3(-delta.X * multiplier, -delta.Y * multiplier, 0), Rotation);
	}

	private void HandleKeyboard(float dt)
	{
		Quaternion rotation = Rotation; // Calculate once.

		if (glfwInput.IsKeyDown(Keys.A))
			Target += Vector3.Transform(new Vector3(MoveSpeed, 0, 0), rotation) * dt;
		if (glfwInput.IsKeyDown(Keys.D))
			Target -= Vector3.Transform(new Vector3(MoveSpeed, 0, 0), rotation) * dt;
		if (glfwInput.IsKeyDown(Keys.W))
			Target += Vector3.Transform(new Vector3(0, 0, MoveSpeed), rotation) * dt;
		if (glfwInput.IsKeyDown(Keys.S))
			Target -= Vector3.Transform(new Vector3(0, 0, MoveSpeed), rotation) * dt;
		if (glfwInput.IsKeyDown(Keys.Space))
			Target += Vector3.Transform(new Vector3(0, MoveSpeed, 0), rotation) * dt;
		if (glfwInput.IsKeyDown(Keys.ShiftLeft))
			Target -= Vector3.Transform(new Vector3(0, MoveSpeed, 0), rotation) * dt;
	}
}
