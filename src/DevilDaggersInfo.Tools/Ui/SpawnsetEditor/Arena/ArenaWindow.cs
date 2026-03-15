using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorStates;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena;

internal sealed class ArenaWindow
{
	public const int TileSize = 8;
	private const int _halfTileSize = TileSize / 2;
	public static readonly Vector2 HalfTileSizeAsVector2 = new(_halfTileSize);

	public static readonly Vector2 ArenaSize = new(TileSize * SpawnsetBinary.ArenaDimensionMax);

	private readonly SpawnsetEditor3DWindow _spawnsetEditor3DWindow;
	private readonly FileStates _fileStates;
	private readonly SpawnsetSaver _spawnsetSaver;

	private readonly ArenaEditorControls _arenaEditorControls;
	private readonly ArenaCanvas _arenaCanvas;
	private readonly ArenaHeightButtons _arenaHeightButtons;

	private readonly ArenaPencilState _pencilState;
	private readonly ArenaLineState _lineState;
	private readonly ArenaRectangleState _rectangleState;
	private readonly ArenaEllipseState _ellipseState;
	private readonly ArenaBucketState _bucketState;
	private readonly ArenaDaggerState _daggerState;

	private float _currentSecond;

	// TODO: Don't inject other windows.
	public ArenaWindow(SpawnsetEditor3DWindow spawnsetEditor3DWindow, ResourceManager resourceManager, FileStates fileStates, SpawnsetSaver spawnsetSaver)
	{
		_spawnsetEditor3DWindow = spawnsetEditor3DWindow;
		_fileStates = fileStates;
		_spawnsetSaver = spawnsetSaver;

		_arenaEditorControls = new ArenaEditorControls(resourceManager, this, fileStates);
		_arenaCanvas = new ArenaCanvas(resourceManager, this, fileStates);
		_arenaHeightButtons = new ArenaHeightButtons(this);

		_pencilState = new ArenaPencilState(this, fileStates, spawnsetSaver);
		_lineState = new ArenaLineState(this, fileStates, spawnsetSaver);
		_rectangleState = new ArenaRectangleState(this, fileStates, spawnsetSaver);
		_ellipseState = new ArenaEllipseState(this, fileStates, spawnsetSaver);
		_bucketState = new ArenaBucketState(this, fileStates, spawnsetSaver);
		_daggerState = new ArenaDaggerState(resourceManager, fileStates, spawnsetSaver);
	}

	public float CurrentSecond => _currentSecond;

	public float SelectedHeight { get; set; }
	public ArenaTool ArenaTool { get; set; }

	private IArenaState GetActiveState()
	{
		return ArenaTool switch
		{
			ArenaTool.Pencil => _pencilState,
			ArenaTool.Line => _lineState,
			ArenaTool.Rectangle => _rectangleState,
			ArenaTool.Ellipse => _ellipseState,
			ArenaTool.Bucket => _bucketState,
			ArenaTool.Dagger => _daggerState,
			_ => throw new UnreachableException(),
		};
	}

	public void Render()
	{
		if (ImGui.Begin("Topdown Arena Editor"))
		{
			if (ImGui.BeginChild("Arena", ArenaSize))
			{
				ImGuiIOPtr io = ImGui.GetIO();

				ArenaMousePosition mousePosition = ArenaMousePosition.Get(io, ImGui.GetCursorScreenPos(), _fileStates.Spawnset.Object.ArenaDimension);

				if (mousePosition.IsValid && io.MouseWheel is < -float.Epsilon or > float.Epsilon)
				{
					float[,] newTiles = _fileStates.Spawnset.Object.ArenaTiles.GetMutableClone();
					newTiles[mousePosition.Tile.X, mousePosition.Tile.Y] -= io.MouseWheel;
					_fileStates.Spawnset.Update(_fileStates.Spawnset.Object with { ArenaTiles = new ImmutableArena(_fileStates.Spawnset.Object.ArenaDimension, newTiles) });
					_spawnsetSaver.Save(SpawnsetEditType.ArenaTileHeight);
				}

				IArenaState activeState = GetActiveState();
				_arenaCanvas.Render();
				activeState.Render(mousePosition);

				// Capture mouse input when the mouse is over the canvas.
				// This prevents dragging the window while drawing on the arena canvas.
				ImGui.InvisibleButton("ArenaCanvas", ArenaSize, ImGuiButtonFlags.MouseButtonLeft);
				bool isArenaHovered = ImGui.IsItemHovered();
				if (isArenaHovered)
					ImGui.SetTooltip(Inline.Span($"{_fileStates.Spawnset.Object.ArenaTiles[mousePosition.Tile.X, mousePosition.Tile.Y]}\n<{mousePosition.Tile.X}, {mousePosition.Tile.Y}>"));

				if (isArenaHovered && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
					activeState.InitializeSession(mousePosition);

				if (mousePosition.IsValid)
					activeState.Handle(mousePosition);
				else
					activeState.HandleOutOfRange(mousePosition);
			}

			ImGui.EndChild();

			ImGui.SliderFloat("Time", ref _currentSecond, 0, _fileStates.Spawnset.Object.GetSliderMaxSeconds());

			_spawnsetEditor3DWindow.ArenaScene.CurrentTick = (int)MathF.Round(_currentSecond * 60);

			_arenaEditorControls.Render();
			_arenaHeightButtons.Render();
		}

		ImGui.End();
	}
}
