using DevilDaggersInfo.Api.App.CustomLeaderboards;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.States;

public record UploadResponseState(GetUploadResponse? UploadResponse)
{
	public static UploadResponseState GetDefault()
	{
		return new((GetUploadResponse?)null);
	}
}
