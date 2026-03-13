namespace DevilDaggersInfo.Tools.EditorFileState;

internal record HistoryEntry<TObject, TEditType>(TObject Object, byte[] Hash, TEditType EditType)
	where TObject : class
	where TEditType : Enum;
