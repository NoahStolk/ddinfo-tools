using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class InputsEvents : IEventTypeRenderer<InputsEventData>
{
	private static readonly string[] _jumpTypeNamesArray = EnumUtils.JumpTypeNames.Values.ToArray();
	private static readonly string[] _shootTypeNamesArray = EnumUtils.ShootTypeNames.Values.ToArray();

	public static int ColumnCount => 12;
	public static int ColumnCountData => 10;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnActions();
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
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		RenderData(eventIndex, e, replayEventsData);
	}

	public static void RenderData(int eventIndex, InputsEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnCheckbox(eventIndex, nameof(InitialInputsEventData.Left), ref e.Left, "On", "Off");
		EventTypeRendererUtils.NextColumnCheckbox(eventIndex, nameof(InitialInputsEventData.Right), ref e.Right, "On", "Off");
		EventTypeRendererUtils.NextColumnCheckbox(eventIndex, nameof(InitialInputsEventData.Forward), ref e.Forward, "On", "Off");
		EventTypeRendererUtils.NextColumnCheckbox(eventIndex, nameof(InitialInputsEventData.Backward), ref e.Backward, "On", "Off");
		EventTypeRendererUtils.NextColumnInputByteEnum(eventIndex, nameof(InitialInputsEventData.Jump), ref e.Jump, EnumUtils.JumpTypes, _jumpTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputByteEnum(eventIndex, nameof(InitialInputsEventData.Shoot), ref e.Shoot, EnumUtils.ShootTypes, _shootTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputByteEnum(eventIndex, nameof(InitialInputsEventData.ShootHoming), ref e.ShootHoming, EnumUtils.ShootTypes, _shootTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputShort(eventIndex, nameof(InitialInputsEventData.MouseX), ref e.MouseX);
		EventTypeRendererUtils.NextColumnInputShort(eventIndex, nameof(InitialInputsEventData.MouseY), ref e.MouseY);
	}
}
