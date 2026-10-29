using DevilDaggersInfo.Tools.Extensions;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Main.Previews;

internal sealed class SpawnsetEditorPreview : IToolPreview
{
	public ToolStatus Status => ToolStatus.WorkInProgress;

	public string Title => "Spawnset Editor";

	public ColorConfiguration ColorConfiguration => Colors.SpawnsetEditor;

	public void RenderCompatibility()
	{
		ImGui.TextColored(new Vector4(0, 1, 0, 1), "Fully supported on all platforms");
	}

	public void Render()
	{
		ImGui.Text(
			"""
			Create and edit custom spawnsets (levels) for Devil Daggers.

			Some things you can do:
			""");

		ImGui.BulletTextWrapped("Create your own set of enemy spawns."u8);
		ImGui.BulletTextWrapped("Create a custom arena."u8);
		ImGui.BulletTextWrapped("Start with any hand upgrade."u8);
		ImGui.BulletTextWrapped("Give yourself 10,000 homing daggers."u8);
		ImGui.BulletTextWrapped("Use the Time Attack game mode, where the goal is to kill all enemies as fast as possible."u8);

		ImGui.Text(
			"""
			Be sure to check out the custom leaderboards to see what's possible.

			Note that using custom spawnsets will not submit your score to the official leaderboards.

			Spawnsets can only be used to practice the main game and play custom levels. They cannot be used to cheat and are completely safe to use.
			""");
	}
}
