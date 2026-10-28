using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Popups;

internal abstract class Popup(string id, Vector2 minSize)
{
	public string Id { get; } = $"{id}##{Guid.NewGuid()}";

	public bool HasOpened { get; set; }

	public Vector2 MinSize { get; } = minSize;

	/// <summary>
	/// Renders the popup and returns whether the popup should be closed.
	/// </summary>
	/// <returns>Whether the popup should be closed.</returns>
	public abstract bool Render();
}
