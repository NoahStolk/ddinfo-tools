using DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorChildren;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.App.Utils;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena;

public static class ArenaEditorControls
{
	public static void Render()
	{
		ImGui.BeginChild("ArenaEditorControls", new(256, 26));

		const int borderSize = 2;
		const int size = 16;
		int offsetX = 0;
		for (int i = 0; i < EnumUtils.ArenaTools.Count; i++)
		{
			ArenaTool arenaTool = EnumUtils.ArenaTools[i];
			ReadOnlySpan<char> arenaToolText = EnumUtils.ArenaToolNames[arenaTool];

			bool isDagger = arenaTool == ArenaTool.Dagger;
			bool isCurrent = arenaTool == ArenaChild.ArenaTool;
			ImGui.SetCursorPos(new(offsetX + borderSize * 2, borderSize));

			if (isDagger)
				ImGui.BeginDisabled(SpawnsetState.Spawnset.GameMode != GameMode.Race);

			if (isCurrent)
				ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetStyle().Colors[(int)ImGuiCol.ButtonHovered]);

			if (ImGuiImage.ImageButton(arenaToolText, GetTexture(arenaTool), new(size)) && ArenaChild.ArenaTool != arenaTool)
				ArenaChild.ArenaTool = arenaTool;

			if (isCurrent)
				ImGui.PopStyleColor();

			if (ImGui.IsItemHovered())
				ImGui.SetTooltip(arenaToolText);

			if (isDagger)
				ImGui.EndDisabled();

			offsetX += 28;
		}

		ImGui.EndChild();

		ImGui.BeginChild("ArenaToolControls", new(256, 112));

		switch (ArenaChild.ArenaTool)
		{
			case ArenaTool.Pencil:
				PencilChild.Render();
				break;
			case ArenaTool.Line:
				LineChild.Render();
				break;
			case ArenaTool.Rectangle:
				RectangleChild.Render();
				break;
			case ArenaTool.Ellipse:
				EllipseChild.Render();
				break;
			case ArenaTool.Bucket:
				BucketChild.Render();
				break;
			case ArenaTool.Dagger:
				DaggerChild.Render();
				break;
		}

		ImGui.EndChild();
	}

	private static uint GetTexture(ArenaTool arenaTool)
	{
		return arenaTool switch
		{
			ArenaTool.Pencil => Root.InternalResources.PencilTexture.Handle,
			ArenaTool.Line => Root.InternalResources.LineTexture.Handle,
			ArenaTool.Rectangle => Root.InternalResources.RectangleTexture.Handle,
			ArenaTool.Ellipse => Root.InternalResources.EllipseTexture.Handle,
			ArenaTool.Bucket => Root.InternalResources.BucketTexture.Handle,
			ArenaTool.Dagger => Root.InternalResources.DaggerTexture.Handle,
			_ => throw new($"Unknown arena tool {arenaTool}."),
		};
	}
}
