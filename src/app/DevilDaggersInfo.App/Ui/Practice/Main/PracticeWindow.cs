using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Extensions;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.Practice.Main;

public static class PracticeWindow
{
	public const int TemplateDescriptionHeight = 48;
	public static Vector2 TemplateContainerSize { get; private set; }
	public static Vector2 TemplateListSize { get; private set; }
	public static float TemplateWidth { get; private set; }

	public static void Render()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(1280, 768));
		ImGui.Begin("Practice", ImGuiWindowFlags.NoCollapse);
		ImGui.PopStyleVar();

		Vector2 windowSize = ImGui.GetWindowSize();
		TemplateContainerSize = new(windowSize.X / 3 - 11, windowSize.Y - 220);
		TemplateListSize = new(TemplateContainerSize.X - 20, TemplateContainerSize.Y - 88);
		TemplateWidth = TemplateListSize.X - 20;

		ImGui.Text("Use these templates to practice specific sections of the game. Click on a template to install it.");
		ImGui.Spacing();

		NoFarmTemplatesChild.Render();

		ImGui.SameLine();

		EndLoopTemplatesChild.Render();

		ImGui.SameLine();

		CustomTemplatesChild.Render();

		InputValuesChild.Render();

		ImGui.SameLine();

		CurrentSpawnsetChild.Render();

		ImGui.End();

		if (ImGui.IsKeyPressed(ImGuiKey.Escape) || ImGui.IsKeyPressed((ImGuiKey)526))
			UiRenderer.Layout = LayoutType.Main;
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
}