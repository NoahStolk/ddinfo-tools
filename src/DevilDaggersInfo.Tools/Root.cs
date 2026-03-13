using DevilDaggersInfo.Core.Encryption;
using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.GameWindow;
using DevilDaggersInfo.Tools.Platforms;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using Serilog;
using Serilog.Core;
#if WINDOWS
using DevilDaggersInfo.Tools.NativeInterface.Services.Windows;
#elif LINUX
using DevilDaggersInfo.Tools.NativeInterface.Services.Linux;
#endif

namespace DevilDaggersInfo.Tools;

// TODO: Remove.
[Obsolete]
internal static class Root
{
	private static readonly InvalidOperationException _notInitializedException = new("Root component is not initialized.");

	public static Application Application
	{
		get => field ?? throw _notInitializedException;
		set;
	}

	public static unsafe ImFontPtr FontGoetheBold20
	{
		get => field.NativePtr == (void*)0 ? throw _notInitializedException : field;
		set;
	}

	public static unsafe ImFontPtr FontGoetheBold30
	{
		get => field.NativePtr == (void*)0 ? throw _notInitializedException : field;
		set;
	}

	public static unsafe ImFontPtr FontGoetheBold60
	{
		get => field.NativePtr == (void*)0 ? throw _notInitializedException : field;
		set;
	}

	public static Logger Log { get; } = new LoggerConfiguration()
		.WriteTo.File($"ddinfo-{AssemblyUtils.EntryAssemblyVersionString}.log", rollingInterval: RollingInterval.Infinite)
		.CreateLogger();

	public static AesBase32Wrapper? AesBase32Wrapper { get; } = CreateAesBase32Wrapper();

#if WINDOWS
	public static IPlatformSpecificValues PlatformSpecificValues { get; } = new WindowsValues();
	public static GameMemoryService GameMemoryService { get; } = new(new WindowsMemoryService());
	public static GameWindowService GameWindowService { get; } = new(new WindowsWindowingService());
#elif LINUX
	public static IPlatformSpecificValues PlatformSpecificValues { get; } = new LinuxValues();
	public static GameMemoryService GameMemoryService { get; } = new(new LinuxMemoryService());
	public static GameWindowService GameWindowService { get; } = new(new LinuxWindowingService());
#endif

	private static AesBase32Wrapper? CreateAesBase32Wrapper()
	{
		using Stream? stream = AssemblyUtils.EntryAssembly.GetManifestResourceStream("DevilDaggersInfo.Tools.Content.encryption.ini");
		if (stream == null)
		{
			Log.Error("Could not get resource stream.");
			return null;
		}

		using StreamReader reader = new(stream);
		string ini = reader.ReadToEnd();
		string[] lines = ini.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

		string? iv = GetValue(lines, "iv");
		string? pass = GetValue(lines, "pass");
		string? salt = GetValue(lines, "salt");

		if (string.IsNullOrWhiteSpace(iv) || string.IsNullOrWhiteSpace(pass) || string.IsNullOrWhiteSpace(salt))
			return null;

		return new AesBase32Wrapper(iv, pass, salt);

		static string? GetValue(string[] iniLines, string key)
		{
			string? line = Array.Find(iniLines, l => l.StartsWith(key, StringComparison.OrdinalIgnoreCase));
			string[]? values = line?.Split('=');
			return values?.Length != 2 ? null : values[1].Trim();
		}
	}
}
