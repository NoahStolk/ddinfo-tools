using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Tools.Extensions;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Popups;

internal sealed class PracticeWarning(GameTime? totalTime, bool doNotShowAgain, Action<bool> setDoNotShowAgain, FontService fontService)
	: Popup("Practice Warning", new Vector2(512, 384))
{
	private const int _requiredTimeInSeconds = 36_000;

	private bool _doNotShowAgain = doNotShowAgain;

	public override bool Render()
	{
		ImGui.PushFont(fontService.GoetheBold20);
		ImGui.TextWrapped("IMPORTANT! Read this is you're practicing for 500!");
		ImGui.PopFont();

		Separator();

		ImGui.TextWrapped($"You need at least {_requiredTimeInSeconds:N0} seconds of real in-game time before the game lets you submit 500+ runs.");

		Separator();

		ImGui.TextWrapped("Using practice (or any spawnset) does not count time towards your total time, and you may end up reaching 500 before having the required total time.");

		Separator();

		Vector4 color = totalTime?.Seconds switch
		{
			null => new Vector4(1, 0.25f, 0, 1),
			>= _requiredTimeInSeconds => new Vector4(0, 1, 0, 1),
			_ => new Vector4(1, 0.5f, 0, 1),
		};

		ReadOnlySpan<byte> text = totalTime?.Seconds switch
		{
			null => Inline.Utf8("You're seeing this because the app currently does not know your highscore, or if you have enough total time."),
			_ => Inline.Utf8($"You currently have {totalTime.Value.Seconds:N0} seconds of total time on the leaderboards."),
		};

		TextColoredWrapped(color, text);

		Separator();

		ImGui.PushFont(fontService.GoetheBold20);
		ImGui.TextWrapped("Your 500+ scores won't submit if you don't have enough time. Make sure you have enough before attempting real runs.");
		ImGui.PopFont();

		Separator();

		if (ImGui.Checkbox("Do not show again", ref _doNotShowAgain))
			setDoNotShowAgain(_doNotShowAgain);

		return ImGui.Button("OK", new Vector2(120, 0)) || ImGui.IsEnterPressed();
	}

	private static void Separator()
	{
		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();
	}

	private static void TextColoredWrapped(Vector4 color, ReadOnlySpan<byte> text)
	{
		ImGui.PushStyleColor(ImGuiCol.Text, color);
		ImGui.TextWrapped(text);
		ImGui.PopStyleColor();
	}
}
