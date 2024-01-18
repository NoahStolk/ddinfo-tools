using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main;

public static class PracticeWindow
{
	public const int TemplateDescriptionHeight = 48;
	public static Vector2 TemplateContainerSize { get; private set; }
	public static Vector2 TemplateListSize { get; private set; }
	public static float TemplateWidth { get; private set; }

	public static void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(1208, 768);
		if (ImGui.Begin("Practice", ImGuiWindowFlags.NoCollapse))
		{
			Vector2 windowSize = ImGui.GetWindowSize();
			TemplateContainerSize = new(MathF.Ceiling(windowSize.X / 3 - 11), windowSize.Y - 260);
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
		}

		ImGui.End(); // End Practice
	}

	public static void GetGemsOrHomingText(HandLevel handLevel, int additionalGems, Span<char> text, out Color textColor)
	{
		EffectivePlayerSettings effectivePlayerSettings = SpawnsetBinary.GetEffectivePlayerSettings(PracticeLogic.SpawnVersion, handLevel, additionalGems);
		effectivePlayerSettings.GemsOrHoming.TryFormat(text, out int charsWritten);

		switch (handLevel)
		{
			case HandLevel.Level1 or HandLevel.Level2: " gems".AsSpan().CopyTo(text[charsWritten..]); break;
			case HandLevel.Level3 or HandLevel.Level4: " homing".AsSpan().CopyTo(text[charsWritten..]); break;
			default: throw new UnreachableException($"Invalid hand level '{handLevel}'.");
		}

		textColor = effectivePlayerSettings.HandLevel switch
		{
			HandLevel.Level1 or HandLevel.Level2 => Color.Red,
			_ => effectivePlayerSettings.HandLevel.GetColor(),
		};
	}

	public static (byte BackgroundAlpha, byte TextAlpha) GetAlpha(bool isActive)
	{
		return isActive ? ((byte)48, (byte)255) : ((byte)16, (byte)191);
	}
}
