using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Extensions;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.Practice.Main;

public static class PracticeWindow
{
	public const int TemplateWidth = 360;
	public const int TemplateDescriptionHeight = 48;
	public static readonly Vector2 TemplateContainerSize = new(400, 544);
	public static readonly Vector2 TemplateListSize = new(380, 456);

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
