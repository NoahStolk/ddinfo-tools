using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

public static class UpdateWindow
{
	public static Version? AvailableUpdateVersion { get; set; }

	public static void Render(ref bool show)
	{
		if (!show)
			return;

		Vector2 center = ImGui.GetMainViewport().GetCenter();
		Vector2 windowSize = new(384, 384);
		ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new(0.5f, 0.5f));
		ImGui.SetNextWindowSize(windowSize);
		if (ImGui.Begin("Update available", ref show, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize))
		{
			ImGui.PushTextWrapPos(windowSize.X - 16);

			// TODO: Refactor this. AvailableUpdateVersion should never be null here.
			if (AvailableUpdateVersion == null)
			{
				ImGui.Text("Internal error getting new version number.");
			}
			else
			{
				ImGui.Text(Inline.Span($"""
					Version {AvailableUpdateVersion} is available.

					The current version is {AssemblyUtils.EntryAssemblyVersion}.
					"""));
				ImGui.Spacing();

				ImGuiExt.Hyperlink("https://github.com/NoahStolk/ddinfo-tools/releases", "Download from GitHub");
				ImGui.Spacing();

				ImGuiExt.Hyperlink("https://github.com/NoahStolk/ddinfo-tools/blob/main/CHANGELOG.md", "View the full changelog");
				ImGui.Spacing();
			}

			ImGui.PopTextWrapPos();
		}

		ImGui.End(); // End Update available
	}
}
