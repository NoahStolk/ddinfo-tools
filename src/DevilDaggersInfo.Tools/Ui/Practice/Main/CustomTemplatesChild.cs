using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.Practice.Main.Data;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.User.Settings.Model;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main;

internal sealed class CustomTemplatesChild(ResourceManager resourceManager, PracticeLogic practiceLogic, UserSettings userSettings)
{
	private int? _customTemplateIndexToReorder;

	public void Render(Vector2 templateContainerSize, Vector2 templateListSize, float templateWidth)
	{
		if (ImGui.BeginChild("CustomTemplates", templateContainerSize, ImGuiChildFlags.Border)) // TODO: Borders in ImGui update.
		{
			ImGui.Text("Custom templates");

			if (ImGui.BeginChild("CustomTemplateDescription", templateListSize with { Y = PracticeWindow.TemplateDescriptionHeight }))
			{
				ImGui.PushTextWrapPos(ImGui.GetCursorPos().X + templateWidth);
				ImGui.Text("You can make your own templates and save them. Your custom templates are saved locally on your computer. Right-click to rename a template.");
				ImGui.PopTextWrapPos();
			}

			ImGui.EndChild();

			if (ImGui.BeginChild("CustomTemplateList", templateListSize))
			{
				RenderDragDropTarget(-1, templateWidth);
				for (int i = 0; i < userSettings.Model.PracticeTemplates.Count; i++)
					RenderCustomTemplate(i, userSettings.Model.PracticeTemplates[i], templateWidth);

				if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
					_customTemplateIndexToReorder = null;
			}

			ImGui.EndChild();
		}

		ImGui.EndChild();
	}

	private void RenderCustomTemplate(int i, UserSettingsPracticeTemplate customTemplate, float templateWidth)
	{
		RenderTemplateButton(customTemplate, templateWidth);

		ImGui.SameLine();

		RenderDragIndicator(i, customTemplate);

		ImGui.SameLine();

		RenderDeleteButton(i, customTemplate);

		RenderDragDropTarget(i, templateWidth);
	}

	private void RenderTemplateButton(UserSettingsPracticeTemplate customTemplate, float templateWidth)
	{
		Vector2 buttonSize = new(templateWidth - 96, 48);
		ReadOnlySpan<char> buttonName = Inline.Span($"{EnumUtils.HandLevelNames[customTemplate.HandLevel]}-{customTemplate.AdditionalGems}-{customTemplate.TimerStart}");

		Color color = Color.White;

		const int bufferLength = 32;
		Span<char> gemsOrHomingText = stackalloc char[bufferLength];
		PracticeWindow.GetGemsOrHomingText(customTemplate.HandLevel, customTemplate.AdditionalGems, gemsOrHomingText, out Color gemColor);
		gemsOrHomingText = gemsOrHomingText.SliceUntilNull(bufferLength);

		(byte backgroundAlpha, byte textAlpha) = PracticeWindow.GetAlpha(practiceLogic.IsActive(customTemplate));

		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
		if (ImGui.BeginChild(buttonName, buttonSize, ImGuiChildFlags.Border)) // TODO: Borders in ImGui update.
		{
			bool hover = ImGui.IsWindowHovered();
			ImGui.PushStyleColor(ImGuiCol.ChildBg, color with { A = (byte)(hover ? backgroundAlpha + 16 : backgroundAlpha) });

			if (ImGui.BeginChild(Inline.Span($"{buttonName}Child"), buttonSize, ImGuiChildFlags.None, ImGuiWindowFlags.NoInputs))
			{
				if (hover && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
				{
					practiceLogic.State = new PracticeState(customTemplate.HandLevel, customTemplate.AdditionalGems, customTemplate.TimerStart);
					practiceLogic.GenerateAndApplyPracticeSpawnset();
				}

				float windowWidth = ImGui.GetWindowWidth();

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 8));

				ImGui.TextColored(color with { A = textAlpha }, string.IsNullOrWhiteSpace(customTemplate.Name) ? "<untitled>" : customTemplate.Name);

				ReadOnlySpan<char> timerStartString = Inline.Span(customTemplate.TimerStart, StringFormats.TimeFormat);
				ImGui.SameLine(windowWidth - ImGui.CalcTextSize(timerStartString).X - 8);
				ImGui.TextColored(Color.White with { A = textAlpha }, timerStartString);

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 0));

