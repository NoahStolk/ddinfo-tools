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
#endif
		RenderPlayer(header);
		RenderData("Time", UnsafeSpan.Get(header.Time, StringFormats.TimeFormat));
		RenderData("Start Time", UnsafeSpan.Get(header.StartTime, StringFormats.TimeFormat));
		RenderData("Kills", UnsafeSpan.Get(header.Kills));
		RenderData("Gems", UnsafeSpan.Get(header.Gems));

		RenderAccuracy(header);
		RenderData("Death Type", Deaths.GetDeathByType(GameConstants.CurrentVersion, (byte)header.DeathType)?.Name ?? "?");
		RenderSpawnsetMd5(header);
		RenderData("UTC Date", UnsafeSpan.Get(LocalReplayBinaryHeader.GetDateTimeOffsetFromTimestampSinceGameRelease(header.TimestampSinceGameRelease), "yyyy-MM-dd HH:mm:ss"));
	}

	// Separate method so hot reload doesn't complain about stackalloc.
	private static void RenderPlayer(LocalReplayBinaryHeader header)
	{
		Span<char> player = stackalloc char[64];
		header.Username.CopyTo(player);
		player[header.Username.Length] = ' ';
		player[header.Username.Length + 1] = '(';
		header.PlayerId.TryFormat(player[(header.Username.Length + 2)..], out int charsWrittenPlayerId);
		player[header.Username.Length + 2 + charsWrittenPlayerId] = ')';

		RenderData("Player", player);
	}

	// Separate method so hot reload doesn't complain about stackalloc.
	private static void RenderAccuracy(LocalReplayBinaryHeader header)
	{
		Span<char> accuracy = stackalloc char[32];
		header.Accuracy.TryFormat(accuracy, out int charsWritten, StringFormats.AccuracyFormat);
		accuracy[charsWritten++] = ' ';
		accuracy[charsWritten++] = '(';
		header.DaggersHit.TryFormat(accuracy[charsWritten..], out int charsWrittenDaggersHit);
		charsWritten += charsWrittenDaggersHit;
		accuracy[charsWritten++] = '/';
		header.DaggersFired.TryFormat(accuracy[charsWritten..], out int charsWrittenDaggersFired);
		charsWritten += charsWrittenDaggersFired;
		accuracy[charsWritten] = ')';

		RenderData("Accuracy", accuracy);
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
