using DevilDaggersInfo.App.Engine.Intersections;
using DevilDaggersInfo.App.Engine.Maths;
using DevilDaggersInfo.App.User.Settings;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using System.Numerics;

namespace DevilDaggersInfo.App.Scenes.GameObjects;

public class Camera
{
	private const MouseButton _lookButton = MouseButton.Right;

	private readonly bool _isMenuCamera;

	private Quaternion _rotationState = Quaternion.Identity;

	private Vector3 _axisAlignedSpeed;
	private float _yaw;
	private float _pitch;
	private Vector2D<int>? _lockedMousePosition;

	private int _windowWidth;
	private int _windowHeight;

	public Camera(bool isMenuCamera)
	{
		if (Root.Mouse != null)
		{
			Root.Mouse.MouseDown += OnMouseDown;
			Root.Mouse.MouseUp += OnMouseUp;
		}

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
			float time = (float)Root.Window.Time * 0.7f;
			Position = new(MathF.Sin(time) * 5, 6, MathF.Cos(time) * 5);
			_rotationState = Quaternion.CreateFromRotationMatrix(SetRotationFromDirectionalVector(new Vector3(0, 4, 0) - Position));
			return;
		}

		if (activateKeyboard)
		{
			ImGuiIOPtr io = ImGui.GetIO();
			HandleKeys(io, delta);
		}

		if (activateMouse)
		{
			HandleMouse();
		}

		const float moveSpeed = 25;

		Matrix4x4 rotMat = Matrix4x4.CreateFromQuaternion(_rotationState);
		Vector3 transformed = RotateVector(_axisAlignedSpeed, rotMat) + new Vector3(0, _axisAlignedSpeed.Y, 0);
		Position += transformed * moveSpeed * delta;

		static Vector3 RotateVector(Vector3 vector, Matrix4x4 rotationMatrix)
		{
			Vector3 right = new(rotationMatrix.M11, rotationMatrix.M12, rotationMatrix.M13);
			Vector3 forward = -Vector3.Cross(Vector3.UnitY, right);
			return right * vector.X + forward * vector.Z;
		}
	}

	private void HandleKeys(ImGuiIOPtr io, float delta)
	{
		const float acceleration = 20;
		const float friction = 20;
		const Key forwardInput = Key.W;
		const Key leftInput = Key.A;
		const Key backwardInput = Key.S;
		const Key rightInput = Key.D;
		const Key upInput = Key.Space;
		const Key downInput = Key.ShiftLeft;
		bool forwardHold = io.KeysDown[(int)forwardInput];
		bool leftHold = io.KeysDown[(int)leftInput];
		bool backwardHold = io.KeysDown[(int)backwardInput];
		bool rightHold = io.KeysDown[(int)rightInput];
		bool upHold = io.KeysDown[(int)upInput];
		bool downHold = io.KeysDown[(int)downInput];

		float accelerationDt = acceleration * delta;
		float frictionDt = friction * delta;

		if (leftHold)
			_axisAlignedSpeed.X += accelerationDt;
		if (rightHold)
			_axisAlignedSpeed.X -= accelerationDt;

		if (upHold)
			_axisAlignedSpeed.Y += accelerationDt;
		if (downHold)
			_axisAlignedSpeed.Y -= accelerationDt;

		if (forwardHold)
			_axisAlignedSpeed.Z += accelerationDt;
		if (backwardHold)
			_axisAlignedSpeed.Z -= accelerationDt;

		if (!leftHold && !rightHold)
			_axisAlignedSpeed.X -= _axisAlignedSpeed.X * frictionDt;

		if (!upHold && !downHold)
			_axisAlignedSpeed.Y -= _axisAlignedSpeed.Y * frictionDt;

		if (!forwardHold && !backwardHold)
			_axisAlignedSpeed.Z -= _axisAlignedSpeed.Z * frictionDt;

		_axisAlignedSpeed.X = Math.Clamp(_axisAlignedSpeed.X, -1, 1);
		_axisAlignedSpeed.Y = Math.Clamp(_axisAlignedSpeed.Y, -1, 1);
		_axisAlignedSpeed.Z = Math.Clamp(_axisAlignedSpeed.Z, -1, 1);
	}

	private void OnMouseDown(IMouse mouse, MouseButton mouseButton)
	{
		if (Root.Mouse == null || mouseButton != _lookButton)
			return;

		_lockedMousePosition = FloorToVector2Int32(Root.Mouse.Position);
		Root.Mouse.Cursor.CursorMode = CursorMode.Hidden;
	}

	private void OnMouseUp(IMouse mouse, MouseButton mouseButton)
	{
		if (Root.Mouse == null)
			return;

		_lockedMousePosition = null;
		Root.Mouse.Cursor.CursorMode = CursorMode.Normal;
	}

	private void HandleMouse()
	{
		if (Root.Mouse == null)
			return;

		Vector2D<int> mousePosition = FloorToVector2Int32(Root.Mouse.Position);
		if (!Root.Mouse.IsButtonPressed(_lookButton) || !_lockedMousePosition.HasValue || mousePosition == _lockedMousePosition)
			return;

		float lookSpeed = UserSettings.Model.LookSpeed;

		Vector2D<int> delta = mousePosition - _lockedMousePosition.Value;
		_yaw -= lookSpeed * delta.X * 0.0001f;
		_pitch -= lookSpeed * delta.Y * 0.0001f;

		_pitch = Math.Clamp(_pitch, MathUtils.ToRadians(-89.9f), MathUtils.ToRadians(89.9f));
		_rotationState = Quaternion.CreateFromYawPitchRoll(_yaw, -_pitch, 0);

		Root.Mouse.Position = new(_lockedMousePosition.Value.X, _lockedMousePosition.Value.Y);
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
		Vector2 mousePosition = Root.Mouse == null ? default : Root.Mouse.Position - FramebufferOffset;
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
		return new(Position, direction);
	}

	private static Vector2D<int> FloorToVector2Int32(Vector2 vector)
		=> new((int)MathF.Floor(vector.X), (int)MathF.Floor(vector.Y));
}
