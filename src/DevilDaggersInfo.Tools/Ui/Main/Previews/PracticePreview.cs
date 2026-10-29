using DevilDaggersInfo.Tools.Extensions;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Main.Previews;

internal sealed class PracticePreview : IToolPreview
{
	public ToolStatus Status => ToolStatus.FeatureComplete;

	public string Title => "Practice";

	public ColorConfiguration ColorConfiguration => Colors.Practice;

	public void RenderCompatibility()
	{
		ImGui.TextColored(new Vector4(0, 1, 0, 1), "Fully supported on all platforms");
	}

	public void Render()
	{
		ImGui.Text(
			"""
			Practice the main game by starting at any point in time with any hand upgrade and amount of gems/homing, using custom spawnsets that are automatically generated.

			Save templates to quickly load your desired practice settings.

			View live data from the current run:
			""");

		ImGui.BulletTextWrapped("Splits"u8);
		ImGui.BulletTextWrapped("Homing usage"u8);
		ImGui.BulletTextWrapped("Gem collection"u8);

		ImGui.Text(
			"""
			Note that using practice will not submit your score to the official leaderboards.

			Spawnsets can only be used to practice the main game and play custom levels. They cannot be used to cheat and are completely safe to use.
			""");
	}
}
