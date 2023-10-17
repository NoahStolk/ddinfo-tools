using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Numerics;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;

public static class InsertEventPopup
{
	private const int _tempEntityId = 1;
	private static readonly BoidSpawnEventData _boidSpawnEventData = new(1, BoidType.Skull1, Int16Vec3.Zero, Int16Mat3x3.Identity, Vector3.Zero, 0f);
	private static readonly DaggerSpawnEventData _daggerSpawnEventData = new(-1, Int16Vec3.Zero, Int16Mat3x3.Identity, false, DaggerType.Level1);
	private static readonly DeathEventData _deathEventData = new(0);
	private static readonly EndEventData _endEventData = new();
	private static readonly EntityOrientationEventData _entityOrientationEventData = new(_tempEntityId, Int16Mat3x3.Identity);
	private static readonly EntityPositionEventData _entityPositionEventData = new(_tempEntityId, Int16Vec3.Zero);
	private static readonly EntityTargetEventData _entityTargetEventData = new(_tempEntityId, Int16Vec3.Zero);
	private static readonly GemEventData _gemEventData = new();
	private static readonly HitEventData _hitEventData = new(_tempEntityId, _tempEntityId, 0);
	private static readonly InitialInputsEventData _initialInputsEventData = new(false, false, false, false, JumpType.None, ShootType.None, ShootType.None, 0, 0, 2);
	private static readonly InputsEventData _inputsEventData = new(false, false, false, false, JumpType.None, ShootType.None, ShootType.None, 0, 0);
	private static readonly LeviathanSpawnEventData _leviathanSpawnEventData = new(-1);
	private static readonly PedeSpawnEventData _pedeSpawnEventData = new(PedeType.Centipede, -1, Vector3.Zero, Vector3.Zero, Matrix3x3.Identity);
	private static readonly SpiderEggSpawnEventData _spiderEggSpawnEventData = new(_tempEntityId, Vector3.Zero, Vector3.Zero);
	private static readonly SpiderSpawnEventData _spiderSpawnEventData = new(SpiderType.Spider1, -1, Vector3.Zero);
	private static readonly SquidSpawnEventData _squidSpawnEventData = new(SquidType.Squid1, -1, Vector3.Zero, Vector3.Zero, 0f);
	private static readonly ThornSpawnEventData _thornSpawnEventData = new(-1, Vector3.Zero, 0f);
	private static readonly TransmuteEventData _transmuteEventData = new(_tempEntityId, Int16Vec3.Zero, Int16Vec3.Zero, Int16Vec3.Zero, Int16Vec3.Zero);

