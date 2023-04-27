using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Extensions;
using DevilDaggersInfo.Core.CriteriaExpression.Extensions;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.Recording.Results;

public class RecordingResultScrollArea : ScrollArea
{
	public RecordingResultScrollArea(IBounds bounds)
		: base(bounds, 32, 16, ScrollAreaStyles.Default)
	{
		StateManager.Subscribe<SetSuccessfulUpload>(ShowResult);
	}

	private void ShowResult()
	{
		foreach (AbstractComponent component in NestingContext.OrderedComponents)
			NestingContext.Remove(component);

		GetUploadResponse? response = StateManager.UploadResponseState.UploadResponse;
		if (response == null)
		{
			Root.Dependencies.Log.Warning("ShowResult was called but state UploadResponse is null.");
			return;
		}

		if (response.NoHighscore != null)
			ShowNoHighscoreResult(response.NoHighscore, response.IsAscending);
		else if (response.FirstScore != null)
			ShowFirstScoreResult(response.FirstScore);
		else if (response.Highscore != null)
			ShowHighscoreResult(response.Highscore, response.IsAscending);
		else if (response.CriteriaRejection != null)
			ShowRejectionResult(response.CriteriaRejection);
		else
			Root.Dependencies.NativeDialogService.ReportError("Invalid upload response from server.");
	}

	private void ShowNoHighscoreResult(GetUploadResponseNoHighscore response, bool isAscending)
	{
		RecordingResultNoHighscore noHighscore = new(ContentBounds.CreateNested(0, 0, ContentBounds.Size.X, 404), response, isAscending) { Depth = Depth + 2 };
		NestingContext.Add(noHighscore);
	}

	private void ShowFirstScoreResult(GetUploadResponseFirstScore response)
	{
		RecordingResultFirstScore firstScore = new(ContentBounds.CreateNested(0, 0, ContentBounds.Size.X, 420), response) { Depth = Depth + 2 };
		NestingContext.Add(firstScore);
	}

	private void ShowHighscoreResult(GetUploadResponseHighscore response, bool isAscending)
	{
		RecordingResultHighscore highscore = new(ContentBounds.CreateNested(0, 0, ContentBounds.Size.X, 420), response, isAscending) { Depth = Depth + 2 };
		NestingContext.Add(highscore);
	}

	private void ShowRejectionResult(GetUploadResponseCriteriaRejection response)
	{
		Label header = new(ContentBounds.CreateNested(0, 0, ContentBounds.Size.X, 16), $"{response.CriteriaName}\nMust be {response.CriteriaOperator.ToCore().ShortString()} {response.ExpectedValue}\nValue was {response.ActualValue}", LabelStyles.DefaultLeft with { TextColor = Color.Red }) { Depth = Depth + 2 };
		NestingContext.Add(header);
	}
}
