using DevilDaggersInfo.App.Tools.Enums;
using DevilDaggersInfo.App.Tools.Layouts;
using DevilDaggersInfo.App.Tools.States;
using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using Warp;
using Warp.Debugging;
using Warp.Extensions;
using Warp.Games;
using Warp.Text;
using Warp.Ui;

namespace DevilDaggersInfo.App.Tools;

public partial class Game : GameBase
{
	private readonly Matrix4x4 _projectionMatrix;

	private Viewport _viewport;
	private int _leftOffset;
	private int _bottomOffset;

	public Game()
		: base("DevilDaggers.info Tools", 1920, 1080, false)
	{
		_projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0, InitialWindowWidth, InitialWindowHeight, 0, -1024, 1024);
	}

	public Vector2 ViewportOffset => new(_leftOffset, _bottomOffset);
	private Vector2 MousePositionWithOffset => Input.GetMousePosition() - ViewportOffset;

	private Layout? _activeLayout;
	public Layout? ActiveLayout
	{
		get => _activeLayout;
		set
		{
			if (_activeLayout == value)
				throw new InvalidOperationException("This layout is already active.");

			_activeLayout = value;
		}
	}

	public MainLayout MainLayout { get; } = new();
	public OpenLayout OpenLayout { get; } = new();
	public SaveLayout SaveLayout { get; } = new();

	public MonoSpaceFont Font { get; private set; } = null!;
	public MonoSpaceFont FontSmall { get; private set; } = null!;

	public Renderers.MonoSpaceFontRenderer MonoSpaceFontRenderer { get; private set; } = null!;
	public Renderers.MonoSpaceFontRenderer MonoSpaceSmallFontRenderer { get; private set; } = null!;
	public Renderers.UiRenderer UiRenderer { get; private set; } = null!;

	public string? CursorText { get; set; }

	protected override void LoadContent()
	{
		InitializeContent();

		MonoSpaceFontRenderer = new();
		MonoSpaceSmallFontRenderer = new();
		UiRenderer = new();

		Font = new(Textures.Font, @" 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!?:()[]{}<>|@^$%#&/\+*`,'=~;.-_  ");
		FontSmall = new(Textures.FontSmall, " 0123456789.-");
		MonoSpaceFontRenderer.SetFont(Font);
		MonoSpaceSmallFontRenderer.SetFont(FontSmall);

		StateManager.SetSpawnset("(untitled)", SpawnsetBinary.CreateDefault());
		Base.Game.ActiveLayout = Base.Game.MainLayout;

		Gl.Enable(EnableCap.DepthTest);
		Gl.Enable(EnableCap.Blend);
		Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
	}

	public override void OnChangeWindowSize(int width, int height)
	{
		base.OnChangeWindowSize(width, height);

		_leftOffset = (width - InitialWindowWidth) / 2;
		_bottomOffset = (height - InitialWindowHeight) / 2;
		SetViewport(_leftOffset, _bottomOffset, InitialWindowWidth, InitialWindowHeight);

		void SetViewport(float x, float y, float w, float h)
		{
			_viewport = new((int)x, (int)y, (int)w, (int)h);
			Gl.Viewport(_viewport.X, _viewport.Y, (uint)_viewport.Width, (uint)_viewport.Height);
		}
	}

	protected override void Update()
	{
		base.Update();

		Base.Game.CursorText = null;
		MouseUiContext.Reset(MousePositionWithOffset);

		ActiveLayout?.NestingContext.Update(default);

		if (Input.IsKeyHeld(Keys.ControlLeft) || Input.IsKeyHeld(Keys.ControlRight))
		{
			if (Input.IsKeyPressed(Keys.Z))
				SpawnsetHistoryManager.Undo();
			else if (Input.IsKeyPressed(Keys.Y))
				SpawnsetHistoryManager.Redo();
		}
	}

	protected override void Render()
	{
		Gl.ClearColor(0, 0, 0, 1);
		Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		Shaders.Ui.Use();
		Shaders.Ui.SetMatrix4x4("projection", _projectionMatrix);

		UiRenderer.RenderTopLeft(new(WindowWidth, WindowHeight), default, -100, new(0.1f));

		ActiveLayout?.NestingContext.Render(default);

		if (!string.IsNullOrWhiteSpace(CursorText))
		{
			Vector2i<int> tooltipPosition = MousePositionWithOffset.RoundToVector2Int32() + new Vector2i<int>(16, 16);
			Vector2i<int> textSize = Font.MeasureText(CursorText);
			UiRenderer.RenderTopLeft(textSize, tooltipPosition, 1000, Color.Black);
		}

		Shaders.Font.Use();
		Shaders.Font.SetMatrix4x4("projection", _projectionMatrix);

		ActiveLayout?.NestingContext.RenderText(default);

		MonoSpaceFontRenderer.Render(Vector2i<int>.One, new(0, 640), 500, Color.Green, DebugStack.GetString(), TextAlign.Left);
		MonoSpaceFontRenderer.Render(Vector2i<int>.One, new(1792, 1016), 500, Color.Green, $"{Fps} FPS", TextAlign.Left);
		MonoSpaceFontRenderer.Render(Vector2i<int>.One, new(1792, 1048), 500, Color.Green, $"{Tps} TPS", TextAlign.Left);

		if (!string.IsNullOrWhiteSpace(CursorText))
			MonoSpaceFontRenderer.Render(Vector2i<int>.One, MousePositionWithOffset.RoundToVector2Int32() + new Vector2i<int>(16, 16), 1001, Color.White, CursorText, TextAlign.Left);
	}
}
