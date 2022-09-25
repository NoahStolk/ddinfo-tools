using DevilDaggersInfo.App.Tools.Renderers;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion.Layouts;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion.Layouts.SurvivalEditor;
using DevilDaggersInfo.App.Ui.Base.Enums;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Layouts;
using DevilDaggersInfo.App.Ui.SurvivalEditor.States;
using DevilDaggersInfo.Core.Spawnset;
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
	private readonly Matrix4x4 _uiProjectionMatrix;

	private Viewport _viewportUi;
	private Viewport _viewport3d;
	private int _leftOffset;
	private int _bottomOffset;

	private IExtendedLayout? _activeLayout;

	private MonoSpaceFont _font12X12 = null!;
	private MonoSpaceFont _font8X8 = null!;
	private MonoSpaceFont _font4X6 = null!;

	public Game()
		: base("DevilDaggers.info Tools", 1024, 768, false)
	{
		Root.Game = this; // TODO: Move to Program.cs once source generator is optional.
		_uiProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0, InitialWindowWidth, InitialWindowHeight, 0, -1024, 1024);
	}

	public Vector2 ViewportOffset => new(_leftOffset, _bottomOffset);
	private Vector2 MousePositionWithOffset => Input.GetMousePosition() - ViewportOffset;

	public IExtendedLayout? ActiveLayout
	{
		get => _activeLayout;
		set
		{
			if (_activeLayout == value)
				throw new InvalidOperationException("This layout is already active.");

			_activeLayout = value;
		}
	}

	public IMonoSpaceFontRenderer FontRenderer12X12 { get; private set; } = null!;
	public IMonoSpaceFontRenderer FontRenderer8X8 { get; private set; } = null!;
	public IMonoSpaceFontRenderer FontRenderer4X6 { get; private set; } = null!;
	public IUiRenderer UiRenderer { get; private set; } = null!;

	public IExtendedLayout ConfigLayout { get; } = new Layouts.ConfigLayout();
	public IMainLayout MainLayout { get; } = new Layouts.MainLayout();

	public ISurvivalEditorMainLayout SurvivalEditorMainLayout { get; } = new SurvivalEditorMainLayout();
	public IFileDialogLayout SurvivalEditorOpenLayout { get; } = new SurvivalEditorOpenLayout();
	public IFileDialogLayout SurvivalEditorSaveLayout { get; } = new SurvivalEditorSaveLayout();

	public string? TooltipText { get; set; }

	protected override void LoadContent()
	{
		InitializeContent();

		FontRenderer12X12 = new MonoSpaceFontRenderer();
		FontRenderer8X8 = new MonoSpaceFontRenderer();
		FontRenderer4X6 = new MonoSpaceFontRenderer();
		UiRenderer = new UiRenderer();

		_font12X12 = new(Textures.Font12x12, @" 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!?:()[]{}<>|@^$%#&/\+*`,'=~;.-_  ");
		_font8X8 = new(Textures.Font8x8, @" 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!?:()[]{}<>|@^$%#&/\+*`,'=~;.-_  ");
		_font4X6 = new(Textures.Font4x6, " 0123456789.-");
		FontRenderer12X12.SetFont(_font12X12);
		FontRenderer8X8.SetFont(_font8X8);
		FontRenderer4X6.SetFont(_font4X6);

		StateManager.SetSpawnset("(untitled)", SpawnsetBinary.CreateDefault());
		ActiveLayout = ConfigLayout;

		Gl.Enable(EnableCap.DepthTest);
		Gl.Enable(EnableCap.Blend);
		Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
	}

	public override void OnChangeWindowSize(int width, int height)
	{
		base.OnChangeWindowSize(width, height);

		_viewport3d = new(0, 0, width, height);
		_leftOffset = (width - InitialWindowWidth) / 2;
		_bottomOffset = (height - InitialWindowHeight) / 2;
		_viewportUi = new(_leftOffset, _bottomOffset, InitialWindowWidth, InitialWindowHeight);
	}

	protected override void Update()
	{
		base.Update();

		TooltipText = null;
		MouseUiContext.Reset(MousePositionWithOffset);

		ActiveLayout?.Update();
		ActiveLayout?.NestingContext.Update(default);
	}

	protected override void Render()
	{
		Gl.ClearColor(0, 0, 0, 1);
		Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		ActivateViewport(_viewport3d);
		ActiveLayout?.Render3d();

		ActivateViewport(_viewportUi);
		Shaders.Ui.Use();
		Shaders.Ui.SetMatrix4x4("projection", _uiProjectionMatrix);

		ActiveLayout?.Render();
		ActiveLayout?.NestingContext.Render(default);

		if (!string.IsNullOrWhiteSpace(TooltipText))
		{
			Vector2i<int> tooltipPosition = MousePositionWithOffset.RoundToVector2Int32() + new Vector2i<int>(16, 16);
			Vector2i<int> textSize = _font12X12.MeasureText(TooltipText);
			UiRenderer.RenderTopLeft(textSize, tooltipPosition, 1000, Color.Black);
		}

		Shaders.Font.Use();
		Shaders.Font.SetMatrix4x4("projection", _uiProjectionMatrix);

		ActiveLayout?.RenderText();
		ActiveLayout?.NestingContext.RenderText(default);

		FontRenderer8X8.Render(Vector2i<int>.One, new(0, 640), 500, Color.Green, DebugStack.GetString(), TextAlign.Left);
		FontRenderer8X8.Render(Vector2i<int>.One, new(960, 640), 500, Color.Green, $"{Fps} FPS\n{Tps} TPS", TextAlign.Left);

		if (!string.IsNullOrWhiteSpace(TooltipText))
			FontRenderer8X8.Render(Vector2i<int>.One, MousePositionWithOffset.RoundToVector2Int32() + new Vector2i<int>(16, 16), 1001, Color.White, TooltipText, TextAlign.Left);

		static void ActivateViewport(Viewport viewport)
		{
			Gl.Viewport(viewport.X, viewport.Y, (uint)viewport.Width, (uint)viewport.Height);
		}
	}
}
