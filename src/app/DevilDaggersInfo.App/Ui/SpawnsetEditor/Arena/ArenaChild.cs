using DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorStates;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena;

public static class ArenaChild
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

	private static IArenaState GetActiveState() => ArenaTool switch
	{
		 ArenaTool.Pencil => _pencilState,
		 ArenaTool.Line => _lineState,
		 ArenaTool.Rectangle => _rectangleState,
		 ArenaTool.Ellipse => _ellipseState,
		 ArenaTool.Bucket => _bucketState,
		 ArenaTool.Dagger => _daggerState,
		 _ => throw new UnreachableException(),
	};

	public static void Render(bool isSpawnsetEditorWindowFocused, bool isSpawnsetEditorWindowFocusedFirstFrame)
	{
		if (ImGui.BeginChild("ArenaChild", new(416 - 8, 768 - 64)))
		{
			if (ImGui.BeginChild("Arena", ArenaSize))
			{
				ImGuiIOPtr io = ImGui.GetIO();

				ArenaMousePosition mousePosition = ArenaMousePosition.Get(io, ImGui.GetCursorScreenPos());

				if (mousePosition.IsValid && isSpawnsetEditorWindowFocused)
				{
					ImGui.SetTooltip(UnsafeSpan.Get($"{SpawnsetState.Spawnset.ArenaTiles[mousePosition.Tile.X, mousePosition.Tile.Y]}\n<{mousePosition.Tile.X}, {mousePosition.Tile.Y}>"));

					if (io.MouseWheel != 0)
					{
						float[,] newTiles = SpawnsetState.Spawnset.ArenaTiles.GetMutableClone();
						newTiles[mousePosition.Tile.X, mousePosition.Tile.Y] -= io.MouseWheel;
						SpawnsetState.Spawnset = SpawnsetState.Spawnset with { ArenaTiles = new(SpawnsetState.Spawnset.ArenaDimension, newTiles) };
						SpawnsetHistoryUtils.Save(SpawnsetEditType.ArenaTileHeight);
					}
				}

				IArenaState activeState = GetActiveState();

				Vector2 pos = ImGui.GetCursorScreenPos();
				bool isArenaHovered = ImGui.IsMouseHoveringRect(pos, pos + ArenaSize);

				if (isSpawnsetEditorWindowFocusedFirstFrame && ImGui.IsMouseDown(ImGuiMouseButton.Left) ||
					isSpawnsetEditorWindowFocused && isArenaHovered && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
				{
					activeState.InitializeSession(mousePosition);
				}

				if (isSpawnsetEditorWindowFocused)
				{
					if (mousePosition.IsValid)
						activeState.Handle(mousePosition);
					else
						activeState.HandleOutOfRange(mousePosition);
				}

				ArenaCanvas.Render();
				activeState.Render(mousePosition);

				// Capture mouse input when the mouse is over the canvas.
				// This prevents dragging the window while drawing on the arena canvas.
				ImGui.InvisibleButton("ArenaCanvas", ArenaSize, ImGuiButtonFlags.MouseButtonLeft);
			}

			ImGui.EndChild(); // End Arena

			ImGui.SliderFloat("Time", ref _currentSecond, 0, SpawnsetState.Spawnset.GetSliderMaxSeconds());

			SpawnsetEditor3DWindow.ArenaScene.CurrentTick = (int)MathF.Round(_currentSecond * 60);

			ArenaEditorControls.Render();
			ArenaHeightButtons.Render();
		}

		ImGui.EndChild(); // End ArenaChild
	}
}
