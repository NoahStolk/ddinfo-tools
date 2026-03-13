using DevilDaggersInfo.Tools.Scenes;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools;

internal unsafe class FramebufferData(GL gl)
{
	public uint TextureHandle { get; private set; }
	public uint Framebuffer { get; private set; }
	public int Width { get; private set; }
	public int Height { get; private set; }

	public void ResizeIfNecessary(int width, int height)
	{
		if (width == Width && height == Height)
			return;

		Width = width;
		Height = height;

		// Delete previous data.
		if (Framebuffer != 0)
			gl.DeleteFramebuffer(Framebuffer);

		if (TextureHandle != 0)
			gl.DeleteTexture(TextureHandle);

		// Create new data.
		Framebuffer = gl.GenFramebuffer();
		gl.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

		TextureHandle = gl.GenTexture();
		gl.BindTexture(TextureTarget.Texture2D, TextureHandle);
		gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)Width, (uint)Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, null);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
		gl.TexParameterI(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
		gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TextureHandle, 0);

		uint rbo = gl.GenRenderbuffer();
		gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);

		gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent24, (uint)Width, (uint)Height);
		gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rbo);

		if (gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
			Root.Log.Warning("Framebuffer is not complete.");

		gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		gl.DeleteRenderbuffer(rbo);
	}

	public void RenderArena(bool activateMouse, bool activateKeyboard, float delta, ArenaScene arenaScene)
	{
		arenaScene.Update(activateMouse, activateKeyboard, delta);

		gl.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

		int framebufferWidth = Width;
		int framebufferHeight = Height;

		// Keep track of the original viewport so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		gl.GetInteger(GLEnum.Viewport, originalViewport);
		gl.Viewport(0, 0, (uint)framebufferWidth, (uint)framebufferHeight);

		gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		gl.Enable(EnableCap.DepthTest);
		gl.Enable(EnableCap.Blend);
		gl.Enable(EnableCap.CullFace);
		gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		arenaScene.Render(activateMouse, framebufferWidth, framebufferHeight);

		gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
