using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.State;
using System.Security.Cryptography;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;

public static class SpawnsetHistoryUtils
{
	private const int _maxHistoryEntries = 100;

	static SpawnsetHistoryUtils()
	{
		SpawnsetBinary spawnset = SpawnsetBinary.CreateDefault();
		History = new List<SpawnsetHistoryEntry> { new(spawnset, MD5.HashData(spawnset.ToBytes()), SpawnsetEditType.Reset) };
	}

	// Note; the history should never be empty.
	public static IReadOnlyList<SpawnsetHistoryEntry> History { get; private set; }

	public static int CurrentHistoryIndex { get; private set; }

	private static void UpdateHistory(IReadOnlyList<SpawnsetHistoryEntry> history, int currentHistoryIndex)
	{
		History = history;
		CurrentHistoryIndex = currentHistoryIndex;
		HistoryChild.UpdateScroll = true;
	}

	public static void SetHistoryIndex(int index)
	{
		CurrentHistoryIndex = Math.Clamp(index, 0, History.Count - 1);
		FileStates.Spawnset.Update(History[CurrentHistoryIndex].Spawnset.DeepCopy());
	}

	public static void Save(SpawnsetEditType spawnsetEditType)
	{
		SpawnsetBinary copy = FileStates.Spawnset.Object.DeepCopy();
		byte[] hash = MD5.HashData(copy.ToBytes());

		if (spawnsetEditType == SpawnsetEditType.Reset)
		{
			UpdateHistory(new List<SpawnsetHistoryEntry> { new(copy, hash, spawnsetEditType) }, 0);
		}
		else
		{
			byte[] originalHash = History[CurrentHistoryIndex].Hash;

			if (originalHash.SequenceEqual(hash))
				return;

			SpawnsetHistoryEntry historyEntry = new(copy, hash, spawnsetEditType);

			// Clear any newer history.
			List<SpawnsetHistoryEntry> newHistory = History.ToList();
			newHistory = newHistory.Take(CurrentHistoryIndex + 1).Append(historyEntry).ToList();

			// Remove history if there are too many entries.
			int newCurrentIndex = CurrentHistoryIndex + 1;
			if (newHistory.Count > _maxHistoryEntries)
			{
				newHistory.RemoveAt(0);
				newCurrentIndex--;
			}

			UpdateHistory(newHistory, newCurrentIndex);
		}
	}
}
