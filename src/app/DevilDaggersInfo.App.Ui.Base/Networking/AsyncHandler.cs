using DevilDaggersInfo.App.Ui.Base.DependencyPattern;

namespace DevilDaggersInfo.App.Ui.Base.Networking;

public static class AsyncHandler
{
#if TEST
	private static readonly HttpClientHandler _clientHandler = new();

	static AsyncHandler()
	{
		_clientHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
	}

	public static AppApiHttpClient Client { get; } = new(new(_clientHandler) { BaseAddress = new("https://localhost:5001/") });
#else
	public static AppApiHttpClient Client { get; } = new(new() { BaseAddress = new("https://devildaggers.info") });
#endif

	public static void Run<TResult>(Action<TResult?> callback, Func<Task<TResult>> call)
	{
		Task.Run(async () => callback(await TryCall()));

		async Task<TResult?> TryCall()
		{
			try
			{
				return await call();
			}
			catch (Exception ex)
			{
				Root.Dependencies.NativeDialogService.ReportError("API call failed.", ex);
				Root.Dependencies.Log.Error(ex, "API error");
				return default;
			}
		}
	}
}
