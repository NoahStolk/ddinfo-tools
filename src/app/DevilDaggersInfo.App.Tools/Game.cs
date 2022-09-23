using DevilDaggersInfo.App.Tools.Renderers;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion.SurvivalEditor;
using DevilDaggersInfo.App.Ui.Base.Enums;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Layouts;
using DevilDaggersInfo.App.Ui.SurvivalEditor.States;
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

public partial class Game : GameBase, IDependencyContainer
{
	private readonly Matrix4x4 _projectionMatrix;

	private Viewport _viewport;
	private int _leftOffset;
	private int _bottomOffset;

	private ILayout? _activeLayout;

	public Game()
		: base("DevilDaggers.info Tools", 1920, 1080, false)
	{
		Root.Game = this; // TODO: Move to Program.cs once source generator is optional.
		_projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0, InitialWindowWidth, InitialWindowHeight, 0, -1024, 1024);
	}

	public Vector2 ViewportOffset => new(_leftOffset, _bottomOffset);
	private Vector2 MousePositionWithOffset => Input.GetMousePosition() - ViewportOffset;

	public ILayout? ActiveLayout
	{
		get => _activeLayout;
		set
		{
			if (_activeLayout == value)
				throw new InvalidOperationException("This layout is already active.");

			_activeLayout = value;
		}
	}

	public MonoSpaceFont Font { get; private set; } = null!;
	public MonoSpaceFont FontSmall { get; private set; } = null!;

	public IMonoSpaceFontRenderer MonoSpaceFontRenderer { get; private set; } = null!;
	public IMonoSpaceFontRenderer MonoSpaceSmallFontRenderer { get; private set; } = null!;
	public IUiRenderer UiRenderer { get; private set; } = null!;

	public Layout MainLayout { get; } = new Layouts.MainLayout();
	public ISurvivalEditorMainLayout SurvivalEditorMainLayout { get; } = new MainLayout();
	public Layout OpenLayout { get; } = new OpenLayout();
	public Layout SaveLayout { get; } = new SaveLayout();

	public string? CursorText { get; set; }

	protected override void LoadContent()
	{
		InitializeContent();

		MonoSpaceFontRenderer = new MonoSpaceFontRenderer();
		MonoSpaceSmallFontRenderer = new MonoSpaceFontRenderer();
		UiRenderer = new UiRenderer();

		Font = new(Textures.Font, @" 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!?:()[]{}<>|@^$%#&/\+*`,'=~;.-_  ");
		FontSmall = new(Textures.FontSmall, " 0123456789.-");
		MonoSpaceFontRenderer.SetFont(Font);
		MonoSpaceSmallFontRenderer.SetFont(FontSmall);

		StateManager.SetSpawnset("(untitled)", SpawnsetBinary.CreateDefault());
		ActiveLayout = MainLayout;

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

		CursorText = null;
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

		// TODO: Refactor.
		if (ActiveLayout == SurvivalEditorMainLayout)
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
