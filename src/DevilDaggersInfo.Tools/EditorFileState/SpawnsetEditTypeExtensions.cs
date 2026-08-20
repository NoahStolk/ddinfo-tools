using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.EditorFileState;

internal static class SpawnsetEditTypeExtensions
{
	private const int _colorValue = 63;

	private static readonly Color _colorShrink = new(0, _colorValue, 0, 255);
	private static readonly Color _colorBrightness = new(_colorValue, _colorValue, 0, 255);
	private static readonly Color _colorRaceDagger = new(_colorValue * 2, _colorValue, 0, 255);
	private static readonly Color _colorArena = new(_colorValue, 0, 0, 255);
	private static readonly Color _colorMisc = new(0, 0, _colorValue, 255);
	private static readonly Color _colorPractice = new(0, _colorValue, _colorValue, 255);
	private static readonly Color _colorSpawn = new(_colorValue, 0, _colorValue, 255);

	extension(SpawnsetEditType spawnsetEditType)
	{
		public ReadOnlySpan<byte> GetChange()
		{
			return spawnsetEditType switch
			{
				SpawnsetEditType.Reset => "Spawnset reset"u8,
				SpawnsetEditType.ArenaTileHeight => "Arena tile height edit"u8,
				SpawnsetEditType.ArenaPencil => "Arena pencil edit"u8,
				SpawnsetEditType.ArenaLine => "Arena line edit"u8,
				SpawnsetEditType.ArenaRectangle => "Arena rectangle edit"u8,
				SpawnsetEditType.ArenaEllipse => "Arena ellipse edit"u8,
				SpawnsetEditType.ArenaBucket => "Arena bucket edit"u8,
				SpawnsetEditType.RaceDagger => "Race dagger position change"u8,
				SpawnsetEditType.ShrinkStart => "Shrink start change"u8,
				SpawnsetEditType.ShrinkEnd => "Shrink end change"u8,
				SpawnsetEditType.ShrinkRate => "Shrink rate change"u8,
				SpawnsetEditType.Brightness => "Brightness change"u8,
				SpawnsetEditType.Format => "Format change"u8,
				SpawnsetEditType.GameMode => "Game mode change"u8,
				SpawnsetEditType.HandLevel => "Hand level change"u8,
				SpawnsetEditType.AdditionalGems => "Additional gems change"u8,
				SpawnsetEditType.TimerStart => "Timer start change"u8,
				SpawnsetEditType.SpawnDelete => "Spawn deletion"u8,
				SpawnsetEditType.SpawnAdd => "Spawn addition"u8,
				SpawnsetEditType.SpawnEdit => "Spawn edit"u8,
				SpawnsetEditType.SpawnInsert => "Spawn insertion"u8,
				SpawnsetEditType.SpawnsTransformation => "Spawns transformation"u8,
				_ => throw new UnreachableException(),
			};
		}

		public Color GetColor()
		{
			return spawnsetEditType switch
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
				SpawnsetEditType.SpawnsTransformation => _colorSpawn,
				_ => throw new UnreachableException(),
			};
		}
	}
}
