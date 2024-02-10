using DevilDaggersInfo.Tools.Engine;
using DevilDaggersInfo.Tools.Scenes;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools;

public unsafe class FramebufferData
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
			Graphics.Gl.DeleteFramebuffer(Framebuffer);

		if (TextureHandle != 0)
			Graphics.Gl.DeleteTexture(TextureHandle);

		// Create new data.
		Framebuffer = Graphics.Gl.GenFramebuffer();
		Graphics.Gl.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

		TextureHandle = Graphics.Gl.GenTexture();
		Graphics.Gl.BindTexture(TextureTarget.Texture2D, TextureHandle);
		Graphics.Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)Width, (uint)Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, null);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
		Graphics.Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TextureHandle, 0);

		uint rbo = Graphics.Gl.GenRenderbuffer();
		Graphics.Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);

		Graphics.Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent24, (uint)Width, (uint)Height);
		Graphics.Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rbo);

		if (Graphics.Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
			Root.Log.Warning("Framebuffer is not complete.");

		Graphics.Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		Graphics.Gl.DeleteRenderbuffer(rbo);
	}

	public void RenderArena(bool activateMouse, bool activateKeyboard, float delta, ArenaScene arenaScene)
	{
		arenaScene.Update(activateMouse, activateKeyboard, delta);

		Graphics.Gl.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

		int framebufferWidth = Width;
		int framebufferHeight = Height;

		// Keep track of the original viewport so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		Graphics.Gl.GetInteger(GLEnum.Viewport, originalViewport);
		Graphics.Gl.Viewport(0, 0, (uint)framebufferWidth, (uint)framebufferHeight);

		Graphics.Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		Graphics.Gl.Enable(EnableCap.DepthTest);
		Graphics.Gl.Enable(EnableCap.Blend);
		Graphics.Gl.Enable(EnableCap.CullFace);
		Graphics.Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		arenaScene.Render(activateMouse, framebufferWidth, framebufferHeight);

		Graphics.Gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		Graphics.Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
