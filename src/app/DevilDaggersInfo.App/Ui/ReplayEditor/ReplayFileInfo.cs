using DevilDaggersInfo.App.Ui.ReplayEditor.State;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Common;
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
		RenderData("Version", UnsafeSpan.Get(header.Version));
		RenderData("Timestamp", UnsafeSpan.Get(header.TimestampSinceGameRelease));
		RenderSpawnsetMd5(header);
#endif
		RenderData("Player", UnsafeSpan.Get(header.PlayerId == 0 ? "N/A" : $"{header.Username} ({header.PlayerId})"));
		RenderData("Time", UnsafeSpan.Get(header.Time, StringFormats.TimeFormat));
		RenderData("Start Time", UnsafeSpan.Get(header.StartTime, StringFormats.TimeFormat));
		RenderData("Kills", UnsafeSpan.Get(header.Kills));
		RenderData("Gems", UnsafeSpan.Get(header.Gems));

		RenderData("Accuracy", UnsafeSpan.Get($"{header.Accuracy:0.00%} ({header.DaggersHit}/{header.DaggersFired})"));
		RenderData("Death Type", Deaths.GetDeathByType(GameConstants.CurrentVersion, (byte)header.DeathType)?.Name ?? "?");
		RenderData("UTC Date", UnsafeSpan.Get(LocalReplayBinaryHeader.GetDateTimeOffsetFromTimestampSinceGameRelease(header.TimestampSinceGameRelease), "yyyy-MM-dd HH:mm:ss"));
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
