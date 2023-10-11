using DevilDaggersInfo.Tools.Ui.Popups;
using System.Security.Cryptography;

namespace DevilDaggersInfo.Tools.EditorFileState;

public class FileState<T>
{
	private readonly Func<T, byte[]> _toBytes;

	private T _obj;
	private byte[] _memoryMd5Hash;
	private byte[] _fileMd5Hash;

	public FileState(T obj, Func<T, byte[]> toBytes)
	{
		_obj = obj;
		_toBytes = toBytes;

		byte[] fileBytes = _toBytes(obj);
		_memoryMd5Hash = MD5.HashData(fileBytes);
		_fileMd5Hash = MD5.HashData(fileBytes);
	}

	public T Object
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

	public void Update(T obj)
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

	public void PromptSaveSpawnset(Action action)
	{
		if (!IsModified)
			action();
		else
			PopupManager.ShowSaveSpawnsetPrompt(action);
	}
}
