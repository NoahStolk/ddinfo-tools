using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Extensions;

internal static class ImDrawListPtrExtensions
{
	extension(ImDrawListPtr drawListPtr)
	{
		public void AddFramebufferImage(FramebufferData framebufferData, Vector2 pMin, Vector2 pMax)
		{
			drawListPtr.AddFramebufferImage(framebufferData, pMin, pMax, Color.White);
		}

		public void AddFramebufferImage(FramebufferData framebufferData, Vector2 pMin, Vector2 pMax, Color color)
		{
			// Framebuffers are flipped vertically, so we need to flip the UVs.
			drawListPtr.AddImage(ImGui.GetTextureRef(framebufferData.TextureHandle), pMin, pMax, Vector2.UnitY, Vector2.UnitX, ImGui.GetColorU32(color));
		}

		public void AddImage(uint imageId, Vector2 pMin, Vector2 pMax, Color color)
		{
			drawListPtr.AddImage(ImGui.GetTextureRef(imageId), pMin, pMax, Vector2.Zero, Vector2.One, ImGui.GetColorU32(color));
		}
	}
}
