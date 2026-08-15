using Serilog;
using StrongInject;

namespace DevilDaggersInfo.Tools;

internal static class Program
{
	private static int _fatalLogged;

	public static void Main()
	{
		StaticLog.Initialize();

		AppDomain.CurrentDomain.UnhandledException += (_, e) =>
		{
			if (Interlocked.Exchange(ref _fatalLogged, 1) == 0)
				Log.Logger.Fatal(e.ExceptionObject as Exception, "Unhandled exception (outside main loop)");
		};

		try
		{
			using Container container = new();
			using Owned<Application> app = container.Resolve();
			app.Value.Run();
		}
		catch (Exception ex)
		{
			if (Interlocked.Exchange(ref _fatalLogged, 1) == 0)
				Log.Logger.Fatal(ex, "Unhandled exception");

			throw;
		}
	}
}
