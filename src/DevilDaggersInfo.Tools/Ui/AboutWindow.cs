using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

public static class AboutWindow
{
	private static readonly string _versionInfo = $"Version {AssemblyUtils.EntryAssemblyVersionString} (build time: {AssemblyUtils.EntryAssemblyBuildTime})";

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

			ImGui.Text("ddinfo tools is a collection of tools for Devil Daggers. The tools are part of the DevilDaggers.info project.");

			ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
			ImGuiExt.Title("Open source", Root.FontGoetheBold20);

			ImGui.Text("The source code is available on GitHub.");

			ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);

			RenderGitHubButton();

			ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
			ImGuiExt.Title("Alpha notice", Root.FontGoetheBold20);

			ImGui.Text("""
				The tools are currently in alpha. I develop and maintain the entire DevilDaggers.info project in my free time, which means I cannot promise a release date any time soon.

				If you have any feature requests, or encounter any issues, please report them on Discord or GitHub.

				Thank you for testing!
				""");

			ImGui.SetCursorPos(new(8, windowSize.Y - 72));

			ImGui.TextColored(Colors.TitleColor, "© DevilDaggers.info 2017-2024");

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

			ImGui.TextColored(Color.Gray(0.6f), _versionInfo);

			ImGui.PopTextWrapPos();
		}

		ImGui.End(); // End About ddinfo tools
	}

	private static void RenderGitHubButton()
	{
		Vector2 buttonSize = new(168, 40);

		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
		if (ImGui.BeginChild("GitHub", buttonSize, ImGuiChildFlags.Border))
		{
			ImGui.PopStyleVar();

			bool hover = ImGui.IsWindowHovered();

			ImGuiStylePtr style = ImGui.GetStyle();
			ImGui.PushStyleColor(ImGuiCol.ChildBg, style.Colors[(int)(hover ? ImGuiCol.ButtonHovered : ImGuiCol.Button)]);
			if (ImGui.BeginChild("GitHubChild", buttonSize, ImGuiChildFlags.None, ImGuiWindowFlags.NoInputs))
			{
				if (hover && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
					Process.Start(new ProcessStartInfo("https://github.com/NoahStolk/ddinfo-tools") { UseShellExecute = true });

				ImGui.SetCursorPos(new(8));
				ImGuiImage.Image(Root.InternalResources.GitHubTexture.Handle, new(24));
				ImGui.SameLine();

				ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3);
				ImGui.PushFont(Root.FontGoetheBold20);
				ImGui.Text("View on GitHub");
				ImGui.PopFont();
			}

			ImGui.EndChild(); // End GitHubChild

			ImGui.PopStyleColor();
		}
		else
		{
			ImGui.PopStyleVar();
		}

		ImGui.EndChild(); // End GitHub
	}
}
