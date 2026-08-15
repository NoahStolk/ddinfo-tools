using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

internal static class ReplayFileInfo
{
	public static void Render(EditorReplayModel model)
	{
#if DEBUG
		RenderData("Version"u8, Inline.Utf8(model.Version));
		RenderData("Timestamp"u8, Inline.Utf8(model.TimestampSinceGameRelease));
#endif
		RenderData("Player"u8, Inline.Utf8(model.PlayerId == 0 ? "N/A" : $"{model.Username} ({model.PlayerId})"));
		RenderData("Time"u8, Inline.Utf8(model.Time, StringFormats.TimeFormat));
		RenderData("Start Time"u8, Inline.Utf8(model.StartTime, StringFormats.TimeFormat));
		RenderData("Kills"u8, Inline.Utf8(model.Kills));
		RenderData("Gems"u8, Inline.Utf8(model.Gems));

		float accuracy = model.DaggersFired == 0 ? 0 : model.DaggersHit / (float)model.DaggersFired;
		RenderData("Accuracy"u8, Inline.Utf8($"{accuracy:0.00%} ({model.DaggersHit}/{model.DaggersFired})"));
		RenderData("Death Type"u8, Inline.Utf8(Deaths.GetDeathByType(GameConstants.CurrentVersion, (byte)model.DeathType)?.Name ?? "?"));
		RenderData("UTC Date"u8, Inline.Utf8(LocalReplayBinaryHeader.GetDateTimeOffsetFromTimestampSinceGameRelease(model.TimestampSinceGameRelease), "yyyy-MM-dd HH:mm:ss"));
	}

	private static void RenderData(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
	{
		Vector2 position = ImGui.GetCursorScreenPos();
		ImGui.Text(left);

		ImGui.SetCursorScreenPos(position + new Vector2(96, 0));
		ImGui.TextUnformatted(right);
	}
}
