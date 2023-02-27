using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Rendering;
using DevilDaggersInfo.App.Ui.Base.Rendering.Renderers;
using DevilDaggersInfo.App.Ui.Base.Settings;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Core.Versioning;
using Silk.NET.OpenGL;
using Warp.NET.Debugging;
using Warp.NET.Extensions;
using Warp.NET.Text;
using Warp.NET.Ui;
using Constants = DevilDaggersInfo.App.Ui.Base.Constants;

namespace DevilDaggersInfo.App;

[GenerateGame]
public sealed partial class Game : GameBase, IGame
{
	private readonly Matrix4x4 _uiProjectionMatrix;

	private Game()
	{
		AppDomain.CurrentDomain.UnhandledException += (_, args) => Root.Dependencies.Log.Fatal(args.ExceptionObject.ToString());

		_uiProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0, InitialWindowState.Width, InitialWindowState.Height, 0, -Constants.DepthMax, Constants.DepthMax);

		Audio.Initialize();

		Gl.Enable(EnableCap.DepthTest);
		Gl.Enable(EnableCap.Blend);
		Gl.Enable(EnableCap.CullFace);
		Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		if (!AppVersion.TryParse(VersionUtils.EntryAssemblyVersion, out AppVersion? appVersion))
			throw new InvalidOperationException("The current version number is invalid.");

		AppVersion = appVersion;

		UserSettings.Load();
	}

	public AppVersion AppVersion { get; }

	public TooltipContext? TooltipContext { get; set; }

	public MonoSpaceFontRenderer MonoSpaceFontRenderer8 { get; } = new(new(WarpTextures.Spleen5x8, WarpCharsets.Ascii_32_126));
	public MonoSpaceFontRenderer MonoSpaceFontRenderer12 { get; } = new(new(WarpTextures.Spleen6x12, WarpCharsets.Ascii_32_126));
	public MonoSpaceFontRenderer MonoSpaceFontRenderer16 { get; } = new(new(WarpTextures.Spleen8x16, WarpCharsets.Ascii_32_126));
	public MonoSpaceFontRenderer MonoSpaceFontRenderer24 { get; } = new(new(WarpTextures.Spleen12x24, WarpCharsets.Ascii_32_126));
	public MonoSpaceFontRenderer MonoSpaceFontRenderer32 { get; } = new(new(WarpTextures.Spleen16x32, WarpCharsets.Ascii_32_126));
	public MonoSpaceFontRenderer MonoSpaceFontRenderer64 { get; } = new(new(WarpTextures.Spleen32x64, WarpCharsets.Ascii_32_126));

	public SpriteRenderer SpriteRenderer { get; } = new();
	public RectangleRenderer RectangleRenderer { get; } = new();
	public EllipseRenderer EllipseRenderer { get; } = new();
	public LineRenderer LineRenderer { get; } = new();

	protected override void Update()
	{
		StateManager.ReduceAll();

		base.Update();

		TooltipContext = null;

		MouseUiContext.Reset(ViewportState.MousePosition);
		StateManager.LayoutState.CurrentLayout?.Update();
		StateManager.LayoutState.CurrentLayout?.NestingContext.Update(default);
	}

	protected override void PrepareRender()
	{
		base.PrepareRender();

		StateManager.LayoutState.CurrentLayout?.Render();
		StateManager.LayoutState.CurrentLayout?.NestingContext.Render(default);

		if (UserSettings.Model.ShowDebugOutput)
		{
			MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, new(0, 640), 500, Color.Green, DebugStack.GetString(), TextAlign.Left);
			MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, new(960, 736), 500, Color.Green, $"{Fps} FPS\n{Tps} TPS", TextAlign.Left);
		}

		if (TooltipContext == null)
			return;

		Vector2i<int> tooltipOffset = new Vector2i<int>(16, 16) / ViewportState.Scale.FloorToVector2Int32();
		Vector2i<int> textSize = MonoSpaceFontRenderer12.Font.MeasureText(TooltipContext.Value.Text);
		Vector2i<int> tooltipPosition = ViewportState.MousePosition.RoundToVector2Int32() + tooltipOffset + textSize / 2;
		RectangleRenderer.Schedule(textSize, tooltipPosition, 1000, TooltipContext.Value.BackgroundColor);
		MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, ViewportState.MousePosition.RoundToVector2Int32() + tooltipOffset, 1001, TooltipContext.Value.ForegroundColor, TooltipContext.Value.Text, TextAlign.Left);
	}

	protected override void Render()
	{
		Gl.ClearColor(0, 0, 0, 1);
		Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		if (!UserSettings.Model.RenderWhileWindowIsInactive && !WindowIsActive)
			return;

		ActivateViewport(ViewportState.Viewport3d);

		StateManager.LayoutState.CurrentLayout?.Render3d();

		ActivateViewport(ViewportState.Viewport);

		WarpShaders.Ui.Use();
		Shader.SetMatrix4x4(UiUniforms.Projection, _uiProjectionMatrix);
		RectangleRenderer.Render();
		EllipseRenderer.Render();
		LineRenderer.Render();

		WarpShaders.Font.Use();
		Shader.SetMatrix4x4(FontUniforms.Projection, _uiProjectionMatrix);
		MonoSpaceFontRenderer8.Render();
		MonoSpaceFontRenderer12.Render();
		MonoSpaceFontRenderer16.Render();
		MonoSpaceFontRenderer24.Render();
		MonoSpaceFontRenderer32.Render();
		MonoSpaceFontRenderer64.Render();

		WarpShaders.Sprite.Use();
		Shader.SetMatrix4x4(SpriteUniforms.Projection, _uiProjectionMatrix);
		SpriteRenderer.Render();
	}

	private static void ActivateViewport(Viewport viewport)
	{
		Gl.Viewport(viewport.X, viewport.Y, (uint)viewport.Width, (uint)viewport.Height);
	}
}
