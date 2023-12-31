using System.Diagnostics;
using System.Reflection;

namespace DevilDaggersInfo.Tools.Utils;

public static class AssemblyUtils
{
	public static readonly Assembly EntryAssembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Could not get entry assembly.");

	public static readonly string EntryAssemblyVersionString = GetEntryAssemblyVersionString();

	public static readonly Version EntryAssemblyVersion = new(EntryAssemblyVersionString);

	public static readonly string EntryAssemblyBuildTime = Assembly.GetEntryAssembly()?.GetCustomAttribute<BuildTimeAttribute>()?.BuildTime ?? "Unknown build time";

	public static readonly string InstallationDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? throw new InvalidOperationException("Could not get installation directory of the current executing assembly.");

	private static string GetEntryAssemblyVersionString()
	{
		AssemblyInformationalVersionAttribute? attribute = EntryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
		if (attribute == null)
			throw new InvalidOperationException("Could not get informational version attribute.");

		int index = attribute.InformationalVersion.IndexOf('+');
		return index != -1 ? attribute.InformationalVersion[..index] : attribute.InformationalVersion;
	}
}
