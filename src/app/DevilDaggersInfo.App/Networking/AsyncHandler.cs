using DevilDaggersInfo.App.Ui.Popups;

namespace DevilDaggersInfo.App.Networking;

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
				PopupManager.ShowError("API call failed.\n\n" + ex.Message);
				Root.Log.Error(ex, "API error");
				return default;
			}
		}
	}
}
