using StrongInject;

namespace DevilDaggersInfo.Tools;

internal static class Program
{
	private static int _fatalLogged;

	public static void Main()
	{
		AppDomain.CurrentDomain.UnhandledException += (_, e) =>
		{
			if (Interlocked.Exchange(ref _fatalLogged, 1) == 0)
				StaticLog.Log.Fatal(e.ExceptionObject as Exception, "Unhandled exception (outside main loop)");
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
				StaticLog.Log.Fatal(ex, "Unhandled exception");

			throw;
		}
	}
}
