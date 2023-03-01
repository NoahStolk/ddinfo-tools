using DevilDaggersInfo.Api.App.CustomLeaderboards;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;

public record SetSuccessfulUpload(GetUploadResponse UploadResponse) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.UploadResponseState = new(UploadResponse);
		stateReducer.RecordingState = stateReducer.RecordingState with
		{
			ShowUploadResponse = true,
			LastSubmission = DateTime.Now,
		};
	}
}
