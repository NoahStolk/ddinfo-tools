using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorStates;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena;

internal sealed class ArenaStateManager
{
	private readonly ArenaPencilState _pencilState;
	private readonly ArenaLineState _lineState;
	private readonly ArenaRectangleState _rectangleState;
	private readonly ArenaEllipseState _ellipseState;
	private readonly ArenaBucketState _bucketState;
	private readonly ArenaDaggerState _daggerState;

	public ArenaStateManager(FileStates fileStates, SpawnsetSaver spawnsetSaver, ResourceManager resourceManager)
	{
		_pencilState = new ArenaPencilState(this, fileStates, spawnsetSaver);
		_lineState = new ArenaLineState(this, fileStates, spawnsetSaver);
		_rectangleState = new ArenaRectangleState(this, fileStates, spawnsetSaver);
		_ellipseState = new ArenaEllipseState(this, fileStates, spawnsetSaver);
		_bucketState = new ArenaBucketState(this, fileStates, spawnsetSaver);
		_daggerState = new ArenaDaggerState(resourceManager, fileStates, spawnsetSaver);
	}

	public float SelectedHeight { get; set; }
	public ArenaTool ArenaTool { get; set; }

	public IArenaState GetActiveState()
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
}
