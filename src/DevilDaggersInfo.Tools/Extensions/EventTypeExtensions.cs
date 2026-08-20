using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Extensions;

internal static class EventTypeExtensions
{
	extension(EventType eventType)
	{
		public Vector4 GetColor()
		{
			return eventType switch
			{
				EventType.BoidSpawn => EnemiesV3_2.Skull4.Color,
				EventType.DaggerSpawn => Color.Purple,
				EventType.End or EventType.Gem => Color.Red,
				EventType.EntityOrientation or EventType.EntityPosition or EventType.EntityTarget => Color.Yellow,
				EventType.Hit => Color.Orange,
				EventType.LeviathanSpawn => EnemiesV3_2.Leviathan.Color,
				EventType.PedeSpawn => EnemiesV3_2.Gigapede.Color,
				EventType.SpiderEggSpawn => EnemiesV3_2.SpiderEgg1.Color,
				EventType.SpiderSpawn => EnemiesV3_2.Spider2.Color,
				EventType.SquidSpawn => EnemiesV3_2.Squid3.Color,
				EventType.ThornSpawn => EnemiesV3_2.Thorn.Color,
				EventType.Transmute => new Vector4(0.75f, 0, 0, 1),
				_ => Color.White,
			};
		}

		public ReadOnlySpan<byte> AsUtf8DisplaySpan()
		{
			return eventType switch
			{
				EventType.BoidSpawn => "Boid Spawn"u8,
				EventType.LeviathanSpawn => "Leviathan Spawn"u8,
				EventType.PedeSpawn => "Pede Spawn"u8,
				EventType.SpiderEggSpawn => "Spider Egg Spawn"u8,
				EventType.SpiderSpawn => "Spider Spawn"u8,
				EventType.SquidSpawn => "Squid Spawn"u8,
				EventType.ThornSpawn => "Thorn Spawn"u8,
				EventType.DaggerSpawn => "Dagger Spawn"u8,
				EventType.EntityOrientation => "Entity Orientation"u8,
				EventType.EntityPosition => "Entity Position"u8,
				EventType.EntityTarget => "Entity Target"u8,
				EventType.Gem => "Gem"u8,
				EventType.Hit => "Hit"u8,
				EventType.Transmute => "Transmute"u8,
				EventType.InitialInputs => "Initial Inputs"u8,
				EventType.Inputs => "Inputs"u8,
				EventType.End => "End"u8,
				_ => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null),
			};
		}
	}
}
