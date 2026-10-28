using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.Practice.Main.Data;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.User.Settings.Model;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main;

internal sealed class CustomTemplatesChild(ResourceManager resourceManager, PracticeLogic practiceLogic, UserSettings userSettings)
{
	private int? _customTemplateIndexToReorder;

	public void Render(Vector2 templateContainerSize, Vector2 templateListSize, float templateWidth)
	{
		if (ImGui.BeginChild("CustomTemplates", templateContainerSize, ImGuiChildFlags.Borders))
		{
			ImGui.SeparatorText("Custom templates");

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
		ReadOnlySpan<byte> buttonName = Inline.Utf8($"{customTemplate.HandLevel.AsUtf8Span()}-{customTemplate.AdditionalGems}-{customTemplate.TimerStart}");

		Vector4 color = Vector4.One;

		Span<byte> gemsOrHomingBuffer = stackalloc byte[32];
		ReadOnlySpan<byte> gemsOrHomingText = PracticeWindow.GetGemsOrHomingText(customTemplate.HandLevel, customTemplate.AdditionalGems, gemsOrHomingBuffer, out Vector4 gemColor);

		(float backgroundAlpha, float textAlpha) = PracticeWindow.GetAlpha(practiceLogic.IsActive(customTemplate));

		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
		if (ImGui.BeginChild(buttonName, buttonSize, ImGuiChildFlags.Borders))
		{
			bool hover = ImGui.IsWindowHovered();
			ImGui.PushStyleColor(ImGuiCol.ChildBg, color with { W = hover ? backgroundAlpha + 0.0625f : backgroundAlpha });

			// The id is scoped to the enclosing child window, which is already unique per template, so a constant is
			// enough. Deriving it from buttonName would interpolate a slice of the Inline buffer back into itself.
			if (ImGui.BeginChild("Child"u8, buttonSize, ImGuiChildFlags.None, ImGuiWindowFlags.NoInputs))
			{
				if (hover && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
				{
					practiceLogic.State = new PracticeState(customTemplate.HandLevel, customTemplate.AdditionalGems, customTemplate.TimerStart);
					practiceLogic.GenerateAndApplyPracticeSpawnset();
				}

				float windowWidth = ImGui.GetWindowWidth();

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 8));

				ImGui.TextColored(color with { W = textAlpha }, string.IsNullOrWhiteSpace(customTemplate.Name) ? "<untitled>" : customTemplate.Name);

				ReadOnlySpan<byte> timerStartString = Inline.Utf8(customTemplate.TimerStart, StringFormats.TimeFormat);
				ImGui.SameLine(windowWidth - ImGui.CalcTextSize(timerStartString).X - 8);
				ImGui.TextColored(Vector4.One with { W = textAlpha }, timerStartString);

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 0));

				ImGui.TextColored(customTemplate.HandLevel.GetColor() with { W = textAlpha }, customTemplate.HandLevel.AsUtf8Span());
				ImGui.SameLine(windowWidth - ImGui.CalcTextSize(gemsOrHomingText).X - 8);
				ImGui.TextColored(gemColor with { W = textAlpha }, gemsOrHomingText);
			}

			ImGui.EndChild();

