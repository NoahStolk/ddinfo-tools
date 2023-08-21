using DevilDaggersInfo.App.Scenes;
using ImGuiNET;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App;

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
			Root.Gl.DeleteFramebuffer(Framebuffer);

		if (TextureHandle != 0)
			Root.Gl.DeleteTexture(TextureHandle);

		// Create new data.
		Framebuffer = Root.Gl.GenFramebuffer();
		Root.Gl.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

		TextureHandle = Root.Gl.GenTexture();
		Root.Gl.BindTexture(TextureTarget.Texture2D, TextureHandle);
		Root.Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)Width, (uint)Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, null);
		Root.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
		Root.Gl.TexParameterI(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
		Root.Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TextureHandle, 0);

		uint rbo = Root.Gl.GenRenderbuffer();
		Root.Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);

		Root.Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent24, (uint)Width, (uint)Height);
		Root.Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rbo);

		if (Root.Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
			Root.Log.Warning("Framebuffer is not complete.");

		Root.Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		Root.Gl.DeleteRenderbuffer(rbo);
	}

	public void RenderArena(float delta, ArenaScene arenaScene)
	{
		bool hover = ImGui.IsWindowHovered();
		bool focus = ImGui.IsWindowFocused();
		arenaScene.Update(hover, hover || focus, delta);

		Root.Gl.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

		int framebufferWidth = Width;
		int framebufferHeight = Height;

		// Keep track of the original viewport so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		Root.Gl.GetInteger(GLEnum.Viewport, originalViewport);
		Root.Gl.Viewport(0, 0, (uint)framebufferWidth, (uint)framebufferHeight);

		Root.Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		Root.Gl.Enable(EnableCap.DepthTest);
		Root.Gl.Enable(EnableCap.Blend);
		Root.Gl.Enable(EnableCap.CullFace);
		Root.Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		arenaScene.Render(framebufferWidth, framebufferHeight);

		Root.Gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		Root.Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
