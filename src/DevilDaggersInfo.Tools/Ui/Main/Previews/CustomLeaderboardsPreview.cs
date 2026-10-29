using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Main.Previews;

internal sealed class CustomLeaderboardsPreview : IToolPreview
{
	public ToolStatus Status => ToolStatus.FeatureComplete;

	public string Title => "Custom Leaderboards";

	public ColorConfiguration ColorConfiguration => Colors.CustomLeaderboards;

	public void RenderCompatibility()
	{
		ImGui.TextColored(new Vector4(0, 1, 0, 1), "Fully supported on Windows");
		ImGui.TextColored(new Vector4(0, 1, 0, 1), "Fully supported on Proton (Linux)");
		ImGui.TextColored(new Vector4(1, 1, 0, 1), "Partially supported on native Linux");
		ImGui.SameLine();
		ImGui.TextColored(new Vector4(0.75f, 0.75f, 0.75f, 1), "(?)");
		if (ImGui.IsItemHovered())
		{
			ImGui.SetTooltip(
				"""
				Most leaderboards will work, but the Time Attack and Race game modes can't be made to work.

				This is because the game's native Linux build isn't up-to-date and doesn't expose the necessary memory.

				The Proton build is therefore recommended.
				""");
		}
	}

	public void Render()
	{
		ImGui.Text(
			"""
			Custom leaderboards are leaderboards for custom spawnsets.

			All game modes are supported:
			""");
		ImGui.BulletText("Survival");
		ImGui.BulletText("Time Attack");
		ImGui.BulletText("Race");

		ImGui.Text("Leaderboards can be sorted by:");

		ImGui.BulletText("Time");
		ImGui.BulletText("Gems collected");
		ImGui.BulletText("Gems despawned");
		ImGui.BulletText("Gems eaten");
		ImGui.BulletText("Enemies killed");
		ImGui.BulletText("Enemies alive");
		ImGui.BulletText("Homing stored");
		ImGui.BulletText("Homing eaten");

		ImGui.Text(
			"""
			Criteria can also be set for custom leaderboards, meaning that in order to submit a score, it has to meet all the criteria.

			This applies to almost every stat in the game, including specific enemy kill counts.

			Examples:
			""");

		ImGui.BulletText("Gems eaten must be less than 30");
		ImGui.BulletText("Squid I kills must be equal to 0");
		ImGui.BulletText("Skull II kills must be greater than or equal to 3");
		ImGui.BulletText("Daggers fired must be less than gems collected + 2");
		ImGui.BulletText("Skull III kills must be greater than Skull I kills + Skull II kills");
	}
}
