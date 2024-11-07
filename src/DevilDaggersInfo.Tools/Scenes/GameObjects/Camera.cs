using DevilDaggersInfo.Tools.Engine.Intersections;
using DevilDaggersInfo.Tools.Engine.Maths;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.User.Settings;
using ImGuiGlfw;
using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

public unsafe class Camera
{
	private const MouseButton _lookButton = MouseButton.Right;
	private const float _friction = 20;

	private readonly Glfw _glfw;
	private readonly GlfwInput _glfwInput;
	private readonly WindowHandle* _window;
	private readonly bool _isMenuCamera;

	private float _totalTime;

	private Quaternion _rotationState = Quaternion.Identity;

	private Vector3 _speed;
	private float _yaw;
	private float _pitch;
	private Vector2D<int>? _lockedMousePosition;

	private int _windowWidth;
	private int _windowHeight;

	public Camera(Glfw glfw, GlfwInput glfwInput, WindowHandle* window, bool isMenuCamera)
	{
		_glfw = glfw;
		_glfwInput = glfwInput;
		_window = window;
		_isMenuCamera = isMenuCamera;
	}

	public Matrix4x4 Projection { get; private set; }
	public Matrix4x4 ViewMatrix { get; private set; }

	public Vector3 Position { get; set; }

	public Vector2 FramebufferOffset { get; set; }

	public void Update(bool activateMouse, bool activateKeyboard, float delta)
	{
		if (_isMenuCamera)
		{
			_totalTime += delta;
			float time = _totalTime * 0.7f;
			Position = new Vector3(MathF.Sin(time) * 5, 6, MathF.Cos(time) * 5);
			_rotationState = Quaternion.CreateFromRotationMatrix(SetRotationFromDirectionalVector(new Vector3(0, 4, 0) - Position));
			return;
		}

		if (activateKeyboard)
		{
			ImGuiIOPtr io = ImGui.GetIO();
			HandleKeys(io, delta);
		}
		else
		{
			float frictionDt = _friction * delta;
			_speed.X -= _speed.X * frictionDt;
			_speed.Y -= _speed.Y * frictionDt;
			_speed.Z -= _speed.Z * frictionDt;
		}

		if (activateMouse)
		{
			HandleMouse();
		}

		const float maxSpeed = 50;
		Position += _speed * maxSpeed * delta;
	}

	private void HandleKeys(ImGuiIOPtr io, float delta)
	{
		const float acceleration = 20;
		const ImGuiKey forwardInput = ImGuiKey.W;
		const ImGuiKey leftInput = ImGuiKey.A;
		const ImGuiKey backwardInput = ImGuiKey.S;
		const ImGuiKey rightInput = ImGuiKey.D;
		const ImGuiKey upInput = ImGuiKey.E;
		const ImGuiKey downInput = ImGuiKey.Q;
		bool forwardHold = io.IsKeyDown(forwardInput);
		bool leftHold = io.IsKeyDown(leftInput);
		bool backwardHold = io.IsKeyDown(backwardInput);
		bool rightHold = io.IsKeyDown(rightInput);
		bool upHold = io.IsKeyDown(upInput);
		bool downHold = io.IsKeyDown(downInput);

		float accelerationDt = acceleration * delta;
		float frictionDt = _friction * delta;

		if (leftHold)
			_speed += Vector3.Transform(Vector3.UnitX, _rotationState) * accelerationDt;
		if (rightHold)
			_speed -= Vector3.Transform(Vector3.UnitX, _rotationState) * accelerationDt;

		if (upHold)
			_speed += Vector3.Transform(Vector3.UnitY, _rotationState) * accelerationDt;
		if (downHold)
			_speed -= Vector3.Transform(Vector3.UnitY, _rotationState) * accelerationDt;

		if (forwardHold)
			_speed += Vector3.Transform(Vector3.UnitZ, _rotationState) * accelerationDt;
		if (backwardHold)
			_speed -= Vector3.Transform(Vector3.UnitZ, _rotationState) * accelerationDt;

		if (_speed.LengthSquared() > 0)
		{
			_speed.X -= _speed.X * frictionDt;
			_speed.Y -= _speed.Y * frictionDt;
			_speed.Z -= _speed.Z * frictionDt;
		}

		_speed.X = Math.Clamp(_speed.X, -1, 1);
		_speed.Y = Math.Clamp(_speed.Y, -1, 1);
		_speed.Z = Math.Clamp(_speed.Z, -1, 1);
	}

