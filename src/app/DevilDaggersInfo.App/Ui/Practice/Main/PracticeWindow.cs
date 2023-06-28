using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Extensions;
using DevilDaggersInfo.App.Ui.Practice.Main.Templates;
using DevilDaggersInfo.App.User.Settings;
using DevilDaggersInfo.App.User.Settings.Model;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.Practice.Main;

public static class PracticeWindow
{
	private static int? _customTemplateIndexToReorder;

	public const int TemplateWidth = 360;
	public const int TemplateDescriptionHeight = 48;
	public static readonly Vector2 TemplateContainerSize = new(400, 480);
	public static readonly Vector2 TemplateListSize = new(380, 380);

	public static void Render()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Constants.MinWindowSize);
		ImGui.Begin("Practice", ImGuiWindowFlags.NoCollapse);
		ImGui.PopStyleVar();

		ImGui.Text("Use these templates to practice specific sections of the game. Click on a template to install it.");
		ImGui.Spacing();

		NoFarmTemplatesChild.Render();

		ImGui.SameLine();

		EndLoopTemplatesChild.Render();

		ImGui.SameLine();
		ImGui.BeginChild("Custom templates", TemplateContainerSize, true);
		ImGui.Text("Custom templates");

		ImGui.BeginChild("Custom template description", TemplateListSize with { Y = TemplateDescriptionHeight });
		ImGui.PushTextWrapPos(ImGui.GetCursorPos().X + TemplateWidth);
		ImGui.Text("You can make your own templates and save them. Your custom templates are saved locally on your computer. Right-click to rename a template.");
		ImGui.PopTextWrapPos();
		ImGui.EndChild();

		ImGui.BeginChild("Custom template list", TemplateListSize);

		RenderDragDropTarget(-1);
		for (int i = 0; i < UserSettings.Model.PracticeTemplates.Count; i++)
			RenderCustomTemplate(i, UserSettings.Model.PracticeTemplates[i]);

		if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
			_customTemplateIndexToReorder = null;

		ImGui.EndChild();
		ImGui.EndChild();

		ImGui.BeginChild("Input values", new(400, 192), true);

		ImGui.Spacing();
		ImGui.Image((IntPtr)Root.InternalResources.IconHandTexture.Handle, new(16), Vector2.Zero, Vector2.One, PracticeLogic.State.HandLevel.GetColor());
		ImGui.SameLine();
		foreach (HandLevel level in Enum.GetValues<HandLevel>())
		{
			if (ImGui.RadioButton($"Lvl {(int)level}", level == PracticeLogic.State.HandLevel) && PracticeLogic.State.HandLevel != level)
				PracticeLogic.State.HandLevel = level;

			if (level != HandLevel.Level4)
				ImGui.SameLine();
		}

		(Texture gemOrHomingTexture, Color tintColor) = PracticeLogic.State.HandLevel is HandLevel.Level3 or HandLevel.Level4 ? (Root.GameResources.IconMaskHomingTexture, Color.White) : (Root.GameResources.IconMaskGemTexture, Color.Red);
		ImGui.Spacing();
		ImGui.Image((IntPtr)gemOrHomingTexture.Handle, new(16), Vector2.UnitY, Vector2.UnitX, tintColor);
		ImGui.SameLine();
		ImGui.InputInt("Added gems", ref PracticeLogic.State.AdditionalGems, 1);

		ImGui.Spacing();
		ImGui.Image((IntPtr)Root.GameResources.IconMaskStopwatchTexture.Handle, new(16), Vector2.UnitY, Vector2.UnitX);
		ImGui.SameLine();
		ImGui.InputFloat("Timer start", ref PracticeLogic.State.TimerStart, 1, 5, "%.4f");

		for (int i = 0; i < 8; i++)
			ImGui.Spacing();

		if (ImGui.Button("Apply", new(80, 30)))
			PracticeLogic.Apply();

		ImGui.SameLine();
		if (ImGui.Button("Save", new(80, 30)))
		{
			UserSettingsModel.UserSettingsPracticeTemplate newTemplate = new(null, PracticeLogic.State.HandLevel, PracticeLogic.State.AdditionalGems, PracticeLogic.State.TimerStart);
			if (!UserSettings.Model.PracticeTemplates.Contains(newTemplate))
			{
				UserSettings.Model = UserSettings.Model with
				{
					PracticeTemplates = UserSettings.Model.PracticeTemplates
						.Append(newTemplate)
						.ToList(),
				};
			}
		}

		ImGui.EndChild();

		ImGui.SameLine();
		ImGui.BeginChild("Current spawnset", new(400, 192), true);

		ImGui.BeginChild("Current practice values", new(400, 64));
		if (SurvivalFileWatcher.Exists)
		{
			string timerStart = SurvivalFileWatcher.TimerStart.ToString(StringFormats.TimeFormat);

			if (ImGui.BeginChild("Current practice values left", new(160, 64)))
			{
				ImGui.TextColored(Color.Yellow, "Effective values");

				if (SurvivalFileWatcher.EffectivePlayerSettings.HandLevel != SurvivalFileWatcher.EffectivePlayerSettings.HandMesh)
					ImGui.Text($"{SurvivalFileWatcher.EffectivePlayerSettings.HandLevel} ({SurvivalFileWatcher.EffectivePlayerSettings.HandMesh} mesh)");
				else
					ImGui.Text(SurvivalFileWatcher.EffectivePlayerSettings.HandLevel.ToString());

				ImGui.Text(SurvivalFileWatcher.EffectivePlayerSettings.GemsOrHoming.ToString());
				ImGui.Text(timerStart);
			}

			ImGui.EndChild();

			ImGui.SameLine();

			if (ImGui.BeginChild("Current practice values right", new(160, 64)))
			{
				ImGui.TextColored(Color.Yellow, "Spawnset values");

				ImGui.Text(SurvivalFileWatcher.HandLevel.ToString());
				ImGui.Text(SurvivalFileWatcher.AdditionalGems.ToString());
				ImGui.Text(timerStart);
			}

			ImGui.EndChild();
		}
		else
		{
			ImGui.Text("<No spawnset enabled>");
		}

		ImGui.EndChild();

		ImGui.BeginDisabled(!SurvivalFileWatcher.Exists);
		if (ImGui.Button("Delete spawnset (restore default)", new(0, 30)))
			DeleteModdedSpawnset();

		ImGui.EndDisabled();

		ImGui.EndChild();

		ImGui.End();

		if (ImGui.IsKeyPressed(ImGuiKey.Escape) || ImGui.IsKeyPressed((ImGuiKey)526))
			UiRenderer.Layout = LayoutType.Main;
	}

	private static void RenderCustomTemplate(int i, UserSettingsModel.UserSettingsPracticeTemplate customTemplate)
	{
		CustomTemplateChild.Render(
			customTemplate: customTemplate,
			isActive: PracticeLogic.State.IsEqual(customTemplate),
			buttonSize: new(TemplateWidth - 96, 48),
			onClick: () =>
			{
				PracticeLogic.State = new(customTemplate.HandLevel, customTemplate.AdditionalGems, customTemplate.TimerStart);
				PracticeLogic.Apply();
			});

		ImGui.SameLine();
		Color gray = Color.Gray(0.3f);
		ImGui.PushStyleColor(ImGuiCol.Button, gray with { A = 159 });
		ImGui.PushStyleColor(ImGuiCol.ButtonActive, gray);
		ImGui.PushStyleColor(ImGuiCol.ButtonHovered, gray with { A = 223 });
		ImGui.PushID($"dd {customTemplate}");

		ImGui.ImageButton((IntPtr)Root.InternalResources.DragIndicatorTexture.Handle, new(32, 48), Vector2.Zero, Vector2.One, 0, _customTemplateIndexToReorder == i ? Color.Gray(0.7f) : gray);

		if (ImGui.IsItemHovered())
			ImGui.SetTooltip("Reorder");

		if (ImGui.BeginDragDropSource())
		{
			_customTemplateIndexToReorder = i;

			ImGui.SetDragDropPayload("CustomTemplateReorder", IntPtr.Zero, 0);
			string templateDragName = customTemplate.Name != null ? $"\"{customTemplate.Name}\"" : $"{customTemplate.HandLevel} (+{customTemplate.AdditionalGems}) {customTemplate.TimerStart.ToString(StringFormats.TimeFormat)}";
			ImGui.Text($"Reorder {templateDragName}");
			ImGui.EndDragDropSource();
		}

		ImGui.PopID();
		ImGui.PopStyleColor(3);

		ImGui.SameLine();
		ImGui.PushStyleColor(ImGuiCol.Button, Color.Red with { A = 159 });
		ImGui.PushStyleColor(ImGuiCol.ButtonActive, Color.Red);
		ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Color.Red with { A = 223 });
		ImGui.PushID(customTemplate.ToString());
		if (ImGui.ImageButton((IntPtr)Root.InternalResources.BinTexture.Handle, new(24), Vector2.Zero, Vector2.One, 12))
		{
			UserSettings.Model = UserSettings.Model with
			{
				PracticeTemplates = UserSettings.Model.PracticeTemplates.Where(pt => customTemplate != pt).ToList(),
			};
		}

		if (ImGui.IsItemHovered())
			ImGui.SetTooltip("Delete permanently");

		ImGui.PopID();
		ImGui.PopStyleColor(3);

		RenderDragDropTarget(i);
	}

	private static void RenderDragDropTarget(int i)
	{
		if (!_customTemplateIndexToReorder.HasValue)
			return;

		float relativeMouseY = ImGui.GetMousePos().Y - ImGui.GetWindowPos().Y + ImGui.GetScrollY();
		float currentY = ImGui.GetCursorPosY();
		const float dropAreaPaddingY = 28;
		if (relativeMouseY > currentY - dropAreaPaddingY && relativeMouseY < currentY + dropAreaPaddingY)
		{
			const float spacingY = 8;
			const float dropAreaHeight = 14;

			ImGui.SetCursorPosY(ImGui.GetCursorPosY() - spacingY);

			ImGui.PushStyleColor(ImGuiCol.Button, Color.Green with { A = 111 });
			ImGui.Button("##drop", new(TemplateWidth - 96, dropAreaHeight));
			ImGui.PopStyleColor();

			ImGui.SetCursorPosY(ImGui.GetCursorPosY() - dropAreaHeight + spacingY - 4);

			if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
			{
				UserSettingsModel.UserSettingsPracticeTemplate originalTemplate = UserSettings.Model.PracticeTemplates[_customTemplateIndexToReorder.Value];

				List<UserSettingsModel.UserSettingsPracticeTemplate> newPracticeTemplates = UserSettings.Model.PracticeTemplates
					.Where(pt => pt != originalTemplate)
					.ToList();

				if (i < _customTemplateIndexToReorder)
					newPracticeTemplates.Insert(i + 1, originalTemplate);
				else
					newPracticeTemplates.Insert(i, originalTemplate);

				UserSettings.Model = UserSettings.Model with
				{
					PracticeTemplates = newPracticeTemplates,
				};

				_customTemplateIndexToReorder = null;
			}
		}
	}

	public static (string Text, Color TextColor) GetGemsOrHomingText(HandLevel handLevel, int additionalGems)
	{
		EffectivePlayerSettings effectivePlayerSettings = SpawnsetBinary.GetEffectivePlayerSettings(PracticeLogic.SpawnVersion, handLevel, additionalGems);
		return effectivePlayerSettings.HandLevel switch
		{
			HandLevel.Level3 => ($"{effectivePlayerSettings.GemsOrHoming} homing", HandLevel.Level3.GetColor()),
			HandLevel.Level4 => ($"{effectivePlayerSettings.GemsOrHoming} homing", HandLevel.Level4.GetColor()),
			_ => ($"{effectivePlayerSettings.GemsOrHoming} gems", Color.Red),
		};
	}

	public static (byte BackgroundAlpha, byte TextAlpha) GetAlpha(bool isActive)
	{
		return isActive ? ((byte)48, (byte)255) : ((byte)16, (byte)191);
	}

	private static void DeleteModdedSpawnset()
	{
		if (File.Exists(UserSettings.ModsSurvivalPath))
			File.Delete(UserSettings.ModsSurvivalPath);
	}
}
