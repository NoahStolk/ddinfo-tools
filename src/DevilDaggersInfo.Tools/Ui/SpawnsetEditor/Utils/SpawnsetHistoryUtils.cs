using DevilDaggersInfo.Tools.EditorFileState;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;

internal static class SpawnsetHistoryUtils
{
	public static void Save(SpawnsetEditType editType)
	{
		FileStates.Spawnset.Save(editType);

		HistoryWindow.UpdateScroll = true;
	}
}
