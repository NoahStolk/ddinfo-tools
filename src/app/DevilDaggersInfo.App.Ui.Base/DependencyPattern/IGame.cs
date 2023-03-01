using DevilDaggersInfo.App.Ui.Base.Rendering.Renderers;
using DevilDaggersInfo.App.Ui.Base.Rendering.Text;
using DevilDaggersInfo.Core.Versioning;

namespace DevilDaggersInfo.App.Ui.Base.DependencyPattern;

public interface IGame
{
	AppVersion AppVersion { get; }

	float Dt { get; }
	float Tt { get; }
	float MainLoopRate { get; set; }

	TooltipContext? TooltipContext { get; set; }

	#region Renderers

	MonoSpaceFontRenderer MonoSpaceFontRenderer8 { get; }
	MonoSpaceFontRenderer MonoSpaceFontRenderer12 { get; }
	MonoSpaceFontRenderer MonoSpaceFontRenderer16 { get; }
	MonoSpaceFontRenderer MonoSpaceFontRenderer24 { get; }
	MonoSpaceFontRenderer MonoSpaceFontRenderer32 { get; }
	MonoSpaceFontRenderer MonoSpaceFontRenderer64 { get; }

	SpriteRenderer SpriteRenderer { get; }
	RectangleRenderer RectangleRenderer { get; }
	EllipseRenderer EllipseRenderer { get; }
	LineRenderer LineRenderer { get; }

	#endregion Renderers

	public MonoSpaceFontRenderer GetFontRenderer(FontSize fontSize) => fontSize switch
	{
		FontSize.H8 => MonoSpaceFontRenderer8,
		FontSize.H12 => MonoSpaceFontRenderer12,
		FontSize.H16 => MonoSpaceFontRenderer16,
		FontSize.H24 => MonoSpaceFontRenderer24,
		FontSize.H32 => MonoSpaceFontRenderer32,
		FontSize.H64 => MonoSpaceFontRenderer64,
		_ => throw new NotSupportedException($"Font size '{fontSize}' is not supported."),
	};
}
