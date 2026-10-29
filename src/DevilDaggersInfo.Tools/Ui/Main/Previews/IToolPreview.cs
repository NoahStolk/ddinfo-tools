namespace DevilDaggersInfo.Tools.Ui.Main.Previews;

internal interface IToolPreview
{
	ToolStatus Status { get; }

	string Title { get; }

	ColorConfiguration ColorConfiguration { get; }

	void RenderCompatibility();

	void Render();
}
