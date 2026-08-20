namespace DevilDaggersInfo.Tools.Networking;

internal sealed record ApiError(Exception? Exception, string? Message);
