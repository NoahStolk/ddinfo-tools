using DevilDaggersInfo.App.Core.ApiClient;
using DevilDaggersInfo.App.Renderers;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion.Layouts;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion.Layouts.SurvivalEditor;
using DevilDaggersInfo.App.Ui.Base.Enums;
using DevilDaggersInfo.App.Ui.Base.Rendering;
using DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Layouts;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Layouts;
using DevilDaggersInfo.App.Ui.SurvivalEditor.States;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Types.Web;
using Serilog;
using Serilog.Core;
using Silk.NET.OpenGL;
using Warp;
using Warp.Debugging;
using Warp.Extensions;
using Warp.Games;
using Warp.Ui;
using Constants = DevilDaggersInfo.App.Ui.Base.Constants;

namespace DevilDaggersInfo.App;

public sealed partial class Game : GameBase, IGameBase<Game>, IDependencyContainer
{
#if WINDOWS
	private const ToolBuildType _toolBuildType = ToolBuildType.WindowsWarp;
#elif LINUX
	private const ToolBuildType _toolBuildType = ToolBuildType.LinuxWarp;
#endif

	private static readonly Logger _log = new LoggerConfiguration()
		.WriteTo.File("ddinfo.log", rollingInterval: RollingInterval.Infinite)
		.CreateLogger();

	private readonly Matrix4x4 _uiProjectionMatrix;
	private readonly UiRenderer _uiRenderer;
	private readonly SpriteRenderer _spriteRenderer;
	private readonly MonoSpaceFontRenderer _fontRenderer12X12;
	private readonly MonoSpaceFontRenderer _fontRenderer8X8;
	private readonly MonoSpaceFontRenderer _fontRenderer4X6;

	private IExtendedLayout? _activeLayout;

	private Game(string initialWindowTitle, int initialWindowWidth, int initialWindowHeight, bool initialWindowFullScreen)
		: base(initialWindowTitle, initialWindowWidth, initialWindowHeight, initialWindowFullScreen)
	{
		AppDomain.CurrentDomain.UnhandledException += (_, args) => _log.Fatal(args.ExceptionObject.ToString());

		_uiProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0, InitialWindowWidth, InitialWindowHeight, 0, -Constants.DepthMax, Constants.DepthMax);

		_uiRenderer = new();
		_spriteRenderer = new();
		_fontRenderer12X12 = new(new(Textures.Font12x12, @" 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!?:()[]{}<>|@^$%#&/\+*`,'=~;.-_  "));
		_fontRenderer8X8 = new(new(Textures.Font8x8, @" 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!?:()[]{}<>|@^$%#&/\+*`,'=~;.-_  "));
		_fontRenderer4X6 = new(new(Textures.Font4x6, " 0123456789.-"));

		Gl.Enable(EnableCap.DepthTest);
		Gl.Enable(EnableCap.Blend);
		Gl.Enable(EnableCap.CullFace);
		Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
	}

	public Vector2 ViewportOffset => new(Program.LeftOffset, Program.BottomOffset);
	public Vector2 UiScale => Program.UiScale;
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

	#region Dependencies

	public AsyncHandler AsyncHandler { get; } = new(VersionUtils.EntryAssemblyVersion, _toolBuildType);

	public IConfigLayout ConfigLayout { get; } = new Layouts.ConfigLayout();
	public IMainLayout MainLayout { get; } = new Layouts.MainLayout();

	public ISurvivalEditorMainLayout SurvivalEditorMainLayout { get; } = new SurvivalEditorMainLayout();
	public IFileDialogLayout SurvivalEditorOpenLayout { get; } = new SurvivalEditorOpenLayout();
	public IFileDialogLayout SurvivalEditorSaveLayout { get; } = new SurvivalEditorSaveLayout();
	public ISurvivalEditor3dLayout SurvivalEditor3dLayout { get; } = new SurvivalEditor3dLayout(Shaders.Mesh);
	public IExtendedLayout CustomLeaderboardsRecorderMainLayout { get; } = new CustomLeaderboardsRecorderMainLayout();

	#endregion Dependencies

	public string? TooltipText { get; set; }

	public static Game Construct(string initialWindowTitle, int initialWindowWidth, int initialWindowHeight, bool initialWindowFullScreen)
	{
		return new(initialWindowTitle, initialWindowWidth, initialWindowHeight, initialWindowFullScreen);
	}

	public void Initialize()
	{
		StateManager.NewSpawnset();
		ActiveLayout = ConfigLayout;
		ConfigLayout.ValidateInstallation();
	}

	protected override void Update()
	{
		base.Update();

		if (!WindowIsActive && !IsPaused)
			TogglePause();

		TooltipText = null;

		StateManager.EmptyUiQueue();
		SpawnsetHistoryManager.EmptyUiQueue();

		MouseUiContext.Reset(MousePositionWithOffset);
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
		Vector2i<int> textSize = _fontRenderer8X8.MeasureText(TooltipText);
		RenderBatchCollector.RenderRectangleTopLeft(textSize, tooltipPosition, 1000, Color.Black);
		RenderBatchCollector.RenderMonoSpaceText(FontSize.F8X8, Vector2i<int>.One, MousePositionWithOffset.RoundToVector2Int32() + tooltipOffset, 1001, Color.White, TooltipText, TextAlign.Left);
	}

	protected override void Render()
	{
		Gl.ClearColor(0, 0, 0, 1);
		Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		ActivateViewport(Program.Viewport3d);

		ActiveLayout?.Render3d();

		ActivateViewport(Program.ViewportUi);

		Shaders.Ui.Use();
		Shaders.Ui.SetMatrix4x4("projection", _uiProjectionMatrix);
		_uiRenderer.RenderRectangleTriangles();
		_uiRenderer.RenderCircleLines();

		Shaders.Font.Use();
		Shaders.Font.SetMatrix4x4("projection", _uiProjectionMatrix);
		_fontRenderer4X6.Render(RenderBatchCollector.MonoSpaceTexts4X6);
		_fontRenderer8X8.Render(RenderBatchCollector.MonoSpaceTexts8X8);
		_fontRenderer12X12.Render(RenderBatchCollector.MonoSpaceTexts12X12);

		Shaders.Sprite.Use();
		Shaders.Sprite.SetMatrix4x4("projection", _uiProjectionMatrix);
		_spriteRenderer.Render();

		RenderBatchCollector.Clear();

		static void ActivateViewport(Viewport viewport)
		{
			Gl.Viewport(viewport.X, viewport.Y, (uint)viewport.Width, (uint)viewport.Height);
		}
	}
}
