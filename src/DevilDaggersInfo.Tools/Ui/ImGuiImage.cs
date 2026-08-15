using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

internal static class ImGuiImage
{
	public static void AddFramebufferImage(this ImDrawListPtr drawListPtr, FramebufferData framebufferData, Vector2 pMin, Vector2 pMax)
	{
		drawListPtr.AddFramebufferImage(framebufferData, pMin, pMax, Color.White);
	}

	public static void AddFramebufferImage(this ImDrawListPtr drawListPtr, FramebufferData framebufferData, Vector2 pMin, Vector2 pMax, Color color)
	{
		// Framebuffers are flipped vertically, so we need to flip the UVs.
		drawListPtr.AddImage(GetTextureRef(framebufferData.TextureHandle), pMin, pMax, Vector2.UnitY, Vector2.UnitX, ImGui.GetColorU32(color));
	}

	public static void AddImage(this ImDrawListPtr drawListPtr, uint imageId, Vector2 pMin, Vector2 pMax)
	{
		AddImage(drawListPtr, imageId, pMin, pMax, Color.White);
	}

	public static void AddImage(this ImDrawListPtr drawListPtr, uint imageId, Vector2 pMin, Vector2 pMax, Color color)
	{
		drawListPtr.AddImage(GetTextureRef(imageId), pMin, pMax, Vector2.Zero, Vector2.One, ImGui.GetColorU32(color));
	}

	public static void Image(uint imageId, Vector2 size)
	{
		Image(imageId, size, Color.White);
	}

	public static void Image(uint imageId, Vector2 size, Color color)
	{
		// Dear ImGui 1.92 dropped the tint colour from ImGui.Image, so the tinted variant goes through the draw list.
		// Dummy reserves the same layout space ImGui.Image would have.
		Vector2 position = ImGui.GetCursorScreenPos();
		ImGui.GetWindowDrawList().AddImage(GetTextureRef(imageId), position, position + size, Vector2.Zero, Vector2.One, ImGui.GetColorU32(color));
		ImGui.Dummy(size);
	}

	public static bool ImageButton(ReadOnlySpan<byte> strId, uint imageId, Vector2 size, Color backgroundColor = default)
	{
		return ImGui.ImageButton(strId, GetTextureRef(imageId), size, Vector2.Zero, Vector2.One, backgroundColor);
	}

	/// <summary>
	/// Wraps a raw OpenGL texture handle for ImGui. These textures are owned by the app, not by ImGui, so they are
	/// referenced by id only and never go through the <see cref="ImTextureData" /> protocol in <see cref="ImGuiController" />.
	/// </summary>
	private static unsafe ImTextureRef GetTextureRef(uint imageId)
	{
		return new ImTextureRef(null, new ImTextureID(imageId));
	}
}