				ImGui.TextColored(customTemplate.HandLevel.GetColor() with { A = textAlpha }, EnumUtils.HandLevelNames[customTemplate.HandLevel]);
				ImGui.SameLine(windowWidth - ImGui.CalcTextSize(gemsOrHomingText).X - 8);
				ImGui.TextColored(gemColor with { A = textAlpha }, gemsOrHomingText);
			}

			ImGui.EndChild();

			ImGui.PopStyleColor();
		}

		ImGui.PopStyleVar();

		ImGui.EndChild();

		if (ImGui.BeginPopupContextItem(Inline.Span($"{buttonName} rename"), ImGuiPopupFlags.MouseButtonRight))
		{
			string name = customTemplate.Name ?? string.Empty;
			ImGui.SetKeyboardFocusHere();
			if (ImGui.InputText("##name", ref name, 24))
			{
				UserSettingsPracticeTemplate originalTemplate = userSettings.Model.PracticeTemplates.First(pt => pt == customTemplate);
				int index = userSettings.Model.PracticeTemplates.IndexOf(originalTemplate);

				List<UserSettingsPracticeTemplate> newPracticeTemplates = userSettings.Model.PracticeTemplates
					.Where(pt => pt != originalTemplate)
					.ToList();

				newPracticeTemplates.Insert(index, originalTemplate with { Name = name });

				userSettings.Model = userSettings.Model with
				{
					PracticeTemplates = newPracticeTemplates,
				};
			}

			if (ImGuiUtils.IsEnterPressed())
				ImGui.CloseCurrentPopup();

			ImGui.EndPopup();
		}
	}

	private void RenderDragIndicator(int i, UserSettingsPracticeTemplate customTemplate)
	{
		Color gray = Color.Gray(0.3f);
		ImGui.PushStyleColor(ImGuiCol.Button, gray with { A = 159 });
		ImGui.PushStyleColor(ImGuiCol.ButtonActive, gray);
		ImGui.PushStyleColor(ImGuiCol.ButtonHovered, gray with { A = 223 });
		ImGui.PushID(Inline.Span($"drag indicator {i}"));

		ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);
		ImGuiImage.ImageButton("CustomTemplateReorderImageButton", resourceManager.InternalResources.DragIndicatorTexture.Id, new Vector2(32, 48), _customTemplateIndexToReorder == i ? Color.Gray(0.7f) : gray);
		ImGui.PopStyleVar();

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

	private void RenderDeleteButton(int i, UserSettingsPracticeTemplate customTemplate)
	{
		ImGui.PushStyleColor(ImGuiCol.Button, Color.Red with { A = 159 });
		ImGui.PushStyleColor(ImGuiCol.ButtonActive, Color.Red);
		ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Color.Red with { A = 223 });
		ImGui.PushID(Inline.Span($"delete button {i}"));

		ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(12));
		if (ImGuiImage.ImageButton("CustomTemplateDeleteImageButton", resourceManager.InternalResources.BinTexture.Id, new Vector2(24)))
		{
			userSettings.Model = userSettings.Model with
			{
				PracticeTemplates = userSettings.Model.PracticeTemplates.Where(pt => customTemplate != pt).ToList(),
			};
		}

		ImGui.PopStyleVar();

		if (ImGui.IsItemHovered())
			ImGui.SetTooltip("Delete permanently");

		ImGui.PopID();
		ImGui.PopStyleColor(3);
	}

	private void RenderDragDropTarget(int i, float templateWidth)
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
			ImGui.Button("##drop", new Vector2(templateWidth - 96, dropAreaHeight));
			ImGui.PopStyleColor();

			ImGui.SetCursorPosY(ImGui.GetCursorPosY() - dropAreaHeight + spacingY - 4);

			if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
			{
				UserSettingsPracticeTemplate originalTemplate = userSettings.Model.PracticeTemplates[_customTemplateIndexToReorder.Value];

				List<UserSettingsPracticeTemplate> newPracticeTemplates = userSettings.Model.PracticeTemplates
					.Where(pt => pt != originalTemplate)
					.ToList();

				if (i < _customTemplateIndexToReorder)
					newPracticeTemplates.Insert(i + 1, originalTemplate);
				else
					newPracticeTemplates.Insert(i, originalTemplate);

				userSettings.Model = userSettings.Model with
				{
					PracticeTemplates = newPracticeTemplates,
				};

				_customTemplateIndexToReorder = null;
			}
		}
	}
}
