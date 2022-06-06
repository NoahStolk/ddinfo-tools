using System.Reflection;

namespace DevilDaggersInfo.App.CustomLeaderboard.Wpf.Utils;

public static class VersionUtils
{
	public static readonly string WpfVersion = Assembly.GetExecutingAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Failed to get version";
}
