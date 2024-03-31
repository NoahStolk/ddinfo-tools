using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Extensions;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Core.Wiki.Objects;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

public static class HitEventExplanation
{
	public static void Render(HitEventData e, EditorReplayModel replay)
	{
		if (e.EntityIdA == 0)
		{
			Death? death = e.EntityIdB < 0 ? null : Deaths.GetDeathByType(GameConstants.CurrentVersion, (byte)e.EntityIdB);
			ImGui.TextColored(Color.Red, "Player died");
			ImGui.SameLine();
			ImGui.Text("-");
			ImGui.SameLine();
			ImGui.TextColored(death?.Color.ToEngineColor() ?? Color.White, Inline.Span($"{death?.Name ?? "Unknown death type"}"));
			return;
		}

		EntityType? entityTypeA = replay.GetEntityTypeIncludingNegated(e.EntityIdA);
		if (!entityTypeA.HasValue)
		{
			ImGui.TextColored(Color.Red, "Entity Id A out of bounds");
			return;
		}

		EntityType? entityTypeB = replay.GetEntityTypeIncludingNegated(e.EntityIdB);
		if (!entityTypeB.HasValue)
		{
			ImGui.TextColored(Color.Red, "Entity Id B out of bounds");
			return;
		}

		if (entityTypeA.Value.IsDagger() && e.EntityIdB == 0)
		{
			TextEntityType(entityTypeA.Value, e.EntityIdA);
			ImGui.SameLine();
			ImGui.Text("despawned");
			return;
		}

		if (entityTypeA == EntityType.Ghostpede && entityTypeB is EntityType.Level3HomingDagger or EntityType.Level4HomingDagger)
		{
			TextEntityType(entityTypeB.Value, e.EntityIdB);
			ImGui.SameLine();
			ImGui.Text("eaten by");
			ImGui.SameLine();
			TextEntityType(entityTypeA.Value, e.EntityIdA);
			return;
		}

		if (entityTypeA.Value.IsEnemy() && entityTypeB.Value.IsDagger())
		{
			TextEntityType(entityTypeA.Value, e.EntityIdA);
			ImGui.SameLine();
			ImGui.Text("hit by");
			ImGui.SameLine();
			TextEntityType(entityTypeB.Value, e.EntityIdB);
			ImGui.SameLine();
			ImGui.Text("-");
			ImGui.SameLine();

			Color noDamageColor = Color.Gray(0.5f);

			// Negative entity IDs are used for dead pede segments.
			if (e.EntityIdA < 0)
			{
				ImGui.TextColored(noDamageColor, "Did not take damage");
				return;
			}

			int damage = entityTypeA.Value.GetDamage(entityTypeB.Value, e.UserData);
			int damageablePartCount = entityTypeA switch
			{
				EntityType.Squid1 or EntityType.Spider1 or EntityType.Spider2 => 1,
				EntityType.Squid2 => 2,
				EntityType.Squid3 => 3,
				EntityType.Leviathan => 6,
				EntityType.Centipede => 25,
				EntityType.Gigapede => 50,
				EntityType.Ghostpede => 10,
				_ => 0,
			};

			if (damageablePartCount == 0)
			{
				ImGui.Text(Inline.Span($"Took {damage} damage"));
				return;
			}

			// Negative user data is invalid.
			// User data is the index of the gem that was hit, so if it is over the amount of damageable parts, no damage was taken.
			if (e.UserData < 0 || e.UserData >= damageablePartCount)
			{
				ImGui.TextColored(noDamageColor, "Did not take damage");
				return;
			}

			if (damageablePartCount == 1)
			{
				ImGui.Text(Inline.Span($"Took {damage} damage"));
				return;
			}

			ReadOnlySpan<char> number = e.UserData switch
			{
				0 => "1st",
				1 => "2nd",
				2 => "3rd",
				_ => $"{e.UserData + 1}th",
			};

			ImGui.Text(Inline.Span($"{number} gem took {damage} damage"));
			return;
		}

		ImGui.TextColored(Color.Red, "Hit event data not understood");

		static void TextEntityType(EntityType entityType, int entityId)
		{
			ImGui.TextColored(entityType.GetColor(), Inline.Span($"{EnumUtils.EntityTypeShortNames[entityType]}"));
			ImGui.SameLine();
			ImGui.TextColored(Color.Gray(0.5f), Inline.Span($"(id {Math.Abs(entityId)})"));
		}
	}
}
