using DevilDaggersInfo.App.Ui.Base.User.Settings;
using Warp.NET;

namespace DevilDaggersInfo.App.Ui.Base.Rendering;

public static class ViewportState
{
	public static Viewport Viewport3d { get; private set; }

	public static Viewport Viewport { get; private set; }

	public static Vector2 Offset { get; private set; }

	public static Vector2 Scale { get; private set; }

	public static Vector2 MousePosition => (Input.GetMousePosition() - Offset) / Scale;

	public static void UpdateViewports(int width, int height)
	{
		Viewport3d = new(0, 0, width, height);

		const float nativeAspectRatio = Constants.NativeWidth / (float)Constants.NativeHeight;

		if (UserSettings.Model.ScaleUiToWindow)
		{
			int originalWidth = width;
			int originalHeight = height;
			if (width < height * nativeAspectRatio)
				height = (int)(width / nativeAspectRatio); // Adjusted for aspect ratio
			else
				width = (int)(height * nativeAspectRatio); // Adjusted for aspect ratio

			int leftOffset = (originalWidth - width) / 2;
			int bottomOffset = (originalHeight - height) / 2;
			Offset = new(leftOffset, bottomOffset);
			Viewport = new(leftOffset, bottomOffset, width, height); // Fix viewport to maintain aspect ratio
			Scale = new(Viewport.Width / (float)Constants.NativeWidth, Viewport.Height / (float)Constants.NativeHeight);
		}
		else
		{
			int minDimension = (int)Math.Min(height, width / nativeAspectRatio);
			int clampedHeight = Math.Max(Constants.NativeHeight, minDimension / Constants.NativeHeight * Constants.NativeHeight);

			float adjustedWidth = clampedHeight * nativeAspectRatio; // Adjusted for aspect ratio

			int leftOffset = (int)((width - adjustedWidth) / 2);
			int bottomOffset = (height - clampedHeight) / 2;
			Offset = new(leftOffset, bottomOffset);
			Viewport = new(leftOffset, bottomOffset, (int)adjustedWidth, clampedHeight); // Fix viewport to maintain aspect ratio
			Scale = new(Viewport.Width / (float)Constants.NativeWidth, Viewport.Height / (float)Constants.NativeHeight);
		}
	}
}
