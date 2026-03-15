using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Settings;

namespace DevilDaggersInfo.Tools.Ui.ModManager;

internal sealed class ModManagerState(PopupManager popupManager)
{
	private string? _selectedFileName;

	public ModBinaryType? BinaryType { get; private set; }
	public int AssetCount { get; private set; }
	public int ProhibitedAssetCount { get; private set; }
	public long? ModFileSize { get; private set; }
	public List<ModBinaryTocEntry> DisplayedTocEntries { get; set; } = [];

	public string? SelectedFileName
	{
		get => _selectedFileName;
		set
		{
			_selectedFileName = value;
			LoadTocEntries();
		}
	}

	public void LoadTocEntries()
	{
		if (_selectedFileName == null)
			return;

		string filePath = Path.Combine(UserSettings.ModsDirectory, _selectedFileName);
		if (!File.Exists(filePath))
			return;

		try
		{
			using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
			ModFileSize = fs.Length;
			using BinaryReader reader = new(fs);
			ModBinaryToc modBinaryToc = ModBinaryToc.FromReader(reader);
			BinaryType = modBinaryToc.Type;
			AssetCount = modBinaryToc.Entries.Count;
			ProhibitedAssetCount = modBinaryToc.Entries.Count(c => AssetContainer.IsProhibited(c.AssetType, c.Name));
			DisplayedTocEntries.Clear();
			DisplayedTocEntries.AddRange(modBinaryToc.Entries);
		}
		catch (InvalidModBinaryException)
		{
			ClearState();
		}
		catch (Exception ex) when (ex.IsFileIoException())
		{
			Root.Log.Error(ex, $"Error loading mod binary '{_selectedFileName}'.");
			popupManager.ShowError($"Error loading mod binary '{_selectedFileName}'.", ex);
			ClearState();
		}
	}

	public void DeleteIfSelected(string fileName)
	{
		if (SelectedFileName == fileName)
			SelectedFileName = null;
	}

	public void UpdateIfSelected(string oldFileName, string newFileName)
	{
		if (SelectedFileName == oldFileName)
			SelectedFileName = newFileName;
	}

	private void ClearState()
	{
		DisplayedTocEntries.Clear();
		BinaryType = null;
		AssetCount = 0;
		ProhibitedAssetCount = 0;
		_selectedFileName = null;
		ModFileSize = null;
	}
}
