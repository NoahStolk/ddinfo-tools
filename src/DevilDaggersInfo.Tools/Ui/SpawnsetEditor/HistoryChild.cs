using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor;

public static class HistoryChild
{
	public static bool UpdateScroll { get; set; }

	public static void Render()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(8, 1));
		if (ImGui.BeginChild("HistoryChild", new(244, 512)))
		{
			for (int i = 0; i < SpawnsetHistoryUtils.History.Count; i++)
			{
				HistoryEntry<SpawnsetBinary, SpawnsetEditType> history = SpawnsetHistoryUtils.History[i];

				if (UpdateScroll && i == SpawnsetHistoryUtils.CurrentHistoryIndex)
				{
					ImGui.SetScrollHereY();
					UpdateScroll = false;
				}

				const int borderSize = 2;
				ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, borderSize);
				ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Vector2(0, 0.5f));

				Color color = history.EditType.GetColor();
				ImGui.PushStyleColor(ImGuiCol.Button, color);
				ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color + new Vector4(0.3f, 0.3f, 0.3f, 0));
				ImGui.PushStyleColor(ImGuiCol.ButtonActive, color + new Vector4(0.5f, 0.5f, 0.5f, 0));
				ImGui.PushStyleColor(ImGuiCol.Border, i == SpawnsetHistoryUtils.CurrentHistoryIndex ? Color.White : Color.Black);

				ImGui.PushID(Inline.Span($"HistoryButton{i}"));
				if (ImGui.Button(history.EditType.GetChange(), new(226, 20)))
					SetHistoryIndex(i);

				ImGui.PopID();

				ImGui.PopStyleColor(4);

				ImGui.PopStyleVar(2);
			}

			ImGui.PopStyleVar();
		}

		ImGui.EndChild(); // End HistoryChild

		ImGuiIOPtr io = ImGui.GetIO();
		if (io.KeyCtrl)
		{
			if (ImGui.IsKeyPressed(ImGuiKey.Z))
				Undo();
			else if (ImGui.IsKeyPressed(ImGuiKey.Y))
				Redo();
		}
	}

	public static void Undo()
	{
		SetHistoryIndex(SpawnsetHistoryUtils.CurrentHistoryIndex - 1);
	}

	public static void Redo()
	{
		SetHistoryIndex(SpawnsetHistoryUtils.CurrentHistoryIndex + 1);
	}

	private static void SetHistoryIndex(int index)
	{
		if (index < 0 || index >= SpawnsetHistoryUtils.History.Count)
			return;

		// Remove the keyboard focus to prevent undo/redo not working when a text input is focused, and the next/previous history entry changes the value of that text input.
		// TODO: This currently introduces a little bug; there is an additional history entry added when undoing/redoing, but this is better than the alternative.
		// A way to fix this would be to unfocus the keyboard earlier?
		ImGui.SetKeyboardFocusHere(-1);

		SpawnsetHistoryUtils.SetHistoryIndex(index);

		UpdateScroll = true;
		SpawnsChild.ClearUnusedSelections();
	}
}
