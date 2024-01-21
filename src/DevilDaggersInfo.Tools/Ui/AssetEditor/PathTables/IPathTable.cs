namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

public interface IPathTable<T>
	where T : IPathTable<T>
{
	static abstract int ColumnCount { get; }

	static abstract int PathCount { get; }

	static abstract void SetupColumns();

	static abstract void RenderPath(int index);

	static abstract void Sort(uint sorting, bool sortAscending);
}
