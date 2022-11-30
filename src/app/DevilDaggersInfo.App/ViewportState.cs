namespace DevilDaggersInfo.App;

public static class ViewportState
{
	public static Viewport Viewport { get; set; }

	public static Vector2 Offset { get; set; }

	public static Vector2 Scale { get; set; }

	public static Vector2 MousePosition => (Input.GetMousePosition() - Offset) / Scale;
}
