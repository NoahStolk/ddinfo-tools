using DevilDaggersInfo.App.Ui.ReplayEditor.State;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Replay;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor;

public static class ReplayEditorFileInfo
{
	public static void Render()
	{
		LocalReplayBinaryHeader header = ReplayState.Replay.Header;

		RenderData("Version", UnsafeSpan.Get(header.Version));
		RenderData("Timestamp", UnsafeSpan.Get(header.TimestampSinceGameRelease));
		RenderData("Date", UnsafeSpan.Get(LocalReplayBinaryHeader.GetTimestampFromTimestampSinceGameRelease(header.TimestampSinceGameRelease)));
		RenderData("Time", UnsafeSpan.Get(header.Time, StringFormats.TimeFormat));
		RenderData("Start Time", UnsafeSpan.Get(header.StartTime, StringFormats.TimeFormat));
		RenderData("Kills", UnsafeSpan.Get(header.Kills));
		RenderData("Gems", UnsafeSpan.Get(header.Gems));
		RenderData("Daggers Hit", UnsafeSpan.Get(header.DaggersHit));
		RenderData("Daggers Fired", UnsafeSpan.Get(header.DaggersFired));
		RenderData("Accuracy", UnsafeSpan.Get(header.Accuracy, StringFormats.AccuracyFormat));
		RenderData("Death Type", UnsafeSpan.Get(header.DeathType));
		RenderData("Player ID", UnsafeSpan.Get(header.PlayerId));
		RenderData("Player Name", header.Username);
		RenderData("Spawnset MD5", BitConverter.ToString(header.SpawnsetMd5));

		static void RenderData(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
		{
			Vector2 position = ImGui.GetCursorScreenPos();
			ImGui.Text(left);

			ImGui.SetCursorScreenPos(position + new Vector2(96, 0));
			ImGui.TextUnformatted(right);
		}
	}
}
