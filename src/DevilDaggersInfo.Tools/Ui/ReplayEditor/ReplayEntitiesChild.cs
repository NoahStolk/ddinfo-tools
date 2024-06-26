using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Extensions;
using DevilDaggersInfo.Core.Replay.PostProcessing.HitLog;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Utils;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

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

	public static void Render(EditorReplayModel replay)
	{
		if (ImGui.BeginChild("ReplayEntities", new Vector2(320, 0)))
		{
			const int maxIds = 1000;

			Vector2 iconSize = new(16);
			if (ImGuiImage.ImageButton("Start", Root.InternalResources.ArrowStartTexture.Id, iconSize))
				_startId = 0;
			ImGui.SameLine();
			if (ImGuiImage.ImageButton("Back", Root.InternalResources.ArrowLeftTexture.Id, iconSize))
				_startId = Math.Max(0, _startId - maxIds);
			ImGui.SameLine();
			if (ImGuiImage.ImageButton("Forward", Root.InternalResources.ArrowRightTexture.Id, iconSize))
				_startId = Math.Min(replay.Cache.Entities.Count - maxIds, _startId + maxIds);
			ImGui.SameLine();
			if (ImGuiImage.ImageButton("End", Root.InternalResources.ArrowEndTexture.Id, iconSize))
				_startId = replay.Cache.Entities.Count - maxIds;

			_startId = Math.Max(0, Math.Min(_startId, replay.Cache.Entities.Count - maxIds));

			ImGui.Text(Inline.Span($"Showing {_startId} - {_startId + maxIds - 1} of {replay.Cache.Entities.Count + 1}"));

			ImGui.Checkbox("Show enemies", ref _showEnemies);
			ImGui.SameLine();
			ImGui.Checkbox("Show daggers", ref _showDaggers);

			if (ImGui.BeginChild("ReplayEntitiesChild", new Vector2(0, 0)))
			{
				if (ImGui.BeginTable("ReplayEntitiesTable", 2, ImGuiTableFlags.None))
				{
					ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.WidthFixed, 64);
					ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.None, 128);
					ImGui.TableHeadersRow();

					for (int i = _startId; i < Math.Min(_startId + maxIds, replay.Cache.Entities.Count + 1); i++)
					{
						EntityType? entityType = replay.GetEntityType(i);
						if (!entityType.HasValue)
							continue;

						if (!_showDaggers && entityType.Value.IsDagger())
							continue;

						if (!_showEnemies && entityType.Value.IsEnemy())
							continue;

						ImGui.TableNextRow();

						ImGui.TableNextColumn();
						if (ImGui.Selectable(Inline.Span(i), false, ImGuiSelectableFlags.SpanAllColumns))
							_enemyHitLog = EnemyHitLogBuilder.Build(replay.Cache.Events, i);

						ImGui.TableNextColumn();
						ImGui.TextColored(entityType.GetColor(), EnumUtils.EntityTypeShortNames[entityType.Value]);
					}

					ImGui.EndTable();
				}
			}

			ImGui.EndChild(); // ReplayEntitiesChild
		}

		ImGui.EndChild(); // ReplayEntities

		ImGui.SameLine();

		if (ImGui.BeginChild("ReplayEnemyHitLog"))
			RenderEnemyHitLog(replay.StartTime);

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
			ImGui.Text("NOTE: This feature is a work in progress and not entirely accurate for some enemy types.");

			ImGui.Text(Inline.Span($"Enemy hit log for {EnumUtils.EntityTypeShortNames[_enemyHitLog.EntityType]} (id {_enemyHitLog.EntityId}):"));

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
				ImGui.Text(Inline.Span($"{TimeUtils.TickToTime(_enemyHitLog.SpawnTick, startTime):0.0000} ({_enemyHitLog.SpawnTick})"));
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
					ImGui.Text(Inline.Span($"{TimeUtils.TickToTime(hit.Tick, startTime):0.0000} ({hit.Tick})"));
					ImGui.TableNextColumn();
					ImGui.TextColored(hit.Hp < 0 ? Color.Red : Color.Lerp(Color.Red, Color.White, hit.Hp / (float)initialHp), hit.Hp <= 0 ? "Dead" : Inline.Span($"{hit.Hp} / {initialHp}"));
					ImGui.TableNextColumn();
					ImGui.TextColored(hit.Damage > 0 ? Color.Red : Color.White, Inline.Span(hit.Damage));
					ImGui.TableNextColumn();
					ImGui.Text(EnumUtils.DaggerTypeNames[hit.DaggerType]);
					ImGui.TableNextColumn();
					ImGui.Text(Inline.Span(hit.UserData));
				}

				ImGui.EndTable();
			}
		}
	}
}
