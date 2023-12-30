using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

public static class UpdateWindow
{
	private static bool _updateInProgress;

	public static Version? AvailableUpdateVersion { get; set; }

	public static List<string> LogMessages { get; } = new();

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

					If you decide to update, the update will be downloaded from the devildaggers.info server.
					"""));
				ImGui.Spacing();

 #pragma warning disable S1075
				const string changelogUrl = "https://github.com/NoahStolk/ddinfo-tools/blob/main/CHANGELOG.md";
 #pragma warning restore S1075
				ImGuiExt.Hyperlink(changelogUrl, Inline.Span($"See what's new in version {AvailableUpdateVersion}"));
				ImGui.Spacing();
			}

			ImGui.BeginDisabled(_updateInProgress);
			if (ImGui.Button("Update and restart", new(160, 24)))
			{
				LogMessages.Clear();
				_updateInProgress = true;
				Task.Run(async () =>
				{
					await UpdateLogic.RunAsync();
					_updateInProgress = false;
				});
			}

			ImGui.EndDisabled();

			for (int i = 0; i < LogMessages.Count; i++)
				ImGui.Text(LogMessages[i]);

			ImGui.PopTextWrapPos();
		}

		ImGui.End(); // End Update available
	}
}
