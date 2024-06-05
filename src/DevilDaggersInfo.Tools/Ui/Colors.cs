using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

public static class Colors
{
	private const byte _alphaButton = 102;
	private const byte _alphaButtonActive = 191;
	private const byte _alphaHeader = 79;
	private const byte _alphaHeaderHovered = 204;
	private const byte _alphaFrameBackground = 138;
	private const byte _alphaTitleBackgroundActive = 138;
	private const byte _alphaFrameBackgroundHovered = 102;
	private const byte _alphaFrameBackgroundActive = 171;
	private const byte _alphaSeparatorHovered = 199;

	public static Vector4 TitleColor => Vector4.Lerp(new Vector4(1, 0.2f, 0.05f, 1), new Vector4(1, 0.5f, 0.2f, 1), MathF.Sin(Root.Application.TotalTime));

	public static ColorConfiguration Main { get; } = new()
	{
		Primary = new Color(250, 66, 66, 255),
		Secondary = new Color(224, 61, 61, 255),
		Tertiary = new Color(122, 41, 41, 255),
		Quaternary = new Color(191, 26, 26, 255),
	};

	public static ColorConfiguration SpawnsetEditor { get; } = new()
	{
		Primary = new Color(250, 66, 66, 255),
		Secondary = new Color(224, 61, 61, 255),
		Tertiary = new Color(122, 41, 41, 255),
		Quaternary = new Color(191, 26, 26, 255),
	};

	public static ColorConfiguration CustomLeaderboards { get; } = new()
	{
		Primary = new Color(150, 66, 250, 255),
		Secondary = new Color(133, 61, 224, 255),
		Tertiary = new Color(74, 41, 122, 255),
		Quaternary = new Color(102, 26, 191, 255),
	};

	public static ColorConfiguration ReplayEditor { get; } = new()
	{
		Primary = new Color(66, 66, 250, 255),
		Secondary = new Color(61, 61, 224, 255),
		Tertiary = new Color(41, 41, 122, 255),
		Quaternary = new Color(26, 26, 191, 255),
	};

	public static ColorConfiguration Practice { get; } = new()
	{
		Primary = new Color(250, 150, 66, 255),
		Secondary = new Color(224, 133, 61, 255),
		Tertiary = new Color(122, 74, 41, 255),
		Quaternary = new Color(191, 102, 26, 255),
	};

	public static ColorConfiguration AssetEditor { get; } = new()
	{
		Primary = new Color(66, (byte)(150 * 1.25f), 66, 255),
		Secondary = new Color(61, (byte)(133 * 1.25f), 61, 255),
		Tertiary = new Color(41, (byte)(74 * 1.25f), 41, 255),
		Quaternary = new Color(26, (byte)(102 * 1.25f), 26, 255),
	};

	public static ColorConfiguration ModManager { get; } = new()
	{
		Primary = new Color(150, 150, 66, 255),
		Secondary = new Color(133, 133, 61, 255),
		Tertiary = new Color(74, 74, 41, 255),
		Quaternary = new Color(102, 102, 26, 255),
	};

	public static void SetColors(ColorConfiguration colorConfiguration)
	{
		ImGuiStylePtr style = ImGui.GetStyle();
		style.Colors[(int)ImGuiCol.CheckMark] = colorConfiguration.Primary;
		style.Colors[(int)ImGuiCol.SliderGrab] = colorConfiguration.Secondary;
		style.Colors[(int)ImGuiCol.SliderGrabActive] = colorConfiguration.Primary;
		style.Colors[(int)ImGuiCol.Button] = colorConfiguration.Primary with { A = _alphaButton };
		style.Colors[(int)ImGuiCol.ButtonHovered] = colorConfiguration.Primary;
		style.Colors[(int)ImGuiCol.ButtonActive] = colorConfiguration.Primary with { B = _alphaButtonActive };
		style.Colors[(int)ImGuiCol.Header] = colorConfiguration.Primary with { A = _alphaHeader };
		style.Colors[(int)ImGuiCol.HeaderHovered] = colorConfiguration.Primary with { A = _alphaHeaderHovered };
		style.Colors[(int)ImGuiCol.HeaderActive] = colorConfiguration.Primary;
		style.Colors[(int)ImGuiCol.FrameBg] = colorConfiguration.Tertiary with { A = _alphaFrameBackground };
		style.Colors[(int)ImGuiCol.TitleBgActive] = colorConfiguration.Tertiary with { A = _alphaTitleBackgroundActive };
		style.Colors[(int)ImGuiCol.FrameBgHovered] = colorConfiguration.Primary with { A = _alphaFrameBackgroundHovered };
		style.Colors[(int)ImGuiCol.FrameBgActive] = colorConfiguration.Primary with { A = _alphaFrameBackgroundActive };
		style.Colors[(int)ImGuiCol.SeparatorHovered] = colorConfiguration.Quaternary with { A = _alphaSeparatorHovered };
		style.Colors[(int)ImGuiCol.SeparatorActive] = colorConfiguration.Quaternary;
		style.Colors[(int)ImGuiCol.Tab] = colorConfiguration.Tertiary;
		style.Colors[(int)ImGuiCol.TabHovered] = colorConfiguration.Secondary;
		style.Colors[(int)ImGuiCol.TabActive] = colorConfiguration.Primary;
	}
}
