using System.Reflection;

namespace DevilDaggersInfo.App;

public static class VersionUtils
{
	public static readonly string EntryAssemblyVersion = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown version";
}
