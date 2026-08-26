using DevilDaggersInfo.Web.ApiSpec.Tools;

namespace DevilDaggersInfo.Tools.Platforms;

internal sealed class OSXValues : IPlatformSpecificValues
{
	// AppOperatingSystem ships in DevilDaggersInfo.Web.ApiSpec.Tools (an external NuGet package) and has no macOS
	// member. Reporting Linux is the least-bad option available from this repo; it only affects what the server is
	// told about the submitting client's OS.
	public AppOperatingSystem AppOperatingSystem => AppOperatingSystem.Linux;

	// The Mac build ships as an .app bundle. Unlike Windows and Linux, where dd/, res/ and mods/ sit beside the
	// executable, on macOS they live inside Contents/Resources — so the bundle's parent directory is NOT a valid
	// installation directory and fails validation with "File 'dd/survival' does not exist."
	public string DefaultInstallationPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Steam", "steamapps", "common", "devildaggers", "Devil Daggers.app", "Contents", "Resources");
}
