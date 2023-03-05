using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.App.Ui.Base.Utils;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Utils;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Common.Exceptions;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Types.Core.Spawnsets;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetSettings;

public class SettingsWrapper : AbstractComponent
{
	private const int _offset = 16;

	private static readonly ButtonStyle _spawnsetSetting = new(Color.Black, Color.Gray(0.5f), Color.Gray(0.5f), 1);
	private static readonly ButtonStyle _selectedSpawnsetSetting = new(Color.Gray(0.5f), Color.White, Color.Gray(0.75f), 1);

	private readonly int _halfWidth;
	private readonly int _thirdWidth;
	private readonly int _quarterWidth;

	private readonly TextButton _buttonV0V1;
	private readonly TextButton _buttonV2V3;
	private readonly TextButton _buttonV3Next;

	private readonly TextButton _buttonSurvival;
	private readonly TextButton _buttonTimeAttack;
	private readonly TextButton _buttonRace;

	private readonly TextButton _buttonLevel1;
	private readonly TextButton _buttonLevel2;
	private readonly TextButton _buttonLevel3;
	private readonly TextButton _buttonLevel4;

	private readonly Label _labelAdditionalGems;
	private readonly Label _labelTimerStart;
	private readonly SpawnsetTextInput _textInputShrinkStart;
	private readonly SpawnsetTextInput _textInputShrinkEnd;
	private readonly SpawnsetTextInput _textInputShrinkRate;
	private readonly SpawnsetTextInput _textInputBrightness;
	private readonly SpawnsetTextInput _textInputAdditionalGems;
	private readonly SpawnsetTextInput _textInputTimerStart;

	public SettingsWrapper(IBounds bounds)
		: base(bounds)
	{
		int width = bounds.Size.X;
		_halfWidth = width / 2;
		_thirdWidth = (int)MathF.Ceiling(width / 3f);
		_quarterWidth = _halfWidth / 2;

		Label title = new(bounds.CreateNested(0, 0, width, 48), "Settings", LabelStyles.Title);
		NestingContext.Add(title);

		int y = title.Bounds.Size.Y;
		_buttonV0V1 = CreateFormatButton(y, 0, 8, 4);
		_buttonV2V3 = CreateFormatButton(y, 1, 9, 4);
		_buttonV3Next = CreateFormatButton(y, 2, 9, 6);

		y += _offset;
		_buttonSurvival = CreateGameModeButton(y, GameMode.Survival);
		_buttonTimeAttack = CreateGameModeButton(y, GameMode.TimeAttack);
		_buttonRace = CreateGameModeButton(y, GameMode.Race);

		y += _offset;
		(_textInputShrinkStart, _) = AddSetting("Shrink start", ref y, ChangeShrinkStart);
		(_textInputShrinkEnd, _) = AddSetting("Shrink end", ref y, ChangeShrinkEnd);
		(_textInputShrinkRate, _) = AddSetting("Shrink rate", ref y, ChangeShrinkRate);
		(_textInputBrightness, _) = AddSetting("Brightness", ref y, ChangeBrightness);

		y += _offset;
		_buttonLevel1 = CreateHandButton(y, HandLevel.Level1);
		_buttonLevel2 = CreateHandButton(y, HandLevel.Level2);
		_buttonLevel3 = CreateHandButton(y, HandLevel.Level3);
		_buttonLevel4 = CreateHandButton(y, HandLevel.Level4);

		y += _offset;
		(_textInputAdditionalGems, _labelAdditionalGems) = AddSetting("Added gems", ref y, ChangeAdditionalGems);
		(_textInputTimerStart, _labelTimerStart) = AddSetting("Timer start", ref y, ChangeTimerStart);

		NestingContext.Add(_buttonV0V1);
		NestingContext.Add(_buttonV2V3);
		NestingContext.Add(_buttonV3Next);

		NestingContext.Add(_buttonSurvival);
		NestingContext.Add(_buttonTimeAttack);
		NestingContext.Add(_buttonRace);

		NestingContext.Add(_buttonLevel1);
		NestingContext.Add(_buttonLevel2);
		NestingContext.Add(_buttonLevel3);
		NestingContext.Add(_buttonLevel4);

		SetSpawnset();

		StateManager.Subscribe<LoadSpawnset>(SetSpawnset);
		StateManager.Subscribe<SetSpawnsetHistoryIndex>(SetSpawnset);
		StateManager.Subscribe<UpdateAdditionalGems>(SetSpawnset);
		StateManager.Subscribe<UpdateBrightness>(SetSpawnset);
		StateManager.Subscribe<UpdateFormat>(SetSpawnset);
		StateManager.Subscribe<UpdateGameMode>(SetSpawnset);
		StateManager.Subscribe<UpdateHandLevel>(SetSpawnset);
		StateManager.Subscribe<UpdateShrinkStart>(SetSpawnset);
		StateManager.Subscribe<UpdateShrinkEnd>(SetSpawnset);
		StateManager.Subscribe<UpdateShrinkRate>(SetSpawnset);
		StateManager.Subscribe<UpdateTimerStart>(SetSpawnset);
	}

	private TextButton CreateFormatButton(int y, int index, int worldVersion, int spawnVersion)
	{
		string str = SpawnsetBinary.GetGameVersionString(worldVersion, spawnVersion);
		return new(Bounds.CreateNested(index * _thirdWidth, y, _thirdWidth, _offset), () => StateManager.Dispatch(new UpdateFormat(worldVersion, spawnVersion)), _spawnsetSetting, TextButtonStyles.DefaultMiddle, str);
	}

