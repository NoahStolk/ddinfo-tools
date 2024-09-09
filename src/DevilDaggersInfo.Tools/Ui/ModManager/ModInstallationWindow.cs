using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ModManager;

public static class ModInstallationWindow
{
	private static Dictionary<string, List<EffectiveAsset>> _effectiveAssets = new();
	private static int _activeAssets;
	private static int _activeProhibitedAssets;

	private static readonly List<string> _errors = [];

	public static void LoadEffectiveAssets()
	{
		_effectiveAssets.Clear();
		_errors.Clear();

		string[] filePaths = Directory.GetFiles(UserSettings.ModsDirectory);
		List<Mod> mods = [];
		foreach (string filePath in filePaths)
		{
			string fileName = Path.GetFileName(filePath);
			if (!fileName.StartsWith("audio") && !fileName.StartsWith("dd"))
				continue;

			try
			{
				using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
				using BinaryReader reader = new(fs);
				mods.Add(new Mod(ModBinaryToc.FromReader(reader), fileName));
			}
			catch (InvalidModBinaryException)
			{
				// Ignore.
			}
			catch (Exception ex)
			{
				_errors.Add($"Error loading file '{filePath}'.");
				Root.Log.Error(ex, $"Error loading file '{filePath}'.");
			}
		}

		List<EffectiveAsset> effectiveAssets = [];
		foreach (Mod mod in mods.OrderBy(t => t.FileName))
		{
			foreach (ModBinaryTocEntry tocEntry in mod.Toc.Entries)
			{
				List<EffectiveAsset> existingAssets = effectiveAssets.Where(c => c.TocEntry.AssetType == tocEntry.AssetType && c.TocEntry.Name == tocEntry.Name).ToList();
				foreach (EffectiveAsset existingAsset in existingAssets)
					existingAsset.OverriddenByModFileName = mod.FileName;

				effectiveAssets.Add(new EffectiveAsset(tocEntry, mod.FileName, null));
			}
		}

		foreach (EffectiveAsset effectiveAsset in effectiveAssets.OrderBy(c => c.TocEntry.AssetType).ThenBy(c => c.TocEntry.Name))
		{
			if (!_effectiveAssets.ContainsKey(effectiveAsset.ContainingModFileName))
				_effectiveAssets.Add(effectiveAsset.ContainingModFileName, []);

			_effectiveAssets[effectiveAsset.ContainingModFileName].Add(effectiveAsset);
		}

		_effectiveAssets = _effectiveAssets.OrderByDescending(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

		_activeAssets = _effectiveAssets.Sum(kvp => kvp.Value.Count(c => c.OverriddenByModFileName == null && c.TocEntry.IsEnabled));
		_activeProhibitedAssets = _effectiveAssets.Sum(kvp => kvp.Value.Count(c => c.OverriddenByModFileName == null && c.TocEntry.IsEnabled && AssetContainer.IsProhibited(c.TocEntry.AssetType, c.TocEntry.Name)));
	}

	public static void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(768, 384);
		if (ImGui.Begin("Mod Installation", ImGuiWindowFlags.NoCollapse))
		{
			if (ModsDirectoryLogic.IsLoading)
			{
				ImGui.Text("Loading...");
			}
			else
			{
				Title("Summary");

				if (ImGui.BeginTable("Mod installation summary", 2, ImGuiTableFlags.Borders, new Vector2(256, 0)))
				{
					ImGui.TableSetupColumn("##left", ImGuiTableColumnFlags.WidthStretch);
					ImGui.TableSetupColumn("##right", ImGuiTableColumnFlags.WidthFixed, 48);

					NextColumnText("Active mod files");
					NextColumnText(Inline.Span(_effectiveAssets.Count));

					NextColumnText("Active assets");
					NextColumnText(Inline.Span(_activeAssets));

					NextColumnText("Active prohibited assets");
					NextColumnText(Inline.Span(_activeProhibitedAssets));

					ImGui.EndTable();
				}

				ImGui.Spacing();
				ImGui.Spacing();
				Title("Effective Assets");

				ImGui.TextWrapped(
				"""
				It is possible to play multiple mods at the same time, since the game replaces every asset individually.

				When two mods contain the same asset, the last mod loaded (alphabetically) will be the effective one.

				For example, if you have two mods named "dd_blue_gem" and "dd_yellow_gem" installed, which both change the gem texture, "dd_yellow_gem" will be the effective one.

				In the table below, mods listed at the top take precedence over mods listed at the bottom.
				""");

				if (_errors.Count > 0)
				{
					ImGui.Separator();
					ImGui.TextColored(Color.Red, "Errors:");
					for (int i = 0; i < _errors.Count; i++)
					{
						string error = _errors[i];
						ImGui.Text(error);
					}
				}

				RenderEffectiveAssetsTable();
			}
		}

		ImGui.End();
	}

	private static void RenderEffectiveAssetsTable()
	{
		if (ImGui.BeginTable("EffectiveAssetsModsTable", 2, ImGuiTableFlags.Borders))
		{
			ImGui.TableSetupColumn("Mod file", ImGuiTableColumnFlags.WidthFixed, 192);
			ImGui.TableSetupColumn("Assets", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableHeadersRow();

			foreach (KeyValuePair<string, List<EffectiveAsset>> kvp in _effectiveAssets)
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
						ImGui.TextColored(isDisabled ? disabledColor : effectiveAsset.TocEntry.AssetType.GetColor(), EnumUtils.AssetTypeNames[effectiveAsset.TocEntry.AssetType]);

						ImGui.TableNextColumn();
						if (isOverridden)
							ImGui.TextColored(new Vector4(1, 0.2f, 0.4f, 1), Inline.Span($"Overridden by {effectiveAsset.OverriddenByModFileName}"));
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

	private static void Title(ReadOnlySpan<char> label)
	{
		ImGui.PushFont(Root.FontGoetheBold20);
		ImGui.Text(label);
		ImGui.PopFont();
	}

	private static void NextColumnText(ReadOnlySpan<char> label)
	{
		ImGui.TableNextColumn();
		ImGui.Text(label);
	}

	private sealed record Mod(ModBinaryToc Toc, string FileName);

	private sealed record EffectiveAsset(ModBinaryTocEntry TocEntry, string ContainingModFileName, string? OverriddenByModFileName)
	{
		public string? OverriddenByModFileName { get; set; } = OverriddenByModFileName;
	}
}
