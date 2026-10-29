using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Main.Previews;

internal sealed class ModManagerPreview : IToolPreview
{
	public ToolStatus Status => ToolStatus.WorkInProgress;

	public string Title => "Mod Manager";

	public ColorConfiguration ColorConfiguration => Colors.ModManager;

	public void RenderCompatibility()
	{
		ImGui.TextColored(new Vector4(0, 1, 0, 1), "Fully supported on all platforms");
	}

	public void Render()
	{
		ImGui.Text(
			"""
			View all currently installed mods.

			Features:
			""");

		ImGui.BulletText("Enable/disable mods and change their load order to further customize the game.");
		ImGui.BulletText("Browse and find new mods to install directly from the devildaggers.info website.");
		ImGui.BulletText("View which assets are contained in each mod, and which ones are prohibited for 1000+ scores.");
		ImGui.BulletText("Enable/disable prohibited assets for each mod.");
		ImGui.BulletText("View all effective game assets and their source mod, including whether they're being overridden by another mod.");
	}
}
