using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorStates;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena;

public static class ArenaWindow
{
	public const int TileSize = 8;
	private const int _halfTileSize = TileSize / 2;
	public static readonly Vector2 HalfTileSizeAsVector2 = new(_halfTileSize);

	public static readonly Vector2 ArenaSize = new(TileSize * SpawnsetBinary.ArenaDimensionMax);

	private static readonly ArenaPencilState _pencilState = new();
	private static readonly ArenaLineState _lineState = new();
	private static readonly ArenaRectangleState _rectangleState = new();
	private static readonly ArenaEllipseState _ellipseState = new();
	private static readonly ArenaBucketState _bucketState = new();
	private static readonly ArenaDaggerState _daggerState = new();

	private static float _currentSecond;

	public static float CurrentSecond => _currentSecond;

	public static float SelectedHeight { get; set; }
	public static ArenaTool ArenaTool { get; set; }

	private static IArenaState GetActiveState()
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

	public static void Render()
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
					FileStates.Spawnset.Update(FileStates.Spawnset.Object with { ArenaTiles = new(FileStates.Spawnset.Object.ArenaDimension, newTiles) });
					SpawnsetHistoryUtils.Save(SpawnsetEditType.ArenaTileHeight);
				}

				IArenaState activeState = GetActiveState();

				Vector2 pos = ImGui.GetCursorScreenPos();
				bool isArenaHovered = ImGui.IsMouseHoveringRect(pos, pos + ArenaSize);
				if (isArenaHovered)
					ImGui.SetTooltip(Inline.Span($"{FileStates.Spawnset.Object.ArenaTiles[mousePosition.Tile.X, mousePosition.Tile.Y]}\n<{mousePosition.Tile.X}, {mousePosition.Tile.Y}>"));

				if (isArenaHovered && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
					activeState.InitializeSession(mousePosition);

				if (mousePosition.IsValid)
					activeState.Handle(mousePosition);
				else
					activeState.HandleOutOfRange(mousePosition);

				ArenaCanvas.Render();
				activeState.Render(mousePosition);

				// Capture mouse input when the mouse is over the canvas.
				// This prevents dragging the window while drawing on the arena canvas.
				ImGui.InvisibleButton("ArenaCanvas", ArenaSize, ImGuiButtonFlags.MouseButtonLeft);
			}

			ImGui.EndChild(); // End Arena

			ImGui.SliderFloat("Time", ref _currentSecond, 0, FileStates.Spawnset.Object.GetSliderMaxSeconds());

			SpawnsetEditor3DWindow.ArenaScene.CurrentTick = (int)MathF.Round(_currentSecond * 60);

			ArenaEditorControls.Render();
			ArenaHeightButtons.Render();
		}

		ImGui.EndChild(); // End ArenaWindow
	}
}
