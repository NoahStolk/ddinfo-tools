using DevilDaggersInfo.Tools.Engine.Intersections;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes;

internal static class CameraUtils
{
	private const float _nearPlaneDistance = 0.05f;
	private const float _farPlaneDistance = 10_000f;

	public static (Matrix4x4 View, Matrix4x4 Projection) CreatePerspective(Vector3 position, Vector3 target, float fieldOfView, float aspectRatio)
	{
		Matrix4x4 view = Matrix4x4.CreateLookAt(position, target, Vector3.UnitY);
		Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(float.DegreesToRadians(fieldOfView), aspectRatio, _nearPlaneDistance, _farPlaneDistance);
		return (view, projection);
	}

	public static Ray ScreenToWorldPoint(Vector2 cursorPosition, Vector2 framebufferOffset, Vector2 canvasSize, float fieldOfView, Vector3 position, Quaternion rotation)
	{
		float aspectRatio = canvasSize.X / canvasSize.Y;

		// Remap so (0, 0) is the center of the window and the edges are at -0.5 and +0.5.
		Vector2 mousePosition = cursorPosition - framebufferOffset;
		Vector2 relative = -new Vector2(mousePosition.X / canvasSize.X - 0.5f, mousePosition.Y / canvasSize.Y - 0.5f);

		// Angle in radians from the view axis to the top plane of the view pyramid.
		float verticalAngle = 0.5f * float.DegreesToRadians(fieldOfView);

		// World space height of the view pyramid measured at 1m depth from the camera.
		float worldHeight = 2f * MathF.Tan(verticalAngle);

		// Convert relative position to world units.
		Vector2 temp = relative * worldHeight;
		Vector3 worldUnits = new(temp.X * aspectRatio, temp.Y, 1);

		// Rotate to match camera orientation.
		Vector3 direction = Vector3.Transform(worldUnits, rotation);

		// Output a ray from camera position, along this direction.
		return new Ray(position, direction);
	}
}
