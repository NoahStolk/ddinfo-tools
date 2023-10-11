using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor;

public static class SpawnsetWarningsChild
{
	public static void Render()
	{
		const int padding = 6;

		ImGui.PushStyleColor(ImGuiCol.ChildBg, Color.Gray(0.035f));
		if (ImGui.BeginChild("Warnings", new(528, 192)))
		{
			ImGui.SetCursorPosY(ImGui.GetCursorPosY() + padding);
			ImGui.Indent(padding);

			float? endLoopLength = GetEndLoopLength();
			bool isEndLoopTooShort = endLoopLength < 0.1f;
			bool isStartTileVoid = IsStartTileVoid();
			int warningCount = 0;
			if (isEndLoopTooShort)
				warningCount++;
			if (isStartTileVoid)
				warningCount++;

			ImGui.PushTextWrapPos(512);

			if (warningCount == 0)
				ImGui.TextColored(Color.Green, "No warnings");
			else
				ImGui.TextColored(Color.Red, warningCount == 1 ? "1 warning" : $"{warningCount} warnings");

			if (endLoopLength.HasValue && isEndLoopTooShort) // endLoopLength.HasValue is always true here.
				ImGui.Text(Inline.Span($"The end loop is only {endLoopLength.Value} seconds long, which will probably result in severe lag or a crash."));

			if (isStartTileVoid)
				ImGui.Text("The center tile of the arena is void, which means the player will die instantly.");

			ImGui.PopTextWrapPos();

			ImGui.Unindent();
		}

		ImGui.EndChild(); // End Warnings

		ImGui.PopStyleColor();
	}

	private static float? GetEndLoopLength()
	{
		if (FileStates.Spawnset.Object.GameMode != GameMode.Survival)
			return null;

		(SpawnSectionInfo PreLoopSection, SpawnSectionInfo LoopSection) sections = FileStates.Spawnset.Object.CalculateSections();
		return sections.LoopSection.Length;
	}

	private static bool IsStartTileVoid()
	{
		if (FileStates.Spawnset.Object.ArenaDimension <= 25)
			return false;

		return FileStates.Spawnset.Object.ArenaTiles[25, 25] < -1;
	}
}
