using DevilDaggersInfo.Tools.Engine.Loaders;
using DevilDaggersInfo.Tools.Extensions;
using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using System.Numerics;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.Ui;

// TODO: Use new implementation.
internal sealed class ImGuiController
{
	private const string _vertexShader = """
		#version 330
		layout (location = 0) in vec2 position;
		layout (location = 1) in vec2 uv;
		layout (location = 2) in vec4 color;

		uniform mat4 projectionMatrix;

		out vec2 fragUv;
		out vec4 fragColor;

		void main()
		{
			fragUv = uv;
			fragColor = color;
			gl_Position = projectionMatrix * vec4(position.xy, 0, 1);
		}
		""";

	private const string _fragmentShader = """
		#version 330
		in vec2 fragUv;
		in vec4 fragColor;

		uniform sampler2D image;

		layout (location = 0) out vec4 outColor;

		void main()
		{
			outColor = fragColor * texture(image, fragUv.st);
		}
		""";

	private readonly uint _vbo;
	private readonly uint _ebo;
	private uint _vao;

	private readonly GL _gl;
	private readonly GlfwInput _glfwInput;
	private readonly IntPtr _context;
	private readonly uint _shaderId;
	private readonly int _projectionMatrixLocation;
	private readonly int _imageLocation;

	private int _windowWidth;
	private int _windowHeight;
	private int _framebufferWidth;
	private int _framebufferHeight;

	public ImGuiController(GL gl, GlfwInput glfwInput, ShaderLoader shaderLoader, int windowWidth, int windowHeight)
	{
		_gl = gl;
		_glfwInput = glfwInput;
		_windowWidth = windowWidth;
		_windowHeight = windowHeight;
		_framebufferWidth = windowWidth;
		_framebufferHeight = windowHeight;

		_context = ImGui.CreateContext();
		ImGui.SetCurrentContext(_context);
		ImGui.StyleColorsDark();

		ImGuiIOPtr io = ImGui.GetIO();
		io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
		io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

		_vbo = _gl.GenBuffer();
		_ebo = _gl.GenBuffer();

		_shaderId = shaderLoader.Load(_vertexShader, _fragmentShader);
		_projectionMatrixLocation = _gl.GetUniformLocation(_shaderId, "projectionMatrix");
		_imageLocation = _gl.GetUniformLocation(_shaderId, "image");
	}

	#region Initialization

