using DevilDaggersInfo.Tools.Utils;
using Hexa.NET.ImGui;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui;

/// <summary>
/// Holds the custom fonts. <see cref="Load" /> must be called exactly once, before the ImGui font atlas is built.
/// </summary>
internal sealed class FontService
{
	private static readonly InvalidOperationException _notLoadedException = new("Fonts have not been loaded yet.");

	public Font GoetheBold20
	{
		get => field.Ptr.IsNull ? throw _notLoadedException : field;
		private set;
	}

	public Font GoetheBold30
	{
		get => field.Ptr.IsNull ? throw _notLoadedException : field;
		private set;
	}

	public Font GoetheBold60
	{
		get => field.Ptr.IsNull ? throw _notLoadedException : field;
		private set;
	}

	/// <param name="dpiScale">The framebuffer/window ratio (e.g. 3.0 on Wayland with 300% scaling at 4K).</param>
	public unsafe void Load(float dpiScale)
	{
		string fontPath = Path.Combine(AssemblyUtils.InstallationDirectory, "goethebold.ttf");
		Debug.Assert(File.Exists(fontPath), $"Font file not found: {fontPath}");

		ImGuiIOPtr io = ImGui.GetIO();

		// Since Dear ImGui 1.92 glyphs are baked on demand at (size * RasterizerDensity). Rasterizing at the physical
		// pixel density keeps text crisp on HiDPI displays while every size stays expressed in logical pixels, which is
		// what the hard-coded window and widget dimensions throughout the UI assume.
		//
		// Note this is deliberately not ImGuiStyle.FontScaleDpi: that scales the logical font size, which would make
		// text larger relative to those hard-coded dimensions instead of just sharper.
		ImFontConfigPtr config = ImGui.ImFontConfig();
		try
		{
			config.RasterizerDensity = dpiScale;

			// Add the default font first so it is actually used by default.
			io.Fonts.AddFontDefault(config);

			GoetheBold20 = AddFont(20);
			GoetheBold30 = AddFont(30);
			GoetheBold60 = AddFont(60);

			Font AddFont(float sizeInPixels)
			{
				return new Font(io.Fonts.AddFontFromFileTTF(fontPath, sizeInPixels, config), sizeInPixels);
			}
		}
		finally
		{
			config.Destroy();
		}
	}
}
