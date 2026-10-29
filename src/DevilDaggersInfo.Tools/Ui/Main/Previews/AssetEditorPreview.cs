using DevilDaggersInfo.Tools.Extensions;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Main.Previews;

internal sealed class AssetEditorPreview : IToolPreview
{
	public ToolStatus Status => ToolStatus.WorkInProgress;

	public string Title => "Asset Editor";

	public ColorConfiguration ColorConfiguration => Colors.AssetEditor;

	public void RenderCompatibility()
	{
		ImGui.TextColored(new Vector4(0, 1, 0, 1), "Fully supported on all platforms");
	}

	public void Render()
	{
		ImGui.Text(
			"""
			Create and edit mods for Devil Daggers.

			The following assets can be changed:
			""");

		ImGui.BulletTextWrapped("Audio"u8);
		ImGui.BulletTextWrapped("Meshes"u8);
		ImGui.BulletTextWrapped("Object bindings"u8);
		ImGui.BulletTextWrapped("Shaders"u8);
		ImGui.BulletTextWrapped("Textures"u8);

		ImGui.TextColored(new Vector4(1, 1, 0, 1), "Some mods are prohibited, meaning that you cannot submit scores over 1000 seconds with them.");
	}
}
