using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

internal static class Colors
{
	private static readonly Vector4 _unit = new(255f);

	public static ColorConfiguration Main { get; } = new()
	{
		Primary = new Vector4(250, 66, 66, 255) / _unit,
		Secondary = new Vector4(224, 61, 61, 255) / _unit,
		Tertiary = new Vector4(122, 41, 41, 255) / _unit,
		Quaternary = new Vector4(191, 26, 26, 255) / _unit,
	};

	public static ColorConfiguration SpawnsetEditor { get; } = new()
	{
		Primary = new Vector4(250, 66, 66, 255) / _unit,
		Secondary = new Vector4(224, 61, 61, 255) / _unit,
		Tertiary = new Vector4(122, 41, 41, 255) / _unit,
		Quaternary = new Vector4(191, 26, 26, 255) / _unit,
	};

	public static ColorConfiguration CustomLeaderboards { get; } = new()
	{
		Primary = new Vector4(150, 66, 250, 255) / _unit,
		Secondary = new Vector4(133, 61, 224, 255) / _unit,
		Tertiary = new Vector4(74, 41, 122, 255) / _unit,
		Quaternary = new Vector4(102, 26, 191, 255) / _unit,
	};

	public static ColorConfiguration ReplayEditor { get; } = new()
	{
		Primary = new Vector4(66, 66, 250, 255) / _unit,
		Secondary = new Vector4(61, 61, 224, 255) / _unit,
		Tertiary = new Vector4(41, 41, 122, 255) / _unit,
		Quaternary = new Vector4(26, 26, 191, 255) / _unit,
	};

	public static ColorConfiguration Practice { get; } = new()
	{
		Primary = new Vector4(250, 150, 66, 255) / _unit,
		Secondary = new Vector4(224, 133, 61, 255) / _unit,
		Tertiary = new Vector4(122, 74, 41, 255) / _unit,
		Quaternary = new Vector4(191, 102, 26, 255) / _unit,
	};

	public static ColorConfiguration AssetEditor { get; } = new()
	{
		Primary = new Vector4(66, 187, 66, 255) / _unit,
		Secondary = new Vector4(61, 166, 61, 255) / _unit,
		Tertiary = new Vector4(41, 92, 41, 255) / _unit,
		Quaternary = new Vector4(26, 127, 26, 255) / _unit,
	};

	public static ColorConfiguration ModManager { get; } = new()
	{
		Primary = new Vector4(150, 150, 66, 255) / _unit,
		Secondary = new Vector4(133, 133, 61, 255) / _unit,
		Tertiary = new Vector4(74, 74, 41, 255) / _unit,
		Quaternary = new Vector4(102, 102, 26, 255) / _unit,
	};

	public static Vector4 TitleColor(float totalTime)
	{
		return Vector4.Lerp(new Vector4(1, 0.2f, 0.05f, 1), new Vector4(1, 0.5f, 0.2f, 1), MathF.Sin(totalTime));
	}

	public static void SetColors(ColorConfiguration colorConfiguration)
	{
		const float alphaButton = 0.4f;
		const float alphaButtonActive = 0.75f;
		const float alphaHeader = 0.31f;
		const float alphaHeaderHovered = 0.8f;
		const float alphaFrameBackground = 0.54f;
		const float alphaTitleBackgroundActive = 0.54f;
		const float alphaFrameBackgroundHovered = 0.4f;
		const float alphaFrameBackgroundActive = 0.67f;
		const float alphaSeparatorHovered = 0.78f;

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
