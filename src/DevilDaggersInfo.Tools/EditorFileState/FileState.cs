namespace DevilDaggersInfo.Tools.EditorFileState;

public class FileState<TObject, TEditType>
	where TObject : class
	where TEditType : Enum
{
	private const int _maxHistoryEntries = 100;

	private readonly TEditType _defaultEditType;
	private readonly Func<TObject, byte[]> _toHash;
	private readonly Func<TObject, byte[]> _save;
	private readonly Func<TObject, TObject> _deepCopy;
	private readonly Func<TEditType, TEditType, bool> _editTypeEquals;
	private readonly Action<Action> _savePromptAction;

	private TObject _obj;
	private byte[] _memoryMd5Hash;
	private byte[] _fileMd5Hash;

	public FileState(
		TObject obj,
		TEditType defaultEditType,
		Func<TObject, byte[]> toHash,
		Func<TObject, byte[]> save,
		Func<TObject, TObject> deepCopy,
		Func<TEditType, TEditType, bool> editTypeEquals,
		Action<Action> savePromptAction)
	{
		_obj = obj;
		_toHash = toHash;
		_save = save;
		_deepCopy = deepCopy;
		_defaultEditType = defaultEditType;
		_editTypeEquals = editTypeEquals;
		_savePromptAction = savePromptAction;

		byte[] hash = _toHash(obj);
		_memoryMd5Hash = hash;
		_fileMd5Hash = hash;

		History = new List<HistoryEntry<TObject, TEditType>> { new(obj, hash, defaultEditType) };
	}

	public TObject Object
	{
		get => _obj;
		private set
		{
			_obj = value;

			_memoryMd5Hash = _toHash(_obj);
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
		File.WriteAllBytes(path, _save(_obj));
		SetFile(path, Path.GetFileName(path));
	}

	public void PromptSave(Action action)
	{
		if (!IsModified)
			action();
		else
			_savePromptAction(action);
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
		byte[] hash = _toHash(copy);

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
