namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

internal interface IPathTable
{
	int ColumnCount { get; }

	int PathCount { get; }

	void SetupColumns();

	void RenderPath(int index);

	void Sort(uint sorting, bool sortAscending);
}
