using DevilDaggersInfo.App.Networking;
using DevilDaggersInfo.App.Networking.TaskHandlers;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.Config;

public static class ConfigLayout
{
	private static bool _contentInitialized;
	private static bool? _isUpToDate;

	public static void Initialize()
	{
		AsyncHandler.Run(
			static newVersion =>
			{
				if (newVersion != null)
				{
					_isUpToDate = false;
					Task.Run(async () => await UpdateLogic.RunAsync());
				}
				else
				{
					_isUpToDate = true;
				}
			},
			() => FetchLatestVersion.HandleAsync(Root.Application.AppVersion, Root.PlatformSpecificValues.BuildType));
	}

	public static void Update()
	{
		if (_contentInitialized || _isUpToDate != true)
			return;

		// This must be called once when we know the app is up-to-date and can run.
		// IMPORTANT: This method must run on the main thread.
		InstallationWindow.ValidateInstallation();
		_contentInitialized = true;
	}

	public static void Render()
	{
		ImGui.Begin("Configuration", Constants.LayoutFlags);

		if (!_isUpToDate.HasValue)
			ImGui.Text("Checking for updates...");
		else if (_isUpToDate.Value)
			InstallationWindow.Render();
		else
			ImGui.Text("Updating...");

		ImGui.End();
	}
}
