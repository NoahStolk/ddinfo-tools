using DevilDaggersInfo.Tools.Extensions;
using Hexa.NET.ImGui;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Main.Previews;

internal sealed class ToolPreviewChild(FontService fontService)
{
	public void Render(IToolPreview toolPreview, Vector2 size)
	{
		ImGui.PushStyleColor(ImGuiCol.ChildBg, toolPreview.ColorConfiguration.Primary with { W = 0.1f });
		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(8));

		if (ImGui.BeginChild("Preview", size, ImGuiChildFlags.AlwaysUseWindowPadding))
		{
			ImGui.PushTextWrapPos(size.X - 16);
			RenderContents(toolPreview);
			ImGui.PopTextWrapPos();
		}

		ImGui.EndChild();

		ImGui.PopStyleVar();
		ImGui.PopStyleColor();
	}

	private void RenderContents(IToolPreview toolPreview)
	{
		ImGui.Title(toolPreview.Title, fontService.GoetheBold30);

		if (ImGui.BeginTable("ToolInfoTable", 2, ImGuiTableFlags.BordersInner))
		{
			ImGui.TableSetupColumn("Left", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Right", ImGuiTableColumnFlags.WidthStretch, 128);

			ImGui.TableNextRow();

			(Vector4 statusColor, string statusText) = toolPreview.Status switch
			{
				ToolStatus.WorkInProgress => (new Vector4(1, 0.5f, 0, 1), "Work In Progress"),
				ToolStatus.FeatureComplete => (new Vector4(0, 1, 0, 1), "Feature Complete"),
				_ => throw new UnreachableException(),
			};

			ImGui.TableNextColumn();
			ImGui.Text("Status");
			ImGui.TableNextColumn();
			ImGui.TextColored(statusColor, statusText);

			ImGui.TableNextColumn();
			ImGui.Text("Compatibility");
			ImGui.TableNextColumn();
			toolPreview.RenderCompatibility();
		}

		ImGui.EndTable();

		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();

		toolPreview.Render();
	}
}