	private void HandleMouse()
	{
		if (_glfwInput.IsMouseButtonReleased(_lookButton))
		{
			_lockedMousePosition = null;
			_glfw.SetInputMode(_window, CursorStateAttribute.Cursor, CursorModeValue.CursorNormal);
		}

		if (_glfwInput.IsMouseButtonPressed(_lookButton))
		{
			_lockedMousePosition = FloorToVector2Int32(_glfwInput.CursorPosition);
			_glfw.SetInputMode(_window, CursorStateAttribute.Cursor, CursorModeValue.CursorHidden);
		}

		Vector2D<int> mousePosition = FloorToVector2Int32(_glfwInput.CursorPosition);
		if (!_glfwInput.IsMouseButtonDown(_lookButton) || !_lockedMousePosition.HasValue || mousePosition == _lockedMousePosition)
			return;

		float lookSpeed = UserSettings.Model.LookSpeed;

		Vector2D<int> delta = mousePosition - _lockedMousePosition.Value;
		_yaw -= lookSpeed * delta.X * 0.0001f;
		_pitch -= lookSpeed * delta.Y * 0.0001f;

		_pitch = Math.Clamp(_pitch, MathUtils.ToRadians(-89.9f), MathUtils.ToRadians(89.9f));
		_rotationState = Quaternion.CreateFromYawPitchRoll(_yaw, -_pitch, 0);

		_glfw.SetCursorPos(_window, _lockedMousePosition.Value.X, _lockedMousePosition.Value.Y);
	}

	public void PreRender(int windowWidth, int windowHeight)
	{
		_windowWidth = windowWidth;
		_windowHeight = windowHeight;

		Vector3 upDirection = Vector3.Transform(Vector3.UnitY, _rotationState);
		Vector3 lookDirection = Vector3.Transform(Vector3.UnitZ, _rotationState);
		ViewMatrix = Matrix4x4.CreateLookAt(Position, Position + lookDirection, upDirection);

		float aspectRatio = windowWidth / (float)windowHeight;

		const float nearPlaneDistance = 0.05f;
		const float farPlaneDistance = 10_000f;
		Projection = Matrix4x4.CreatePerspectiveFieldOfView(MathUtils.ToRadians(UserSettings.Model.FieldOfView), aspectRatio, nearPlaneDistance, farPlaneDistance);
	}

	private static Matrix4x4 SetRotationFromDirectionalVector(Vector3 direction)
	{
		Vector3 m3 = Vector3.Normalize(direction);
		Vector3 m1 = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, m3));
		Vector3 m2 = Vector3.Normalize(Vector3.Cross(m3, m1));

		Matrix4x4 matrix = Matrix4x4.Identity;

		matrix.M11 = m1.X;
		matrix.M12 = m1.Y;
		matrix.M13 = m1.Z;

		matrix.M21 = m2.X;
		matrix.M22 = m2.Y;
		matrix.M23 = m2.Z;

		matrix.M31 = m3.X;
		matrix.M32 = m3.Y;
		matrix.M33 = m3.Z;

		return matrix;
	}

	public Ray ScreenToWorldPoint()
	{
		float aspectRatio = _windowWidth / (float)_windowHeight;

		// Remap so (0, 0) is the center of the window and the edges are at -0.5 and +0.5.
		Vector2 mousePosition = _glfwInput.CursorPosition - FramebufferOffset;
		Vector2 relative = -new Vector2(mousePosition.X / _windowWidth - 0.5f, mousePosition.Y / _windowHeight - 0.5f);

		// Angle in radians from the view axis to the top plane of the view pyramid.
		float verticalAngle = 0.5f * MathUtils.ToRadians(UserSettings.Model.FieldOfView);

		// World space height of the view pyramid measured at 1m depth from the camera.
		float worldHeight = 2f * MathF.Tan(verticalAngle);

		// Convert relative position to world units.
		Vector2 temp = relative * worldHeight;
		Vector3 worldUnits = new(temp.X * aspectRatio, temp.Y, 1);

		// Rotate to match camera orientation.
		Vector3 direction = Vector3.Transform(worldUnits, _rotationState);

		// Output a ray from camera position, along this direction.
		return new Ray(Position, direction);
	}

	private static Vector2D<int> FloorToVector2Int32(Vector2 vector)
	{
		return new Vector2D<int>((int)MathF.Floor(vector.X), (int)MathF.Floor(vector.Y));
	}
}
