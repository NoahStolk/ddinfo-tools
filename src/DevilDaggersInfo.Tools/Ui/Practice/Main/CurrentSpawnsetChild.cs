using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main;

internal sealed class CurrentSpawnsetChild(FontService fontService, SurvivalFileWatcher survivalFileWatcher)
{
	public void Render()
	{
		if (ImGui.BeginChild("CurrentSpawnset", new Vector2(0, 200), ImGuiChildFlags.Borders))
		{
			ImGui.SeparatorText("Current practice configuration");
			ImGui.Spacing();

			if (survivalFileWatcher.Exists)
			{
				ImGui.PushFont(fontService.GoetheBold20);
				ImGui.Text("Practice is enabled!");
				ImGui.PopFont();
				ImGui.Spacing();
				ImGui.Text("Press the restart button in the game to start practicing.");
				ImGui.Spacing();
				ImGui.Text("The current practice spawnset is enabled:");
				ImGui.Spacing();

				RenderCurrentSpawnsetTable();
			}
			else
			{
				ImGui.PushFont(fontService.GoetheBold20);
				ImGui.Text("Practice is disabled.");
				ImGui.PopFont();
				ImGui.Text("Click on a template to enable practice, then press the restart button in the game.");
			}
		}

		ImGui.EndChild();
	}

	private void RenderCurrentSpawnsetTable()
	{
		if (!ImGui.BeginTable("CurrentSpawnsetTable", 2, ImGuiTableFlags.Borders, new Vector2(320, 0)))
			return;

		EffectivePlayerSettings settings = survivalFileWatcher.EffectivePlayerSettings;

		ImGui.TableNextColumn();
		ImGui.Text("Hand");

		ImGui.TableNextColumn();
		Vector4 handColor = settings.HandLevel.GetColor();
		ImGui.TextColored(handColor, FormatHandText(settings));

		ImGui.TableNextColumn();
		ImGui.Text("Gems/Homing");

		ImGui.TableNextColumn();
		if (settings.HandLevel is HandLevel.Level3 or HandLevel.Level4)
			ImGui.TextColored(handColor, Inline.Utf8($"{settings.GemsOrHoming} homing"));
		else
			ImGui.TextColored(new Vector4(1, 0, 0, 1), Inline.Utf8($"{settings.GemsOrHoming} gems"));

		ImGui.TableNextColumn();
		ImGui.Text("Starts at");

		ImGui.TableNextColumn();
		ImGui.Text(Inline.Utf8(survivalFileWatcher.TimerStart, StringFormats.TimeFormat));

		ImGui.EndTable();
	}

	private static ReadOnlySpan<byte> FormatHandText(EffectivePlayerSettings settings)
	{
		return settings.HandLevel != settings.HandMesh
			? Inline.Utf8($"{settings.HandLevel.AsUtf8Span()} ({settings.HandMesh.AsUtf8Span()} hand mesh)")
			: settings.HandLevel.AsUtf8Span();
	}
}
