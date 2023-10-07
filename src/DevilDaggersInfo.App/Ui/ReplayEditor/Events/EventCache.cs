using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Interfaces;
using System.Diagnostics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Events;

public sealed class EventCache
{
	private readonly List<(int Index, BoidSpawnEvent Event)> _boidSpawnEvents = new();
	private readonly List<(int Index, DaggerSpawnEvent Event)> _daggerSpawnEvents = new();
	private readonly List<(int Index, DeathEvent Event)> _deathEvents = new();
	private readonly List<(int Index, EndEvent Event)> _endEvents = new();
	private readonly List<(int Index, EntityOrientationEvent Event)> _entityOrientationEvents = new();
	private readonly List<(int Index, EntityPositionEvent Event)> _entityPositionEvents = new();
	private readonly List<(int Index, EntityTargetEvent Event)> _entityTargetEvents = new();
	private readonly List<(int Index, GemEvent Event)> _gemEvents = new();
	private readonly List<(int Index, HitEvent Event)> _hitEvents = new();
	private readonly List<(int Index, LeviathanSpawnEvent Event)> _leviathanSpawnEvents = new();
	private readonly List<(int Index, PedeSpawnEvent Event)> _pedeSpawnEvents = new();
	private readonly List<(int Index, SpiderEggSpawnEvent Event)> _spiderEggSpawnEvents = new();
	private readonly List<(int Index, SpiderSpawnEvent Event)> _spiderSpawnEvents = new();
	private readonly List<(int Index, SquidSpawnEvent Event)> _squidSpawnEvents = new();
	private readonly List<(int Index, ThornSpawnEvent Event)> _thornSpawnEvents = new();
	private readonly List<(int Index, TransmuteEvent Event)> _transmuteEvents = new();

	public IReadOnlyList<(int Index, BoidSpawnEvent Event)> BoidSpawnEvents => _boidSpawnEvents;
	public IReadOnlyList<(int Index, DaggerSpawnEvent Event)> DaggerSpawnEvents => _daggerSpawnEvents;
	public IReadOnlyList<(int Index, DeathEvent Event)> DeathEvents => _deathEvents;
	public IReadOnlyList<(int Index, EndEvent Event)> EndEvents => _endEvents;
	public IReadOnlyList<(int Index, EntityOrientationEvent Event)> EntityOrientationEvents => _entityOrientationEvents;
	public IReadOnlyList<(int Index, EntityPositionEvent Event)> EntityPositionEvents => _entityPositionEvents;
	public IReadOnlyList<(int Index, EntityTargetEvent Event)> EntityTargetEvents => _entityTargetEvents;
	public IReadOnlyList<(int Index, GemEvent Event)> GemEvents => _gemEvents;
	public IReadOnlyList<(int Index, HitEvent Event)> HitEvents => _hitEvents;
	public IReadOnlyList<(int Index, LeviathanSpawnEvent Event)> LeviathanSpawnEvents => _leviathanSpawnEvents;
	public IReadOnlyList<(int Index, PedeSpawnEvent Event)> PedeSpawnEvents => _pedeSpawnEvents;
	public IReadOnlyList<(int Index, SpiderEggSpawnEvent Event)> SpiderEggSpawnEvents => _spiderEggSpawnEvents;
	public IReadOnlyList<(int Index, SpiderSpawnEvent Event)> SpiderSpawnEvents => _spiderSpawnEvents;
	public IReadOnlyList<(int Index, SquidSpawnEvent Event)> SquidSpawnEvents => _squidSpawnEvents;
	public IReadOnlyList<(int Index, ThornSpawnEvent Event)> ThornSpawnEvents => _thornSpawnEvents;
	public IReadOnlyList<(int Index, TransmuteEvent Event)> TransmuteEvents => _transmuteEvents;

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
			case BoidSpawnEvent bse: _boidSpawnEvents.Add((index, bse)); break;
			case DaggerSpawnEvent dse: _daggerSpawnEvents.Add((index, dse)); break;
			case DeathEvent de: _deathEvents.Add((index, de)); break;
			case EndEvent ee: _endEvents.Add((index, ee)); break;
			case EntityOrientationEvent eoe: _entityOrientationEvents.Add((index, eoe)); break;
			case EntityPositionEvent epe: _entityPositionEvents.Add((index, epe)); break;
			case EntityTargetEvent ete: _entityTargetEvents.Add((index, ete)); break;
			case GemEvent ge: _gemEvents.Add((index, ge)); break;
			case HitEvent he: _hitEvents.Add((index, he)); break;
			case LeviathanSpawnEvent lse: _leviathanSpawnEvents.Add((index, lse)); break;
			case PedeSpawnEvent pse: _pedeSpawnEvents.Add((index, pse)); break;
			case SpiderEggSpawnEvent sese: _spiderEggSpawnEvents.Add((index, sese)); break;
			case SpiderSpawnEvent sse: _spiderSpawnEvents.Add((index, sse)); break;
			case SquidSpawnEvent sse: _squidSpawnEvents.Add((index, sse)); break;
			case ThornSpawnEvent tse: _thornSpawnEvents.Add((index, tse)); break;
			case TransmuteEvent te: _transmuteEvents.Add((index, te)); break;
			default: throw new UnreachableException();
		}
	}
}
