using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;
using DevilDaggersInfo.Common.Exceptions;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Extensions;

public static class SpawnsetEditTypeExtensions
{
	private const int _colorValue = 63;

	private static readonly Color _colorShrink = new(0, _colorValue, 0, 255);
	private static readonly Color _colorBrightness = new(_colorValue, _colorValue, 0, 255);
	private static readonly Color _colorRaceDagger = new(_colorValue * 2, _colorValue, 0, 255);
	private static readonly Color _colorArena = new(_colorValue, 0, 0, 255);
	private static readonly Color _colorMisc = new(0, 0, _colorValue, 255);
	private static readonly Color _colorPractice = new(0, _colorValue, _colorValue, 255);
	private static readonly Color _colorSpawn = new(_colorValue, 0, _colorValue, 255);

	public static string GetChange(this SpawnsetEditType spawnsetEditType) => spawnsetEditType switch
	{
		SpawnsetEditType.Reset => "Spawnset reset",
		SpawnsetEditType.ArenaTileHeight => "Arena tile height edit",
		SpawnsetEditType.ArenaPencil => "Arena pencil edit",
		SpawnsetEditType.ArenaLine => "Arena line edit",
		SpawnsetEditType.ArenaRectangle => "Arena rectangle edit",
		SpawnsetEditType.ArenaEllipse => "Arena ellipse edit",
		SpawnsetEditType.ArenaBucket => "Arena bucket edit",
		SpawnsetEditType.RaceDagger => "Race dagger position change",
		SpawnsetEditType.ShrinkStart => "Shrink start change",
		SpawnsetEditType.ShrinkEnd => "Shrink end change",
		SpawnsetEditType.ShrinkRate => "Shrink rate change",
		SpawnsetEditType.Brightness => "Brightness change",
		SpawnsetEditType.Format => "Format change",
		SpawnsetEditType.GameMode =>"Game mode change",
		SpawnsetEditType.HandLevel => "Hand level change",
		SpawnsetEditType.AdditionalGems => "Additional gems change",
		SpawnsetEditType.TimerStart => "Timer start change",
		SpawnsetEditType.SpawnDelete => "Spawn deletion",
		SpawnsetEditType.SpawnAdd => "Spawn addition",
		SpawnsetEditType.SpawnEdit => "Spawn edit",
		SpawnsetEditType.SpawnInsert => "Spawn insertion",
		_ => throw new InvalidEnumConversionException(spawnsetEditType),
	};

	public static Color GetColor(this SpawnsetEditType spawnsetEditType) => spawnsetEditType switch
	{
		SpawnsetEditType.Reset => _colorMisc,
		SpawnsetEditType.ArenaTileHeight => _colorArena,
		SpawnsetEditType.ArenaPencil => _colorArena,
		SpawnsetEditType.ArenaLine => _colorArena,
		SpawnsetEditType.ArenaRectangle => _colorArena,
		SpawnsetEditType.ArenaEllipse => _colorArena,
		SpawnsetEditType.ArenaBucket => _colorArena,
		SpawnsetEditType.RaceDagger => _colorRaceDagger,
		SpawnsetEditType.ShrinkStart => _colorShrink,
		SpawnsetEditType.ShrinkEnd => _colorShrink,
		SpawnsetEditType.ShrinkRate => _colorShrink,
		SpawnsetEditType.Brightness => _colorBrightness,
		SpawnsetEditType.Format => _colorMisc,
		SpawnsetEditType.GameMode => _colorMisc,
		SpawnsetEditType.HandLevel => _colorPractice,
		SpawnsetEditType.AdditionalGems => _colorPractice,
		SpawnsetEditType.TimerStart => _colorPractice,
		SpawnsetEditType.SpawnDelete => _colorSpawn,
		SpawnsetEditType.SpawnAdd => _colorSpawn,
		SpawnsetEditType.SpawnEdit => _colorSpawn,
		SpawnsetEditType.SpawnInsert => _colorSpawn,
		_ => throw new InvalidEnumConversionException(spawnsetEditType),
	};
}
