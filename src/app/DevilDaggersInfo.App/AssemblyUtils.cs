using System.Diagnostics;
using System.Reflection;

namespace DevilDaggersInfo.App;

public static class AssemblyUtils
{
	public static readonly string EntryAssemblyVersion = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown version";

	public static readonly string EntryAssemblyBuildTime = Assembly.GetEntryAssembly()?.GetCustomAttribute<BuildTimeAttribute>()?.BuildTime ?? "Unknown build time";

	public static readonly string InstallationDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? throw new InvalidOperationException("Could not get installation directory of the current executing assembly.");
}
