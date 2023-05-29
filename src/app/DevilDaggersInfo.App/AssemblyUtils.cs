using System.Reflection;

namespace DevilDaggersInfo.App;

public static class AssemblyUtils
{
	public static readonly string EntryAssemblyVersion = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown version";

	public static readonly string InstallationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException("Could not get installation directory of the current executing assembly.");
}
