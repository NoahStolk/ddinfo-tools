using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorStates;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena;

public sealed class ArenaWindow
{
	public const int TileSize = 8;
	private const int _halfTileSize = TileSize / 2;
	public static readonly Vector2 HalfTileSizeAsVector2 = new(_halfTileSize);

	public static readonly Vector2 ArenaSize = new(TileSize * SpawnsetBinary.ArenaDimensionMax);

	private readonly SpawnsetEditor3DWindow _spawnsetEditor3DWindow;

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
	public ArenaWindow(SpawnsetEditor3DWindow spawnsetEditor3DWindow, ResourceManager resourceManager)
	{
		_spawnsetEditor3DWindow = spawnsetEditor3DWindow;

		_arenaEditorControls = new ArenaEditorControls(resourceManager, this);
		_arenaCanvas = new ArenaCanvas(resourceManager, this);
		_arenaHeightButtons = new ArenaHeightButtons(this);

		_pencilState = new ArenaPencilState(this);
		_lineState = new ArenaLineState(this);
		_rectangleState = new ArenaRectangleState(this);
		_ellipseState = new ArenaEllipseState(this);
		_bucketState = new ArenaBucketState(this);
		_daggerState = new ArenaDaggerState(resourceManager);
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

				ArenaMousePosition mousePosition = ArenaMousePosition.Get(io, ImGui.GetCursorScreenPos());

				if (mousePosition.IsValid && io.MouseWheel is < -float.Epsilon or > float.Epsilon)
				{
					float[,] newTiles = FileStates.Spawnset.Object.ArenaTiles.GetMutableClone();
					newTiles[mousePosition.Tile.X, mousePosition.Tile.Y] -= io.MouseWheel;
					FileStates.Spawnset.Update(FileStates.Spawnset.Object with { ArenaTiles = new ImmutableArena(FileStates.Spawnset.Object.ArenaDimension, newTiles) });
					SpawnsetHistoryUtils.Save(SpawnsetEditType.ArenaTileHeight);
				}

				IArenaState activeState = GetActiveState();
				_arenaCanvas.Render();
				activeState.Render(mousePosition);

				// Capture mouse input when the mouse is over the canvas.
				// This prevents dragging the window while drawing on the arena canvas.
				ImGui.InvisibleButton("ArenaCanvas", ArenaSize, ImGuiButtonFlags.MouseButtonLeft);
				bool isArenaHovered = ImGui.IsItemHovered();
				if (isArenaHovered)
					ImGui.SetTooltip(Inline.Span($"{FileStates.Spawnset.Object.ArenaTiles[mousePosition.Tile.X, mousePosition.Tile.Y]}\n<{mousePosition.Tile.X}, {mousePosition.Tile.Y}>"));

				if (isArenaHovered && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
					activeState.InitializeSession(mousePosition);

				if (mousePosition.IsValid)
					activeState.Handle(mousePosition);
				else
					activeState.HandleOutOfRange(mousePosition);
			}

			ImGui.EndChild();

			ImGui.SliderFloat("Time", ref _currentSecond, 0, FileStates.Spawnset.Object.GetSliderMaxSeconds());

			_spawnsetEditor3DWindow.ArenaScene.CurrentTick = (int)MathF.Round(_currentSecond * 60);

			_arenaEditorControls.Render();
			_arenaHeightButtons.Render();
		}

		ImGui.EndChild();
	}
}
