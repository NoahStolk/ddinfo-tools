using DevilDaggersInfo.App.Engine.Maths.Numerics;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui;

public static class ImGuiImage
{
	public static void AddFramebufferImage(this ImDrawListPtr drawListPtr, FramebufferData framebufferData, Vector2 pMin, Vector2 pMax)
	{
		// Framebuffers are flipped vertically, so we need to flip the UVs.
		drawListPtr.AddImage((IntPtr)framebufferData.TextureHandle, pMin, pMax, Vector2.UnitY, Vector2.UnitX);
	}

	public static void AddImage(this ImDrawListPtr drawListPtr, uint imageId, Vector2 pMin, Vector2 pMax)
	{
		AddImage(drawListPtr, imageId, pMin, pMax, Color.White);
	}

	public static void AddImage(this ImDrawListPtr drawListPtr, uint imageId, Vector2 pMin, Vector2 pMax, Color color)
	{
		drawListPtr.AddImage((IntPtr)imageId, pMin, pMax, Vector2.Zero, Vector2.One, ImGui.GetColorU32(color));
	}

	public static void Image(uint imageId, Vector2 size)
	{
		Image(imageId, size, Color.White);
	}

	public static void Image(uint imageId, Vector2 size, Color color)
	{
		ImGui.Image((IntPtr)imageId, size, Vector2.Zero, Vector2.One, color);
	}

	public static bool ImageButton(ReadOnlySpan<char> strId, uint imageId, Vector2 size, Color backgroundColor = default)
	{
		return ImGui.ImageButton(strId, (IntPtr)imageId, size, Vector2.Zero, Vector2.One, backgroundColor);
	}
}
