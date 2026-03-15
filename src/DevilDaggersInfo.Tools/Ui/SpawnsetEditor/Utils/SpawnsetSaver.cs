using DevilDaggersInfo.Tools.EditorFileState;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;

internal sealed class SpawnsetSaver(FileStates fileStates)
{
	public void Save(SpawnsetEditType editType)
	{
		fileStates.Spawnset.Save(editType);

		HistoryWindow.UpdateScroll = true;
	}
}
