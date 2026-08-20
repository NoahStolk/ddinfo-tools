using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;
using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory.Data;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ModManager;

internal sealed class ModInstallationWindow(ModsDirectoryLogic modsDirectoryLogic, FontService fontService)
{
	public void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(768, 384);
		if (ImGui.Begin("Mod Installation", ImGuiWindowFlags.NoCollapse))
		{
			if (modsDirectoryLogic.IsLoading)
			{
				ImGui.Text("Loading...");
			}
			else
			{
				Title("Summary"u8);

				if (ImGui.BeginTable("Mod installation summary", 2, ImGuiTableFlags.Borders, new Vector2(256, 0)))
				{
					ImGui.TableSetupColumn("##left", ImGuiTableColumnFlags.WidthStretch);
					ImGui.TableSetupColumn("##right", ImGuiTableColumnFlags.WidthFixed, 48);

					NextColumnText("Active mod files"u8);
					NextColumnText(Inline.Utf8(modsDirectoryLogic.EffectiveAssets.Count));

					NextColumnText("Active assets"u8);
					NextColumnText(Inline.Utf8(modsDirectoryLogic.ActiveAssets));

					NextColumnText("Active prohibited assets"u8);
					NextColumnText(Inline.Utf8(modsDirectoryLogic.ActiveProhibitedAssets));

					ImGui.EndTable();
				}

				ImGui.Spacing();
				ImGui.Spacing();
				Title("Effective Assets"u8);

				ImGui.TextWrapped(
				"""
				It is possible to play multiple mods at the same time, since the game replaces every asset individually.

				When two mods contain the same asset, the last mod loaded (alphabetically) will be the effective one.

				For example, if you have two mods named "dd_blue_gem" and "dd_yellow_gem" installed, which both change the gem texture, "dd_yellow_gem" will be the effective one.

				In the table below, mods listed at the top take precedence over mods listed at the bottom.
				""");

				if (modsDirectoryLogic.Errors.Count > 0)
				{
					ImGui.Separator();
					ImGui.TextColored(Color.Red, "Errors:");
					for (int i = 0; i < modsDirectoryLogic.Errors.Count; i++)
					{
						string error = modsDirectoryLogic.Errors[i];
						ImGui.Text(error);
					}
				}

				RenderEffectiveAssetsTable();
			}
		}

		ImGui.End();
	}

	private void RenderEffectiveAssetsTable()
	{
		if (ImGui.BeginTable("EffectiveAssetsModsTable", 2, ImGuiTableFlags.Borders))
		{
			ImGui.TableSetupColumn("Mod file", ImGuiTableColumnFlags.WidthFixed, 192);
			ImGui.TableSetupColumn("Assets", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableHeadersRow();

			foreach (KeyValuePair<string, List<EffectiveAsset>> kvp in modsDirectoryLogic.EffectiveAssets)
			{
				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text(kvp.Key);

				ImGui.TableNextColumn();
				if (ImGui.BeginTable("TocEntriesTable", 3, ImGuiTableFlags.None))
				{
					ImGui.TableSetupColumn("Asset name", ImGuiTableColumnFlags.WidthFixed, 128);
					ImGui.TableSetupColumn("Asset type", ImGuiTableColumnFlags.WidthFixed, 96);
					ImGui.TableSetupColumn("Status", ImGuiTableColumnFlags.WidthStretch);

					for (int i = 0; i < kvp.Value.Count; i++)
					{
						ImGui.PushStyleColor(ImGuiCol.Border, i % 2 == 0 ? Color.Invisible : Color.White);

						ImGui.TableNextRow();

						EffectiveAsset effectiveAsset = kvp.Value[i];
						bool isOverridden = effectiveAsset.OverriddenByModFileName != null;
						bool isDisabled = isOverridden || !effectiveAsset.TocEntry.IsEnabled;
						Vector4 disabledColor = Color.Gray(0.4f);

						ImGui.TableNextColumn();
						ImGui.TextColored(isDisabled ? disabledColor : Color.White, effectiveAsset.TocEntry.Name);

						ImGui.TableNextColumn();
						ImGui.TextColored(isDisabled ? disabledColor : effectiveAsset.TocEntry.AssetType.GetColor(), effectiveAsset.TocEntry.AssetType.AsUtf8Span());

						ImGui.TableNextColumn();
						if (isOverridden)
							ImGui.TextColored(new Vector4(1, 0.2f, 0.4f, 1), Inline.Utf8($"Overridden by {effectiveAsset.OverriddenByModFileName}"));
						else if (AssetContainer.IsProhibited(effectiveAsset.TocEntry.AssetType, effectiveAsset.TocEntry.Name))
							ImGui.TextColored(Color.Orange, "Prohibited");
						else if (!effectiveAsset.TocEntry.IsEnabled)
							ImGui.TextColored(Color.Gray(0.4f), "Disabled");
						else
							ImGui.TextColored(Color.Green, "OK");

						ImGui.PopStyleColor();
					}
				}

				ImGui.EndTable();
			}

			ImGui.EndTable();
		}
	}

	private void Title(ReadOnlySpan<byte> label)
	{
		ImGuiExt.Title(label, fontService.GoetheBold20);
	}

	private static void NextColumnText(ReadOnlySpan<byte> label)
	{
		ImGui.TableNextColumn();
		ImGui.Text(label);
	}
}
