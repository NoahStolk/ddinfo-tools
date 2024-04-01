using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;

public static class ReplayEventsViewerChild
{
	private static readonly Dictionary<EventType, bool> _eventTypeEnabled = Enum.GetValues<EventType>().ToDictionary(et => et, _ => true);

	private static int _startIndex;
	private static int _targetIndex;

	public static void Reset()
	{
		_startIndex = 0;
		_targetIndex = 0;
	}

	private static void ToggleAll(bool enabled)
	{
		for (int i = 0; i < EnumUtils.EventTypes.Count; i++)
		{
			EventType eventType = EnumUtils.EventTypes[i];
			_eventTypeEnabled[eventType] = enabled;
		}
	}

	public static void Render(EditorReplayModel replay)
	{
		const int maxEvents = 60;
		const int height = 216;
		const int filteringHeight = 160;

		ImGui.PushStyleColor(ImGuiCol.ChildBg, Color.Gray(0.13f));
		if (ImGui.BeginChild("NavigationAndFilteringWrapper", new(0, height)))
		{
			if (ImGui.BeginChild("TickNavigation", new(448 + 8, height)))
			{
				const int padding = 4;
				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(padding));

				Vector2 iconSize = new(16);
				if (ImGuiImage.ImageButton("Start", Root.InternalResources.ArrowStartTexture.Id, iconSize))
					_startIndex = 0;
				ImGui.SameLine();
				if (ImGuiImage.ImageButton("Back", Root.InternalResources.ArrowLeftTexture.Id, iconSize))
					_startIndex = Math.Max(0, _startIndex - maxEvents);
				ImGui.SameLine();
				if (ImGuiImage.ImageButton("Forward", Root.InternalResources.ArrowRightTexture.Id, iconSize))
					_startIndex = Math.Min(replay.Cache.Events.Count - maxEvents, _startIndex + maxEvents);
				ImGui.SameLine();
				if (ImGuiImage.ImageButton("End", Root.InternalResources.ArrowEndTexture.Id, iconSize))
					_startIndex = replay.Cache.Events.Count - maxEvents;

				ImGui.SameLine();
				ImGui.Text("Go to index:");
				ImGui.SameLine();
				ImGui.PushItemWidth(120);

				if (ImGui.InputInt("##target_index", ref _targetIndex))
					_startIndex = _targetIndex;

				if (ImGui.IsItemFocused() && ImGui.IsKeyPressed(ImGuiKey.Enter))
					_startIndex = _targetIndex;

				ImGui.PopItemWidth();

				_startIndex = Math.Max(0, Math.Min(_startIndex, replay.Cache.Events.Count - maxEvents));
				int endIndex = Math.Min(_startIndex + maxEvents - 1, replay.Cache.Events.Count);

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(padding));
				ImGui.Text(Inline.Span($"Showing {_startIndex} - {endIndex} of {replay.Cache.Events.Count} events"));
			}

			ImGui.EndChild(); // TickNavigation

			ImGui.SameLine();

			if (ImGui.BeginChild("EventTypeFiltering", new(0, height)))
			{
				const int checkboxesPerRow = 7;
				int rows = (int)Math.Ceiling((float)EnumUtils.EventTypes.Count / checkboxesPerRow);
				for (int i = 0; i < rows; i++)
				{
					if (ImGui.BeginChild(Inline.Span($"EventTypeFiltering{i}"), new(256, filteringHeight)))
					{
						int start = i * checkboxesPerRow;
						for (int j = start; j < Math.Min(EnumUtils.EventTypes.Count, start + checkboxesPerRow); j++)
						{
							EventType eventType = EnumUtils.EventTypes[j];
							bool temp = _eventTypeEnabled[eventType];
							if (ImGui.Checkbox(EventTypeRendererUtils.EventTypeNames[eventType], ref temp))
								_eventTypeEnabled[eventType] = temp;
						}
					}

					ImGui.EndChild();

					if (i < rows - 1)
						ImGui.SameLine();
				}

				ImGui.Separator();

				if (ImGui.Button("Enable all"))
					ToggleAll(true);

				ImGui.SameLine();

				if (ImGui.Button("Disable all"))
					ToggleAll(false);
			}

