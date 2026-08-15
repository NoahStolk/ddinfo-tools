using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using Hexa.NET.ImGui;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main;

internal sealed class PracticeWindow(
	CurrentSpawnsetChild currentSpawnsetChild,
	CustomTemplatesChild customTemplatesChild,
	EndLoopTemplatesChild endLoopTemplatesChild,
	NoFarmTemplatesChild noFarmTemplatesChild,
	InputValuesChild inputValuesChild)
{
	public const int TemplateDescriptionHeight = 48;

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

			noFarmTemplatesChild.Render(templateContainerSize, templateListSize, templateWidth);

			ImGui.SameLine();
			endLoopTemplatesChild.Render(templateContainerSize, templateListSize, templateWidth);

			ImGui.SameLine();
			customTemplatesChild.Render(templateContainerSize, templateListSize, templateWidth);

			inputValuesChild.Render();

			ImGui.SameLine();
			currentSpawnsetChild.Render();
		}

		ImGui.End();
	}

	/// <summary>
	/// Writes the gems or homing text into <paramref name="buffer" /> as null-terminated UTF-8, and returns it without
	/// the terminator. The buffer is caller-owned rather than <see cref="Inline" />'s shared one, because callers keep
	/// the result alive across other <see cref="Inline" /> calls.
	/// </summary>
	public static ReadOnlySpan<byte> GetGemsOrHomingText(HandLevel handLevel, int additionalGems, Span<byte> buffer, out Color textColor)
	{
		EffectivePlayerSettings effectivePlayerSettings = SpawnsetBinary.GetEffectivePlayerSettings(PracticeLogic.SpawnVersion, handLevel, additionalGems);

		ReadOnlySpan<byte> suffix = handLevel switch
		{
			HandLevel.Level1 or HandLevel.Level2 => " gems"u8,
			HandLevel.Level3 or HandLevel.Level4 => " homing"u8,
			_ => throw new UnreachableException($"Invalid hand level '{handLevel}'."),
		};

		if (!effectivePlayerSettings.GemsOrHoming.TryFormat(buffer, out int bytesWritten) || !suffix.TryCopyTo(buffer[bytesWritten..]))
			throw new ArgumentException("The buffer is too small to hold the gems or homing text.", nameof(buffer));

		bytesWritten += suffix.Length;
		if (bytesWritten >= buffer.Length)
			throw new ArgumentException("The buffer is too small to hold the null terminator.", nameof(buffer));

		buffer[bytesWritten] = 0x00;

		textColor = effectivePlayerSettings.HandLevel switch
		{
			HandLevel.Level1 or HandLevel.Level2 => Color.Red,
			_ => effectivePlayerSettings.HandLevel.GetColor(),
		};

		return buffer[..bytesWritten];
	}

	public static (byte BackgroundAlpha, byte TextAlpha) GetAlpha(bool isActive)
	{
		return isActive ? ((byte)48, (byte)255) : ((byte)16, (byte)191);
	}
}
