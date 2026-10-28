namespace DevilDaggersInfo.Tools.Networking;

internal static class Clients
{
#if TEST
	private static readonly HttpClientHandler _clientHandler = new();

	static AsyncHandler()
	{
		_clientHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
	}

	public static DevilDaggersInfoClient DevilDaggersInfo { get; } = new(new(_clientHandler) { BaseAddress = new("https://localhost:5001/") });
#else
	public static DevilDaggersInfoClient DevilDaggersInfo { get; } = new(new HttpClient { BaseAddress = new Uri("https://devildaggers.info") });
#endif

	public static DevilDaggersClient DevilDaggers { get; } = new(new HttpClient { BaseAddress = new Uri("http://dd.hasmodai.com") });
}
