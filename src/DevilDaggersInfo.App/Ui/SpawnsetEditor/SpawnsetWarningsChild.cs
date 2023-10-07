using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor;

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

			if (isEndLoopTooShort)
				ImGui.Text($"The end loop is only {endLoopLength} seconds long, which will probably result in severe lag or a crash.");

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
		if (SpawnsetState.Spawnset.GameMode != GameMode.Survival)
			return null;

		(SpawnSectionInfo PreLoopSection, SpawnSectionInfo LoopSection) sections = SpawnsetState.Spawnset.CalculateSections();
		return sections.LoopSection.Length;
	}

	private static bool IsStartTileVoid()
	{
		if (SpawnsetState.Spawnset.ArenaDimension <= 25)
			return false;

		return SpawnsetState.Spawnset.ArenaTiles[25, 25] < -1;
	}
}
