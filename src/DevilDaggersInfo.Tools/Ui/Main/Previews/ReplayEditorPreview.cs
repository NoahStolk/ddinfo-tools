using DevilDaggersInfo.Tools.Extensions;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Main.Previews;

internal sealed class ReplayEditorPreview : IToolPreview
{
	public ToolStatus Status => ToolStatus.WorkInProgress;

	public string Title => "Replay Editor";

	public ColorConfiguration ColorConfiguration => Colors.ReplayEditor;

	public void RenderCompatibility()
	{
		ImGui.TextColored(new Vector4(0, 1, 0, 1), "Fully supported on all platforms");
	}

	public void Render()
	{
		ImGui.Text(
			"""
			Create, analyze, and edit replays for Devil Daggers.

			You can download replays from the official leaderboards and save them as a local replay.

			This tool will likely not be useful for most players; it is mostly intended to figure out how some things in the game work.

			It could be used to:
			""");

		ImGui.BulletTextWrapped("Figure out how homing daggers, gems, or certain enemies behave under certain conditions, since their behavior is implicit and cannot reliably be modified using replays."u8);
		ImGui.BulletTextWrapped("Figure out how certain movement techniques work in more detail (for optimizing race spawnsets)."u8);
		ImGui.BulletTextWrapped("Detect cheated replays (for example, shotgun tech intervals could be analyzed to detect if a player is using a macro)."u8);
	}
}
