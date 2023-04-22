using DevilDaggersInfo.App.Engine;
using DevilDaggersInfo.App.Engine.Debugging;
using DevilDaggersInfo.App.Engine.Extensions;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Rendering;
using DevilDaggersInfo.App.Ui.Base.Rendering.Renderers;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.User.Cache;
using DevilDaggersInfo.App.Ui.Base.User.Settings;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Core.Versioning;
using Silk.NET.OpenGL;
using System.Diagnostics;
using Constants = DevilDaggersInfo.App.Ui.Base.Constants;

namespace DevilDaggersInfo.App;

public sealed class Game : GameBase, IGame
{
	private readonly Matrix4x4 _uiProjectionMatrix;

	public Game()
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
		UserCache.Load();
	}

	public AppVersion AppVersion { get; }

	public TooltipContext? TooltipContext { get; set; }

	public MonoSpaceFontRenderer MonoSpaceFontRenderer8 { get; } = new(new(Textures.Spleen5x8, Charsets.Ascii_32_126));
	public MonoSpaceFontRenderer MonoSpaceFontRenderer12 { get; } = new(new(Textures.Spleen6x12, Charsets.Ascii_32_126));
	public MonoSpaceFontRenderer MonoSpaceFontRenderer16 { get; } = new(new(Textures.Spleen8x16, Charsets.Ascii_32_126));
	public MonoSpaceFontRenderer MonoSpaceFontRenderer24 { get; } = new(new(Textures.Spleen12x24, Charsets.Ascii_32_126));
	public MonoSpaceFontRenderer MonoSpaceFontRenderer32 { get; } = new(new(Textures.Spleen16x32, Charsets.Ascii_32_126));
	public MonoSpaceFontRenderer MonoSpaceFontRenderer64 { get; } = new(new(Textures.Spleen32x64, Charsets.Ascii_32_126));

	public SpriteRenderer SpriteRenderer { get; } = new();
	public RectangleRenderer RectangleRenderer { get; } = new();
	public EllipseRenderer EllipseRenderer { get; } = new();
	public LineRenderer LineRenderer { get; } = new();

	protected override void Update()
	{
		StateManager.ReduceAll();

		TooltipContext = null;

		MouseUiContext.Reset(ViewportState.MousePosition);
		StateManager.LayoutState.CurrentLayout?.Update();
		StateManager.LayoutState.CurrentLayout?.NestingContext.Update(default);
	}

	protected override void PrepareRender()
	{
		StateManager.LayoutState.CurrentLayout?.Render();
		StateManager.LayoutState.CurrentLayout?.NestingContext.Render(default);

		if (UserSettings.Model.ShowDebugOutput)
		{
			MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, new(0, 640), 500, Color.Green, DebugStack.GetString(), TextAlign.Left);
			MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, new(960, 736), 500, Color.Green, $"{Fps} FPS\n{Tps} TPS", TextAlign.Left);
		}

		if (TooltipContext == null)
			return;

		Vector2i<int> textSize = MonoSpaceFontRenderer12.Font.MeasureText(TooltipContext.Value.Text);
		Vector2i<int> tooltipOffset = TooltipContext.Value.TextAlign switch
		{
			TextAlign.Left => new Vector2i<int>(16, 16) / ViewportState.Scale.FloorToVector2Int32(),
			TextAlign.Middle => new Vector2i<int>(-textSize.X / 2, 0) + new Vector2i<int>(0, 16) / ViewportState.Scale.FloorToVector2Int32(),
			TextAlign.Right => new Vector2i<int>(-textSize.X, 0) + new Vector2i<int>(-4, 16) / ViewportState.Scale.FloorToVector2Int32(),
			_ => throw new UnreachableException(),
		};
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

		ViewportState.Viewport3d.Activate();

		StateManager.LayoutState.CurrentLayout?.Render3d();

		ViewportState.Viewport.Activate();

		UiShader.Use();
		UiShader.SetProjection(_uiProjectionMatrix);
		RectangleRenderer.Render();
		EllipseRenderer.Render();
		LineRenderer.Render();

		FontShader.Use();
		FontShader.SetProjection(_uiProjectionMatrix);
		MonoSpaceFontRenderer8.Render();
		MonoSpaceFontRenderer12.Render();
		MonoSpaceFontRenderer16.Render();
		MonoSpaceFontRenderer24.Render();
		MonoSpaceFontRenderer32.Render();
		MonoSpaceFontRenderer64.Render();

		SpriteShader.Use();
		SpriteShader.SetProjection(_uiProjectionMatrix);
		SpriteRenderer.Render();

		Gl.Disable(EnableCap.ScissorTest);
	}
}
