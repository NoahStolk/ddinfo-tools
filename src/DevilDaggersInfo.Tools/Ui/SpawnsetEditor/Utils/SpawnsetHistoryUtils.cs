using DevilDaggersInfo.Tools.EditorFileState;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;

public static class SpawnsetHistoryUtils
{
	public static void Save(SpawnsetEditType editType)
	{
		FileStates.Spawnset.Save(editType);

		HistoryChild.UpdateScroll = true;
	}
}
