using DevilDaggersInfo.App.Engine.Maths.Numerics;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui;

public static class AboutWindow
{
	public static void Render(ref bool show)
	{
		if (!show)
			return;

		Vector2 windowSize = new(512, 512);
		ImGui.SetNextWindowSize(windowSize);
		if (ImGui.Begin("About ddinfo tools", ref show, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize))
		{
			ImGui.PushTextWrapPos(windowSize.X - 16);

			ImGuiExt.Title("About");

			ImGui.Text("""
				ddinfo tools is a collection of tools for Devil Daggers. The tools are part of the DevilDaggers.info project, which is free to use and open source.

				The tools are currently in alpha. I develop and maintain the entire DevilDaggers.info project in my free time, which means I cannot promise a release date any time soon.

				If you have any feature requests, or encounter any issues, please report them on Discord or GitHub.

				Thank you for testing!
				""");

			ImGuiExt.Hyperlink("https://github.com/NoahStolk/DevilDaggersInfo", "GitHub");

			ImGui.SetCursorPos(new(8, windowSize.Y - 88));

			ImGui.Text("© DevilDaggers.info 2017-2023");

			ImGuiExt.Hyperlink("https://devildaggers.com/", "Devil Daggers");
			ImGui.SameLine();
			ImGui.Text("is created by");
			ImGui.SameLine();
			ImGuiExt.Hyperlink("https://sorath.com/", "Sorath");

			ImGuiExt.Hyperlink("https://devildaggers.info/", "DevilDaggers.info");
			ImGui.SameLine();
			ImGui.Text("is created by");
			ImGui.SameLine();
			ImGuiExt.Hyperlink("https://noahstolk.com/", "Noah Stolk");

			ImGui.SetCursorPos(new(8, windowSize.Y - 24));
			ImGui.TextColored(Color.Gray(0.6f), $"Version {AssemblyUtils.EntryAssemblyVersion} (build time {AssemblyUtils.EntryAssemblyBuildTime})");

			ImGui.PopTextWrapPos();
		}

		ImGui.End();
	}
}
