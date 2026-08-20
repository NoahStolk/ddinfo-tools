using DevilDaggersInfo.Tools.Scenes;
using DevilDaggersInfo.Tools.Scenes.Rendering;
using Serilog;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools;

internal sealed unsafe class FramebufferData(GL gl, ILogger logger)
{
	public uint TextureHandle { get; private set; }
	public uint Framebuffer { get; private set; }
	public int Width { get; private set; }
	public int Height { get; private set; }

	private uint DepthRenderbuffer { get; set; }

	public void ResizeIfNecessary(int width, int height)
	{
		// ImGui ignores size constraints for docked windows, so callers deriving the framebuffer size by subtracting
		// padding from the window size can end up asking for a zero or negative size. Creating a framebuffer from that
		// yields GL_INVALID_VALUE and an incomplete framebuffer, which renders as garbage rather than failing loudly.
		if (width < 1 || height < 1)
		{
			logger.Warning("Ignoring framebuffer resize to invalid size {Width}x{Height}.", width, height);
			return;
		}

		if (width == Width && height == Height)
			return;

		Width = width;
		Height = height;

		// Delete previous data. The framebuffer goes first so that deleting its attachments below releases their storage
		// straight away rather than leaving it held by a container that is about to be destroyed anyway.
		if (Framebuffer != 0)
			gl.DeleteFramebuffer(Framebuffer);

		if (TextureHandle != 0)
			gl.DeleteTexture(TextureHandle);

		if (DepthRenderbuffer != 0)
			gl.DeleteRenderbuffer(DepthRenderbuffer);

		// Create new data.
		Framebuffer = gl.GenFramebuffer();
		gl.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

		TextureHandle = gl.GenTexture();
		gl.BindTexture(TextureTarget.Texture2D, TextureHandle);
		gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)Width, (uint)Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, null);

		int linear = (int)GLEnum.Linear;
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, in linear);
		gl.TexParameterI(TextureTarget.Texture2D, GLEnum.TextureMagFilter, in linear);
		gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TextureHandle, 0);

		// The handle is kept so this renderbuffer can be deleted on the next resize. Deleting it here while it is attached
		// happens to be legal, because an unbound framebuffer's attachment keeps the storage alive, but it discards the
		// only way to ever free it explicitly and leaves the correctness resting on the unbind ordering.
		DepthRenderbuffer = gl.GenRenderbuffer();
		gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthRenderbuffer);

		gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent24, (uint)Width, (uint)Height);
		gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, DepthRenderbuffer);

		if (gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
			logger.Warning("Framebuffer is not complete.");

		gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
		gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}

	public void RenderArena(bool activateMouse, bool activateKeyboard, float delta, ArenaScene arenaScene, ArenaRenderer arenaRenderer)
	{
		// No valid framebuffer yet, e.g. because the very first requested size was degenerate. Updating anyway would build
		// camera matrices from a zero-sized viewport, giving a NaN aspect ratio and dividing by zero during tile picking.
		if (Width < 1 || Height < 1)
			return;

		arenaScene.Update(activateMouse, activateKeyboard, delta, Width, Height);

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

		arenaRenderer.Render(arenaScene, activateMouse);

		gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
