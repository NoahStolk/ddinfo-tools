using DevilDaggersInfo.Tools.Scenes;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools;

public unsafe class FramebufferData
{
	private readonly GL _gl;

	public FramebufferData(GL gl)
	{
		_gl = gl;
	}

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
			_gl.DeleteFramebuffer(Framebuffer);

		if (TextureHandle != 0)
			_gl.DeleteTexture(TextureHandle);

		// Create new data.
		Framebuffer = _gl.GenFramebuffer();
		_gl.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

		TextureHandle = _gl.GenTexture();
		_gl.BindTexture(TextureTarget.Texture2D, TextureHandle);
		_gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)Width, (uint)Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, null);
		_gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
		_gl.TexParameterI(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
		_gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TextureHandle, 0);

		uint rbo = _gl.GenRenderbuffer();
		_gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);

		_gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent24, (uint)Width, (uint)Height);
		_gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rbo);

		if (_gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
			Root.Log.Warning("Framebuffer is not complete.");

		_gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		_gl.DeleteRenderbuffer(rbo);
	}

	public void RenderArena(bool activateMouse, bool activateKeyboard, float delta, ArenaScene arenaScene)
	{
		arenaScene.Update(activateMouse, activateKeyboard, delta);

		_gl.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

		int framebufferWidth = Width;
		int framebufferHeight = Height;

		// Keep track of the original viewport so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		_gl.GetInteger(GLEnum.Viewport, originalViewport);
		_gl.Viewport(0, 0, (uint)framebufferWidth, (uint)framebufferHeight);

		_gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		_gl.Enable(EnableCap.DepthTest);
		_gl.Enable(EnableCap.Blend);
		_gl.Enable(EnableCap.CullFace);
		_gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		arenaScene.Render(activateMouse, framebufferWidth, framebufferHeight);

		_gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		_gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
