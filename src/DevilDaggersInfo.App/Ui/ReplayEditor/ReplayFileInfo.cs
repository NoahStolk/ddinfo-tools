using DevilDaggersInfo.App.Ui.ReplayEditor.State;
using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor;

public static class ReplayFileInfo
{
	public static void Render()
	{
		LocalReplayBinaryHeader header = ReplayState.Replay.Header;

#if DEBUG
		RenderData("Version", Inline.Span(header.Version));
		RenderData("Timestamp", Inline.Span(header.TimestampSinceGameRelease));
		RenderSpawnsetMd5(header);
#endif
		RenderData("Player", Inline.Span(header.PlayerId == 0 ? "N/A" : $"{header.Username} ({header.PlayerId})"));
		RenderData("Time", Inline.Span(header.Time, StringFormats.TimeFormat));
		RenderData("Start Time", Inline.Span(header.StartTime, StringFormats.TimeFormat));
		RenderData("Kills", Inline.Span(header.Kills));
		RenderData("Gems", Inline.Span(header.Gems));

		RenderData("Accuracy", Inline.Span($"{header.Accuracy:0.00%} ({header.DaggersHit}/{header.DaggersFired})"));
		RenderData("Death Type", Deaths.GetDeathByType(GameConstants.CurrentVersion, (byte)header.DeathType)?.Name ?? "?");
		RenderData("UTC Date", Inline.Span(LocalReplayBinaryHeader.GetDateTimeOffsetFromTimestampSinceGameRelease(header.TimestampSinceGameRelease), "yyyy-MM-dd HH:mm:ss"));
	}

	// Separate method so hot reload doesn't complain about stackalloc.
	private static void RenderSpawnsetMd5(LocalReplayBinaryHeader header)
	{
		Span<char> md5 = stackalloc char[header.SpawnsetMd5.Length * 2];
		for (int i = 0; i < header.SpawnsetMd5.Length; i++)
			header.SpawnsetMd5[i].TryFormat(md5[(i * 2)..], out _, "X2");

		RenderData("Spawnset MD5", md5);
	}

	private static void RenderData(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
	{
		Vector2 position = ImGui.GetCursorScreenPos();
		ImGui.Text(left);

		ImGui.SetCursorScreenPos(position + new Vector2(96, 0));
		ImGui.TextUnformatted(right);
	}
}
