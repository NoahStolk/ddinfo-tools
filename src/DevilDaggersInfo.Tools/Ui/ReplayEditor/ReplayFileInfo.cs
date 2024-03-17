using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

public static class ReplayFileInfo
{
	public static void Render()
	{
		EditorReplayModel model = FileStates.Replay.Object;

#if DEBUG
		RenderData("Version", Inline.Span(model.Version));
		RenderData("Timestamp", Inline.Span(model.TimestampSinceGameRelease));
#endif
		RenderData("Player", Inline.Span(model.PlayerId == 0 ? "N/A" : $"{model.Username} ({model.PlayerId})"));
		RenderData("Time", Inline.Span(model.Time, StringFormats.TimeFormat));
		RenderData("Start Time", Inline.Span(model.StartTime, StringFormats.TimeFormat));
		RenderData("Kills", Inline.Span(model.Kills));
		RenderData("Gems", Inline.Span(model.Gems));

		float accuracy = model.DaggersFired == 0 ? 0 : model.DaggersHit / (float)model.DaggersFired;
		RenderData("Accuracy", Inline.Span($"{accuracy:0.00%} ({model.DaggersHit}/{model.DaggersFired})"));
		RenderData("Death Type", Deaths.GetDeathByType(GameConstants.CurrentVersion, (byte)model.DeathType)?.Name ?? "?");
		RenderData("UTC Date", Inline.Span(LocalReplayBinaryHeader.GetDateTimeOffsetFromTimestampSinceGameRelease(model.TimestampSinceGameRelease), "yyyy-MM-dd HH:mm:ss"));
	}

	private static void RenderData(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
	{
		Vector2 position = ImGui.GetCursorScreenPos();
		ImGui.Text(left);

		ImGui.SetCursorScreenPos(position + new Vector2(96, 0));
		ImGui.TextUnformatted(right);
	}
}
