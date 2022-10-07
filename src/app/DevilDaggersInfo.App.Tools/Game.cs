using DevilDaggersInfo.App.Tools.Renderers;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion.Layouts;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion.Layouts.SurvivalEditor;
using DevilDaggersInfo.App.Ui.Base.Enums;
using DevilDaggersInfo.App.Ui.Base.Rendering;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Layouts;
using DevilDaggersInfo.App.Ui.SurvivalEditor.States;
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
	private const float _nativeAspectRatio = Constants.NativeWidth / (float)Constants.NativeHeight;

	private readonly Matrix4x4 _uiProjectionMatrix;

	private Viewport _viewportUi;
	private int _leftOffset;
	private int _bottomOffset;

	private Viewport _viewport3d;

	private IExtendedLayout? _activeLayout;

	private MonoSpaceFont _font12X12 = null!;
	private MonoSpaceFont _font8X8 = null!;
	private MonoSpaceFont _font4X6 = null!;

	public Game()
		: base("DEVIL DAGGERS EXTENSIONS", Constants.NativeWidth, Constants.NativeHeight, false)
	{
		Root.Game = this; // TODO: Move to Program.cs once source generator is optional.
		_uiProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0, InitialWindowWidth, InitialWindowHeight, 0, -Constants.DepthMax, Constants.DepthMax);
	}

	public Vector2 ViewportOffset => new(_leftOffset, _bottomOffset);
	public Vector2 UiScale { get; private set; }
	public Vector2 MousePositionWithOffset => (Input.GetMousePosition() - ViewportOffset) / UiScale;

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

	public UiRenderer UiRenderer { get; private set; } = null!;
	public SpriteRenderer SpriteRenderer { get; private set; } = null!;
	public MonoSpaceFontRenderer FontRenderer12X12 { get; private set; } = null!;
	public MonoSpaceFontRenderer FontRenderer8X8 { get; private set; } = null!;
	public MonoSpaceFontRenderer FontRenderer4X6 { get; private set; } = null!;

	public IConfigLayout ConfigLayout { get; } = new Layouts.ConfigLayout();
	public IMainLayout MainLayout { get; } = new Layouts.MainLayout();

	public ISurvivalEditorMainLayout SurvivalEditorMainLayout { get; } = new SurvivalEditorMainLayout();
	public IFileDialogLayout SurvivalEditorOpenLayout { get; } = new SurvivalEditorOpenLayout();
	public IFileDialogLayout SurvivalEditorSaveLayout { get; } = new SurvivalEditorSaveLayout();

	public string? TooltipText { get; set; }

	protected override void LoadContent()
	{
		InitializeContent();

		_font12X12 = new(Textures.Font12x12, @" 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!?:()[]{}<>|@^$%#&/\+*`,'=~;.-_  ");
		_font8X8 = new(Textures.Font8x8, @" 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!?:()[]{}<>|@^$%#&/\+*`,'=~;.-_  ");
		_font4X6 = new(Textures.Font4x6, " 0123456789.-");

		UiRenderer = new();
		SpriteRenderer = new();
		FontRenderer12X12 = new(_font12X12);
		FontRenderer8X8 = new(_font8X8);
		FontRenderer4X6 = new(_font4X6);

		StateManager.NewSpawnset();
		ActiveLayout = ConfigLayout;
		ConfigLayout.ValidateInstallation();

		Gl.Enable(EnableCap.DepthTest);
		Gl.Enable(EnableCap.Blend);
		Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
	}

	public override void OnChangeWindowSize(int width, int height)
	{
		base.OnChangeWindowSize(width, height);

		_viewport3d = new(0, 0, width, height);

		int minDimension = (int)Math.Min(height, width / _nativeAspectRatio);
		int clampedHeight = Math.Max(Constants.NativeHeight, minDimension / Constants.NativeHeight * Constants.NativeHeight);

		float originalAspectRatio = InitialWindowWidth / (float)InitialWindowHeight;
		float adjustedWidth = clampedHeight * originalAspectRatio; // Adjusted for aspect ratio
		_leftOffset = (int)((width - adjustedWidth) / 2);
		_bottomOffset = (height - clampedHeight) / 2;
		_viewportUi = new(_leftOffset, _bottomOffset, (int)adjustedWidth, clampedHeight); // Fix viewport to maintain aspect ratio

		UiScale = new(_viewportUi.Width / (float)InitialWindowWidth, _viewportUi.Height / (float)InitialWindowHeight);
	}

	protected override void Update()
	{
		base.Update();

		TooltipText = null;

		if (WindowIsActive)
			MouseUiContext.Reset(MousePositionWithOffset);

		StateManager.EmptyUiQueue();
		SpawnsetHistoryManager.EmptyUiQueue();

		ActiveLayout?.Update();
		ActiveLayout?.NestingContext.Update(default);
	}

	protected override void PrepareRender()
	{
		base.PrepareRender();

		ActiveLayout?.Render();
		ActiveLayout?.NestingContext.Render(default);

		RenderBatchCollector.RenderMonoSpaceText(FontSize.F8X8, Vector2i<int>.One, new(0, 640), 500, Color.Green, DebugStack.GetString(), TextAlign.Left);
		RenderBatchCollector.RenderMonoSpaceText(FontSize.F8X8, Vector2i<int>.One, new(960, 736), 500, Color.Green, $"{Fps} FPS\n{Tps} TPS", TextAlign.Left);

		if (string.IsNullOrWhiteSpace(TooltipText))
			return;

		Vector2i<int> tooltipOffset = new Vector2i<int>(16, 16) / UiScale.FloorToVector2Int32();
		Vector2i<int> tooltipPosition = MousePositionWithOffset.RoundToVector2Int32() + tooltipOffset;
		Vector2i<int> textSize = _font8X8.MeasureText(TooltipText);
		RenderBatchCollector.RenderRectangleTopLeft(textSize, tooltipPosition, 1000, Color.Black);
		RenderBatchCollector.RenderMonoSpaceText(FontSize.F8X8, Vector2i<int>.One, MousePositionWithOffset.RoundToVector2Int32() + tooltipOffset, 1001, Color.White, TooltipText, TextAlign.Left);
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
		UiRenderer.RenderRectangleTriangles();
		UiRenderer.RenderCircleLines();

		Shaders.Font.Use();
		Shaders.Font.SetMatrix4x4("projection", _uiProjectionMatrix);
		FontRenderer4X6.Render(RenderBatchCollector.MonoSpaceTexts4X6);
		FontRenderer8X8.Render(RenderBatchCollector.MonoSpaceTexts8X8);
		FontRenderer12X12.Render(RenderBatchCollector.MonoSpaceTexts12X12);

		Shaders.Sprite.Use();
		Shaders.Sprite.SetMatrix4x4("projection", _uiProjectionMatrix);
		SpriteRenderer.Render();

		RenderBatchCollector.Clear();

		static void ActivateViewport(Viewport viewport)
		{
			Gl.Viewport(viewport.X, viewport.Y, (uint)viewport.Width, (uint)viewport.Height);
		}
	}
}
