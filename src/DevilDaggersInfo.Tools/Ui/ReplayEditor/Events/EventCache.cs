using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Data;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;

public sealed class EventCache
{
	private readonly List<(int EventIndex, int EntityId, BoidSpawnEventData Event)> _boidSpawnEvents = [];
	private readonly List<(int EventIndex, int EntityId, DaggerSpawnEventData Event)> _daggerSpawnEvents = [];
	private readonly List<(int EventIndex, int EntityId, EndEventData Event)> _endEvents = [];
	private readonly List<(int EventIndex, int EntityId, EntityOrientationEventData Event)> _entityOrientationEvents = [];
	private readonly List<(int EventIndex, int EntityId, EntityPositionEventData Event)> _entityPositionEvents = [];
	private readonly List<(int EventIndex, int EntityId, EntityTargetEventData Event)> _entityTargetEvents = [];
	private readonly List<(int EventIndex, int EntityId, GemEventData Event)> _gemEvents = [];
	private readonly List<(int EventIndex, int EntityId, HitEventData Event)> _hitEvents = [];
	private readonly List<(int EventIndex, int EntityId, InitialInputsEventData Event)> _initialInputsEvents = [];
	private readonly List<(int EventIndex, int EntityId, InputsEventData Event)> _inputsEvents = [];
	private readonly List<(int EventIndex, int EntityId, LeviathanSpawnEventData Event)> _leviathanSpawnEvents = [];
	private readonly List<(int EventIndex, int EntityId, PedeSpawnEventData Event)> _pedeSpawnEvents = [];
	private readonly List<(int EventIndex, int EntityId, SpiderEggSpawnEventData Event)> _spiderEggSpawnEvents = [];
	private readonly List<(int EventIndex, int EntityId, SpiderSpawnEventData Event)> _spiderSpawnEvents = [];
	private readonly List<(int EventIndex, int EntityId, SquidSpawnEventData Event)> _squidSpawnEvents = [];
	private readonly List<(int EventIndex, int EntityId, ThornSpawnEventData Event)> _thornSpawnEvents = [];
	private readonly List<(int EventIndex, int EntityId, TransmuteEventData Event)> _transmuteEvents = [];

	public IReadOnlyList<(int EventIndex, int EntityId, BoidSpawnEventData Event)> BoidSpawnEvents => _boidSpawnEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, DaggerSpawnEventData Event)> DaggerSpawnEvents => _daggerSpawnEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, EndEventData Event)> EndEvents => _endEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, EntityOrientationEventData Event)> EntityOrientationEvents => _entityOrientationEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, EntityPositionEventData Event)> EntityPositionEvents => _entityPositionEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, EntityTargetEventData Event)> EntityTargetEvents => _entityTargetEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, GemEventData Event)> GemEvents => _gemEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, HitEventData Event)> HitEvents => _hitEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, InitialInputsEventData Event)> InitialInputsEvents => _initialInputsEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, InputsEventData Event)> InputsEvents => _inputsEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, LeviathanSpawnEventData Event)> LeviathanSpawnEvents => _leviathanSpawnEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, PedeSpawnEventData Event)> PedeSpawnEvents => _pedeSpawnEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, SpiderEggSpawnEventData Event)> SpiderEggSpawnEvents => _spiderEggSpawnEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, SpiderSpawnEventData Event)> SpiderSpawnEvents => _spiderSpawnEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, SquidSpawnEventData Event)> SquidSpawnEvents => _squidSpawnEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, ThornSpawnEventData Event)> ThornSpawnEvents => _thornSpawnEvents;
	public IReadOnlyList<(int EventIndex, int EntityId, TransmuteEventData Event)> TransmuteEvents => _transmuteEvents;

	public int Count { get; private set; }

	public void Clear()
	{
		Count = 0;
		_boidSpawnEvents.Clear();
		_daggerSpawnEvents.Clear();
		_endEvents.Clear();
		_entityOrientationEvents.Clear();
		_entityPositionEvents.Clear();
		_entityTargetEvents.Clear();
		_gemEvents.Clear();
		_hitEvents.Clear();
		_initialInputsEvents.Clear();
		_inputsEvents.Clear();
		_leviathanSpawnEvents.Clear();
		_pedeSpawnEvents.Clear();
		_spiderEggSpawnEvents.Clear();
		_spiderSpawnEvents.Clear();
		_squidSpawnEvents.Clear();
		_thornSpawnEvents.Clear();
		_transmuteEvents.Clear();
	}

	public void Add(int index, ReplayEvent replayEvent)
	{
		Count++;

		if (replayEvent is EntitySpawnReplayEvent spawnEvent)
		{
			switch (replayEvent.Data)
			{
				case BoidSpawnEventData bs: _boidSpawnEvents.Add((index, spawnEvent.EntityId, bs)); break;
				case DaggerSpawnEventData ds: _daggerSpawnEvents.Add((index, spawnEvent.EntityId, ds)); break;
				case LeviathanSpawnEventData ls: _leviathanSpawnEvents.Add((index, spawnEvent.EntityId, ls)); break;
				case PedeSpawnEventData ps: _pedeSpawnEvents.Add((index, spawnEvent.EntityId, ps)); break;
				case SpiderEggSpawnEventData ses: _spiderEggSpawnEvents.Add((index, spawnEvent.EntityId, ses)); break;
				case SpiderSpawnEventData ss: _spiderSpawnEvents.Add((index, spawnEvent.EntityId, ss)); break;
				case SquidSpawnEventData ss: _squidSpawnEvents.Add((index, spawnEvent.EntityId, ss)); break;
				case ThornSpawnEventData ts: _thornSpawnEvents.Add((index, spawnEvent.EntityId, ts)); break;
				default: throw new UnreachableException();
			}
		}
		else
		{
			switch (replayEvent.Data)
			{
				case EndEventData e: _endEvents.Add((index, -1, e)); break;
				case EntityOrientationEventData eo: _entityOrientationEvents.Add((index, -1, eo)); break;
				case EntityPositionEventData ep: _entityPositionEvents.Add((index, -1, ep)); break;
				case EntityTargetEventData et: _entityTargetEvents.Add((index, -1, et)); break;
				case GemEventData g: _gemEvents.Add((index, -1, g)); break;
				case HitEventData h: _hitEvents.Add((index, -1, h)); break;
				case InitialInputsEventData ii: _initialInputsEvents.Add((index, -1, ii)); break;
				case InputsEventData i: _inputsEvents.Add((index, -1, i)); break;
				case TransmuteEventData t: _transmuteEvents.Add((index, -1, t)); break;
				default: throw new UnreachableException();
			}
		}
	}
}
