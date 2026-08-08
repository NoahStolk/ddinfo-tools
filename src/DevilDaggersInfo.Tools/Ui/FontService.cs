using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui;

/// <summary>
/// Holds the custom fonts. <see cref="Load" /> must be called exactly once, before the ImGui font atlas is built.
/// </summary>
internal sealed class FontService
{
	private static readonly InvalidOperationException _notLoadedException = new("Fonts have not been loaded yet.");

	public unsafe ImFontPtr GoetheBold20
	{
		get => field.NativePtr == (void*)0 ? throw _notLoadedException : field;
		private set;
	}

	public unsafe ImFontPtr GoetheBold30
	{
		get => field.NativePtr == (void*)0 ? throw _notLoadedException : field;
		private set;
	}

	public unsafe ImFontPtr GoetheBold60
	{
		get => field.NativePtr == (void*)0 ? throw _notLoadedException : field;
		private set;
	}

	/// <param name="dpiScale">The framebuffer/window ratio (e.g. 3.0 on Wayland with 300% scaling at 4K).</param>
	public void Load(float dpiScale)
	{
		string fontPath = Path.Combine(AssemblyUtils.InstallationDirectory, "goethebold.ttf");
		Debug.Assert(File.Exists(fontPath), $"Font file not found: {fontPath}");

		ImGuiIOPtr io = ImGui.GetIO();

		// Rasterize the fonts at physical pixel size for crispness, then scale back so layout stays in logical pixels.
		float fontScale = 1f / dpiScale;

		GoetheBold20 = AddFont(20);
		GoetheBold30 = AddFont(30);
		GoetheBold60 = AddFont(60);

		ImFontPtr AddFont(float sizeInPixels)
		{
			ImFontPtr font = io.Fonts.AddFontFromFileTTF(fontPath, sizeInPixels * dpiScale);
			font.Scale = fontScale;
			return font;
		}
	}
}
