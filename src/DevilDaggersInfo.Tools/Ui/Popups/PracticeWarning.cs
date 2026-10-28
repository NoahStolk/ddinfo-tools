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

	public static readonly GameTime RequiredGameTime = GameTime.FromSeconds(_requiredTimeInSeconds);

	public override bool Render()
	{
		ImGui.PushFont(fontService.GoetheBold20);
		ImGui.TextWrapped("IMPORTANT! Read this is you're practicing for 500!");
		ImGui.PopFont();

		ImGui.TextWrapped($"You need at least {RequiredGameTime.Seconds:N0} seconds of real in-game time before the game lets you submit 500+ runs.");

		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();

		Vector4 color = totalTime?.Seconds switch
		{
			null => new Vector4(1, 0, 0, 1),
			>= _requiredTimeInSeconds => new Vector4(0, 1, 0, 1),
			_ => new Vector4(1, 0.5f, 0, 1),
		};

		ReadOnlySpan<byte> text = totalTime?.Seconds switch
		{
			null => Inline.Utf8("You're seeing this because the app currently does not know your highscore, or if you have enough total time."),
			_ => Inline.Utf8($"You currently have {totalTime.Value.Seconds:N0} seconds of total time on the leaderboards."),
		};

		TextColoredWrapped(color, text);

		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();

		ImGui.TextWrapped(
			"""
			This was added to prevent cheaters and alt accounts from flooding the leaderboards.

			The total time is counted by the game itself (not Steam hours).

			Using practice (or any spawnset) does not count time towards your total time, and you may end up hitting 500 before hitting the required total time.
			""");

		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();

		if (ImGui.Checkbox("Do not show again", ref _doNotShowAgain))
			setDoNotShowAgain(_doNotShowAgain);

		return ImGui.Button("OK", new Vector2(120, 0)) || ImGui.IsEnterPressed();
	}

	private static void TextColoredWrapped(Vector4 color, ReadOnlySpan<byte> text)
	{
		ImGui.PushStyleColor(ImGuiCol.Text, color);
		ImGui.TextWrapped(text);
		ImGui.PopStyleColor();
	}
}
