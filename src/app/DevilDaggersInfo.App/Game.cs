using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion.Layouts;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion.Layouts.SurvivalEditor;
using DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Layouts;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Layouts;
using DevilDaggersInfo.App.Ui.SurvivalEditor.States;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Core.Versioning;
using DevilDaggersInfo.Types.Web;
using Serilog;
using Serilog.Core;
using Silk.NET.OpenGL;
using Warp.NET.Debugging;
using Warp.NET.Extensions;
using Warp.NET.RenderImpl.Ui;
using Warp.NET.Text;
using Warp.NET.Ui;
using Constants = DevilDaggersInfo.App.Ui.Base.Constants;

namespace DevilDaggersInfo.App;

[GenerateGame]
public sealed partial class Game : RenderImplUiGameBase, IDependencyContainer
{
#if WINDOWS
	public const ToolBuildType BuildType = ToolBuildType.WindowsWarp;
#elif LINUX
	public const ToolBuildType BuildType = ToolBuildType.LinuxWarp;
#endif

	private static readonly Logger _log = new LoggerConfiguration()
		.WriteTo.File("ddinfo.log", rollingInterval: RollingInterval.Infinite)
		.CreateLogger();

	private readonly Matrix4x4 _uiProjectionMatrix;

	private IExtendedLayout? _activeLayout;

	private Game(GameParameters gameParameters)
		: base(gameParameters)
	{
		AppDomain.CurrentDomain.UnhandledException += (_, args) => _log.Fatal(args.ExceptionObject.ToString());

		_uiProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0, InitialWindowWidth, InitialWindowHeight, 0, -Constants.DepthMax, Constants.DepthMax);

		Gl.Enable(EnableCap.DepthTest);
		Gl.Enable(EnableCap.Blend);
		Gl.Enable(EnableCap.CullFace);
		Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		if (!AppVersion.TryParse(VersionUtils.EntryAssemblyVersion, out AppVersion? appVersion))
			throw new InvalidOperationException("The current version number is invalid.");

		AppVersion = appVersion;
	}

	public AppVersion AppVersion { get; }

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

	public IConfigLayout ConfigLayout { get; } = new Layouts.ConfigLayout();
	public IMainLayout MainLayout { get; } = new Layouts.MainLayout();

	public ISurvivalEditorMainLayout SurvivalEditorMainLayout { get; } = new SurvivalEditorMainLayout();
	public IFileDialogLayout SurvivalEditorOpenLayout { get; } = new SurvivalEditorOpenLayout();
	public IFileDialogLayout SurvivalEditorSaveLayout { get; } = new SurvivalEditorSaveLayout();
	public ISurvivalEditor3dLayout SurvivalEditor3dLayout { get; } = new SurvivalEditor3dLayout();
	public IExtendedLayout CustomLeaderboardsRecorderMainLayout { get; } = new CustomLeaderboardsRecorderMainLayout();

	#endregion Dependencies

	public string? TooltipText { get; set; }

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

		MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, new(0, 640), 500, Color.Green, DebugStack.GetString(), TextAlign.Left);
		MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, new(960, 736), 500, Color.Green, $"{Fps} FPS\n{Tps} TPS", TextAlign.Left);

		if (string.IsNullOrWhiteSpace(TooltipText))
			return;

		Vector2i<int> tooltipOffset = new Vector2i<int>(16, 16) / UiScale.FloorToVector2Int32();
		Vector2i<int> textSize = MonoSpaceFontRenderer12.Font.MeasureText(TooltipText);
		Vector2i<int> tooltipPosition = MousePositionWithOffset.RoundToVector2Int32() + tooltipOffset + textSize / 2;
		RectangleRenderer.Schedule(textSize, tooltipPosition, 1000, Color.Black);
		MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, MousePositionWithOffset.RoundToVector2Int32() + tooltipOffset, 1001, Color.White, TooltipText, TextAlign.Left);
	}

	protected override void Render()
	{
		Gl.ClearColor(0, 0, 0, 1);
		Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		ActivateViewport(Program.Viewport3d);

		ActiveLayout?.Render3d();

		ActivateViewport(Program.ViewportUi);

		WarpRenderImplUiShaders.Ui.Use();
		Shader.SetMatrix4x4(UiUniforms.Projection, _uiProjectionMatrix);
		RectangleRenderer.Render();
		CircleRenderer.Render();

		WarpRenderImplUiShaders.Font.Use();
		Shader.SetMatrix4x4(FontUniforms.Projection, _uiProjectionMatrix);
		MonoSpaceFontRenderer8.Render();
		MonoSpaceFontRenderer12.Render();
		MonoSpaceFontRenderer16.Render();
		MonoSpaceFontRenderer24.Render();
		MonoSpaceFontRenderer32.Render();
		MonoSpaceFontRenderer64.Render();

		WarpRenderImplUiShaders.Sprite.Use();
		Shader.SetMatrix4x4(SpriteUniforms.Projection, _uiProjectionMatrix);
		SpriteRenderer.Render();

		static void ActivateViewport(Viewport viewport)
		{
			Gl.Viewport(viewport.X, viewport.Y, (uint)viewport.Width, (uint)viewport.Height);
		}
	}
}
