using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Interfaces;
using System.Diagnostics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor;

public sealed class EventCache
{
	private readonly Dictionary<int, BoidSpawnEvent> _boidSpawnEvents = new();
	private readonly Dictionary<int, DaggerSpawnEvent> _daggerSpawnEvents = new();
	private readonly Dictionary<int, DeathEvent> _deathEvents = new();
	private readonly Dictionary<int, EndEvent> _endEvents = new();
	private readonly Dictionary<int, EntityOrientationEvent> _entityOrientationEvents = new();
	private readonly Dictionary<int, EntityPositionEvent> _entityPositionEvents = new();
	private readonly Dictionary<int, EntityTargetEvent> _entityTargetEvents = new();
	private readonly Dictionary<int, GemEvent> _gemEvents = new();
	private readonly Dictionary<int, HitEvent> _hitEvents = new();
	private readonly Dictionary<int, LeviathanSpawnEvent> _leviathanSpawnEvents = new();
	private readonly Dictionary<int, PedeSpawnEvent> _pedeSpawnEvents = new();
	private readonly Dictionary<int, SpiderEggSpawnEvent> _spiderEggSpawnEvents = new();
	private readonly Dictionary<int, SpiderSpawnEvent> _spiderSpawnEvents = new();
	private readonly Dictionary<int, SquidSpawnEvent> _squidSpawnEvents = new();
	private readonly Dictionary<int, ThornSpawnEvent> _thornSpawnEvents = new();
	private readonly Dictionary<int, TransmuteEvent> _transmuteEvents = new();

	public IReadOnlyDictionary<int, BoidSpawnEvent> BoidSpawnEvents => _boidSpawnEvents;
	public IReadOnlyDictionary<int, DaggerSpawnEvent> DaggerSpawnEvents => _daggerSpawnEvents;
	public IReadOnlyDictionary<int, DeathEvent> DeathEvents => _deathEvents;
	public IReadOnlyDictionary<int, EndEvent> EndEvents => _endEvents;
	public IReadOnlyDictionary<int, EntityOrientationEvent> EntityOrientationEvents => _entityOrientationEvents;
	public IReadOnlyDictionary<int, EntityPositionEvent> EntityPositionEvents => _entityPositionEvents;
	public IReadOnlyDictionary<int, EntityTargetEvent> EntityTargetEvents => _entityTargetEvents;
	public IReadOnlyDictionary<int, GemEvent> GemEvents => _gemEvents;
	public IReadOnlyDictionary<int, HitEvent> HitEvents => _hitEvents;
	public IReadOnlyDictionary<int, LeviathanSpawnEvent> LeviathanSpawnEvents => _leviathanSpawnEvents;
	public IReadOnlyDictionary<int, PedeSpawnEvent> PedeSpawnEvents => _pedeSpawnEvents;
	public IReadOnlyDictionary<int, SpiderEggSpawnEvent> SpiderEggSpawnEvents => _spiderEggSpawnEvents;
	public IReadOnlyDictionary<int, SpiderSpawnEvent> SpiderSpawnEvents => _spiderSpawnEvents;
	public IReadOnlyDictionary<int, SquidSpawnEvent> SquidSpawnEvents => _squidSpawnEvents;
	public IReadOnlyDictionary<int, ThornSpawnEvent> ThornSpawnEvents => _thornSpawnEvents;
	public IReadOnlyDictionary<int, TransmuteEvent> TransmuteEvents => _transmuteEvents;

	public int Count { get; private set; }

	public void Clear()
	{
		Count = 0;
		_boidSpawnEvents.Clear();
		_daggerSpawnEvents.Clear();
		_deathEvents.Clear();
		_endEvents.Clear();
		_entityOrientationEvents.Clear();
		_entityPositionEvents.Clear();
		_entityTargetEvents.Clear();
		_gemEvents.Clear();
		_hitEvents.Clear();
		_leviathanSpawnEvents.Clear();
		_pedeSpawnEvents.Clear();
		_spiderEggSpawnEvents.Clear();
		_spiderSpawnEvents.Clear();
		_squidSpawnEvents.Clear();
		_thornSpawnEvents.Clear();
		_transmuteEvents.Clear();
	}

	public void Add(int index, IEvent e)
	{
		Count++;

		switch (e)
		{
			case BoidSpawnEvent bse: _boidSpawnEvents.Add(index, bse); break;
			case DaggerSpawnEvent dse: _daggerSpawnEvents.Add(index, dse); break;
			case DeathEvent de: _deathEvents.Add(index, de); break;
			case EndEvent ee: _endEvents.Add(index, ee); break;
			case EntityOrientationEvent eoe: _entityOrientationEvents.Add(index, eoe); break;
			case EntityPositionEvent epe: _entityPositionEvents.Add(index, epe); break;
			case EntityTargetEvent ete: _entityTargetEvents.Add(index, ete); break;
			case GemEvent ge: _gemEvents.Add(index, ge); break;
			case HitEvent he: _hitEvents.Add(index, he); break;
			case LeviathanSpawnEvent lse: _leviathanSpawnEvents.Add(index, lse); break;
			case PedeSpawnEvent pse: _pedeSpawnEvents.Add(index, pse); break;
			case SpiderEggSpawnEvent sese: _spiderEggSpawnEvents.Add(index, sese); break;
			case SpiderSpawnEvent sse: _spiderSpawnEvents.Add(index, sse); break;
			case SquidSpawnEvent sse: _squidSpawnEvents.Add(index, sse); break;
			case ThornSpawnEvent tse: _thornSpawnEvents.Add(index, tse); break;
			case TransmuteEvent te: _transmuteEvents.Add(index, te); break;
			default: throw new UnreachableException();
		}
	}
}
