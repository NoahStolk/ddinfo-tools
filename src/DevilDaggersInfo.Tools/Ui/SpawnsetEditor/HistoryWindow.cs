using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.Popups;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor;

// TODO: Don't inject other windows.
internal sealed class HistoryWindow(SpawnsWindow spawnsWindow, FileStates fileStates)
{
	public static bool UpdateScroll { get; set; }

	public void Render()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(8, 1));
		if (ImGui.Begin("History"))
		{
			ImGui.Text(Inline.Span($"{fileStates.Spawnset.FileName ?? FileStates.UntitledName}{(fileStates.Spawnset.IsModified && fileStates.Spawnset.FileName != null ? "*" : string.Empty)}"));
			ImGui.Separator();

			float buttonWidth = ImGui.GetContentRegionAvail().X;

			for (int i = 0; i < fileStates.Spawnset.History.Count; i++)
			{
				HistoryEntry<SpawnsetBinary, SpawnsetEditType> history = fileStates.Spawnset.History[i];

				if (UpdateScroll && i == fileStates.Spawnset.CurrentHistoryIndex)
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
				ImGui.PushStyleColor(ImGuiCol.Border, i == fileStates.Spawnset.CurrentHistoryIndex ? Color.White : Color.Black);

				ImGui.PushID(Inline.Span($"HistoryButton{i}"));
				if (ImGui.Button(history.EditType.GetChange(), new Vector2(buttonWidth, 20)))
					SetHistoryIndex(i);

				ImGui.PopID();
				ImGui.PopStyleColor(4);
				ImGui.PopStyleVar(2);
			}
		}

		ImGui.End();

		ImGui.PopStyleVar();

		ImGuiIOPtr io = ImGui.GetIO();
		if (io.KeyCtrl)
		{
			if (ImGui.IsKeyPressed(ImGuiKey.Z))
				Undo();
			else if (ImGui.IsKeyPressed(ImGuiKey.Y))
				Redo();
		}
	}

	public void Undo()
	{
		SetHistoryIndex(fileStates.Spawnset.CurrentHistoryIndex - 1);
	}

	public void Redo()
	{
		SetHistoryIndex(fileStates.Spawnset.CurrentHistoryIndex + 1);
	}

	private void SetHistoryIndex(int index)
	{
		if (index < 0 || index >= fileStates.Spawnset.History.Count)
			return;

		// Remove the keyboard focus to prevent undo/redo not working when a text input is focused, and the next/previous history entry changes the value of that text input.
		// TODO: This currently introduces a little bug; there is an additional history entry added when undoing/redoing, but this is better than the alternative.
		// A way to fix this would be to unfocus the keyboard earlier?
		ImGui.SetKeyboardFocusHere(-1);

		fileStates.Spawnset.SetHistoryIndex(index);

		UpdateScroll = true;
		spawnsWindow.ClearUnusedSelections();
	}
}
