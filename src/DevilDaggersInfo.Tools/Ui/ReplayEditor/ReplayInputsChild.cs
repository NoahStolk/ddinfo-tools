using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Utils;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

internal sealed class ReplayInputsChild(ResourceManager resourceManager)
{
	private int _startTick;
	private int _endTick;

	public void Render(EditorReplayModel replay)
	{
		const int maxTicks = 60;
		const int height = 64;

		if (ImGui.BeginChild("TickNavigation", new Vector2(448 + 8, height)))
		{
			const int padding = 4;
			ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(padding));

			Vector2 iconSize = new(16);
			if (ImGui.ImageButton("Start"u8, resourceManager.InternalResources.ArrowStartTexture.Id, iconSize))
				_startTick = 0;
			ImGui.SameLine();
			if (ImGui.ImageButton("Back"u8, resourceManager.InternalResources.ArrowLeftTexture.Id, iconSize))
				_startTick = Math.Max(0, _startTick - maxTicks);
			ImGui.SameLine();
			if (ImGui.ImageButton("Forward"u8, resourceManager.InternalResources.ArrowRightTexture.Id, iconSize))
				_startTick = Math.Min(replay.TickCount - maxTicks, _startTick + maxTicks);
			ImGui.SameLine();
			if (ImGui.ImageButton("End"u8, resourceManager.InternalResources.ArrowEndTexture.Id, iconSize))
				_startTick = replay.TickCount - maxTicks;

			_startTick = Math.Max(0, Math.Min(_startTick, replay.TickCount - maxTicks));
			_endTick = Math.Min(_startTick + maxTicks - 1, replay.TickCount);

			ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(padding));
			ImGui.Text(Inline.Utf8(
				$"""
				Showing {_startTick} - {_endTick} of {replay.TickCount} ticks
				{TimeUtils.TickToTime(_startTick, replay.StartTime):0.0000} - {TimeUtils.TickToTime(_endTick, replay.StartTime):0.0000}
				"""));
		}

		ImGui.EndChild();

		if (ImGui.BeginChild("ReplayInputsChild", new Vector2(0, 0)))
		{
			RenderReplayInputsTable(replay);
		}

		ImGui.EndChild();
	}

	private void RenderReplayInputsTable(EditorReplayModel replay)
	{
		if (!ImGui.BeginTable("ReplayInputsTable", 2, ImGuiTableFlags.BordersInnerH | ImGuiTableFlags.SizingStretchSame))
			return;

		ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("Inputs", ImGuiTableColumnFlags.None, 384);
		ImGui.TableHeadersRow();

		ImGui.TextColored(Vector4.One, Inline.Utf8($"Look Speed: {replay.LookSpeed}"));

		int i = 0;
		foreach (InputsEventData inputs in replay.InputsEvents)
		{
			if (i < _startTick)
			{
				i++;
				continue;
			}

			ImGui.TableNextRow();

			ImGui.TableNextColumn();
			ImGui.Text(Inline.Utf8($"{TimeUtils.TickToTime(i, replay.StartTime):0.0000} ({i})"));

			ImGui.TableNextColumn();
			RenderInputsEvent(inputs.Left, inputs.Right, inputs.Forward, inputs.Backward, inputs.Jump, inputs.Shoot, inputs.ShootHoming, inputs.MouseX, inputs.MouseY);

			i++;
			if (i > _endTick)
				break;
		}

		ImGui.EndTable();
	}

	private static void RenderInputsEvent(
		bool left,
		bool right,
		bool forward,
		bool backward,
		JumpType jump,
		ShootType shoot,
		ShootType shootHoming,
		short mouseX,
		short mouseY)
	{
		ImGui.TextColored(forward ? new Vector4(1, 0, 0, 1) : Vector4.One, "W");
		ImGui.SameLine();
		ImGui.TextColored(left ? new Vector4(1, 0, 0, 1) : Vector4.One, "A");
		ImGui.SameLine();
		ImGui.TextColored(backward ? new Vector4(1, 0, 0, 1) : Vector4.One, "S");
		ImGui.SameLine();
		ImGui.TextColored(right ? new Vector4(1, 0, 0, 1) : Vector4.One, "D");
		ImGui.SameLine();
		ImGui.TextColored(GetJumpTypeColor(jump), "[Space]");
		ImGui.SameLine();
		ImGui.TextColored(GetShootTypeColor(shoot), "[LMB]");
		ImGui.SameLine();
		ImGui.TextColored(GetShootTypeColor(shootHoming), "[RMB]");
		ImGui.SameLine();
		ImGui.TextColored(mouseX == 0 ? Vector4.One : new Vector4(1, 0, 0, 1), Inline.Utf8($"X:{mouseX}"));
		ImGui.SameLine();
		ImGui.TextColored(mouseY == 0 ? Vector4.One : new Vector4(1, 0, 0, 1), Inline.Utf8($"Y:{mouseY}"));

		static Vector4 GetJumpTypeColor(JumpType jumpType) => jumpType switch
		{
			JumpType.Hold => new Vector4(1, 0.5f, 0, 1),
			JumpType.StartedPress => new Vector4(1, 0, 0, 1),
			_ => Vector4.One,
		};

		static Vector4 GetShootTypeColor(ShootType shootType) => shootType switch
		{
			ShootType.Hold => new Vector4(1, 0.5f, 0, 1),
			ShootType.Release => new Vector4(1, 0, 0, 1),
			_ => Vector4.One,
		};
	}
}