	private TextButton CreateGameModeButton(int y, GameMode gameMode)
	{
		int index = (int)gameMode;
		return new(Bounds.CreateNested(index * _thirdWidth, y, _thirdWidth, _offset), () => StateManager.Dispatch(new UpdateGameMode(gameMode)), _spawnsetSetting, TextButtonStyles.DefaultMiddle, ToShortString());

		string ToShortString() => gameMode switch
		{
			GameMode.Survival => nameof(GameMode.Survival),
			GameMode.TimeAttack => "TA",
			GameMode.Race => nameof(GameMode.Race),
			_ => throw new InvalidEnumConversionException(gameMode),
		};
	}

	private TextButton CreateHandButton(int y, HandLevel handLevel)
	{
		int index = (int)handLevel - 1;
		return new(Bounds.CreateNested(index * _quarterWidth, y, _quarterWidth, _offset), () => StateManager.Dispatch(new UpdateHandLevel(handLevel)), ButtonStyles.HandLevels[handLevel], TextButtonStyles.DefaultMiddle, ToShortString());

		string ToShortString() => handLevel switch
		{
			HandLevel.Level1 => "Lvl 1",
			HandLevel.Level2 => "Lvl 2",
			HandLevel.Level3 => "Lvl 3",
			HandLevel.Level4 => "Lvl 4",
			_ => throw new InvalidEnumConversionException(handLevel),
		};
	}

	private (SpawnsetTextInput TextInput, Label Label) AddSetting(string labelText, ref int y, Action<string> onInput)
	{
		Label label = new(Bounds.CreateNested(0, y, _halfWidth, _offset), labelText, LabelStyles.DefaultLeft);
		SpawnsetTextInput textInput = SpawnsetComponentBuilder.CreateSpawnsetTextInput(Bounds.CreateNested(_halfWidth, y, _halfWidth, _offset), onInput);
		NestingContext.Add(label);
		NestingContext.Add(textInput);
		y += _offset;

		return (textInput, label);
	}

	private static void ChangeAdditionalGems(string input) => ChangeSetting<int>(v => StateManager.Dispatch(new UpdateAdditionalGems(v)), input);

	private static void ChangeTimerStart(string input) => ChangeSetting<float>(v => StateManager.Dispatch(new UpdateTimerStart(v)), input);

	private static void ChangeShrinkStart(string input) => ChangeSetting<float>(v => StateManager.Dispatch(new UpdateShrinkStart(v)), input);

	private static void ChangeShrinkEnd(string input) => ChangeSetting<float>(v => StateManager.Dispatch(new UpdateShrinkEnd(v)), input);

	private static void ChangeShrinkRate(string input) => ChangeSetting<float>(v => StateManager.Dispatch(new UpdateShrinkRate(v)), input);

	private static void ChangeBrightness(string input) => ChangeSetting<float>(v => StateManager.Dispatch(new UpdateBrightness(v)), input);

	private static void ChangeSetting<T>(Action<T> action, string input)
		where T : IParsable<T>
	{
		ParseUtils.TryParseAndExecute(input, action);
	}

	private void SetSpawnset()
	{
		SpawnsetBinary spawnset = StateManager.SpawnsetState.Spawnset;

		_buttonV0V1.ButtonStyle = GetFormatBackground(8, 4);
		_buttonV2V3.ButtonStyle = GetFormatBackground(9, 4);
		_buttonV3Next.ButtonStyle = GetFormatBackground(9, 6);

		_buttonSurvival.ButtonStyle = spawnset.GameMode == GameMode.Survival ? _selectedSpawnsetSetting : _spawnsetSetting;
		_buttonTimeAttack.ButtonStyle = spawnset.GameMode == GameMode.TimeAttack ? _selectedSpawnsetSetting : _spawnsetSetting;
		_buttonRace.ButtonStyle = spawnset.GameMode == GameMode.Race ? _selectedSpawnsetSetting : _spawnsetSetting;

		bool practice = spawnset.SpawnVersion > 4;
		_buttonLevel1.IsActive = practice;
		_buttonLevel2.IsActive = practice;
		_buttonLevel3.IsActive = practice;
		_buttonLevel4.IsActive = practice;

		_labelAdditionalGems.IsActive = practice;
		_textInputAdditionalGems.IsActive = practice;
		_textInputAdditionalGems.SetTextIfDeselected(spawnset.AdditionalGems.ToString());

		bool timerStart = spawnset.SpawnVersion > 5;
		_labelTimerStart.IsActive = timerStart;
		_textInputTimerStart.IsActive = timerStart;
		_textInputTimerStart.SetTextIfDeselected(spawnset.TimerStart.ToString(StringFormats.TimeFormat));

		_textInputShrinkStart.SetTextIfDeselected(spawnset.ShrinkStart.ToString("0.0"));
		_textInputShrinkEnd.SetTextIfDeselected(spawnset.ShrinkEnd.ToString("0.0"));
		_textInputShrinkRate.SetTextIfDeselected(spawnset.ShrinkRate.ToString("0.000"));
		_textInputBrightness.SetTextIfDeselected(spawnset.Brightness.ToString("0.0"));

		_buttonLevel1.ButtonStyle = GetStyle(HandLevel.Level1);
		_buttonLevel2.ButtonStyle = GetStyle(HandLevel.Level2);
		_buttonLevel3.ButtonStyle = GetStyle(HandLevel.Level3);
		_buttonLevel4.ButtonStyle = GetStyle(HandLevel.Level4);

		ButtonStyle GetStyle(HandLevel handLevel) => handLevel == spawnset.HandLevel ? ButtonStyles.SelectedHandLevels[handLevel] : ButtonStyles.HandLevels[handLevel];

		ButtonStyle GetFormatBackground(int worldVersion, int spawnVersion) => worldVersion == spawnset.WorldVersion && spawnVersion == spawnset.SpawnVersion ? _selectedSpawnsetSetting : _spawnsetSetting;
	}
}
