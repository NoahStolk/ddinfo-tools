using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

internal static class Colors
{
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

	public static Vector4 TitleColor(float totalTime)
	{
		return Vector4.Lerp(new Vector4(1, 0.2f, 0.05f, 1), new Vector4(1, 0.5f, 0.2f, 1), MathF.Sin(totalTime));
	}

	public static void SetColors(ColorConfiguration colorConfiguration)
	{
		const float alphaButton = 102 / 255f;
		const float alphaButtonActive = 191 / 255f;
		const float alphaHeader = 79 / 255f;
		const float alphaHeaderHovered = 204 / 255f;
		const float alphaFrameBackground = 138 / 255f;
		const float alphaTitleBackgroundActive = 138 / 255f;
		const float alphaFrameBackgroundHovered = 102 / 255f;
		const float alphaFrameBackgroundActive = 171 / 255f;
		const float alphaSeparatorHovered = 199 / 255f;

		ImGuiStylePtr style = ImGui.GetStyle();
		style.Colors[(int)ImGuiCol.CheckMark] = colorConfiguration.Primary;
		style.Colors[(int)ImGuiCol.SliderGrab] = colorConfiguration.Secondary;
		style.Colors[(int)ImGuiCol.SliderGrabActive] = colorConfiguration.Primary;
		style.Colors[(int)ImGuiCol.Button] = colorConfiguration.Primary with { W = alphaButton };
		style.Colors[(int)ImGuiCol.ButtonHovered] = colorConfiguration.Primary;
		style.Colors[(int)ImGuiCol.ButtonActive] = colorConfiguration.Primary with { W = alphaButtonActive };
		style.Colors[(int)ImGuiCol.Header] = colorConfiguration.Primary with { W = alphaHeader };
		style.Colors[(int)ImGuiCol.HeaderHovered] = colorConfiguration.Primary with { W = alphaHeaderHovered };
		style.Colors[(int)ImGuiCol.HeaderActive] = colorConfiguration.Primary;
		style.Colors[(int)ImGuiCol.FrameBg] = colorConfiguration.Tertiary with { W = alphaFrameBackground };
		style.Colors[(int)ImGuiCol.TitleBgActive] = colorConfiguration.Tertiary with { W = alphaTitleBackgroundActive };
		style.Colors[(int)ImGuiCol.FrameBgHovered] = colorConfiguration.Primary with { W = alphaFrameBackgroundHovered };
		style.Colors[(int)ImGuiCol.FrameBgActive] = colorConfiguration.Primary with { W = alphaFrameBackgroundActive };
		style.Colors[(int)ImGuiCol.SeparatorHovered] = colorConfiguration.Quaternary with { W = alphaSeparatorHovered };
		style.Colors[(int)ImGuiCol.SeparatorActive] = colorConfiguration.Quaternary;
		style.Colors[(int)ImGuiCol.Tab] = colorConfiguration.Tertiary;
		style.Colors[(int)ImGuiCol.TabHovered] = colorConfiguration.Secondary;
		style.Colors[(int)ImGuiCol.TabSelected] = colorConfiguration.Primary;
		style.Colors[(int)ImGuiCol.TabSelectedOverline] = colorConfiguration.Secondary;
		style.Colors[(int)ImGuiCol.TabDimmed] = colorConfiguration.Tertiary;
		style.Colors[(int)ImGuiCol.TabDimmedSelected] = colorConfiguration.Secondary;
		style.Colors[(int)ImGuiCol.TabDimmedSelectedOverline] = colorConfiguration.Tertiary;
		style.Colors[(int)ImGuiCol.ResizeGrip] = colorConfiguration.Tertiary;
		style.Colors[(int)ImGuiCol.ResizeGripHovered] = colorConfiguration.Secondary;
		style.Colors[(int)ImGuiCol.ResizeGripActive] = colorConfiguration.Primary;
	}
}
