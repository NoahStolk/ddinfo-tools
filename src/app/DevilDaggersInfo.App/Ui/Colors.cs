using DevilDaggersInfo.App.Engine.Maths.Numerics;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui;

public static class Colors
{
	public static Color SpawnsetEditor => new(250, 66, 66, 255);
	public static Color CustomLeaderboards => new(150, 66, 250, 255);
	public static Color AssetEditor => new(66, 250, 66, 255);
	public static Color ReplayEditor => new(66, 66, 250, 255);
	public static Color Practice => new(250, 150, 66, 255);
	public static Color ModManager => new(66, 250, 150, 255);

	public static void SetCustomLeaderboardsColors()
	{
		ImGuiStylePtr style = ImGui.GetStyle();
		style.Colors[(int)ImGuiCol.CheckMark] = CustomLeaderboards;
		style.Colors[(int)ImGuiCol.SliderGrab] = new Color(133, 61, 224, 255);
		style.Colors[(int)ImGuiCol.SliderGrabActive] = CustomLeaderboards;
		style.Colors[(int)ImGuiCol.Button] = CustomLeaderboards with { A = 102 };
		style.Colors[(int)ImGuiCol.ButtonHovered] = CustomLeaderboards;
		style.Colors[(int)ImGuiCol.ButtonActive] = CustomLeaderboards with { B = 191 };
		style.Colors[(int)ImGuiCol.Header] = CustomLeaderboards with { A = 79 };
		style.Colors[(int)ImGuiCol.HeaderHovered] = CustomLeaderboards with { A = 204 };
		style.Colors[(int)ImGuiCol.HeaderActive] = CustomLeaderboards with { A = 255 };
		style.Colors[(int)ImGuiCol.FrameBg] = new Color(74, 41, 122, 138);
		style.Colors[(int)ImGuiCol.TitleBgActive] = new Color(74, 41, 122, 138);
		style.Colors[(int)ImGuiCol.FrameBgHovered] = CustomLeaderboards with { A = 102 };
		style.Colors[(int)ImGuiCol.FrameBgActive] = CustomLeaderboards with { A = 171 };
		style.Colors[(int)ImGuiCol.SeparatorHovered] = new Color(102, 26, 191, 199);
		style.Colors[(int)ImGuiCol.SeparatorActive] = new Color(102, 26, 191, 255);
	}

	public static void SetSpawnsetEditorColors()
	{
		ImGuiStylePtr style = ImGui.GetStyle();
		style.Colors[(int)ImGuiCol.CheckMark] = SpawnsetEditor;
		style.Colors[(int)ImGuiCol.SliderGrab] = new Color(224, 61, 61, 255);
		style.Colors[(int)ImGuiCol.SliderGrabActive] = SpawnsetEditor;
		style.Colors[(int)ImGuiCol.Button] = SpawnsetEditor with { A = 102 };
		style.Colors[(int)ImGuiCol.ButtonHovered] = SpawnsetEditor;
		style.Colors[(int)ImGuiCol.ButtonActive] = SpawnsetEditor with { B = 191 };
		style.Colors[(int)ImGuiCol.Header] = SpawnsetEditor with { A = 79 };
		style.Colors[(int)ImGuiCol.HeaderHovered] = SpawnsetEditor with { A = 204 };
		style.Colors[(int)ImGuiCol.HeaderActive] = SpawnsetEditor with { A = 255 };
		style.Colors[(int)ImGuiCol.FrameBg] = new Color(122, 41, 41, 138);
		style.Colors[(int)ImGuiCol.TitleBgActive] = new Color(122, 41, 41, 138);
		style.Colors[(int)ImGuiCol.FrameBgHovered] = SpawnsetEditor with { A = 102 };
		style.Colors[(int)ImGuiCol.FrameBgActive] = SpawnsetEditor with { A = 171 };
		style.Colors[(int)ImGuiCol.SeparatorHovered] = new Color(191, 26, 26, 199);
		style.Colors[(int)ImGuiCol.SeparatorActive] = new Color(191, 26, 26, 255);
	}

	public static void SetReplayEditorColors()
	{
		ImGuiStylePtr style = ImGui.GetStyle();
		style.Colors[(int)ImGuiCol.CheckMark] = ReplayEditor;
		style.Colors[(int)ImGuiCol.SliderGrab] = new Color(61, 61, 224, 255);
		style.Colors[(int)ImGuiCol.SliderGrabActive] = ReplayEditor;
		style.Colors[(int)ImGuiCol.Button] = ReplayEditor with { A = 102 };
		style.Colors[(int)ImGuiCol.ButtonHovered] = ReplayEditor;
		style.Colors[(int)ImGuiCol.ButtonActive] = ReplayEditor with { B = 191 };
		style.Colors[(int)ImGuiCol.Header] = ReplayEditor with { A = 79 };
		style.Colors[(int)ImGuiCol.HeaderHovered] = ReplayEditor with { A = 204 };
		style.Colors[(int)ImGuiCol.HeaderActive] = ReplayEditor with { A = 255 };
		style.Colors[(int)ImGuiCol.FrameBg] = new Color(41, 41, 122, 138);
		style.Colors[(int)ImGuiCol.TitleBgActive] = new Color(41, 41, 122, 138);
		style.Colors[(int)ImGuiCol.FrameBgHovered] = ReplayEditor with { A = 102 };
		style.Colors[(int)ImGuiCol.FrameBgActive] = ReplayEditor with { A = 171 };
		style.Colors[(int)ImGuiCol.SeparatorHovered] = new Color(26, 26, 191, 199);
		style.Colors[(int)ImGuiCol.SeparatorActive] = new Color(26, 26, 191, 255);
	}
}
