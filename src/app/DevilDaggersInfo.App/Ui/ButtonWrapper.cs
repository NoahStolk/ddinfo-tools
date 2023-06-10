using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui;

public static class ButtonWrapper
{
	public static void Render(
		string buttonName,
		Vector2 buttonSize,
		Vector4 color,
		Vector4 colorHovered,
		Action onClick,
		Vector4 topLeftColor,
		ReadOnlySpan<char> topLeftText,
		Vector4 topRightColor,
		ReadOnlySpan<char> topRightText,
		Vector4 bottomLeftColor,
		ReadOnlySpan<char> bottomLeftText,
		Vector4 bottomRightColor,
		ReadOnlySpan<char> bottomRightText)
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
		if (ImGui.BeginChild(buttonName, buttonSize, true))
		{
			bool hover = ImGui.IsWindowHovered();
			ImGui.PushStyleColor(ImGuiCol.ChildBg, hover ? colorHovered : color);

			if (ImGui.BeginChild(buttonName + " child", buttonSize, false, ImGuiWindowFlags.NoInputs))
			{
				if (hover && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
					onClick.Invoke();

				float windowWidth = ImGui.GetWindowWidth();

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 8));

				string topLeftTextString = topLeftText.ToString();
				string topRightTextString = topRightText.ToString();
				string bottomLeftTextString = bottomLeftText.ToString();
				string bottomRightTextString = bottomRightText.ToString();

				ImGui.TextColored(topLeftColor, topLeftTextString);
				ImGui.SameLine(windowWidth - ImGui.CalcTextSize(topRightTextString).X - 8);
				ImGui.TextColored(topRightColor, topRightTextString);

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 0));

				ImGui.TextColored(bottomLeftColor, bottomLeftTextString);
				ImGui.SameLine(windowWidth - ImGui.CalcTextSize(bottomRightTextString).X - 8);
				ImGui.TextColored(bottomRightColor, bottomRightTextString);
			}

			ImGui.EndChild();

			ImGui.PopStyleColor();
		}

		ImGui.PopStyleVar();

		ImGui.EndChild();
	}

	public static void Render(
		string buttonName,
		Vector2 buttonSize,
		Vector4 color,
		Vector4 colorHovered,
		Action onClick,
		Vector4 topLeftColor,
		ReadOnlySpan<char> topLeftText,
		Vector4 topRightColor,
		ReadOnlySpan<char> topRightText)
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
		if (ImGui.BeginChild(buttonName, buttonSize, true))
		{
			bool hover = ImGui.IsWindowHovered();
			ImGui.PushStyleColor(ImGuiCol.ChildBg, hover ? colorHovered : color);

			if (ImGui.BeginChild(buttonName + " child", buttonSize, false, ImGuiWindowFlags.NoInputs))
			{
				if (hover && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
					onClick.Invoke();

				float windowWidth = ImGui.GetWindowWidth();

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 8));

				string topLeftTextString = topLeftText.ToString();
				string topRightTextString = topRightText.ToString();

				ImGui.TextColored(topLeftColor, topLeftTextString);
				ImGui.SameLine(windowWidth - ImGui.CalcTextSize(topRightTextString).X - 8);
				ImGui.TextColored(topRightColor, topRightTextString);
			}

			ImGui.EndChild();

			ImGui.PopStyleColor();
		}

		ImGui.PopStyleVar();

		ImGui.EndChild();
	}
}
