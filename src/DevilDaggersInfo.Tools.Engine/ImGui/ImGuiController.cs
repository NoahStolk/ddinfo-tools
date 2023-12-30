using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace DevilDaggersInfo.Tools.Engine.ImGui;

public sealed class ImGuiController : IDisposable
{
	private static readonly Key[] _keyEnumArr = (Key[])Enum.GetValues(typeof(Key));

	private GL _gl = null!;
	private IView _view = null!;
	private IInputContext _input = null!;
	private bool _frameBegun;
	private readonly List<char> _pressedChars = new();
	private IKeyboard _keyboard = null!;

	private int _attribLocationTex;
	private int _attribLocationProjMtx;
	private int _attribLocationVtxPos;
	private int _attribLocationVtxUv;
	private int _attribLocationVtxColor;
	private uint _vboHandle;
	private uint _elementsHandle;
	private uint _vertexArrayObject;

	private Texture _fontTexture = null!;
	private Shader _shader = null!;

	private int _windowWidth;
	private int _windowHeight;

	private IntPtr _context;

	public ImGuiController(GL gl, IView view, IInputContext input, Action onConfigureIo)
	{
		Init(gl, view, input);

		ImGuiIOPtr io = ImGuiNET.ImGui.GetIO();

		onConfigureIo.Invoke();

		io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

		CreateDeviceResources();

		SetPerFrameImGuiData(1f / 60f);

		BeginFrame();
	}

	private void Init(GL gl, IView view, IInputContext input)
	{
		_gl = gl;
		_view = view;
		_input = input;
		_windowWidth = view.Size.X;
		_windowHeight = view.Size.Y;

		_context = ImGuiNET.ImGui.CreateContext();
		ImGuiNET.ImGui.SetCurrentContext(_context);
		ImGuiNET.ImGui.StyleColorsDark();
	}

	private void CreateDeviceResources()
	{
		_gl.GetInteger(GLEnum.TextureBinding2D, out int lastTexture);
		_gl.GetInteger(GLEnum.ArrayBufferBinding, out int lastArrayBuffer);
		_gl.GetInteger(GLEnum.VertexArrayBinding, out int lastVertexArray);

		const string vertexSource =
			"""
			#version 330
			layout (location = 0) in vec2 Position;
			layout (location = 1) in vec2 UV;
			layout (location = 2) in vec4 Color;
			uniform mat4 ProjMtx;
			out vec2 Frag_UV;
			out vec4 Frag_Color;
			void main()
			{
				Frag_UV = UV;
				Frag_Color = Color;
				gl_Position = ProjMtx * vec4(Position.xy,0,1);
			}
			""";

		const string fragmentSource =
			"""
			#version 330
			in vec2 Frag_UV;
			in vec4 Frag_Color;
			uniform sampler2D Texture;
			layout (location = 0) out vec4 Out_Color;
			void main()
			{
				Out_Color = Frag_Color * texture(Texture, Frag_UV.st);
			}
			""";

		_shader = new Shader(_gl, vertexSource, fragmentSource);

		_attribLocationTex = _shader.GetUniformLocation("Texture");
		_attribLocationProjMtx = _shader.GetUniformLocation("ProjMtx");
		_attribLocationVtxPos = _shader.GetAttribLocation("Position");
		_attribLocationVtxUv = _shader.GetAttribLocation("UV");
		_attribLocationVtxColor = _shader.GetAttribLocation("Color");

		_vboHandle = _gl.GenBuffer();
		_elementsHandle = _gl.GenBuffer();

		RecreateFontTexture();

		_gl.BindTexture(GLEnum.Texture2D, (uint)lastTexture);
		_gl.BindBuffer(GLEnum.ArrayBuffer, (uint)lastArrayBuffer);

		_gl.BindVertexArray((uint)lastVertexArray);
	}

	private void RecreateFontTexture()
	{
		ImGuiIOPtr io = ImGuiNET.ImGui.GetIO();

		io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int _);

		_gl.GetInteger(GLEnum.TextureBinding2D, out int lastTexture);

		_fontTexture = new Texture(_gl, width, height, pixels);
		_fontTexture.Bind();
		_fontTexture.SetMagFilter(TextureMagFilter.Linear);
		_fontTexture.SetMinFilter(TextureMinFilter.Linear);

		io.Fonts.SetTexID((IntPtr)_fontTexture.GlTexture);

