using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorChildren;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena;

// TODO: Register this.
// TODO: Don't inject other windows.
internal sealed class ArenaEditorControls(ResourceManager resourceManager, ArenaWindow arenaWindow, FileStates fileStates)
{

	public void Render()
	{
		if (ImGui.BeginChild("ArenaEditorControls", new Vector2(256, 26)))
		{
			const int borderSize = 2;
			const int size = 16;
			int offsetX = 0;
			for (int i = 0; i < EnumUtils.ArenaTools.Count; i++)
			{
				ArenaTool arenaTool = EnumUtils.ArenaTools[i];
				ReadOnlySpan<char> arenaToolText = EnumUtils.ArenaToolNames[arenaTool];

				bool isDagger = arenaTool == ArenaTool.Dagger;
				bool isCurrent = arenaTool == arenaWindow.ArenaTool;
				ImGui.SetCursorPos(new Vector2(offsetX + borderSize * 2, borderSize));

				if (isDagger)
					ImGui.BeginDisabled(fileStates.Spawnset.Object.GameMode != GameMode.Race);

				if (isCurrent)
					ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetStyle().Colors[(int)ImGuiCol.ButtonHovered]);

				if (ImGuiImage.ImageButton(arenaToolText, GetTexture(arenaTool), new Vector2(size)) && arenaWindow.ArenaTool != arenaTool)
					arenaWindow.ArenaTool = arenaTool;

				if (isCurrent)
					ImGui.PopStyleColor();

				if (ImGui.IsItemHovered())
					ImGui.SetTooltip(arenaToolText);

				if (isDagger)
					ImGui.EndDisabled();

				offsetX += 28;
			}
		}

		ImGui.EndChild();

		if (ImGui.BeginChild("ArenaToolControls", new Vector2(256, 112)))
		{
			switch (arenaWindow.ArenaTool)
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
					DaggerChild.Render(fileStates.Spawnset.Object.GameMode);
					break;
			}
		}

		ImGui.EndChild();
	}

	private uint GetTexture(ArenaTool arenaTool)
	{
		return arenaTool switch
		{
			ArenaTool.Pencil => resourceManager.InternalResources.PencilTexture.Id,
			ArenaTool.Line => resourceManager.InternalResources.LineTexture.Id,
			ArenaTool.Rectangle => resourceManager.InternalResources.RectangleTexture.Id,
			ArenaTool.Ellipse => resourceManager.InternalResources.EllipseTexture.Id,
			ArenaTool.Bucket => resourceManager.InternalResources.BucketTexture.Id,
			ArenaTool.Dagger => resourceManager.InternalResources.DaggerTexture.Id,
			_ => throw new UnreachableException($"Unknown arena tool {arenaTool}."),
		};
	}
}
