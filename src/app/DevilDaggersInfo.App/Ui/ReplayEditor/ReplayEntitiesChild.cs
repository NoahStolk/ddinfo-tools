using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Utils;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Extensions;
using DevilDaggersInfo.Core.Replay.PostProcessing.HitLog;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor;

public static class ReplayEntitiesChild
{
	private static int _startId;
	private static EnemyHitLog? _enemyHitLog;

	public static void Reset()
	{
		_enemyHitLog = null;
	}

	public static void Render(ReplayEventsData eventsData)
	{
		if (ImGui.BeginChild("ReplayEntities", new(320, 0)))
		{
			const int maxIds = 1000;

			Vector2 iconSize = new(16);
			if (ImGuiImage.ImageButton("Start", Root.InternalResources.ArrowStartTexture.Handle, iconSize))
				_startId = 0;
			ImGui.SameLine();
			if (ImGuiImage.ImageButton("Back", Root.InternalResources.ArrowLeftTexture.Handle, iconSize))
				_startId = Math.Max(0, _startId - maxIds);
			ImGui.SameLine();
			if (ImGuiImage.ImageButton("Forward", Root.InternalResources.ArrowRightTexture.Handle, iconSize))
				_startId = Math.Min(eventsData.TickCount - maxIds, _startId + maxIds);
			ImGui.SameLine();
			if (ImGuiImage.ImageButton("End", Root.InternalResources.ArrowEndTexture.Handle, iconSize))
				_startId = eventsData.TickCount - maxIds;

			if (ImGui.BeginTable("ReplayEntitiesTable", 4, ImGuiTableFlags.None))
			{
				ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.WidthFixed, 64);
				ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.None, 128);
				ImGui.TableHeadersRow();

				for (int i = _startId; i < Math.Min(_startId + maxIds, eventsData.EntityTypes.Count); i++)
				{
					ImGui.TableNextRow();

					EntityType entityType = eventsData.EntityTypes[i];

					ImGui.TableNextColumn();
					if (ImGui.Selectable(UnsafeSpan.Get(i), false, ImGuiSelectableFlags.SpanAllColumns))
						_enemyHitLog = EnemyHitLogBuilder.Build(eventsData.Events, i);

					ImGui.TableNextColumn();
					ImGui.TextColored(((EntityType?)entityType).GetColor(), EnumUtils.EntityTypeNames[entityType]);
				}

				ImGui.EndTable();
			}
		}

		ImGui.EndChild(); // ReplayEntities

		ImGui.SameLine();

		if (ImGui.BeginChild("ReplayEnemyHitLog"))
		{
			RenderEnemyHitLog();
		}

		ImGui.EndChild(); // ReplayEnemyHitLog
	}

	private static void RenderEnemyHitLog()
	{
		if (_enemyHitLog == null)
		{
			ImGui.Text("Select an entity from the list.");
		}
		else
		{
			ImGui.Text(UnsafeSpan.Get($"Enemy hit log for {EnumUtils.EntityTypeNames[_enemyHitLog.EntityType]} (id {_enemyHitLog.EntityId}):"));

			int initialHp = _enemyHitLog.EntityType.GetInitialHp();
			if (ImGui.BeginTable("EnemyHitLog", 6, ImGuiTableFlags.None))
			{
				ImGui.TableSetupColumn("Tick", ImGuiTableColumnFlags.None, 128);
				ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.None, 128);
				ImGui.TableSetupColumn("HP", ImGuiTableColumnFlags.None, 128);
				ImGui.TableSetupColumn("Damage", ImGuiTableColumnFlags.None, 128);
				ImGui.TableSetupColumn("Dagger Type", ImGuiTableColumnFlags.None, 128);
				ImGui.TableSetupColumn("User Data", ImGuiTableColumnFlags.None, 128);
				ImGui.TableHeadersRow();

				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.Text(UnsafeSpan.Get(_enemyHitLog.SpawnTick));
				ImGui.TableNextColumn();
				ImGui.Text(UnsafeSpan.Get(_enemyHitLog.SpawnTick / 60f, StringFormats.TimeFormat));
				ImGui.TableNextColumn();
				ImGui.Text(UnsafeSpan.Get(initialHp));
				ImGui.TableNextColumn();
				ImGui.Text("-");
				ImGui.TableNextColumn();
				ImGui.Text("-");
				ImGui.TableNextColumn();
				ImGui.Text("-");

				for (int i = 0; i < _enemyHitLog.Hits.Count; i++)
				{
					EnemyHitLogEvent hit = _enemyHitLog.Hits[i];

					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGui.Text(UnsafeSpan.Get(hit.Tick));
					ImGui.TableNextColumn();
					ImGui.Text(UnsafeSpan.Get(hit.Tick / 60f, StringFormats.TimeFormat));
					ImGui.TableNextColumn();
					ImGui.TextColored(hit.Hp < 0 ? Color.Red : Color.Lerp(Color.Red, Color.White, hit.Hp / (float)initialHp), UnsafeSpan.Get($"{hit.Hp} / {initialHp}"));
					ImGui.TableNextColumn();
					ImGui.TextColored(hit.Damage > 0 ? Color.Red : Color.White, UnsafeSpan.Get(hit.Damage));
					ImGui.TableNextColumn();
					ImGui.Text(EnumUtils.DaggerTypeNames[hit.DaggerType]);
					ImGui.TableNextColumn();
					ImGui.Text(UnsafeSpan.Get(hit.UserData));
				}

				ImGui.EndTable();
			}
		}
	}
}