			ImGui.PopStyleColor();
		}

		ImGui.PopStyleVar();

		ImGui.EndChild();

		RenamePopup(customTemplate);
	}

	private void RenamePopup(UserSettingsPracticeTemplate customTemplate)
	{
		// Built from the template values rather than from buttonName, which is a slice of the Inline buffer and cannot be
		// interpolated back into it. The templates are siblings in one window, so the id has to stay unique per template.
		ReadOnlySpan<byte> renameName = Inline.Utf8($"rename {(int)customTemplate.HandLevel}-{customTemplate.AdditionalGems}-{customTemplate.TimerStart}");
		if (!ImGui.BeginPopupContextItem(renameName, ImGuiPopupFlags.MouseButtonRight))
			return;

		string name = customTemplate.Name ?? string.Empty;
		ImGui.SetKeyboardFocusHere();
		if (ImGui.InputText("##name", ref name, 24))
		{
			UserSettingsPracticeTemplate originalTemplate = userSettings.Model.PracticeTemplates.First(pt => pt == customTemplate);
			int index = userSettings.Model.PracticeTemplates.IndexOf(originalTemplate);

			List<UserSettingsPracticeTemplate> newPracticeTemplates = [.. userSettings.Model.PracticeTemplates.Where(pt => pt != originalTemplate)];

			newPracticeTemplates.Insert(index, originalTemplate with { Name = name });

			userSettings.Model = userSettings.Model with
			{
				PracticeTemplates = newPracticeTemplates,
			};
		}

		if (ImGui.IsEnterPressed())
			ImGui.CloseCurrentPopup();

		ImGui.EndPopup();
	}

	private unsafe void RenderDragIndicator(int i, UserSettingsPracticeTemplate customTemplate)
	{
		Vector4 gray = new(0.3f, 0.3f, 0.3f, 1);
		ImGui.PushStyleColor(ImGuiCol.Button, gray with { W = 0.62f });
		ImGui.PushStyleColor(ImGuiCol.ButtonActive, gray);
		ImGui.PushStyleColor(ImGuiCol.ButtonHovered, gray with { W = 0.87f });
		ImGui.PushID(Inline.Utf8($"drag indicator {i}"));

		ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);
		ImGui.ImageButton(
			strId: "CustomTemplateReorderImageButton"u8,
			imageId: resourceManager.InternalResources.DragIndicatorTexture.Id,
			size: new Vector2(32, 48),
			backgroundColor: _customTemplateIndexToReorder == i ? new Vector4(0.7f, 0.7f, 0.7f, 1) : gray);
		ImGui.PopStyleVar();

		if (ImGui.IsItemHovered())
			ImGui.SetTooltip("Reorder");

		if (ImGui.BeginDragDropSource())
		{
			_customTemplateIndexToReorder = i;

			ImGui.SetDragDropPayload("CustomTemplateReorder"u8, null, 0);
			string templateDragName = customTemplate.Name != null
				? $"\"{customTemplate.Name}\""
				: $"{customTemplate.HandLevel} (+{customTemplate.AdditionalGems}) {customTemplate.TimerStart.ToString(StringFormats.TimeFormat)}";
			ImGui.Text($"Reorder {templateDragName}");
			ImGui.EndDragDropSource();
		}

		ImGui.PopID();
		ImGui.PopStyleColor(3);
	}

	private void RenderDeleteButton(int i, UserSettingsPracticeTemplate customTemplate)
	{
		ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 0, 0, 0.62f));
		ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(1, 0, 0, 1));
		ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(1, 0, 0, 0.87f));
		ImGui.PushID(Inline.Utf8($"delete button {i}"));

		ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(12));
		if (ImGui.ImageButton("CustomTemplateDeleteImageButton"u8, resourceManager.InternalResources.BinTexture.Id, new Vector2(24)))
		{
			userSettings.Model = userSettings.Model with
			{
				PracticeTemplates = [.. userSettings.Model.PracticeTemplates.Where(pt => customTemplate != pt)],
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

			ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 1, 0, 0.43f));
			ImGui.Button("##drop", new Vector2(templateWidth - 96, dropAreaHeight));
			ImGui.PopStyleColor();

			ImGui.SetCursorPosY(ImGui.GetCursorPosY() - dropAreaHeight + spacingY - 4);

			if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
			{
				InsertTemplate(i, _customTemplateIndexToReorder.Value);
				_customTemplateIndexToReorder = null;
			}
		}
	}

	private void InsertTemplate(int i, int customTemplateIndexToReorder)
	{
		UserSettingsPracticeTemplate originalTemplate = userSettings.Model.PracticeTemplates[customTemplateIndexToReorder];

		List<UserSettingsPracticeTemplate> newPracticeTemplates = [.. userSettings.Model.PracticeTemplates.Where(pt => pt != originalTemplate)];

		if (i < customTemplateIndexToReorder)
			newPracticeTemplates.Insert(i + 1, originalTemplate);
		else
			newPracticeTemplates.Insert(i, originalTemplate);

		userSettings.Model = userSettings.Model with
		{
			PracticeTemplates = newPracticeTemplates,
		};
	}
}
