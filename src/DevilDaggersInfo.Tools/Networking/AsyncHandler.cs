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
	public static ApiHttpClient Client { get; } = new(new HttpClient { BaseAddress = new Uri("https://devildaggers.info") });
#endif

	public static bool AutoFailAllCallsForTesting { get; set; }

	public static void Run<TResult>(Action<ApiResult<TResult>> callback, Func<Task<TResult>> call)
		where TResult : class
	{
		Task.Run(async () => callback(await TryCall()));

		async Task<ApiResult<TResult>> TryCall()
		{
			if (AutoFailAllCallsForTesting)
				return ApiResult<TResult>.Error(new ApiError(null, "Auto-failing all calls for testing purposes..."));

			try
			{
				TResult data = await call();
				return ApiResult<TResult>.Ok(data);
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

				return ApiResult<TResult>.Error(new ApiError(ex, sb.ToString()));
			}
		}
	}
}
