using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Extensions;

public static class ReplayEventExtensions
{
	public static EventType GetEventType(this ReplayEvent replayEvent)
	{
		return replayEvent.Data switch
		{
			BoidSpawnEventData => EventType.BoidSpawn,
			LeviathanSpawnEventData => EventType.LeviathanSpawn,
			PedeSpawnEventData => EventType.PedeSpawn,
			SpiderEggSpawnEventData => EventType.SpiderEggSpawn,
			SpiderSpawnEventData => EventType.SpiderSpawn,
			SquidSpawnEventData => EventType.SquidSpawn,
			ThornSpawnEventData => EventType.ThornSpawn,

			DaggerSpawnEventData => EventType.DaggerSpawn,
			EntityOrientationEventData => EventType.EntityOrientation,
			EntityPositionEventData => EventType.EntityPosition,
			EntityTargetEventData => EventType.EntityTarget,
			GemEventData => EventType.Gem,
			HitEventData => EventType.Hit,
			TransmuteEventData => EventType.Transmute,

			InitialInputsEventData => EventType.InitialInputs,
			InputsEventData => EventType.Inputs,
			EndEventData => EventType.End,

			_ => throw new UnreachableException($"Unknown event type '{replayEvent.Data.GetType().Name}'."),
		};
	}
}
