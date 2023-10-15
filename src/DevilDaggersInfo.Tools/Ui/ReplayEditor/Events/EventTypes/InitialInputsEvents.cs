using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class InitialInputsEvents : IEventTypeRenderer<InitialInputsEvent>
{
	private static readonly string[] _jumpTypeNamesArray = EnumUtils.JumpTypeNames.Values.ToArray();
	private static readonly string[] _shootTypeNamesArray = EnumUtils.ShootTypeNames.Values.ToArray();

	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Left", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Right", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Forward", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Backward", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Jump", ImGuiTableColumnFlags.WidthFixed, 128),
		new("Shoot", ImGuiTableColumnFlags.WidthFixed, 128),
		new("Shoot Homing", ImGuiTableColumnFlags.WidthFixed, 128),
		new("Mouse X", ImGuiTableColumnFlags.WidthFixed, 96),
		new("Mouse Y", ImGuiTableColumnFlags.WidthFixed, 96),
		new("Look Speed", ImGuiTableColumnFlags.WidthFixed, 96),
	};

	public static void Render(int index, InitialInputsEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnEventIndex(index);
		EventTypeRendererUtils.NextColumnCheckbox(index, nameof(InitialInputsEvent.Left), ref e.Left, "On", "Off");
		EventTypeRendererUtils.NextColumnCheckbox(index, nameof(InitialInputsEvent.Right), ref e.Right, "On", "Off");
		EventTypeRendererUtils.NextColumnCheckbox(index, nameof(InitialInputsEvent.Forward), ref e.Forward, "On", "Off");
		EventTypeRendererUtils.NextColumnCheckbox(index, nameof(InitialInputsEvent.Backward), ref e.Backward, "On", "Off");
		EventTypeRendererUtils.NextColumnInputEnum(index, nameof(InitialInputsEvent.Jump), ref e.Jump, EnumUtils.JumpTypes, _jumpTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputEnum(index, nameof(InitialInputsEvent.Shoot), ref e.Shoot, EnumUtils.ShootTypes, _shootTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputEnum(index, nameof(InitialInputsEvent.ShootHoming), ref e.ShootHoming, EnumUtils.ShootTypes, _shootTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputShort(index, nameof(InitialInputsEvent.MouseX), ref e.MouseX);
		EventTypeRendererUtils.NextColumnInputShort(index, nameof(InitialInputsEvent.MouseY), ref e.MouseY);
		EventTypeRendererUtils.NextColumnInputFloat(index, nameof(InitialInputsEvent.LookSpeed), ref e.LookSpeed, "%.2f");
	}
}
