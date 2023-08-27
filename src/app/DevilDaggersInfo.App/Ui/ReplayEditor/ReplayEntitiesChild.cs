using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.ReplayEditor.Utils;
using DevilDaggersInfo.App.Utils;
using DevilDaggersInfo.App.ZeroAllocation;
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
	private static bool _showEnemies = true;
	private static bool _showDaggers = true;
	private static EnemyHitLog? _enemyHitLog;

	public static void Reset()
	{
		_enemyHitLog = null;
	}

	public static void Render(ReplayEventsData eventsData, float startTime)
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
				_startId = Math.Min(eventsData.EntityTypes.Count - maxIds, _startId + maxIds);
			ImGui.SameLine();
			if (ImGuiImage.ImageButton("End", Root.InternalResources.ArrowEndTexture.Handle, iconSize))
				_startId = eventsData.EntityTypes.Count - maxIds;

			ImGui.Text(UnsafeSpan.Get($"Showing {_startId} - {_startId + maxIds - 1} of {eventsData.EntityTypes.Count}"));

			ImGui.Checkbox("Show enemies", ref _showEnemies);
			ImGui.SameLine();
			ImGui.Checkbox("Show daggers", ref _showDaggers);

			if (ImGui.BeginChild("ReplayEntitiesChild", new(0, 0)))
			{
				if (ImGui.BeginTable("ReplayEntitiesTable", 2, ImGuiTableFlags.None))
				{
					ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.WidthFixed, 64);
					ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.None, 128);
					ImGui.TableHeadersRow();

					for (int i = _startId; i < Math.Min(_startId + maxIds, eventsData.EntityTypes.Count); i++)
					{
						EntityType entityType = eventsData.EntityTypes[i];

						if (!_showDaggers && entityType.IsDagger())
							continue;

						if (!_showEnemies && entityType.IsEnemy())
							continue;

						ImGui.TableNextRow();

						ImGui.TableNextColumn();
						if (ImGui.Selectable(UnsafeSpan.Get(i), false, ImGuiSelectableFlags.SpanAllColumns))
							_enemyHitLog = EnemyHitLogBuilder.Build(eventsData.Events, i);

						ImGui.TableNextColumn();
						ImGui.TextColored(((EntityType?)entityType).GetColor(), EnumUtils.EntityTypeNames[entityType]);
					}

					ImGui.EndTable();
				}
			}

			ImGui.EndChild(); // ReplayEntitiesChild
		}

		ImGui.EndChild(); // ReplayEntities

		ImGui.SameLine();

		if (ImGui.BeginChild("ReplayEnemyHitLog"))
			RenderEnemyHitLog(startTime);

		ImGui.EndChild(); // ReplayEnemyHitLog
	}

	private static void RenderEnemyHitLog(float startTime)
	{
		if (_enemyHitLog == null)
		{
			ImGui.Text("Select an entity from the list.");
		}
		else
		{
			ImGui.Text(UnsafeSpan.Get($"Enemy hit log for {EnumUtils.EntityTypeNames[_enemyHitLog.EntityType]} (id {_enemyHitLog.EntityId}):"));

			int initialHp = _enemyHitLog.EntityType.GetInitialHp();
			if (ImGui.BeginTable("EnemyHitLog", 5, ImGuiTableFlags.None))
			{
				ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.None, 128);
				ImGui.TableSetupColumn("HP", ImGuiTableColumnFlags.None, 128);
				ImGui.TableSetupColumn("Damage", ImGuiTableColumnFlags.None, 128);
				ImGui.TableSetupColumn("Dagger Type", ImGuiTableColumnFlags.None, 128);
				ImGui.TableSetupColumn("User Data", ImGuiTableColumnFlags.None, 128);
				ImGui.TableHeadersRow();

				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.Text(UnsafeSpan.Get($"{TimeUtils.TickToTime(_enemyHitLog.SpawnTick, startTime):0.0000} ({_enemyHitLog.SpawnTick})"));
				ImGui.TableNextColumn();
				ImGui.TextColored(Color.Green, "Spawn");
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
					ImGui.Text(UnsafeSpan.Get($"{TimeUtils.TickToTime(hit.Tick, startTime):0.0000} ({hit.Tick})"));
					ImGui.TableNextColumn();
					ImGui.TextColored(hit.Hp < 0 ? Color.Red : Color.Lerp(Color.Red, Color.White, hit.Hp / (float)initialHp), hit.Hp <= 0 ? "Dead" : UnsafeSpan.Get($"{hit.Hp} / {initialHp}"));
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
