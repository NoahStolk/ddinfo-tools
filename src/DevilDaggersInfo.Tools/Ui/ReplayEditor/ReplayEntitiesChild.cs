using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Extensions;
using DevilDaggersInfo.Core.Replay.PostProcessing.HitLog;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Utils;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

internal sealed class ReplayEntitiesChild(ResourceManager resourceManager)
{
	private int _startId;
	private bool _showEnemies = true;
	private bool _showDaggers = true;
	private EnemyHitLog? _enemyHitLog;

	public void Reset()
	{
		_enemyHitLog = null;
	}

	public void Render(EditorReplayModel replay)
	{
		if (ImGui.BeginChild("ReplayEntities", new Vector2(320, 0)))
		{
			const int maxIds = 1000;

			Vector2 iconSize = new(16);
			if (ImGui.ImageButton("Start"u8, resourceManager.InternalResources.ArrowStartTexture.Id, iconSize))
				_startId = 0;
			ImGui.SameLine();
			if (ImGui.ImageButton("Back"u8, resourceManager.InternalResources.ArrowLeftTexture.Id, iconSize))
				_startId = Math.Max(0, _startId - maxIds);
			ImGui.SameLine();
			if (ImGui.ImageButton("Forward"u8, resourceManager.InternalResources.ArrowRightTexture.Id, iconSize))
				_startId = Math.Min(replay.Cache.Entities.Count - maxIds, _startId + maxIds);
			ImGui.SameLine();
			if (ImGui.ImageButton("End"u8, resourceManager.InternalResources.ArrowEndTexture.Id, iconSize))
				_startId = replay.Cache.Entities.Count - maxIds;

			_startId = Math.Max(0, Math.Min(_startId, replay.Cache.Entities.Count - maxIds));

			ImGui.Text(Inline.Utf8($"Showing {_startId} - {_startId + maxIds - 1} of {replay.Cache.Entities.Count + 1}"));

			ImGui.Checkbox("Show enemies", ref _showEnemies);
			ImGui.SameLine();
			ImGui.Checkbox("Show daggers", ref _showDaggers);

			if (ImGui.BeginChild("ReplayEntitiesChild", new Vector2(0, 0)))
				RenderReplayEntitiesTable(replay, maxIds);

			ImGui.EndChild();
		}

		ImGui.EndChild();

		ImGui.SameLine();

		if (ImGui.BeginChild("ReplayEnemyHitLog"))
			RenderEnemyHitLog(replay.StartTime);

		ImGui.EndChild();
	}

	private void RenderReplayEntitiesTable(EditorReplayModel replay, int maxIds)
	{
		if (!ImGui.BeginTable("ReplayEntitiesTable", 2, ImGuiTableFlags.SizingStretchSame))
			return;

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
			if (ImGui.Selectable(Inline.Utf8(i), false, ImGuiSelectableFlags.SpanAllColumns))
				_enemyHitLog = EnemyHitLogBuilder.Build(replay.Cache.Events, i);

			ImGui.TableNextColumn();
			ImGui.TextColored(entityType.GetColor(), entityType.Value.AsUtf8ShortSpan());
		}

		ImGui.EndTable();
	}

	private void RenderEnemyHitLog(float startTime)
	{
		if (_enemyHitLog == null)
		{
			ImGui.Text("Select an entity from the list.");
		}
		else
		{
			ImGui.Text("NOTE: This feature is a work in progress and not entirely accurate for some enemy types.");

			ImGui.Text(Inline.Utf8($"Enemy hit log for {_enemyHitLog.EntityType.AsUtf8ShortSpan()} (id {_enemyHitLog.EntityId}):"));

			int initialHp = _enemyHitLog.EntityType.GetInitialHp();
			RenderEnemyHitLogTable(_enemyHitLog, startTime, initialHp);
		}
	}

	private static void RenderEnemyHitLogTable(EnemyHitLog enemyHitLog, float startTime, int initialHp)
	{
		if (!ImGui.BeginTable("EnemyHitLog", 5, ImGuiTableFlags.SizingStretchSame))
			return;

		ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.None, 128);
		ImGui.TableSetupColumn("HP", ImGuiTableColumnFlags.None, 128);
		ImGui.TableSetupColumn("Damage", ImGuiTableColumnFlags.None, 128);
		ImGui.TableSetupColumn("Dagger Type", ImGuiTableColumnFlags.None, 128);
		ImGui.TableSetupColumn("User Data", ImGuiTableColumnFlags.None, 128);
		ImGui.TableHeadersRow();

		ImGui.TableNextRow();
		ImGui.TableNextColumn();
		ImGui.Text(Inline.Utf8($"{TimeUtils.TickToTime(enemyHitLog.SpawnTick, startTime):0.0000} ({enemyHitLog.SpawnTick})"));
		ImGui.TableNextColumn();
		ImGui.TextColored(new Vector4(0, 1, 0, 1), "Spawn");
		ImGui.TableNextColumn();
		ImGui.Text("-");
		ImGui.TableNextColumn();
		ImGui.Text("-");
		ImGui.TableNextColumn();
		ImGui.Text("-");

		Vector4 red = new(1, 0, 0, 1);

		for (int i = 0; i < enemyHitLog.Hits.Count; i++)
		{
			EnemyHitLogEvent hit = enemyHitLog.Hits[i];

			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImGui.Text(Inline.Utf8($"{TimeUtils.TickToTime(hit.Tick, startTime):0.0000} ({hit.Tick})"));
			ImGui.TableNextColumn();
			ImGui.TextColored(hit.Hp < 0 ? red : Vector4.Lerp(red, Vector4.One, hit.Hp / (float)initialHp), hit.Hp <= 0 ? "Dead"u8 : Inline.Utf8($"{hit.Hp} / {initialHp}"));
			ImGui.TableNextColumn();
			ImGui.TextColored(hit.Damage > 0 ? red : Vector4.One, Inline.Utf8(hit.Damage));
			ImGui.TableNextColumn();
			ImGui.Text(hit.DaggerType.AsUtf8Span());
			ImGui.TableNextColumn();
			ImGui.Text(Inline.Utf8(hit.UserData));
		}

		ImGui.EndTable();
	}
}
