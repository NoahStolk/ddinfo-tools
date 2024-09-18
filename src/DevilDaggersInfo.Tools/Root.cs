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
public static class Root
{
	private static readonly InvalidOperationException _notInitializedException = new("Root component is not initialized.");

	private static Application? _application;
	private static ImFontPtr _fontGoetheBold20;
	private static ImFontPtr _fontGoetheBold30;
	private static ImFontPtr _fontGoetheBold60;

	public static Application Application
	{
		get => _application ?? throw _notInitializedException;
		set => _application = value;
	}

	public static unsafe ImFontPtr FontGoetheBold20
	{
		get => _fontGoetheBold20.NativePtr == (void*)0 ? throw _notInitializedException : _fontGoetheBold20;
		set => _fontGoetheBold20 = value;
	}

	public static unsafe ImFontPtr FontGoetheBold30
	{
		get => _fontGoetheBold30.NativePtr == (void*)0 ? throw _notInitializedException : _fontGoetheBold30;
		set => _fontGoetheBold30 = value;
	}

	public static unsafe ImFontPtr FontGoetheBold60
	{
		get => _fontGoetheBold60.NativePtr == (void*)0 ? throw _notInitializedException : _fontGoetheBold60;
		set => _fontGoetheBold60 = value;
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
