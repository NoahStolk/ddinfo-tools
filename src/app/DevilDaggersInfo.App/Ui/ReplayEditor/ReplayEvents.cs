using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Events.Interfaces;
using ImGuiNET;
using System.Diagnostics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor;

public static class ReplayEvents
{
	public static void Render(ReplayEventsData eventsData)
	{
		const int maxTicks = 30;

		if (ImGui.BeginTable("ReplayEventsTable", 3, ImGuiTableFlags.None))
		{
			ImGui.TableSetupColumn("Tick", ImGuiTableColumnFlags.None, 64);
			ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.None, 64);
			ImGui.TableSetupColumn("Events", ImGuiTableColumnFlags.None, 384);
			ImGui.TableHeadersRow();

			int tickIndex = 0;
			for (int i = 0; i < eventsData.Events.Count; i++)
			{
				ImGui.TableNextRow();
				IEvent e = eventsData.Events[i];
				if (e is IInputsEvent ie)
				{
					ImGui.TableNextColumn();
					ImGui.Text(UnsafeSpan.Get(tickIndex));
					ImGui.TableNextColumn();
					ImGui.Text(UnsafeSpan.Get(tickIndex / 60f, StringFormats.TimeFormat));
					ImGui.TableNextColumn();
					RenderInputsEvent(ie);

					tickIndex++;
				}
				else
				{
					ImGui.TableNextColumn();
					ImGui.TableNextColumn();
					ImGui.TableNextColumn();

					if (e is SquidSpawnEvent sse)
					{
						RenderSquidSpawnEvent(sse);
					}
					else if (e is HitEvent he)
					{
						RenderHitEvent(he);
					}
					else
					{
						ImGui.Text("TODO");
					}
				}

				if (tickIndex >= Math.Min(maxTicks, eventsData.TickCount))
					break;
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
		ImGui.TextColored(GetJumpTypeColor(ie.Jump), "[Jump]");
		ImGui.SameLine();
		ImGui.TextColored(GetShootTypeColor(ie.Shoot), "[Shoot]");
		ImGui.SameLine();
		ImGui.TextColored(GetShootTypeColor(ie.ShootHoming), "[Shoot Homing]");
		ImGui.SameLine();
		ImGui.TextColored(ie.MouseX == 0 ? Color.White : Color.Red, UnsafeSpan.Get(ie.MouseX));
		ImGui.SameLine();
		ImGui.TextColored(ie.MouseY == 0 ? Color.White : Color.Red, UnsafeSpan.Get(ie.MouseY));

		if (ie is InitialInputsEvent initial)
			ImGui.TextColored(Color.White, $"Look Speed: {initial.LookSpeed}");
	}

	private static void RenderSquidSpawnEvent(SquidSpawnEvent e)
	{
		if (ImGui.BeginTable("SquidSpawn", 5, ImGuiTableFlags.None))
		{
			ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.None, 64);
			ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.None, 64);
			ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.None, 128);
			ImGui.TableSetupColumn("Direction", ImGuiTableColumnFlags.None, 128);
			ImGui.TableSetupColumn("Rotation", ImGuiTableColumnFlags.None, 128);
			ImGui.TableHeadersRow();

			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImGui.Text(UnsafeSpan.Get(e.EntityId));
			ImGui.TableNextColumn();
			ImGui.TextColored(Color.White, GetSquidTypeText(e.SquidType));
			ImGui.TableNextColumn();
			ImGui.TextColored(Color.White, UnsafeSpan.Get(e.Position, "0.00"));
			ImGui.TableNextColumn();
			ImGui.TextColored(Color.White, UnsafeSpan.Get(e.Direction, "0.00"));
			ImGui.TableNextColumn();
			ImGui.TextColored(Color.White, UnsafeSpan.Get(e.RotationInRadians, "0.00"));

			ImGui.EndTable();
		}
	}

	private static void RenderHitEvent(HitEvent e)
	{
		if (ImGui.BeginTable("HitEvent", 3, ImGuiTableFlags.None))
		{
			ImGui.TableSetupColumn("Id A", ImGuiTableColumnFlags.None, 64);
			ImGui.TableSetupColumn("Id B", ImGuiTableColumnFlags.None, 64);
			ImGui.TableSetupColumn("User Data", ImGuiTableColumnFlags.None, 128);
			ImGui.TableHeadersRow();

			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImGui.Text(UnsafeSpan.Get(e.EntityIdA));
			ImGui.TableNextColumn();
			ImGui.Text(UnsafeSpan.Get(e.EntityIdB));
			ImGui.TableNextColumn();
			ImGui.Text(UnsafeSpan.Get(e.UserData));

			ImGui.EndTable();
		}
	}

	private static ReadOnlySpan<char> GetSquidTypeText(SquidType squidType) => squidType switch
	{
		SquidType.Squid1 => "Squid I",
		SquidType.Squid2 => "Squid II",
		SquidType.Squid3 => "Squid III",
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
