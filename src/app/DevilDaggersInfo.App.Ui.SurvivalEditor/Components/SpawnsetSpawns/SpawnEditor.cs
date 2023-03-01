using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.Extensions;
using DevilDaggersInfo.App.Ui.Base.Rendering.Text;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.App.Ui.Base.Utils;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Types.Core.Spawnsets;
using System.Collections.Immutable;
using Warp.NET.Text;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetSpawns;

public class SpawnEditor : AbstractComponent
{
	private readonly Dictionary<EnemyType, TextButton> _enemyTypeButtons = new();

	// TODO: On spawn select, set these values.
	private EnemyType _selectedEnemyType;
	private float _selectedDelay;

	public SpawnEditor(IBounds bounds)
		: base(bounds)
	{
		const int width = 96;

		AddEnemyButton(96, 0, EnemyType.Empty);
		AddEnemyButton(144, 0, EnemyType.Squid1);
		AddEnemyButton(192, 0, EnemyType.Squid2);
		AddEnemyButton(240, 0, EnemyType.Squid3);
		AddEnemyButton(288, 0, EnemyType.Leviathan);
		AddEnemyButton(336, 0, EnemyType.Thorn);
		AddEnemyButton(144, 16, EnemyType.Spider1);
		AddEnemyButton(192, 16, EnemyType.Spider2);
		AddEnemyButton(240, 16, EnemyType.Centipede);
		AddEnemyButton(288, 16, EnemyType.Gigapede);
		AddEnemyButton(336, 16, EnemyType.Ghostpede);

		Label enemyTypeLabel = new(bounds.CreateNested(0, 0, width, 16), "Enemy", LabelStyles.DefaultLeft);
		Label delayLabel = new(bounds.CreateNested(0, 32, width, 16), "Delay", LabelStyles.DefaultLeft);
		TextInput delayTextInput = new(bounds.CreateNested(288, 32, width, 16), true, OnDelayChange, OnDelayChange, OnDelayChange, TextInputStyles.Default);
		delayTextInput.KeyboardInput.SetText("0.0000");

		NestingContext.Add(enemyTypeLabel);
		NestingContext.Add(delayLabel);
		NestingContext.Add(delayTextInput);

		TextButtonStyle spawnText = new(Color.White, TextAlign.Middle, FontSize.H16);
		TextButton addButton = new(bounds.CreateNested(96, 72, width, 32), AddSpawn, new(new(0, 127, 0, 255), Color.White, new(0, 191, 0, 255), 1), spawnText, "ADD");
		TextButton insertButton = new(bounds.CreateNested(192, 72, width, 32), InsertSpawn, new(new(0, 63, 127, 255), Color.White, new(0, 95, 191, 255), 1), spawnText, "INSERT");
		TextButton editButton = new(bounds.CreateNested(288, 72, width, 32), EditSpawn, new(new(127, 63, 0, 255), Color.White, new(191, 95, 0, 255), 1), spawnText, "EDIT");

		NestingContext.Add(addButton);
		NestingContext.Add(insertButton);
		NestingContext.Add(editButton);

		void AddEnemyButton(int x, int y, EnemyType enemyType)
		{
			ButtonStyle buttonStyle = _selectedEnemyType == enemyType ? ButtonStyles.SelectedEnemies[enemyType] : ButtonStyles.Enemies[enemyType];
			TextButtonStyle textButtonStyle = new(buttonStyle.BackgroundColor.ReadableColorForBrightness(), TextAlign.Left, FontSize.H12);
			TextButton button = new(bounds.CreateNested(x, y, 48, 16), () => SetSelectedEnemyType(enemyType), buttonStyle, textButtonStyle, enemyType.GetShortName());
			_enemyTypeButtons.Add(enemyType, button);
			NestingContext.Add(button);
		}
	}

	private void OnDelayChange(string s) => ParseUtils.TryParseAndExecute<float>(s, 0, f => _selectedDelay = f);

	private void SetSelectedEnemyType(EnemyType enemyType)
	{
		_selectedEnemyType = enemyType;

		foreach (KeyValuePair<EnemyType, TextButton> kvp in _enemyTypeButtons)
			kvp.Value.ButtonStyle = _selectedEnemyType == kvp.Key ? ButtonStyles.SelectedEnemies[kvp.Key] : ButtonStyles.Enemies[kvp.Key];
	}

	private void AddSpawn()
	{
		List<Spawn> newSpawns = StateManager.SpawnsetState.Spawnset.Spawns.ToList();
		newSpawns.Add(new(_selectedEnemyType, _selectedDelay));

		StateManager.Dispatch(new AddSpawn(newSpawns.ToImmutableArray()));
	}

	private void EditSpawn()
	{
		ImmutableArray<Spawn> newSpawns = StateManager.SpawnsetState.Spawnset.Spawns.Select((t, i) => StateManager.SpawnEditorState.SelectedIndices.Contains(i) ? new(_selectedEnemyType, _selectedDelay) : t).ToImmutableArray();

		StateManager.Dispatch(new EditSpawns(newSpawns));
	}

	private void InsertSpawn()
	{
		int firstSelection = StateManager.SpawnEditorState.SelectedIndices.Count == 0 ? 0 : StateManager.SpawnEditorState.SelectedIndices.Min();
		ImmutableArray<Spawn> newSpawns = StateManager.SpawnsetState.Spawnset.Spawns.Insert(firstSelection, new(_selectedEnemyType, _selectedDelay));

		List<int> indices = StateManager.SpawnEditorState.SelectedIndices.Select(i => i + 1).ToList();
		StateManager.Dispatch(new SetSpawnSelections(indices));

		StateManager.Dispatch(new InsertSpawn(newSpawns));
	}
}
