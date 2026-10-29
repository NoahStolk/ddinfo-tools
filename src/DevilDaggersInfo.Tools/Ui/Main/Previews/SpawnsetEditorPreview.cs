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

		ImGui.BulletText("Create your own set of enemy spawns.");
		ImGui.BulletText("Create a custom arena.");
		ImGui.BulletText("Start with any hand upgrade.");
		ImGui.BulletText("Give yourself 10,000 homing daggers.");
		ImGui.BulletText("Use the Time Attack game mode, where the goal is to kill all enemies as fast as possible.");

		ImGui.Text(
			"""
			Be sure to check out the custom leaderboards to see what's possible.

			Note that using custom spawnsets will not submit your score to the official leaderboards.

			Spawnsets can only be used to practice the main game and play custom levels. They cannot be used to cheat and are completely safe to use.
			""");
	}
}
