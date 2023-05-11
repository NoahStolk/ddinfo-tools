using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Core.Spawnset;
using System.Security.Cryptography;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Utils;

public static class SpawnsetHistoryUtils
{
	private const int _maxHistoryEntries = 100;

	public static void Save(SpawnsetEditType spawnsetEditType)
	{
		if (spawnsetEditType == SpawnsetEditType.Reset)
		{
			SpawnsetBinary copy = SpawnsetState.Spawnset.DeepCopy();
			byte[] hash = MD5.HashData(copy.ToBytes());
			HistoryChild.UpdateHistory(new List<SpawnsetHistoryEntry> { new(copy, hash, spawnsetEditType) }, 0);
		}
		else
		{
			SpawnsetBinary copy = SpawnsetState.Spawnset.DeepCopy();
			byte[] originalHash = HistoryChild.History[HistoryChild.CurrentHistoryIndex].Hash;
			byte[] hash = MD5.HashData(copy.ToBytes());

			if (ArrayUtils.AreEqual(originalHash, hash))
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