	public static void Render(int index)
	{
		ImGui.SetNextWindowSize(new(1280, 512), ImGuiCond.Appearing);
		if (!ImGui.BeginPopupModal(Inline.Span($"Insert event at index {index}")))
			return;

		if (ImGui.BeginTabBar("NewEventsTabBar", ImGuiTabBarFlags.None))
		{
			RenderTabItem<BoidSpawnEventData, BoidSpawnEvents>("Boid Spawn event", EventType.BoidSpawn, index, _boidSpawnEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<DaggerSpawnEventData, DaggerSpawnEvents>("Dagger Spawn event", EventType.DaggerSpawn, index, _daggerSpawnEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<DeathEventData, DeathEvents>("Death event", EventType.Death, index, _deathEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<EndEventData, EndEvents>("End event", EventType.End, index, _endEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<EntityOrientationEventData, EntityOrientationEvents>("Entity Orientation event", EventType.EntityOrientation, index, _entityOrientationEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<EntityPositionEventData, EntityPositionEvents>("Entity Position event", EventType.EntityPosition, index, _entityPositionEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<EntityTargetEventData, EntityTargetEvents>("Entity Target event", EventType.EntityTarget, index, _entityTargetEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<GemEventData, GemEvents>("Gem event", EventType.Gem, index, _gemEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<HitEventData, HitEvents>("Hit event", EventType.Hit, index, _hitEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<InitialInputsEventData, InitialInputsEvents>("Initial Inputs event", EventType.InitialInputs, index, _initialInputsEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<InputsEventData, InputsEvents>("Inputs event", EventType.Inputs, index, _inputsEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<LeviathanSpawnEventData, LeviathanSpawnEvents>("Leviathan Spawn event", EventType.LeviathanSpawn, index, _leviathanSpawnEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<PedeSpawnEventData, PedeSpawnEvents>("Pede Spawn event", EventType.PedeSpawn, index, _pedeSpawnEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<SpiderEggSpawnEventData, SpiderEggSpawnEvents>("Spider Egg Spawn event", EventType.SpiderEggSpawn, index, _spiderEggSpawnEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<SpiderSpawnEventData, SpiderSpawnEvents>("Spider Spawn event", EventType.SpiderSpawn, index, _spiderSpawnEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<SquidSpawnEventData, SquidSpawnEvents>("Squid Spawn event", EventType.SquidSpawn, index, _squidSpawnEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<ThornSpawnEventData, ThornSpawnEvents>("Thorn Spawn event", EventType.ThornSpawn, index, _thornSpawnEventData, FileStates.Replay.Object.EventsData);
			RenderTabItem<TransmuteEventData, TransmuteEvents>("Transmute event", EventType.Transmute, index, _transmuteEventData, FileStates.Replay.Object.EventsData);

			ImGui.EndTabBar();
		}

		if (ImGui.Button("Cancel"))
			ImGui.CloseCurrentPopup();

		ImGui.EndPopup();
	}

	private static void RenderTabItem<TEvent, TRenderer>(ReadOnlySpan<char> tabItemId, EventType eventType, int eventIndex, TEvent e, ReplayEventsData replayEventsData)
		where TEvent : IEventData
		where TRenderer : IEventTypeRenderer<TEvent>
	{
		if (!ImGui.BeginTabItem(tabItemId))
			return;

		if (TRenderer.ColumnCountData > 0)
		{
			if (!ImGui.BeginTable(EventTypeRendererUtils.EventTypeNames[eventType], TRenderer.ColumnCountData, ImGuiTableFlags.None))
				return;

			TRenderer.SetupColumnsData();
			ImGui.TableHeadersRow();

			ImGui.TableNextRow();
			TRenderer.RenderData(eventIndex, e, replayEventsData);

			ImGui.EndTable();
		}

		if (ImGui.Button("Insert"))
		{
			IEventData clonedEventData = e switch
			{
				BoidSpawnEventData boid => new BoidSpawnEventData(boid.SpawnerEntityId, boid.BoidType, boid.Position, boid.Orientation, boid.Velocity, boid.Speed),
				DaggerSpawnEventData dagger => new DaggerSpawnEventData(dagger.A, dagger.Position, dagger.Orientation, dagger.IsShot, dagger.DaggerType),
				DeathEventData death => new DeathEventData(death.DeathType),
				EndEventData => new EndEventData(),
				EntityOrientationEventData entityOrientation => new EntityOrientationEventData(entityOrientation.EntityId, entityOrientation.Orientation),
				EntityPositionEventData entityPosition => new EntityPositionEventData(entityPosition.EntityId, entityPosition.Position),
				EntityTargetEventData entityTarget => new EntityTargetEventData(entityTarget.EntityId, entityTarget.TargetPosition),
				GemEventData => new GemEventData(),
				HitEventData hit => new HitEventData(hit.EntityIdA, hit.EntityIdB, hit.UserData),
				InitialInputsEventData initialInputs => new InitialInputsEventData(initialInputs.Left, initialInputs.Right, initialInputs.Forward, initialInputs.Backward, initialInputs.Jump, initialInputs.Shoot, initialInputs.ShootHoming, initialInputs.MouseX, initialInputs.MouseY, initialInputs.LookSpeed),
				InputsEventData inputs => new InputsEventData(inputs.Left, inputs.Right, inputs.Forward, inputs.Backward, inputs.Jump, inputs.Shoot, inputs.ShootHoming, inputs.MouseX, inputs.MouseY),
				LeviathanSpawnEventData leviathan => new LeviathanSpawnEventData(leviathan.A),
				PedeSpawnEventData pede => new PedeSpawnEventData(pede.PedeType, pede.A, pede.Position, pede.B, pede.Orientation),
				SpiderEggSpawnEventData spiderEgg => new SpiderEggSpawnEventData(spiderEgg.SpawnerEntityId, spiderEgg.Position, spiderEgg.TargetPosition),
				SpiderSpawnEventData spider => new SpiderSpawnEventData(spider.SpiderType, spider.A, spider.Position),
				SquidSpawnEventData squid => new SquidSpawnEventData(squid.SquidType, squid.A, squid.Position, squid.Direction, squid.RotationInRadians),
				ThornSpawnEventData thorn => new ThornSpawnEventData(thorn.A, thorn.Position, thorn.RotationInRadians),
				TransmuteEventData transmute => new TransmuteEventData(transmute.EntityId, transmute.A, transmute.B, transmute.C, transmute.D),
				_ => throw new UnreachableException($"Event type '{e.GetType().Name}' is not supported."),
			};
			FileStates.Replay.Object.EventsData.InsertEvent(eventIndex, clonedEventData);
			ImGui.CloseCurrentPopup();
		}

		ImGui.EndTabItem();
	}
}
