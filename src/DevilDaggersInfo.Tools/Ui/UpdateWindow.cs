using DevilDaggersInfo.Tools.Utils;
using DevilDaggersInfo.Web.ApiSpec.Tools;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

public static class UpdateWindow
{
	public static void Render(ref bool show)
	{
		if (!show)
			return;

		Vector2 center = ImGui.GetMainViewport().GetCenter();
		Vector2 windowSize = new(384, 384);
		ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
		ImGui.SetNextWindowSize(windowSize);
		if (ImGui.Begin("Updates", ref show, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDocking))
		{
			ImGui.PushTextWrapPos(windowSize.X - 16);

			ImGui.Text(Inline.Span($"""
                The current version is {AssemblyUtils.EntryAssemblyVersion}.

                Unfortunately, since anti-virus software does not like the way automatic updates were implemented, the auto-update feature has been removed.

                You can download the latest version from the GitHub releases (link below).

                1. Go to the the GitHub releases page.
                2. Under "Assets", download the {GetZipAssetDisplayName()} file.
                3. Extract the contents.
                4. Run {GetExecutableDisplayName()}.

                Your user settings will be preserved.
                """));
			ImGui.Spacing();

			ImGuiExt.Hyperlink("https://github.com/NoahStolk/ddinfo-tools/releases", "Download from GitHub");
			ImGui.Spacing();

			ImGuiExt.Hyperlink("https://github.com/NoahStolk/ddinfo-tools/blob/main/CHANGELOG.md", "View the full changelog");
			ImGui.Spacing();

			ImGui.PopTextWrapPos();
		}

		ImGui.End();
	}

	private static string GetZipAssetDisplayName()
	{
		return Root.PlatformSpecificValues.AppOperatingSystem switch
		{
			AppOperatingSystem.Windows => "ddinfo-tools-win-x64.zip",
			AppOperatingSystem.Linux => "ddinfo-tools-linux-x64.zip",
			_ => "zip",
		};
	}

	private static string GetExecutableDisplayName()
	{
		return Root.PlatformSpecificValues.AppOperatingSystem switch
		{
			AppOperatingSystem.Windows => "ddinfo-tools.exe",
			AppOperatingSystem.Linux => "ddinfo-tools",
			_ => "the executable",
		};
	}
}
