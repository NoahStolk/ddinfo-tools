using DevilDaggersInfo.Tools.Ui.Popups;
using System.Text;

namespace DevilDaggersInfo.Tools.Networking;

public static class AsyncHandler
{
#if TEST
	private static readonly HttpClientHandler _clientHandler = new();

	static AsyncHandler()
	{
		_clientHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
	}

	public static ApiHttpClient Client { get; } = new(new(_clientHandler) { BaseAddress = new("https://localhost:5001/") });
#else
	public static ApiHttpClient Client { get; } = new(new() { BaseAddress = new("https://devildaggers.info") });
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
				StringBuilder sb = new();
				sb.AppendLine(ex.Message);

				// Recursively get all inner exceptions.
				Exception? innerException = ex.InnerException;
				const int maxDepth = 5;
				int depth = 0;
				while (innerException != null && depth++ < maxDepth)
				{
					sb.Append(new string('\t', depth)).AppendLine(innerException.Message);
					innerException = innerException.InnerException;
				}

				PopupManager.ShowError("API call failed.\n\n" + sb);
				Root.Log.Error(ex, "API error");
				return default;
			}
		}
	}
}
