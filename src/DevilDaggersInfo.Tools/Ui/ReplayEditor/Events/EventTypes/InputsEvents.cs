using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class InputsEvents : IEventTypeRenderer<InputsEventData>
{
	private static readonly string[] _jumpTypeNamesArray = EnumUtils.JumpTypeNames.Values.ToArray();
	private static readonly string[] _shootTypeNamesArray = EnumUtils.ShootTypeNames.Values.ToArray();

	public static int ColumnCount => 10;
	public static int ColumnCountData => 9;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnIndex();
		SetupColumnsData();
	}

	public static void SetupColumnsData()
	{
		ImGui.TableSetupColumn("Left", ImGuiTableColumnFlags.WidthFixed, 64);
		ImGui.TableSetupColumn("Right", ImGuiTableColumnFlags.WidthFixed, 64);
		ImGui.TableSetupColumn("Forward", ImGuiTableColumnFlags.WidthFixed, 64);
		ImGui.TableSetupColumn("Backward", ImGuiTableColumnFlags.WidthFixed, 64);
		ImGui.TableSetupColumn("Jump", ImGuiTableColumnFlags.WidthFixed, 96);
		ImGui.TableSetupColumn("Shoot", ImGuiTableColumnFlags.WidthFixed, 96);
		ImGui.TableSetupColumn("Shoot Homing", ImGuiTableColumnFlags.WidthFixed, 96);
		ImGui.TableSetupColumn("Mouse X", ImGuiTableColumnFlags.WidthFixed, 64);
		ImGui.TableSetupColumn("Mouse Y", ImGuiTableColumnFlags.WidthFixed, 64);
	}

	public static void Render(int eventIndex, int entityId, InputsEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		RenderData(eventIndex, e, replayEventsData);
	}

	public static void RenderData(int eventIndex, InputsEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnCheckbox(eventIndex, nameof(InputsEventData.Left), ref e.Left, "On", "Off");
		EventTypeRendererUtils.NextColumnCheckbox(eventIndex, nameof(InputsEventData.Right), ref e.Right, "On", "Off");
		EventTypeRendererUtils.NextColumnCheckbox(eventIndex, nameof(InputsEventData.Forward), ref e.Forward, "On", "Off");
		EventTypeRendererUtils.NextColumnCheckbox(eventIndex, nameof(InputsEventData.Backward), ref e.Backward, "On", "Off");
		EventTypeRendererUtils.NextColumnInputByteEnum(eventIndex, nameof(InputsEventData.Jump), ref e.Jump, EnumUtils.JumpTypes, _jumpTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputByteEnum(eventIndex, nameof(InputsEventData.Shoot), ref e.Shoot, EnumUtils.ShootTypes, _shootTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputByteEnum(eventIndex, nameof(InputsEventData.ShootHoming), ref e.ShootHoming, EnumUtils.ShootTypes, _shootTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputShort(eventIndex, nameof(InputsEventData.MouseX), ref e.MouseX);
		EventTypeRendererUtils.NextColumnInputShort(eventIndex, nameof(InputsEventData.MouseY), ref e.MouseY);
	}

	public static void RenderEdit(int eventIndex, InputsEventData e, ReplayEventsData replayEventsData)
	{
	}
}
