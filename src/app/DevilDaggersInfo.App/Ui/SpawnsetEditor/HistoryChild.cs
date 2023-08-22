using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;
using System.Numerics;
using System.Security.Cryptography;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor;

public static class HistoryChild
{
	private static readonly IdBuffer _idBuffer = new(32);

	private static bool _updateScroll;

	static HistoryChild()
	{
		SpawnsetBinary spawnset = SpawnsetBinary.CreateDefault();
		History = new List<SpawnsetHistoryEntry> { new(spawnset, MD5.HashData(spawnset.ToBytes()), SpawnsetEditType.Reset) };
	}

	// Note; the history should never be empty.
	public static IReadOnlyList<SpawnsetHistoryEntry> History { get; private set; }

	public static int CurrentHistoryIndex { get; private set; }

	public static void UpdateHistory(IReadOnlyList<SpawnsetHistoryEntry> history, int currentHistoryIndex)
	{
		History = history;
		CurrentHistoryIndex = currentHistoryIndex;
		_updateScroll = true;
	}

	public static void Render()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(8, 1));
		if (ImGui.BeginChild("HistoryChild", new(244, 712)))
		{
			for (int i = 0; i < History.Count; i++)
			{
				SpawnsetHistoryEntry history = History[i];

				if (_updateScroll && i == CurrentHistoryIndex)
				{
					ImGui.SetScrollHereY();
					_updateScroll = false;
				}

				const int borderSize = 2;
				ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, borderSize);
				ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Vector2(0, 0.5f));

				Color color = history.EditType.GetColor();
				ImGui.PushStyleColor(ImGuiCol.Button, color);
				ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color + new Vector4(0.3f, 0.3f, 0.3f, 0));
				ImGui.PushStyleColor(ImGuiCol.ButtonActive, color + new Vector4(0.5f, 0.5f, 0.5f, 0));
				ImGui.PushStyleColor(ImGuiCol.Border, i == CurrentHistoryIndex ? Color.White : Color.Black);

				_idBuffer.Overwrite("HistoryButton", i);
				ImGui.PushID(_idBuffer);
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
		SetHistoryIndex(CurrentHistoryIndex - 1);
	}

	public static void Redo()
	{
		SetHistoryIndex(CurrentHistoryIndex + 1);
	}

	private static void SetHistoryIndex(int index)
	{
		if (index < 0 || index >= History.Count)
			return;

		// Workaround: Remove the window focus to prevent undo/redo not working when a text input is focused, and the next/previous history entry changes the value of that text input.
		// TODO: Find another way to fix this. Currently this workaround is not ideal, this is especially noticeable when working with the arena bucket.
		ImGui.SetWindowFocus(null);

		CurrentHistoryIndex = Math.Clamp(index, 0, History.Count - 1);
		SpawnsetState.Spawnset = History[CurrentHistoryIndex].Spawnset.DeepCopy();
		_updateScroll = true;

		SpawnsChild.ClearUnusedSelections();
	}
}
