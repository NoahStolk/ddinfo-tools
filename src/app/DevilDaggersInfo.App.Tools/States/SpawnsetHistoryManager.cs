using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Core.Spawnset;
using System.Security.Cryptography;

namespace DevilDaggersInfo.App.Tools.States;

public static class SpawnsetHistoryManager
{
	public const int MaxHistoryEntries = 100;
	private static readonly List<SpawnsetHistory> _history = new();

	public static IReadOnlyList<SpawnsetHistory> History => _history;
	public static int Index { get; private set; }

	public static void Reset()
	{
		_history.Clear();
		Save("Reset spawnset");
	}

	public static void Save(string change)
	{
		// Clear any newer history.
		for (int i = _history.Count - 1; i > Index; i--)
			_history.RemoveAt(i);

		SpawnsetBinary copy = StateManager.SpawnsetState.Spawnset.DeepCopy();
		byte[] originalHash = History.Count == 0 ? Array.Empty<byte>() : History[^1].Hash;
		byte[] hash = MD5.HashData(copy.ToBytes());

		if (ArrayUtils.AreEqual(originalHash, hash))
			return;

		_history.Add(new(copy, hash, change));

		if (_history.Count > MaxHistoryEntries)
			_history.RemoveAt(0);

		Index = _history.Count - 1;

		Base.Game.MainLayout.HistoryWrapper.SetHistory();

		// TODO: Set scrollbar to end.
	}

	public static void Undo()
	{
		if (Index <= 0)
			return;

		Index--;
		StateManager.SetSpawnset(_history[Index].Spawnset.DeepCopy());

		Base.Game.MainLayout.HistoryWrapper.SetHistory();
	}

	public static void Redo()
	{
		if (Index >= _history.Count - 1)
			return;

		Index++;
		StateManager.SetSpawnset(_history[Index].Spawnset.DeepCopy());

		Base.Game.MainLayout.HistoryWrapper.SetHistory();
	}
}
