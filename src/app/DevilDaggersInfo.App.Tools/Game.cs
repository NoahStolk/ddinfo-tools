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

	private MonoSpaceFont _font = null!;
	private MonoSpaceFont _fontSmall = null!;

	public Game()
		: base("DevilDaggers.info Tools", 1920, 1080, false)
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

	public IMonoSpaceFontRenderer MonoSpaceFontRenderer { get; private set; } = null!;
	public IMonoSpaceFontRenderer MonoSpaceSmallFontRenderer { get; private set; } = null!;
	public IUiRenderer UiRenderer { get; private set; } = null!;

	public IExtendedLayout ConfigLayout { get; } = new Layouts.ConfigLayout();
	public IMainLayout MainLayout { get; } = new Layouts.MainLayout();

	public ISurvivalEditorMainLayout SurvivalEditorMainLayout { get; } = new SurvivalEditorMainLayout();
	public IExtendedLayout SurvivalEditorOpenLayout { get; } = new SurvivalEditorOpenLayout();
	public IExtendedLayout SurvivalEditorSaveLayout { get; } = new SurvivalEditorSaveLayout();

	public string? TooltipText { get; set; }

	protected override void LoadContent()
	{
		InitializeContent();

		MonoSpaceFontRenderer = new MonoSpaceFontRenderer();
		MonoSpaceSmallFontRenderer = new MonoSpaceFontRenderer();
		UiRenderer = new UiRenderer();

		_font = new(Textures.Font, @" 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!?:()[]{}<>|@^$%#&/\+*`,'=~;.-_  ");
		_fontSmall = new(Textures.FontSmall, " 0123456789.-");
		MonoSpaceFontRenderer.SetFont(_font);
		MonoSpaceSmallFontRenderer.SetFont(_fontSmall);

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
			Vector2i<int> textSize = _font.MeasureText(TooltipText);
			UiRenderer.RenderTopLeft(textSize, tooltipPosition, 1000, Color.Black);
		}

		Shaders.Font.Use();
		Shaders.Font.SetMatrix4x4("projection", _uiProjectionMatrix);

		ActiveLayout?.RenderText();
		ActiveLayout?.NestingContext.RenderText(default);

		MonoSpaceFontRenderer.Render(Vector2i<int>.One, new(0, 640), 500, Color.Green, DebugStack.GetString(), TextAlign.Left);
		MonoSpaceFontRenderer.Render(Vector2i<int>.One, new(1792, 1016), 500, Color.Green, $"{Fps} FPS", TextAlign.Left);
		MonoSpaceFontRenderer.Render(Vector2i<int>.One, new(1792, 1048), 500, Color.Green, $"{Tps} TPS", TextAlign.Left);

		if (!string.IsNullOrWhiteSpace(TooltipText))
			MonoSpaceFontRenderer.Render(Vector2i<int>.One, MousePositionWithOffset.RoundToVector2Int32() + new Vector2i<int>(16, 16), 1001, Color.White, TooltipText, TextAlign.Left);

		static void ActivateViewport(Viewport viewport)
		{
			Gl.Viewport(viewport.X, viewport.Y, (uint)viewport.Width, (uint)viewport.Height);
		}
	}
}