	public unsafe void CreateDefaultFont()
	{
		ImGuiIOPtr io = ImGui.GetIO();

		io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

		byte[] data = new byte[width * height * bytesPerPixel];
		Marshal.Copy(pixels, data, 0, data.Length);
		uint textureId = _gl.GenTexture();

		_gl.BindTexture(TextureTarget.Texture2D, textureId);

		_gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
		_gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
		_gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
		_gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

		fixed (byte* b = data)
			_gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)width, (uint)height, 0, GLEnum.Rgba, PixelType.UnsignedByte, b);

		io.Fonts.SetTexID((IntPtr)textureId);
	}

	#endregion Initialization

	public void Destroy()
	{
		_gl.DeleteBuffer(_vbo);
		_gl.DeleteBuffer(_ebo);
		_gl.DeleteVertexArray(_vao);

		ImGui.DestroyContext(_context);
	}

	public void WindowResized(int width, int height)
	{
		_windowWidth = width;
		_windowHeight = height;
	}

	public void FramebufferResized(int width, int height)
	{
		_framebufferWidth = width;
		_framebufferHeight = height;
	}

	public void Render()
	{
		ImGui.Render();
		RenderImDrawData(ImGui.GetDrawData());
	}

	public void Update(float deltaSeconds)
	{
		ImGuiIOPtr io = ImGui.GetIO();
		io.DisplaySize = new Vector2(_windowWidth, _windowHeight);
		float scaleX = _windowWidth > 0 ? (float)_framebufferWidth / _windowWidth : 1f;
		float scaleY = _windowHeight > 0 ? (float)_framebufferHeight / _windowHeight : 1f;
		io.DisplayFramebufferScale = new Vector2(scaleX, scaleY);
		io.DeltaTime = deltaSeconds;

		UpdateImGuiInput();

		ImGui.NewFrame();
	}

	#region Input

	private void UpdateImGuiInput()
	{
		ImGuiIOPtr io = ImGui.GetIO();

		io.AddMousePosEvent(_glfwInput.CursorPosition.X, _glfwInput.CursorPosition.Y);
		io.AddMouseButtonEvent(0, _glfwInput.IsMouseButtonDown(MouseButton.Left));
		io.AddMouseButtonEvent(1, _glfwInput.IsMouseButtonDown(MouseButton.Right));
		io.AddMouseButtonEvent(2, _glfwInput.IsMouseButtonDown(MouseButton.Middle));
		io.AddMouseWheelEvent(0f, _glfwInput.MouseWheelY);

		for (int i = 0; i < _glfwInput.CharsPressed.Count; i++)
			io.AddInputCharacter(_glfwInput.CharsPressed[i]);

		for (int i = 0; i < _glfwInput.KeysChanged.Count; i++)
		{
			Keys key = _glfwInput.KeysChanged[i];
			ImGuiKey imGuiKey = GetImGuiInputKey(key);
			if (imGuiKey != ImGuiKey.None)
				io.AddKeyEvent(imGuiKey, _glfwInput.IsKeyDown(key));
		}
	}

	#endregion Input

	#region Rendering

	private unsafe void SetUpRenderState(ImDrawDataPtr drawDataPtr)
	{
		// TODO: Will probably need to back up the GL state here so we can properly restore it after rendering.
		// Set up render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, polygon fill.
		_gl.Enable(GLEnum.Blend);
		_gl.BlendEquation(GLEnum.FuncAdd);
		_gl.BlendFuncSeparate(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha, GLEnum.One, GLEnum.OneMinusSrcAlpha);
		_gl.Disable(GLEnum.CullFace);
		_gl.Disable(GLEnum.DepthTest);
		_gl.Disable(GLEnum.StencilTest);
		_gl.Enable(GLEnum.ScissorTest);
		_gl.Disable(GLEnum.PrimitiveRestart);
		_gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Fill);

		Matrix4x4 orthographicProjection = Matrix4x4.CreateOrthographicOffCenter(
			left: drawDataPtr.DisplayPos.X,
			right: drawDataPtr.DisplayPos.X + drawDataPtr.DisplaySize.X,
			bottom: drawDataPtr.DisplayPos.Y + drawDataPtr.DisplaySize.Y,
			top: drawDataPtr.DisplayPos.Y,
			zNearPlane: -1,
			zFarPlane: 1);

		_gl.UseProgram(_shaderId);
		_gl.Uniform1(_imageLocation, 0);
		_gl.UniformMatrix4x4(_projectionMatrixLocation, orthographicProjection);

		_gl.BindSampler(0, 0);

		_vao = _gl.GenVertexArray();
		_gl.BindVertexArray(_vao);

		_gl.BindBuffer(GLEnum.ArrayBuffer, _vbo);
		_gl.BindBuffer(GLEnum.ElementArrayBuffer, _ebo);

		_gl.EnableVertexAttribArray(0);
		_gl.EnableVertexAttribArray(1);
		_gl.EnableVertexAttribArray(2);

		_gl.VertexAttribPointer(0, 2, GLEnum.Float, false, (uint)sizeof(ImDrawVert), (void*)0);
		_gl.VertexAttribPointer(1, 2, GLEnum.Float, false, (uint)sizeof(ImDrawVert), (void*)8);
		_gl.VertexAttribPointer(2, 4, GLEnum.UnsignedByte, true, (uint)sizeof(ImDrawVert), (void*)16);
	}

	private unsafe void RenderImDrawData(ImDrawDataPtr drawDataPtr)
	{
		int framebufferWidth = (int)(drawDataPtr.DisplaySize.X * drawDataPtr.FramebufferScale.X);
		int framebufferHeight = (int)(drawDataPtr.DisplaySize.Y * drawDataPtr.FramebufferScale.Y);
		if (framebufferWidth <= 0 || framebufferHeight <= 0)
			return;

		SetUpRenderState(drawDataPtr);

		// Will project scissor/clipping rectangles into framebuffer space.
		Vector2 clipOff = drawDataPtr.DisplayPos; // (0,0) unless using multi-viewports
		Vector2 clipScale = drawDataPtr.FramebufferScale; // (1,1) unless using retina display which are often (2,2)

		for (int i = 0; i < drawDataPtr.CmdListsCount; i++)
		{
			ImDrawListPtr cmdListPtr = drawDataPtr.CmdLists[i];

			_gl.BufferData(GLEnum.ArrayBuffer, (nuint)(cmdListPtr.VtxBuffer.Size * sizeof(ImDrawVert)), (void*)cmdListPtr.VtxBuffer.Data, GLEnum.StreamDraw);
			_gl.BufferData(GLEnum.ElementArrayBuffer, (nuint)(cmdListPtr.IdxBuffer.Size * sizeof(ushort)), (void*)cmdListPtr.IdxBuffer.Data, GLEnum.StreamDraw);

			for (int j = 0; j < cmdListPtr.CmdBuffer.Size; j++)
			{
				ImDrawCmdPtr cmdPtr = cmdListPtr.CmdBuffer[j];
				if (cmdPtr.UserCallback != IntPtr.Zero)
					throw new NotImplementedException();

				Vector4 clipRect;
				clipRect.X = (cmdPtr.ClipRect.X - clipOff.X) * clipScale.X;
				clipRect.Y = (cmdPtr.ClipRect.Y - clipOff.Y) * clipScale.Y;
				clipRect.Z = (cmdPtr.ClipRect.Z - clipOff.X) * clipScale.X;
				clipRect.W = (cmdPtr.ClipRect.W - clipOff.Y) * clipScale.Y;

				if (clipRect.X >= framebufferWidth || clipRect.Y >= framebufferHeight || clipRect.Z < 0.0f || clipRect.W < 0.0f)
					continue;

				_gl.Scissor((int)clipRect.X, (int)(framebufferHeight - clipRect.W), (uint)(clipRect.Z - clipRect.X), (uint)(clipRect.W - clipRect.Y));
				_gl.BindTexture(GLEnum.Texture2D, (uint)cmdPtr.TextureId);
				_gl.DrawElementsBaseVertex(GLEnum.Triangles, cmdPtr.ElemCount, GLEnum.UnsignedShort, (void*)(cmdPtr.IdxOffset * sizeof(ushort)), (int)cmdPtr.VtxOffset);
			}
		}

		// Destroy the temporary VAO.
		_gl.DeleteVertexArray(_vao);
		_vao = 0;

		// Restore scissors.
		_gl.Disable(EnableCap.ScissorTest);
	}

	#endregion Rendering

	private static ImGuiKey GetImGuiInputKey(Keys key)
	{
		return key switch
		{
			>= Keys.F1 and <= Keys.F24 => ConvertRange(key, Keys.F1, ImGuiKey.F1),
			>= Keys.Keypad0 and <= Keys.Keypad9 => ConvertRange(key, Keys.Keypad0, ImGuiKey.Keypad0),
			>= Keys.A and <= Keys.Z => ConvertRange(key, Keys.A, ImGuiKey.A),
			>= Keys.Number0 and <= Keys.Number9 => ConvertRange(key, Keys.Number0, ImGuiKey._0),
			Keys.ShiftLeft or Keys.ShiftRight => ImGuiKey.ModShift,
			Keys.ControlLeft or Keys.ControlRight => ImGuiKey.ModCtrl,
			Keys.AltLeft or Keys.AltRight => ImGuiKey.ModAlt,
			Keys.SuperLeft or Keys.SuperRight => ImGuiKey.ModSuper,
			Keys.Menu => ImGuiKey.Menu,
			Keys.Up => ImGuiKey.UpArrow,
			Keys.Down => ImGuiKey.DownArrow,
			Keys.Left => ImGuiKey.LeftArrow,
			Keys.Right => ImGuiKey.RightArrow,
			Keys.Enter => ImGuiKey.Enter,
			Keys.Escape => ImGuiKey.Escape,
			Keys.Space => ImGuiKey.Space,
			Keys.Tab => ImGuiKey.Tab,
			Keys.Backspace => ImGuiKey.Backspace,
			Keys.Insert => ImGuiKey.Insert,
			Keys.Delete => ImGuiKey.Delete,
			Keys.PageUp => ImGuiKey.PageUp,
			Keys.PageDown => ImGuiKey.PageDown,
			Keys.Home => ImGuiKey.Home,
			Keys.End => ImGuiKey.End,
			Keys.CapsLock => ImGuiKey.CapsLock,
			Keys.ScrollLock => ImGuiKey.ScrollLock,
			Keys.PrintScreen => ImGuiKey.PrintScreen,
			Keys.Pause => ImGuiKey.Pause,
			Keys.NumLock => ImGuiKey.NumLock,
			Keys.KeypadDivide => ImGuiKey.KeypadDivide,
			Keys.KeypadMultiply => ImGuiKey.KeypadMultiply,
			Keys.KeypadSubtract => ImGuiKey.KeypadSubtract,
			Keys.KeypadAdd => ImGuiKey.KeypadAdd,
			Keys.KeypadDecimal => ImGuiKey.KeypadDecimal,
			Keys.KeypadEnter => ImGuiKey.KeypadEnter,
			Keys.GraveAccent => ImGuiKey.GraveAccent,
			Keys.Minus => ImGuiKey.Minus,
			Keys.Equal => ImGuiKey.Equal,
			Keys.LeftBracket => ImGuiKey.LeftBracket,
			Keys.RightBracket => ImGuiKey.RightBracket,
			Keys.Semicolon => ImGuiKey.Semicolon,
			Keys.Apostrophe => ImGuiKey.Apostrophe,
			Keys.Comma => ImGuiKey.Comma,
			Keys.Period => ImGuiKey.Period,
			Keys.Slash => ImGuiKey.Slash,
			Keys.BackSlash => ImGuiKey.Backslash,
			_ => ImGuiKey.None,
		};

		static ImGuiKey ConvertRange(Keys key, Keys startKey, ImGuiKey startImGuiKey)
		{
			int diff = (int)key - (int)startKey;
			return startImGuiKey + diff;
		}
	}
}
