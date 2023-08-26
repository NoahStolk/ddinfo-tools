using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Events.Interfaces;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor;

public static class ReplayEvents
{
	private static readonly EventCache _eventCache = new();

	private static int _startTick;
	private static bool _showEvents = true;
	private static bool _showTicksWithoutEvents = true;

	private static ImGuiTableFlags EventTableFlags => ImGuiTableFlags.Borders | ImGuiTableFlags.NoPadOuterX;
	private static ImGuiTableColumnFlags EventTableColumnFlags => ImGuiTableColumnFlags.None;

	public static void Render(ReplayEventsData eventsData)
	{
		const int maxTicks = 30;

		Vector2 iconSize = new(16);
		if (ImGuiImage.ImageButton("Start", Root.InternalResources.ArrowStartTexture.Handle, iconSize))
			_startTick = 0;
		ImGui.SameLine();
		if (ImGuiImage.ImageButton("Back", Root.InternalResources.ArrowLeftTexture.Handle, iconSize))
			_startTick = Math.Max(0, _startTick - maxTicks);
		ImGui.SameLine();
		if (ImGuiImage.ImageButton("Forward", Root.InternalResources.ArrowRightTexture.Handle, iconSize))
			_startTick = Math.Min(eventsData.TickCount - maxTicks, _startTick + maxTicks);
		ImGui.SameLine();
		if (ImGuiImage.ImageButton("End", Root.InternalResources.ArrowEndTexture.Handle, iconSize))
			_startTick = eventsData.TickCount - maxTicks;

		ImGui.Checkbox("Show events", ref _showEvents);
		ImGui.SameLine();
		ImGui.Checkbox("Show ticks without events", ref _showTicksWithoutEvents);

		Color rowOdd = Color.Gray(0.1f);
		Color rowEven = Color.Gray(0.05f);
		if (ImGui.BeginTable("ReplayEventsTable", 4, ImGuiTableFlags.None))
		{
			ImGui.TableSetupColumn("Tick", ImGuiTableColumnFlags.None, 64);
			ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.None, 64);
			ImGui.TableSetupColumn("Inputs", ImGuiTableColumnFlags.None, 128);
			ImGui.TableSetupColumn("Events", ImGuiTableColumnFlags.None, 384);
			ImGui.TableHeadersRow();

			for (int i = _startTick; i < Math.Min(_startTick + maxTicks, eventsData.TickCount); i++)
			{
				int offset = eventsData.EventOffsetsPerTick[i];
				int count = eventsData.EventOffsetsPerTick[i + 1] - offset;

				IInputsEvent? inputsEvent = null;
				_eventCache.Clear();
				for (int j = offset; j < offset + count; j++)
				{
					IEvent @event = eventsData.Events[j];
					if (@event is IInputsEvent ie)
						inputsEvent = ie;
					else
						_eventCache.Add(j, @event);
				}

				if (!_showTicksWithoutEvents && _eventCache.Count == 0)
					continue;

				ImGui.TableNextRow();

				ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, ImGui.GetColorU32(i % 2 == 0 ? rowEven : rowOdd));

				ImGui.TableNextColumn();
				ImGui.Text(UnsafeSpan.Get(i));
				ImGui.TableNextColumn();
				ImGui.Text(UnsafeSpan.Get(i / 60f, StringFormats.TimeFormat));
				ImGui.TableNextColumn();
				if (inputsEvent != null)
					RenderInputsEvent(inputsEvent);
				else
					ImGui.Text("ERROR: no inputs");

				ImGui.TableNextColumn();

				if (_showEvents)
				{
					if (_eventCache.HitEvents.Count > 0)
						RenderHitEvents(_eventCache.HitEvents);

					if (_eventCache.SquidSpawnEvents.Count > 0)
						RenderSquidSpawnEvents(_eventCache.SquidSpawnEvents);

					if (_eventCache.SpiderSpawnEvents.Count > 0)
						RenderSpiderSpawnEvents(_eventCache.SpiderSpawnEvents);
				}
			}

