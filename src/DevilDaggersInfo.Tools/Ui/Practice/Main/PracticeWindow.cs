using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.User.Settings;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main;

internal sealed class PracticeWindow(ResourceManager resourceManager, PracticeLogic practiceLogic, UserSettings userSettings, SurvivalFileWatcher survivalFileWatcher, ContentManager contentManager)
{
	public const int TemplateDescriptionHeight = 48;

	// TODO: Register these instead.
	private readonly CustomTemplatesChild _customTemplatesChild = new(resourceManager, practiceLogic, userSettings);
	private readonly EndLoopTemplatesChild _endLoopTemplatesChild = new(practiceLogic, contentManager);
	private readonly NoFarmTemplatesChild _noFarmTemplatesChild = new(practiceLogic);
	private readonly InputValuesChild _inputValuesChild = new(resourceManager, practiceLogic, userSettings, survivalFileWatcher);
	private readonly CurrentSpawnsetChild _currentSpawnsetChild = new(survivalFileWatcher);

	public void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(1208, 768);
		if (ImGui.Begin("Practice", ImGuiWindowFlags.NoCollapse))
		{
			Vector2 windowSize = ImGui.GetWindowSize();
			Vector2 templateContainerSize = new(MathF.Ceiling(windowSize.X / 3 - 11), windowSize.Y - 260);
			Vector2 templateListSize = new(templateContainerSize.X - 20, templateContainerSize.Y - 88);
			float templateWidth = templateListSize.X - 20;

			ImGui.Text("Use these templates to practice specific sections of the game. Click on a template to install it.");
			ImGui.Spacing();

			_noFarmTemplatesChild.Render(templateContainerSize, templateListSize, templateWidth);

			ImGui.SameLine();
			_endLoopTemplatesChild.Render(templateContainerSize, templateListSize, templateWidth);

			ImGui.SameLine();
			_customTemplatesChild.Render(templateContainerSize, templateListSize, templateWidth);

			_inputValuesChild.Render();

			ImGui.SameLine();
			_currentSpawnsetChild.Render();
		}

		ImGui.End();
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
