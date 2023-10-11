using DevilDaggersInfo.Tools.Ui.Popups;
using System.Security.Cryptography;

namespace DevilDaggersInfo.Tools.EditorFileState;

public class FileState<TObject, TEditType>
	where TObject : class
	where TEditType : Enum
{
	private const int _maxHistoryEntries = 100;

	private readonly TEditType _defaultEditType;
	private readonly Func<TObject, byte[]> _toBytes;
	private readonly Func<TObject, TObject> _deepCopy;
	private readonly Func<TEditType, TEditType, bool> _editTypeEquals;

	private TObject _obj;
	private byte[] _memoryMd5Hash;
	private byte[] _fileMd5Hash;

	public FileState(TObject obj, TEditType defaultEditType, Func<TObject, byte[]> toBytes, Func<TObject, TObject> deepCopy, Func<TEditType, TEditType, bool> editTypeEquals)
	{
		_obj = obj;
		_toBytes = toBytes;
		_deepCopy = deepCopy;
		_defaultEditType = defaultEditType;
		_editTypeEquals = editTypeEquals;

		byte[] fileBytes = _toBytes(obj);
		_memoryMd5Hash = MD5.HashData(fileBytes);
		_fileMd5Hash = MD5.HashData(fileBytes);

		History = new List<HistoryEntry<TObject, TEditType>> { new(obj, MD5.HashData(fileBytes), defaultEditType) };
	}

	public TObject Object
	{
		get => _obj;
		private set
		{
			_obj = value;

			byte[] fileBytes = _toBytes(_obj);
			_memoryMd5Hash = MD5.HashData(fileBytes);
			IsModified = !_fileMd5Hash.SequenceEqual(_memoryMd5Hash);
		}
	}

	public string? FileName { get; private set; }
	public string? FilePath { get; private set; }
	public bool IsModified { get; private set; }

	// Note; the history should never be empty.
	public IReadOnlyList<HistoryEntry<TObject, TEditType>> History { get; private set; }

	public int CurrentHistoryIndex { get; private set; }

	public void Update(TObject obj)
	{
		Object = obj;
	}

	public void SetFile(string? path, string? name)
	{
		FilePath = path;
		FileName = name;
		_fileMd5Hash = _memoryMd5Hash;
		IsModified = !_fileMd5Hash.SequenceEqual(_memoryMd5Hash);
	}

	public void SaveFile(string path)
	{
		File.WriteAllBytes(path, _toBytes(_obj));
		SetFile(path, Path.GetFileName(path));
	}

	public void PromptSave(Action action)
	{
		if (!IsModified)
			action();
		else
			PopupManager.ShowSaveSpawnsetPrompt(action);
	}

	private void UpdateHistory(IReadOnlyList<HistoryEntry<TObject, TEditType>> history, int currentHistoryIndex)
	{
		History = history;
		CurrentHistoryIndex = currentHistoryIndex;
	}

	public void SetHistoryIndex(int index)
	{
		CurrentHistoryIndex = Math.Clamp(index, 0, History.Count - 1);
		Update(_deepCopy(History[CurrentHistoryIndex].Object));
	}

	public void Save(TEditType editType)
	{
		TObject copy = _deepCopy(Object);
		byte[] hash = MD5.HashData(_toBytes(copy));

		if (_editTypeEquals(editType, _defaultEditType))
		{
			UpdateHistory(new List<HistoryEntry<TObject, TEditType>> { new(copy, hash, editType) }, 0);
		}
		else
		{
			byte[] originalHash = History[CurrentHistoryIndex].Hash;

			if (originalHash.SequenceEqual(hash))
				return;

			HistoryEntry<TObject, TEditType> historyEntry = new(copy, hash, editType);

			// Clear any newer history.
			List<HistoryEntry<TObject, TEditType>> newHistory = History.ToList();
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