			ImGui.EndTable();
		}
	}

	private static void RenderInputsEvent(IInputsEvent ie)
	{
		ImGui.TextColored(ie.Forward ? Color.Red : Color.White, "W");
		ImGui.SameLine();
		ImGui.TextColored(ie.Left ? Color.Red : Color.White, "A");
		ImGui.SameLine();
		ImGui.TextColored(ie.Backward ? Color.Red : Color.White, "S");
		ImGui.SameLine();
		ImGui.TextColored(ie.Right ? Color.Red : Color.White, "D");
		ImGui.SameLine();
		ImGui.TextColored(GetJumpTypeColor(ie.Jump), "[Space]");
		ImGui.SameLine();
		ImGui.TextColored(GetShootTypeColor(ie.Shoot), "[LMB]");
		ImGui.SameLine();
		ImGui.TextColored(GetShootTypeColor(ie.ShootHoming), "[RMB]");
		ImGui.SameLine();
		ImGui.TextColored(ie.MouseX == 0 ? Color.White : Color.Red, UnsafeSpan.Get(ie.MouseX));
		ImGui.SameLine();
		ImGui.TextColored(ie.MouseY == 0 ? Color.White : Color.Red, UnsafeSpan.Get(ie.MouseY));

		if (ie is InitialInputsEvent initial)
			ImGui.TextColored(Color.White, $"Look Speed: {initial.LookSpeed}");
	}

	private static void RenderHitEvents(IReadOnlyList<(int Index, HitEvent Event)> hitEvents)
	{
		ImGui.TextColored(EnemiesV3_2.Skull4.Color, "Hit events");

		if (ImGui.BeginTable("HitEvents", 4, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id A", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Entity Id B", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("User Data", EventTableColumnFlags, 128);
			ImGui.TableHeadersRow();

			for (int i = 0; i < hitEvents.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, HitEvent e) = hitEvents[i];
				NextColumnText(UnsafeSpan.Get(index));
				NextColumnText(UnsafeSpan.Get(e.EntityIdA));
				NextColumnText(UnsafeSpan.Get(e.EntityIdB));
				NextColumnText(UnsafeSpan.Get(e.UserData));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderSquidSpawnEvents(IReadOnlyList<(int Index, SquidSpawnEvent Event)> squidSpawnEvents)
	{
		ImGui.TextColored(EnemiesV3_2.Squid3.Color, "Squid Spawn events");

		if (ImGui.BeginTable("SquidSpawnEvents", 6, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Type", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Position", EventTableColumnFlags, 192);
			ImGui.TableSetupColumn("Direction", EventTableColumnFlags, 192);
			ImGui.TableSetupColumn("Rotation", EventTableColumnFlags, 128);
			ImGui.TableHeadersRow();

			for (int i = 0; i < squidSpawnEvents.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, SquidSpawnEvent e) = squidSpawnEvents[i];
				NextColumnText(UnsafeSpan.Get(index));
				NextColumnText(GetSquidTypeText(e.SquidType));
				NextColumnText(UnsafeSpan.Get(e.Position, "0.00"));
				NextColumnText(UnsafeSpan.Get(e.Direction, "0.00"));
				NextColumnText(UnsafeSpan.Get(e.RotationInRadians, "0.00"));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderSpiderSpawnEvents(IReadOnlyList<(int Index, SpiderSpawnEvent Event)> spiderSpawnEvents)
	{
		ImGui.TextColored(EnemiesV3_2.Spider2.Color, "Spider Spawn events");

		if (ImGui.BeginTable("SpiderSpawnEvents", 5, EventTableFlags))
		{
			ImGui.TableSetupColumn("Event Index", EventTableColumnFlags, 96);
			ImGui.TableSetupColumn("Entity Id", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Type", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("?", EventTableColumnFlags, 128);
			ImGui.TableSetupColumn("Position", EventTableColumnFlags, 196);
			ImGui.TableHeadersRow();

			for (int i = 0; i < spiderSpawnEvents.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, SpiderSpawnEvent e) = spiderSpawnEvents[i];
				NextColumnText(UnsafeSpan.Get(index));
				NextColumnText(UnsafeSpan.Get(e.EntityId));
				NextColumnText(GetSpiderTypeText(e.SpiderType));
				NextColumnText(UnsafeSpan.Get(e.A));
				NextColumnText(UnsafeSpan.Get(e.Position, "0.00"));
			}

			ImGui.EndTable();
		}
	}

	private static void NextColumnText(ReadOnlySpan<char> text)
	{
		ImGui.TableNextColumn();
		ImGui.Text(text);
	}

	private static ReadOnlySpan<char> GetSquidTypeText(SquidType squidType) => squidType switch
	{
		SquidType.Squid1 => "Squid I",
		SquidType.Squid2 => "Squid II",
		SquidType.Squid3 => "Squid III",
		_ => throw new UnreachableException(),
	};

	private static ReadOnlySpan<char> GetSpiderTypeText(SpiderType spiderType) => spiderType switch
	{
		SpiderType.Spider1 => "Spider I",
		SpiderType.Spider2 => "Spider II",
		_ => throw new UnreachableException(),
	};

	private static Color GetJumpTypeColor(JumpType jumpType) => jumpType switch
	{
		JumpType.Hold => Color.Orange,
		JumpType.StartedPress => Color.Red,
		_ => Color.White,
	};

	private static Color GetShootTypeColor(ShootType shootType) => shootType switch
	{
		ShootType.Hold => Color.Orange,
		ShootType.Release => Color.Red,
		_ => Color.White,
	};
}
