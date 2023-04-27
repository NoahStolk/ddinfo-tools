using DevilDaggersInfo.App.Engine;
using DevilDaggersInfo.App.Engine.Extensions;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena;
using DevilDaggersInfo.App.Ui.SurvivalEditor.EditorArena.Data;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Utils;
using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.EditorArena;

public class ArenaDaggerState : IArenaState
{
	private bool _settingRaceDagger;
	private Vector2? _position;

	public void Handle(ArenaMousePosition mousePosition)
	{
		if (StateManager.SpawnsetState.Spawnset.GameMode != GameMode.Race)
			return;

		if (Input.IsButtonPressed(MouseButton.Left))
		{
			_settingRaceDagger = true;
			_position = GetSnappedDaggerPosition();
		}
		else if (Input.IsButtonHeld(MouseButton.Left))
		{
			if (_settingRaceDagger)
				_position = GetSnappedDaggerPosition();
		}
		else if (Input.IsButtonReleased(MouseButton.Left))
		{
			if (!_position.HasValue)
				return;

			Vector2 tileCoordinate = _position.Value / Arena.TileSize;
			Vector2 daggerPosition = new(StateManager.SpawnsetState.Spawnset.TileToWorldCoordinate(tileCoordinate.X), StateManager.SpawnsetState.Spawnset.TileToWorldCoordinate(tileCoordinate.Y));
			StateManager.Dispatch(new UpdateRaceDaggerPosition(daggerPosition));

			Reset();
		}

		Vector2 GetSnappedDaggerPosition()
		{
			return ArenaEditingUtils.Snap(mousePosition.Real.ToVector2(), StateManager.ArenaDaggerState.Snap * Arena.TileSize);
		}
	}

	public void HandleOutOfRange(ArenaMousePosition mousePosition)
	{
		Reset();
	}

	private void Reset()
	{
		_settingRaceDagger = false;
		_position = null;
	}

	public void Render(ArenaMousePosition mousePosition, Vector2i<int> origin, float depth)
	{
		if (!_position.HasValue)
			return;

		Root.Game.SpriteRenderer.Schedule(new(-8, -8), origin.ToVector2() + _position.Value + Arena.HalfTileAsVector2, depth, ContentManager.Content.IconDaggerTexture, Color.HalfTransparentWhite);
	}
}
