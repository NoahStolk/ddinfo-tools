using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Extensions;
using DevilDaggersInfo.App.User.Settings;
using DevilDaggersInfo.App.User.Settings.Model;
using DevilDaggersInfo.Common;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.Practice.Main;

public static class CustomTemplatesChild
{
	private static int? _customTemplateIndexToReorder;

	public static void Render()
	{
		ImGui.BeginChild("Custom templates", PracticeWindow.TemplateContainerSize, true);
		ImGui.Text("Custom templates");

		ImGui.BeginChild("Custom template description", PracticeWindow.TemplateListSize with { Y = PracticeWindow.TemplateDescriptionHeight });
		ImGui.PushTextWrapPos(ImGui.GetCursorPos().X + PracticeWindow.TemplateWidth);
		ImGui.Text("You can make your own templates and save them. Your custom templates are saved locally on your computer. Right-click to rename a template.");
		ImGui.PopTextWrapPos();
		ImGui.EndChild();

		ImGui.BeginChild("Custom template list", PracticeWindow.TemplateListSize);

		RenderDragDropTarget(-1);
		for (int i = 0; i < UserSettings.Model.PracticeTemplates.Count; i++)
			RenderCustomTemplate(i, UserSettings.Model.PracticeTemplates[i]);

		if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
			_customTemplateIndexToReorder = null;

		ImGui.EndChild();
		ImGui.EndChild();
	}

	private static void RenderCustomTemplate(int i, UserSettingsModel.UserSettingsPracticeTemplate customTemplate)
	{
		RenderTemplateButton(
			customTemplate: customTemplate,
			isActive: PracticeLogic.State.IsEqual(customTemplate),
			buttonSize: new(PracticeWindow.TemplateWidth - 96, 48),
			onClick: () =>
			{
				PracticeLogic.State = new(customTemplate.HandLevel, customTemplate.AdditionalGems, customTemplate.TimerStart);
				PracticeLogic.GenerateAndApplyPracticeSpawnset();
			});

		ImGui.SameLine();

		RenderDragIndicator(i, customTemplate);

		ImGui.SameLine();

		RenderDeleteButton(customTemplate);

		RenderDragDropTarget(i);
	}

	private static void RenderTemplateButton(
		UserSettingsModel.UserSettingsPracticeTemplate customTemplate,
		bool isActive,
		Vector2 buttonSize,
		Action onClick)
	{
		string buttonName = $"{customTemplate.HandLevel}-{customTemplate.AdditionalGems}-{customTemplate.TimerStart.ToString(StringFormats.TimeFormat)}";

		Color color = Color.White;

		(string gemsOrHomingText, Color gemColor) = PracticeWindow.GetGemsOrHomingText(customTemplate.HandLevel, customTemplate.AdditionalGems);
		(byte backgroundAlpha, byte textAlpha) = PracticeWindow.GetAlpha(isActive);

		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
		if (ImGui.BeginChild(buttonName, buttonSize, true))
		{
			bool hover = ImGui.IsWindowHovered();
			ImGui.PushStyleColor(ImGuiCol.ChildBg, color with { A = (byte)(hover ? backgroundAlpha + 16 : backgroundAlpha) });

			if (ImGui.BeginChild(buttonName + " child", buttonSize, false, ImGuiWindowFlags.NoInputs))
			{
				if (hover && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
					onClick.Invoke();

				float windowWidth = ImGui.GetWindowWidth();

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 8));

				ImGui.TextColored(color with { A = textAlpha }, string.IsNullOrWhiteSpace(customTemplate.Name) ? "<untitled>" : customTemplate.Name);

				string timerStartString = customTemplate.TimerStart.ToString(StringFormats.TimeFormat);
				ImGui.SameLine(windowWidth - ImGui.CalcTextSize(timerStartString).X - 8);
				ImGui.TextColored(Color.White with { A = textAlpha }, timerStartString);

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 0));

				ImGui.TextColored(customTemplate.HandLevel.GetColor() with { A = textAlpha }, customTemplate.HandLevel.ToString());
				ImGui.SameLine(windowWidth - ImGui.CalcTextSize(gemsOrHomingText).X - 8);
				ImGui.TextColored(gemColor with { A = textAlpha }, gemsOrHomingText);
			}

			ImGui.EndChild();

			ImGui.PopStyleColor();
		}

		ImGui.PopStyleVar();

		ImGui.EndChild();

		if (ImGui.BeginPopupContextItem(buttonName + " rename", ImGuiPopupFlags.MouseButtonRight))
		{
			string name = customTemplate.Name ?? string.Empty;
			ImGui.SetKeyboardFocusHere();
			if (ImGui.InputText("##name", ref name, 24))
			{
				UserSettingsModel.UserSettingsPracticeTemplate originalTemplate = UserSettings.Model.PracticeTemplates.First(pt => pt == customTemplate);
				int index = UserSettings.Model.PracticeTemplates.IndexOf(originalTemplate);

				List<UserSettingsModel.UserSettingsPracticeTemplate> newPracticeTemplates = UserSettings.Model.PracticeTemplates
					.Where(pt => pt != originalTemplate)
					.ToList();

				newPracticeTemplates.Insert(index, originalTemplate with { Name = name });

				UserSettings.Model = UserSettings.Model with
				{
					PracticeTemplates = newPracticeTemplates,
				};
			}

			if (ImGui.IsKeyPressed(ImGuiKey.Enter))
				ImGui.CloseCurrentPopup();

			ImGui.EndPopup();
		}
	}

	private static void RenderDragIndicator(int i, UserSettingsModel.UserSettingsPracticeTemplate customTemplate)
	{
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
	}

	private static void RenderDeleteButton(UserSettingsModel.UserSettingsPracticeTemplate customTemplate)
	{
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
	}

	private static void RenderDragDropTarget(int i)
	{
		if (!_customTemplateIndexToReorder.HasValue)
			return;

		float relativeMouseY = ImGui.GetMousePos().Y - ImGui.GetWindowPos().Y + ImGui.GetScrollY();
		float currentY = ImGui.GetCursorPosY();
		const float dropAreaPaddingY = 26;
		if (relativeMouseY > currentY - dropAreaPaddingY && relativeMouseY < currentY + dropAreaPaddingY)
		{
			const float spacingY = 8;
			const float dropAreaHeight = 14;

			ImGui.SetCursorPosY(ImGui.GetCursorPosY() - spacingY);

			ImGui.PushStyleColor(ImGuiCol.Button, Color.Green with { A = 111 });
			ImGui.Button("##drop", new(PracticeWindow.TemplateWidth - 96, dropAreaHeight));
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
}
