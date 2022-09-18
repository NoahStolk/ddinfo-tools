using DevilDaggersInfo.App.Tools.Components;
using DevilDaggersInfo.App.Tools.Enums;
using DevilDaggersInfo.App.Tools.States;
using Warp.Ui;

namespace DevilDaggersInfo.App.Tools.Layouts;

public class SaveLayout : Layout
{
	private readonly Button _backButton;
	private readonly TextInput _pathTextInput;
	private readonly TextInput _fileTextInput;
	private readonly Button _saveButton;

	private readonly List<Button> _subDirectoryButtons = new(); // TODO: Implement scroll viewer instead.

	private string _path = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";

	public SaveLayout()
		: base(new Rectangle(0, 0, 1920, 1080))
	{
		_backButton = new(Rectangle.At(0, 0, 32, 32), () => Base.Game.ActiveLayout = Base.Game.MainLayout, Color.Black, Color.White, Color.White, Color.Red, "X", TextAlign.Left, 2, false);
		_pathTextInput = new(Rectangle.At(0, 32, 1024, 32), false, Color.Black, Color.Gray(0.75f), Color.Gray(0.25f), Color.White, Color.White, Color.Green, Color.Gray(0.5f), 8, 2);
		_fileTextInput = new(Rectangle.At(0, 64, 512, 32), false, Color.Black, Color.Gray(0.75f), Color.Gray(0.25f), Color.White, Color.White, Color.Green, Color.Gray(0.5f), 8, 2);
		_saveButton = new(Rectangle.At(512, 64, 128, 32), () => SaveSpawnset(Path.Combine(_path, _fileTextInput.Value.ToString())), Color.Black, Color.White, Color.White, Color.Red, "Save", TextAlign.Left, 2, false);

		NestingContext.Add(_backButton);
		NestingContext.Add(_pathTextInput);
		NestingContext.Add(_fileTextInput);
		NestingContext.Add(_saveButton);
		SetComponentsFromPath(_path);
	}

	private void SetComponentsFromPath(string path)
	{
		Clear();
		_pathTextInput.SetText(path);
		_path = path;

		int i = 2;
		DirectoryInfo? parent = Directory.GetParent(path);
		if (parent != null)
			_subDirectoryButtons.Add(new Button.PathButton(new(0, ++i * 32, 1024, i * 32 + 32), () => SetComponentsFromPath(parent.FullName), "..", Color.Green));

		foreach (string directory in Directory.GetDirectories(_path))
			_subDirectoryButtons.Add(new Button.PathButton(new(0, ++i * 32, 1024, i * 32 + 32), () => SetComponentsFromPath(directory), Path.GetFileName(directory), Color.Yellow));

		foreach (string file in Directory.GetFiles(_path))
			_subDirectoryButtons.Add(new Button.PathButton(new(0, ++i * 32, 1024, i * 32 + 32), () => {}, Path.GetFileName(file), Color.White));

		foreach (Button button in _subDirectoryButtons)
			NestingContext.Add(button);
	}

	private static void SaveSpawnset(string filePath)
	{
		if (Directory.Exists(filePath))
			return; // TODO: Show error.

		if (File.Exists(filePath))
			return; // TODO: Ask to overwrite or cancel.

		File.WriteAllBytes(filePath, StateManager.SpawnsetState.Spawnset.ToBytes());
	}

	private void Clear()
	{
		foreach (Button button in _subDirectoryButtons)
			NestingContext.Remove(button);

		_subDirectoryButtons.Clear();
	}
}
