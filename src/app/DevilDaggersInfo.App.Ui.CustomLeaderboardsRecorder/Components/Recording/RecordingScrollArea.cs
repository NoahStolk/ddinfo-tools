using DevilDaggersInfo.App.Core.GameMemory;
using DevilDaggersInfo.App.Core.GameMemory.Extensions;
using DevilDaggersInfo.App.Engine.Content;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Extensions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Core.Wiki.Objects;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.Recording;

public class RecordingScrollArea : ScrollArea
{
	private readonly List<RecordingValue> _recordingValues = new();

	public RecordingScrollArea(IBounds bounds)
		: base(bounds, 64, 16, ScrollAreaStyles.Default)
	{
		int labelWidth = 256 - ScrollbarBounds.Size.X;
		const int labelHeight = 16;

		int height = 0;

		_recordingValues.Add(AddValue(ref height, "Status", Color.White, b => ((GameStatus)b.Status).ToDisplayString()));

		AddSpacing(ref height);
		AddIcon(ref height, Textures.IconEye, Color.Orange);
		_recordingValues.Add(AddValue(ref height, "Player", Color.White, b => b.IsReplay ? $"{b.ReplayPlayerName} ({b.ReplayPlayerId})" : $"{b.PlayerName} ({b.PlayerId})"));
		_recordingValues.Add(AddValue(ref height, "Time", Color.White, b => b.Time.ToString(StringFormats.TimeFormat)));
		_recordingValues.Add(AddValue(ref height, "Hand", Color.White, b => GetUpgrade(b).Name, b => GetUpgrade(b).Color.ToWarpColor()));
		_recordingValues.Add(AddValue(ref height, "Level 2", UpgradesV3_2.Level2.Color.ToWarpColor(), b => b.LevelUpTime2 == 0 ? "-" : b.LevelUpTime2.ToString(StringFormats.TimeFormat)));
		_recordingValues.Add(AddValue(ref height, "Level 3", UpgradesV3_2.Level3.Color.ToWarpColor(), b => b.LevelUpTime3 == 0 ? "-" : b.LevelUpTime3.ToString(StringFormats.TimeFormat)));
		_recordingValues.Add(AddValue(ref height, "Level 4", UpgradesV3_2.Level4.Color.ToWarpColor(), b => b.LevelUpTime4 == 0 ? "-" : b.LevelUpTime4.ToString(StringFormats.TimeFormat)));
		_recordingValues.Add(AddValue(ref height, "Death", Color.White, b => b.IsPlayerAlive ? "-" : GetDeath(b)?.Name ?? "?", b => GetDeath(b)?.Color.ToWarpColor() ?? Color.White));

		AddSpacing(ref height);
		AddIcon(ref height, Textures.IconGem, Color.Red);
		_recordingValues.Add(AddValue(ref height, "Gems collected", Color.Red, b => b.GemsCollected.ToString()));
		_recordingValues.Add(AddValue(ref height, "Gems despawned", Color.Gray(0.6f), b => b.GemsDespawned.ToString()));
		_recordingValues.Add(AddValue(ref height, "Gems eaten", Color.Green, b => b.GemsEaten.ToString()));
		_recordingValues.Add(AddValue(ref height, "Gems total", Color.Red, b => b.GemsTotal.ToString()));

		AddSpacing(ref height);
		AddIcon(ref height, Textures.IconHoming, Color.White);
		_recordingValues.Add(AddValue(ref height, "Homing stored", Color.Purple, b => b.HomingStored.ToString()));
		_recordingValues.Add(AddValue(ref height, "Homing eaten", Color.Red, b => b.HomingEaten.ToString()));

		AddSpacing(ref height);
		AddIcon(ref height, Textures.IconCrosshair, Color.Green);
		_recordingValues.Add(AddValue(ref height, "Daggers fired", Color.Yellow, b => b.DaggersFired.ToString()));
		_recordingValues.Add(AddValue(ref height, "Daggers hit", Color.Red, b => b.DaggersHit.ToString()));
		_recordingValues.Add(AddValue(ref height, "Accuracy", Color.Orange, b => (b.DaggersFired == 0 ? 0 : b.DaggersHit / (float)b.DaggersFired).ToString("0.00%")));

		AddSpacing(ref height);
		AddIcon(ref height, Textures.IconSkull, EnemiesV3_2.Skull4.Color.ToWarpColor());
		_recordingValues.Add(AddValue(ref height, "Enemies killed", Color.Red, b => b.EnemiesKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Enemies alive", Color.Yellow, b => b.EnemiesAlive.ToString()));

		AddSpacing(ref height);
		_recordingValues.Add(AddValue(ref height, "Skull Is killed", EnemiesV3_2.Skull1.Color.ToWarpColor(), b => b.Skull1sKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Skull IIs killed", EnemiesV3_2.Skull2.Color.ToWarpColor(), b => b.Skull2sKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Skull IIIs killed", EnemiesV3_2.Skull3.Color.ToWarpColor(), b => b.Skull3sKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Skull IVs killed", EnemiesV3_2.Skull4.Color.ToWarpColor(), b => b.Skull4sKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Squid Is killed", EnemiesV3_2.Squid1.Color.ToWarpColor(), b => b.Squid1sKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Squid IIs killed", EnemiesV3_2.Squid2.Color.ToWarpColor(), b => b.Squid2sKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Squid IIIs killed", EnemiesV3_2.Squid3.Color.ToWarpColor(), b => b.Squid3sKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Centipedes killed", EnemiesV3_2.Centipede.Color.ToWarpColor(), b => b.CentipedesKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Gigapedes killed", EnemiesV3_2.Gigapede.Color.ToWarpColor(), b => b.GigapedesKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Ghostpedes killed", EnemiesV3_2.Ghostpede.Color.ToWarpColor(), b => b.GhostpedesKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Spider Is killed", EnemiesV3_2.SpiderEgg1.Color.ToWarpColor(), b => b.Spider1sKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Spider IIs killed", EnemiesV3_2.SpiderEgg2.Color.ToWarpColor(), b => b.Spider2sKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Spiderlings killed", EnemiesV3_2.Spiderling.Color.ToWarpColor(), b => b.SpiderlingsKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Spider Eggs killed", EnemiesV3_2.SpiderEgg1.Color.ToWarpColor(), b => b.SpiderEggsKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Leviathans killed", EnemiesV3_2.Leviathan.Color.ToWarpColor(), b => b.LeviathansKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Orbs killed", EnemiesV3_2.TheOrb.Color.ToWarpColor(), b => b.OrbsKilled.ToString()));
		_recordingValues.Add(AddValue(ref height, "Thorns killed", EnemiesV3_2.Thorn.Color.ToWarpColor(), b => b.ThornsKilled.ToString()));

		AddSpacing(ref height);
		_recordingValues.Add(AddValue(ref height, "Skull Is alive", EnemiesV3_2.Skull1.Color.ToWarpColor(), b => b.Skull1sAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Skull IIs alive", EnemiesV3_2.Skull2.Color.ToWarpColor(), b => b.Skull2sAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Skull IIIs alive", EnemiesV3_2.Skull3.Color.ToWarpColor(), b => b.Skull3sAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Skull IVs alive", EnemiesV3_2.Skull4.Color.ToWarpColor(), b => b.Skull4sAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Squid Is alive", EnemiesV3_2.Squid1.Color.ToWarpColor(), b => b.Squid1sAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Squid IIs alive", EnemiesV3_2.Squid2.Color.ToWarpColor(), b => b.Squid2sAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Squid IIIs alive", EnemiesV3_2.Squid3.Color.ToWarpColor(), b => b.Squid3sAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Centipedes alive", EnemiesV3_2.Centipede.Color.ToWarpColor(), b => b.CentipedesAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Gigapedes alive", EnemiesV3_2.Gigapede.Color.ToWarpColor(), b => b.GigapedesAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Ghostpedes alive", EnemiesV3_2.Ghostpede.Color.ToWarpColor(), b => b.GhostpedesAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Spider Is alive", EnemiesV3_2.SpiderEgg1.Color.ToWarpColor(), b => b.Spider1sAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Spider IIs alive", EnemiesV3_2.SpiderEgg2.Color.ToWarpColor(), b => b.Spider2sAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Spiderlings alive", EnemiesV3_2.Spiderling.Color.ToWarpColor(), b => b.SpiderlingsAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Spider Eggs alive", EnemiesV3_2.SpiderEgg1.Color.ToWarpColor(), b => b.SpiderEggsAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Leviathans alive", EnemiesV3_2.Leviathan.Color.ToWarpColor(), b => b.LeviathansAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Orbs alive", EnemiesV3_2.TheOrb.Color.ToWarpColor(), b => b.OrbsAlive.ToString()));
		_recordingValues.Add(AddValue(ref height, "Thorns alive", EnemiesV3_2.Thorn.Color.ToWarpColor(), b => b.ThornsAlive.ToString()));

		RecordingValue AddValue(ref int h, string text, Color color, Func<MainBlock, string> valueGetter, Func<MainBlock, Color>? colorGetter = null)
		{
			RecordingValue recordingValue = new(bounds.CreateNested(0, h, labelWidth, labelHeight), text, color, valueGetter, colorGetter) { Depth = Depth };
			NestingContext.Add(recordingValue);

			h += labelHeight;

			return recordingValue;
		}

		void AddSpacing(ref int h)
		{
			h += labelHeight / 2;
		}

		void AddIcon(ref int h, Texture texture, Color color)
		{
			const int iconSize = 16;
			RecordingIcon recordingIcon = new(bounds.CreateNested(4, h, iconSize, iconSize), texture, color) { Depth = Depth + 100 };
			NestingContext.Add(recordingIcon);

			h += iconSize;
		}
	}

	public void SetState()
	{
		foreach (RecordingValue recordingValue in _recordingValues)
			recordingValue.SetState();
	}

	private static Upgrade GetUpgrade(MainBlock block) => block.LevelGems switch
	{
		< 10 => UpgradesV3_2.Level1,
		< 70 => UpgradesV3_2.Level2,
		70 => UpgradesV3_2.Level3,
		_ => UpgradesV3_2.Level4,
	};

	private static Death? GetDeath(MainBlock block)
	{
		return Deaths.GetDeathByLeaderboardType(GameConstants.CurrentVersion, block.DeathType);
	}
}