		_gl.BindTexture(GLEnum.Texture2D, (uint)lastTexture);
	}

	private void BeginFrame()
	{
		ImGuiNET.ImGui.NewFrame();
		_frameBegun = true;
		_keyboard = _input.Keyboards[0];
		_view.Resize += WindowResized;
		_keyboard.KeyChar += OnKeyChar;
	}

	private void OnKeyChar(IKeyboard arg1, char arg2)
	{
		_pressedChars.Add(arg2);
	}

	private void WindowResized(Vector2D<int> size)
	{
		_windowWidth = size.X;
		_windowHeight = size.Y;
	}

	public void Render()
	{
		if (!_frameBegun)
			return;

		IntPtr oldCtx = ImGuiNET.ImGui.GetCurrentContext();

		if (oldCtx != _context)
		{
			ImGuiNET.ImGui.SetCurrentContext(_context);
		}

		_frameBegun = false;
		ImGuiNET.ImGui.Render();
		RenderImDrawData(ImGuiNET.ImGui.GetDrawData());

		if (oldCtx != _context)
		{
			ImGuiNET.ImGui.SetCurrentContext(oldCtx);
		}
	}

	/// <summary>
	/// Updates ImGui input and IO configuration state.
	/// </summary>
	public void Update(float deltaSeconds)
	{
		IntPtr oldCtx = ImGuiNET.ImGui.GetCurrentContext();

		if (oldCtx != _context)
		{
			ImGuiNET.ImGui.SetCurrentContext(_context);
		}

		if (_frameBegun)
		{
			ImGuiNET.ImGui.Render();
		}

		SetPerFrameImGuiData(deltaSeconds);
		UpdateImGuiInput();

		_frameBegun = true;
		ImGuiNET.ImGui.NewFrame();

		if (oldCtx != _context)
		{
			ImGuiNET.ImGui.SetCurrentContext(oldCtx);
		}

		// Since ImGui.NET 1.90.0.1, ImGui.NewFrame() overrides the key modifiers, so we need to set them after instead of before.
		ImGuiIOPtr io = ImGuiNET.ImGui.GetIO();
		IKeyboard keyboardState = _input.Keyboards[0];
		io.KeyCtrl = keyboardState.IsKeyPressed(Key.ControlLeft) || keyboardState.IsKeyPressed(Key.ControlRight);
		io.KeyAlt = keyboardState.IsKeyPressed(Key.AltLeft) || keyboardState.IsKeyPressed(Key.AltRight);
		io.KeyShift = keyboardState.IsKeyPressed(Key.ShiftLeft) || keyboardState.IsKeyPressed(Key.ShiftRight);
		io.KeySuper = keyboardState.IsKeyPressed(Key.SuperLeft) || keyboardState.IsKeyPressed(Key.SuperRight);
	}

	/// <summary>
	/// Sets per-frame data based on the associated window.
	/// This is called by Update(float).
	/// </summary>
	private void SetPerFrameImGuiData(float deltaSeconds)
	{
		ImGuiIOPtr io = ImGuiNET.ImGui.GetIO();
		io.DisplaySize = new(_windowWidth, _windowHeight);

		if (_windowWidth > 0 && _windowHeight > 0)
			io.DisplayFramebufferScale = new Vector2(_view.FramebufferSize.X / _windowWidth, _view.FramebufferSize.Y / _windowHeight);

		io.DeltaTime = deltaSeconds;
	}

	private void UpdateImGuiInput()
	{
		ImGuiIOPtr io = ImGuiNET.ImGui.GetIO();

		IMouse mouseState = _input.Mice[0];
		IKeyboard keyboardState = _input.Keyboards[0];

		io.MouseDown[0] = mouseState.IsButtonPressed(MouseButton.Left);
		io.MouseDown[1] = mouseState.IsButtonPressed(MouseButton.Right);
		io.MouseDown[2] = mouseState.IsButtonPressed(MouseButton.Middle);
		io.MousePos = mouseState.Position;

		ScrollWheel wheel = mouseState.ScrollWheels[0];
		io.MouseWheel = wheel.Y;
		io.MouseWheelH = wheel.X;

		for (int i = 0; i < _keyEnumArr.Length; i++)
		{
			Key key = _keyEnumArr[i];
			if (key == Key.Unknown)
				continue;

			io.AddKeyEvent(GetImGuiKey(key), keyboardState.IsKeyPressed(key));
		}

		for (int i = 0; i < _pressedChars.Count; i++)
		{
			char c = _pressedChars[i];
			io.AddInputCharacter(c);
		}

		_pressedChars.Clear();
	}

	private unsafe void SetupRenderState(ImDrawDataPtr drawDataPtr)
	{
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

		float l = drawDataPtr.DisplayPos.X;
		float r = drawDataPtr.DisplayPos.X + drawDataPtr.DisplaySize.X;
		float t = drawDataPtr.DisplayPos.Y;
		float b = drawDataPtr.DisplayPos.Y + drawDataPtr.DisplaySize.Y;

		Span<float> orthographicProjection = stackalloc float[]
		{
			2.0f / (r - l), 0.0f, 0.0f, 0.0f,
			0.0f, 2.0f / (t - b), 0.0f, 0.0f,
			0.0f, 0.0f, -1.0f, 0.0f,
			(r + l) / (l - r), (t + b) / (b - t), 0.0f, 1.0f,
		};

		_shader.UseShader();
		_gl.Uniform1(_attribLocationTex, 0);
		_gl.UniformMatrix4(_attribLocationProjMtx, 1, false, orthographicProjection);

		_gl.BindSampler(0, 0);

		// Setup desired GL state
		// Recreate the VAO every time (this is to easily allow multiple GL contexts to be rendered to. VAO are not shared among GL contexts)
		// The renderer would actually work without any VAO bound, but then our VertexAttrib calls would overwrite the default one currently bound.
		_vertexArrayObject = _gl.GenVertexArray();
		_gl.BindVertexArray(_vertexArrayObject);

		// Bind vertex/index buffers and setup attributes for ImDrawVert
		_gl.BindBuffer(GLEnum.ArrayBuffer, _vboHandle);
		_gl.BindBuffer(GLEnum.ElementArrayBuffer, _elementsHandle);
		_gl.EnableVertexAttribArray((uint)_attribLocationVtxPos);
		_gl.EnableVertexAttribArray((uint)_attribLocationVtxUv);
		_gl.EnableVertexAttribArray((uint)_attribLocationVtxColor);
		_gl.VertexAttribPointer((uint)_attribLocationVtxPos, 2, GLEnum.Float, false, (uint)sizeof(ImDrawVert), (void*)0);
		_gl.VertexAttribPointer((uint)_attribLocationVtxUv, 2, GLEnum.Float, false, (uint)sizeof(ImDrawVert), (void*)8);
		_gl.VertexAttribPointer((uint)_attribLocationVtxColor, 4, GLEnum.UnsignedByte, true, (uint)sizeof(ImDrawVert), (void*)16);
	}

	private unsafe void RenderImDrawData(ImDrawDataPtr drawDataPtr)
	{
		int framebufferWidth = (int)(drawDataPtr.DisplaySize.X * drawDataPtr.FramebufferScale.X);
		int framebufferHeight = (int)(drawDataPtr.DisplaySize.Y * drawDataPtr.FramebufferScale.Y);
		if (framebufferWidth <= 0 || framebufferHeight <= 0)
			return;

		// Backup GL state
		_gl.GetInteger(GLEnum.ActiveTexture, out int lastActiveTexture);
		_gl.ActiveTexture(GLEnum.Texture0);

		_gl.GetInteger(GLEnum.CurrentProgram, out int lastProgram);
		_gl.GetInteger(GLEnum.TextureBinding2D, out int lastTexture);

		_gl.GetInteger(GLEnum.SamplerBinding, out int lastSampler);

		_gl.GetInteger(GLEnum.ArrayBufferBinding, out int lastArrayBuffer);
		_gl.GetInteger(GLEnum.VertexArrayBinding, out int lastVertexArrayObject);

		Span<int> lastPolygonMode = stackalloc int[2];
		_gl.GetInteger(GLEnum.PolygonMode, lastPolygonMode);

		Span<int> lastScissorBox = stackalloc int[4];
		_gl.GetInteger(GLEnum.ScissorBox, lastScissorBox);

		_gl.GetInteger(GLEnum.BlendSrcRgb, out int lastBlendSrcRgb);
		_gl.GetInteger(GLEnum.BlendDstRgb, out int lastBlendDstRgb);

		_gl.GetInteger(GLEnum.BlendSrcAlpha, out int lastBlendSrcAlpha);
		_gl.GetInteger(GLEnum.BlendDstAlpha, out int lastBlendDstAlpha);

		_gl.GetInteger(GLEnum.BlendEquationRgb, out int lastBlendEquationRgb);
		_gl.GetInteger(GLEnum.BlendEquationAlpha, out int lastBlendEquationAlpha);

		bool lastEnableBlend = _gl.IsEnabled(GLEnum.Blend);
		bool lastEnableCullFace = _gl.IsEnabled(GLEnum.CullFace);
		bool lastEnableDepthTest = _gl.IsEnabled(GLEnum.DepthTest);
		bool lastEnableStencilTest = _gl.IsEnabled(GLEnum.StencilTest);
		bool lastEnableScissorTest = _gl.IsEnabled(GLEnum.ScissorTest);
		bool lastEnablePrimitiveRestart = _gl.IsEnabled(GLEnum.PrimitiveRestart);

		SetupRenderState(drawDataPtr);

		// Will project scissor/clipping rectangles into framebuffer space
		Vector2 clipOff = drawDataPtr.DisplayPos; // (0,0) unless using multi-viewports
		Vector2 clipScale = drawDataPtr.FramebufferScale; // (1,1) unless using retina display which are often (2,2)

		// Render command lists
		for (int n = 0; n < drawDataPtr.CmdListsCount; n++)
		{
			ImDrawListPtr cmdListPtr = drawDataPtr.CmdLists[n];

			// Upload vertex/index buffers
			_gl.BufferData(GLEnum.ArrayBuffer, (nuint)(cmdListPtr.VtxBuffer.Size * sizeof(ImDrawVert)), (void*)cmdListPtr.VtxBuffer.Data, GLEnum.StreamDraw);
			_gl.BufferData(GLEnum.ElementArrayBuffer, (nuint)(cmdListPtr.IdxBuffer.Size * sizeof(ushort)), (void*)cmdListPtr.IdxBuffer.Data, GLEnum.StreamDraw);

			for (int cmdI = 0; cmdI < cmdListPtr.CmdBuffer.Size; cmdI++)
			{
				ImDrawCmdPtr cmdPtr = cmdListPtr.CmdBuffer[cmdI];

				if (cmdPtr.UserCallback != IntPtr.Zero)
					throw new NotImplementedException();

				Vector4 clipRect;
				clipRect.X = (cmdPtr.ClipRect.X - clipOff.X) * clipScale.X;
				clipRect.Y = (cmdPtr.ClipRect.Y - clipOff.Y) * clipScale.Y;
				clipRect.Z = (cmdPtr.ClipRect.Z - clipOff.X) * clipScale.X;
				clipRect.W = (cmdPtr.ClipRect.W - clipOff.Y) * clipScale.Y;

				if (clipRect.X < framebufferWidth && clipRect.Y < framebufferHeight && clipRect is { Z: >= 0.0f, W: >= 0.0f })
				{
					// Apply scissor/clipping rectangle
					_gl.Scissor((int)clipRect.X, (int)(framebufferHeight - clipRect.W), (uint)(clipRect.Z - clipRect.X), (uint)(clipRect.W - clipRect.Y));

					// Bind texture, Draw
					_gl.BindTexture(GLEnum.Texture2D, (uint)cmdPtr.TextureId);

					_gl.DrawElementsBaseVertex(GLEnum.Triangles, cmdPtr.ElemCount, GLEnum.UnsignedShort, (void*)(cmdPtr.IdxOffset * sizeof(ushort)), (int)cmdPtr.VtxOffset);
				}
			}
		}

		// Destroy the temporary VAO
		_gl.DeleteVertexArray(_vertexArrayObject);
		_vertexArrayObject = 0;

		// Restore modified GL state
		_gl.UseProgram((uint)lastProgram);
		_gl.BindTexture(GLEnum.Texture2D, (uint)lastTexture);

		_gl.BindSampler(0, (uint)lastSampler);

		_gl.ActiveTexture((GLEnum)lastActiveTexture);

		_gl.BindVertexArray((uint)lastVertexArrayObject);

		_gl.BindBuffer(GLEnum.ArrayBuffer, (uint)lastArrayBuffer);
		_gl.BlendEquationSeparate((GLEnum)lastBlendEquationRgb, (GLEnum)lastBlendEquationAlpha);
		_gl.BlendFuncSeparate((GLEnum)lastBlendSrcRgb, (GLEnum)lastBlendDstRgb, (GLEnum)lastBlendSrcAlpha, (GLEnum)lastBlendDstAlpha);

		if (lastEnableBlend)
			_gl.Enable(GLEnum.Blend);
		else
			_gl.Disable(GLEnum.Blend);

		if (lastEnableCullFace)
			_gl.Enable(GLEnum.CullFace);
		else
			_gl.Disable(GLEnum.CullFace);

		if (lastEnableDepthTest)
			_gl.Enable(GLEnum.DepthTest);
		else
			_gl.Disable(GLEnum.DepthTest);

		if (lastEnableStencilTest)
			_gl.Enable(GLEnum.StencilTest);
		else
			_gl.Disable(GLEnum.StencilTest);

		if (lastEnableScissorTest)
			_gl.Enable(GLEnum.ScissorTest);
		else
			_gl.Disable(GLEnum.ScissorTest);

		if (lastEnablePrimitiveRestart)
			_gl.Enable(GLEnum.PrimitiveRestart);
		else
			_gl.Disable(GLEnum.PrimitiveRestart);

		_gl.PolygonMode(GLEnum.FrontAndBack, (GLEnum)lastPolygonMode[0]);

		_gl.Scissor(lastScissorBox[0], lastScissorBox[1], (uint)lastScissorBox[2], (uint)lastScissorBox[3]);
	}

	/// <summary>
	/// Frees all graphics resources used by the renderer.
	/// </summary>
	public void Dispose()
	{
		_view.Resize -= WindowResized;
		_keyboard.KeyChar -= OnKeyChar;

		_gl.DeleteBuffer(_vboHandle);
		_gl.DeleteBuffer(_elementsHandle);
		_gl.DeleteVertexArray(_vertexArrayObject);

		_fontTexture.Dispose();
		_shader.Dispose();

		ImGuiNET.ImGui.DestroyContext(_context);
	}

	private static ImGuiKey GetImGuiKey(Key key)
	{
		return key switch
		{
			>= Key.Number0 and <= Key.Number9 => key - Key.D0 + ImGuiKey._0,
			>= Key.A and <= Key.Z => key - Key.A + ImGuiKey.A,
			>= Key.Keypad0 and <= Key.Keypad9 => key - Key.Keypad0 + ImGuiKey.Keypad0,
			>= Key.F1 and <= Key.F24 => key - Key.F1 + ImGuiKey.F24,
			_ => key switch
			{
				Key.Tab => ImGuiKey.Tab,
				Key.Left => ImGuiKey.LeftArrow,
				Key.Right => ImGuiKey.RightArrow,
				Key.Up => ImGuiKey.UpArrow,
				Key.Down => ImGuiKey.DownArrow,
				Key.PageUp => ImGuiKey.PageUp,
				Key.PageDown => ImGuiKey.PageDown,
				Key.Home => ImGuiKey.Home,
				Key.End => ImGuiKey.End,
				Key.Insert => ImGuiKey.Insert,
				Key.Delete => ImGuiKey.Delete,
				Key.Backspace => ImGuiKey.Backspace,
				Key.Space => ImGuiKey.Space,
				Key.Enter => ImGuiKey.Enter,
				Key.Escape => ImGuiKey.Escape,
				Key.Apostrophe => ImGuiKey.Apostrophe,
				Key.Comma => ImGuiKey.Comma,
				Key.Minus => ImGuiKey.Minus,
				Key.Period => ImGuiKey.Period,
				Key.Slash => ImGuiKey.Slash,
				Key.Semicolon => ImGuiKey.Semicolon,
				Key.Equal => ImGuiKey.Equal,
				Key.LeftBracket => ImGuiKey.LeftBracket,
				Key.BackSlash => ImGuiKey.Backslash,
				Key.RightBracket => ImGuiKey.RightBracket,
				Key.GraveAccent => ImGuiKey.GraveAccent,
				Key.CapsLock => ImGuiKey.CapsLock,
				Key.ScrollLock => ImGuiKey.ScrollLock,
				Key.NumLock => ImGuiKey.NumLock,
				Key.PrintScreen => ImGuiKey.PrintScreen,
				Key.Pause => ImGuiKey.Pause,
				Key.KeypadDecimal => ImGuiKey.KeypadDecimal,
				Key.KeypadDivide => ImGuiKey.KeypadDivide,
				Key.KeypadMultiply => ImGuiKey.KeypadMultiply,
				Key.KeypadSubtract => ImGuiKey.KeypadSubtract,
				Key.KeypadAdd => ImGuiKey.KeypadAdd,
				Key.KeypadEnter => ImGuiKey.KeypadEnter,
				Key.KeypadEqual => ImGuiKey.KeypadEqual,
				Key.ShiftLeft => ImGuiKey.LeftShift,
				Key.ControlLeft => ImGuiKey.LeftCtrl,
				Key.AltLeft => ImGuiKey.LeftAlt,
				Key.SuperLeft => ImGuiKey.LeftSuper,
				Key.ShiftRight => ImGuiKey.RightShift,
				Key.ControlRight => ImGuiKey.RightCtrl,
				Key.AltRight => ImGuiKey.RightAlt,
				Key.SuperRight => ImGuiKey.RightSuper,
				Key.Menu => ImGuiKey.Menu,
				_ => ImGuiKey.None,
			},
		};
	}
}
