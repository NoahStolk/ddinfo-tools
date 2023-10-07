using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.Core.Spawnset;
using System.Security.Cryptography;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Utils;

public static class SpawnsetHistoryUtils
{
	private const int _maxHistoryEntries = 100;

	public static void Save(SpawnsetEditType spawnsetEditType)
	{
		SpawnsetBinary copy = SpawnsetState.Spawnset.DeepCopy();
		byte[] hash = MD5.HashData(copy.ToBytes());

		if (spawnsetEditType == SpawnsetEditType.Reset)
		{
			HistoryChild.UpdateHistory(new List<SpawnsetHistoryEntry> { new(copy, hash, spawnsetEditType) }, 0);
		}
		else
		{
			byte[] originalHash = HistoryChild.History[HistoryChild.CurrentHistoryIndex].Hash;

			if (originalHash.SequenceEqual(hash))
				return;

			SpawnsetHistoryEntry historyEntry = new(copy, hash, spawnsetEditType);

			// Clear any newer history.
			List<SpawnsetHistoryEntry> newHistory = HistoryChild.History.ToList();
			newHistory = newHistory.Take(HistoryChild.CurrentHistoryIndex + 1).Append(historyEntry).ToList();

			// Remove history if there are too many entries.
			int newCurrentIndex = HistoryChild.CurrentHistoryIndex + 1;
			if (newHistory.Count > _maxHistoryEntries)
			{
				newHistory.RemoveAt(0);
				newCurrentIndex--;
			}

			HistoryChild.UpdateHistory(newHistory, newCurrentIndex);
		}
	}
}