			ImGui.EndChild(); // EventTypeFiltering
		}

		ImGui.EndChild(); // NavigationAndFilteringWrapper

		ImGui.PopStyleColor();

		if (ImGui.BeginChild("ReplayEventsChild", new(0, 0)))
		{
			RenderEventsTable(replay, maxEvents);
		}

		ImGui.EndChild(); // ReplayEventsChild
	}

	private static void RenderEventsTable(EditorReplayModel replay, int maxTicks)
	{
		if (!ImGui.BeginTable("ReplayEventsTable", 4, ImGuiTableFlags.BordersInnerH))
			return;

		ImGui.TableSetupColumn("Index", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("Entity Id", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 192);
		ImGui.TableSetupColumn("Data", ImGuiTableColumnFlags.None, 384);
		ImGui.TableHeadersRow();

		for (int i = _startIndex; i < Math.Min(_startIndex + maxTicks, replay.Cache.Events.Count); i++)
		{
			ReplayEvent replayEvent = replay.Cache.Events[i];
			EventType eventType = replayEvent.GetEventType();
			if (!_eventTypeEnabled[eventType])
				continue;

			ImGui.TableNextRow();

			ImGui.TableNextColumn();
			ImGui.Text(Inline.Span(i));

			// TODO: Remove this check and get the entity id from the EditorReplayModelCache.
			if (replayEvent is EntitySpawnReplayEvent spawnReplayEvent)
			{
				EventTypeRendererUtils.NextColumnEntityId(replay, spawnReplayEvent.EntityId);
			}
			else
			{
				ImGui.TableNextColumn();
				ImGui.Text("-");
			}

			ImGui.TableNextColumn();
			ImGui.TextColored(eventType.GetColor(), EnumUtils.EventTypeFriendlyNames[eventType]);

			ImGui.TableNextColumn();
			switch (replayEvent.Data)
			{
				case BoidSpawnEventData boidSpawn: EventTypeRendererUtils.RenderTable<BoidSpawnEventData, BoidSpawnEvents>(eventType, boidSpawn, replay); break;
				case DaggerSpawnEventData daggerSpawn: EventTypeRendererUtils.RenderTable<DaggerSpawnEventData, DaggerSpawnEvents>(eventType, daggerSpawn, replay); break;
				case LeviathanSpawnEventData leviathanSpawn: EventTypeRendererUtils.RenderTable<LeviathanSpawnEventData, LeviathanSpawnEvents>(eventType, leviathanSpawn, replay); break;
				case PedeSpawnEventData pedeSpawn: EventTypeRendererUtils.RenderTable<PedeSpawnEventData, PedeSpawnEvents>(eventType, pedeSpawn, replay); break;
				case SpiderEggSpawnEventData spiderEggSpawn: EventTypeRendererUtils.RenderTable<SpiderEggSpawnEventData, SpiderEggSpawnEvents>(eventType, spiderEggSpawn, replay); break;
				case SpiderSpawnEventData spiderSpawn: EventTypeRendererUtils.RenderTable<SpiderSpawnEventData, SpiderSpawnEvents>(eventType, spiderSpawn, replay); break;
				case SquidSpawnEventData squidSpawn: EventTypeRendererUtils.RenderTable<SquidSpawnEventData, SquidSpawnEvents>(eventType, squidSpawn, replay); break;
				case ThornSpawnEventData thornSpawn: EventTypeRendererUtils.RenderTable<ThornSpawnEventData, ThornSpawnEvents>(eventType, thornSpawn, replay); break;
				case EntityOrientationEventData entityOrientation: EventTypeRendererUtils.RenderTable<EntityOrientationEventData, EntityOrientationEvents>(eventType, entityOrientation, replay); break;
				case EntityPositionEventData entityPosition: EventTypeRendererUtils.RenderTable<EntityPositionEventData, EntityPositionEvents>(eventType, entityPosition, replay); break;
				case EntityTargetEventData entityTarget: EventTypeRendererUtils.RenderTable<EntityTargetEventData, EntityTargetEvents>(eventType, entityTarget, replay); break;
				case HitEventData hit: EventTypeRendererUtils.RenderTable<HitEventData, HitEvents>(eventType, hit, replay); break;
				case InitialInputsEventData initialInputs: EventTypeRendererUtils.RenderTable<InitialInputsEventData, InitialInputsEvents>(eventType, initialInputs, replay); break;
				case InputsEventData inputs: EventTypeRendererUtils.RenderTable<InputsEventData, InputsEvents>(eventType, inputs, replay); break;
				case TransmuteEventData transmute: EventTypeRendererUtils.RenderTable<TransmuteEventData, TransmuteEvents>(eventType, transmute, replay); break;
				case EndEventData or GemEventData: ImGui.Text("No data"); break;
				default: throw new UnreachableException($"Unknown event type '{replayEvent.Data.GetType().Name}'.");
			}
		}

		ImGui.EndTable();
	}
}
