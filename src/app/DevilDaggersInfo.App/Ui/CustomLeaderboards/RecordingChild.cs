using DevilDaggersInfo.App.Core.GameMemory;
using DevilDaggersInfo.App.Core.GameMemory.Extensions;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Extensions;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Core.Wiki.Objects;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboards;

public static class RecordingChild
{
	private static float _statusIntensity;

	private static float _playerIntensity;
	private static float _timeIntensity;
	private static float _handIntensity;
	private static float _level2Intensity;
	private static float _level3Intensity;
	private static float _level4Intensity;
	private static float _deathIntensity;

	private static float _gemsCollectedIntensity;
	private static float _gemsDespawnedIntensity;
	private static float _gemsEatenIntensity;
	private static float _gemsTotalIntensity;

	private static float _homingStoredIntensity;
	private static float _homingEatenIntensity;

	private static float _daggersFiredIntensity;
	private static float _daggersHitIntensity;
	private static float _accuracyIntensity;

	private static float _enemiesKilledIntensity;
	private static float _enemiesAliveIntensity;

	private static float _skull1KillCountIntensity;
	private static float _skull2KillCountIntensity;
	private static float _skull3KillCountIntensity;
	private static float _skull4KillCountIntensity;
	private static float _squid1KillCountIntensity;
	private static float _squid2KillCountIntensity;
	private static float _squid3KillCountIntensity;
	private static float _centipedeKillCountIntensity;
	private static float _gigapedeKillCountIntensity;
	private static float _ghostpedeKillCountIntensity;
	private static float _spider1KillCountIntensity;
	private static float _spider2KillCountIntensity;
	private static float _spiderlingKillCountIntensity;
	private static float _spiderEggKillCountIntensity;
	private static float _leviathanKillCountIntensity;
	private static float _orbKillCountIntensity;
	private static float _thornKillCountIntensity;

	private static float _skull1AliveCountIntensity;
	private static float _skull2AliveCountIntensity;
	private static float _skull3AliveCountIntensity;
	private static float _skull4AliveCountIntensity;
	private static float _squid1AliveCountIntensity;
	private static float _squid2AliveCountIntensity;
	private static float _squid3AliveCountIntensity;
	private static float _centipedeAliveCountIntensity;
	private static float _gigapedeAliveCountIntensity;
	private static float _ghostpedeAliveCountIntensity;
	private static float _spider1AliveCountIntensity;
	private static float _spider2AliveCountIntensity;
	private static float _spiderlingAliveCountIntensity;
	private static float _spiderEggAliveCountIntensity;
	private static float _leviathanAliveCountIntensity;
	private static float _orbAliveCountIntensity;
	private static float _thornAliveCountIntensity;

	public static void Update(float delta)
	{
		const float tolerance = 0.0001f;

		MainBlock b = Root.GameMemoryService.MainBlock;
		MainBlock p = Root.GameMemoryService.MainBlockPrevious;

		_statusIntensity = b.Status != p.Status ? 1 : MathF.Max(0, _statusIntensity - delta);
		_playerIntensity = GetPlayerText(b) != GetPlayerText(p) ? 1 : MathF.Max(0, _playerIntensity - delta);
		_timeIntensity = Math.Abs(b.Time - p.Time) > tolerance ? 1 : MathF.Max(0, _timeIntensity - delta);
		_handIntensity = GetUpgrade(b) != GetUpgrade(p) ? 1 : MathF.Max(0, _handIntensity - delta);
		_level2Intensity = Math.Abs(b.LevelUpTime2 - p.LevelUpTime2) > tolerance ? 1 : MathF.Max(0, _level2Intensity - delta);
		_level3Intensity = Math.Abs(b.LevelUpTime3 - p.LevelUpTime3) > tolerance ? 1 : MathF.Max(0, _level3Intensity - delta);
		_level4Intensity = Math.Abs(b.LevelUpTime4 - p.LevelUpTime4) > tolerance ? 1 : MathF.Max(0, _level4Intensity - delta);
		_deathIntensity = b.IsPlayerAlive != p.IsPlayerAlive ? 1 : MathF.Max(0, _deathIntensity - delta);

		_gemsCollectedIntensity = b.GemsCollected != p.GemsCollected ? 1 : MathF.Max(0, _gemsCollectedIntensity - delta);
		_gemsDespawnedIntensity = b.GemsDespawned != p.GemsDespawned ? 1 : MathF.Max(0, _gemsDespawnedIntensity - delta);
		_gemsEatenIntensity = b.GemsEaten != p.GemsEaten ? 1 : MathF.Max(0, _gemsEatenIntensity - delta);
		_gemsTotalIntensity = b.GemsTotal != p.GemsTotal ? 1 : MathF.Max(0, _gemsTotalIntensity - delta);

		_homingStoredIntensity = b.HomingStored != p.HomingStored ? 1 : MathF.Max(0, _homingStoredIntensity - delta);
		_homingEatenIntensity = b.HomingEaten != p.HomingEaten ? 1 : MathF.Max(0, _homingEatenIntensity - delta);

		_daggersFiredIntensity = b.DaggersFired != p.DaggersFired ? 1 : MathF.Max(0, _daggersFiredIntensity - delta);
		_daggersHitIntensity = b.DaggersHit != p.DaggersHit ? 1 : MathF.Max(0, _daggersHitIntensity - delta);
		_accuracyIntensity = Math.Abs(GetAccuracy(b) - GetAccuracy(p)) > tolerance ? 1 : MathF.Max(0, _accuracyIntensity - delta);

		_enemiesKilledIntensity = b.EnemiesKilled != p.EnemiesKilled ? 1 : MathF.Max(0, _enemiesKilledIntensity - delta);
		_enemiesAliveIntensity = b.EnemiesAlive != p.EnemiesAlive ? 1 : MathF.Max(0, _enemiesAliveIntensity - delta);

		_skull1KillCountIntensity = b.Skull1KillCount != p.Skull1KillCount ? 1 : MathF.Max(0, _skull1KillCountIntensity - delta);
		_skull2KillCountIntensity = b.Skull2KillCount != p.Skull2KillCount ? 1 : MathF.Max(0, _skull2KillCountIntensity - delta);
		_skull3KillCountIntensity = b.Skull3KillCount != p.Skull3KillCount ? 1 : MathF.Max(0, _skull3KillCountIntensity - delta);
		_skull4KillCountIntensity = b.Skull4KillCount != p.Skull4KillCount ? 1 : MathF.Max(0, _skull4KillCountIntensity - delta);
		_squid1KillCountIntensity = b.Squid1KillCount != p.Squid1KillCount ? 1 : MathF.Max(0, _squid1KillCountIntensity - delta);
		_squid2KillCountIntensity = b.Squid2KillCount != p.Squid2KillCount ? 1 : MathF.Max(0, _squid2KillCountIntensity - delta);
		_squid3KillCountIntensity = b.Squid3KillCount != p.Squid3KillCount ? 1 : MathF.Max(0, _squid3KillCountIntensity - delta);
		_centipedeKillCountIntensity = b.CentipedeKillCount != p.CentipedeKillCount ? 1 : MathF.Max(0, _centipedeKillCountIntensity - delta);
		_gigapedeKillCountIntensity = b.GigapedeKillCount != p.GigapedeKillCount ? 1 : MathF.Max(0, _gigapedeKillCountIntensity - delta);
		_ghostpedeKillCountIntensity = b.GhostpedeKillCount != p.GhostpedeKillCount ? 1 : MathF.Max(0, _ghostpedeKillCountIntensity - delta);
		_spider1KillCountIntensity = b.Spider1KillCount != p.Spider1KillCount ? 1 : MathF.Max(0, _spider1KillCountIntensity - delta);
		_spider2KillCountIntensity = b.Spider2KillCount != p.Spider2KillCount ? 1 : MathF.Max(0, _spider2KillCountIntensity - delta);
		_spiderlingKillCountIntensity = b.SpiderlingKillCount != p.SpiderlingKillCount ? 1 : MathF.Max(0, _spiderlingKillCountIntensity - delta);
		_spiderEggKillCountIntensity = b.SpiderEggKillCount != p.SpiderEggKillCount ? 1 : MathF.Max(0, _spiderEggKillCountIntensity - delta);
		_leviathanKillCountIntensity = b.LeviathanKillCount != p.LeviathanKillCount ? 1 : MathF.Max(0, _leviathanKillCountIntensity - delta);
		_orbKillCountIntensity = b.OrbKillCount != p.OrbKillCount ? 1 : MathF.Max(0, _orbKillCountIntensity - delta);
		_thornKillCountIntensity = b.ThornKillCount != p.ThornKillCount ? 1 : MathF.Max(0, _thornKillCountIntensity - delta);

		_skull1AliveCountIntensity = b.Skull1AliveCount != p.Skull1AliveCount ? 1 : MathF.Max(0, _skull1AliveCountIntensity - delta);
		_skull2AliveCountIntensity = b.Skull2AliveCount != p.Skull2AliveCount ? 1 : MathF.Max(0, _skull2AliveCountIntensity - delta);
		_skull3AliveCountIntensity = b.Skull3AliveCount != p.Skull3AliveCount ? 1 : MathF.Max(0, _skull3AliveCountIntensity - delta);
		_skull4AliveCountIntensity = b.Skull4AliveCount != p.Skull4AliveCount ? 1 : MathF.Max(0, _skull4AliveCountIntensity - delta);
		_squid1AliveCountIntensity = b.Squid1AliveCount != p.Squid1AliveCount ? 1 : MathF.Max(0, _squid1AliveCountIntensity - delta);
		_squid2AliveCountIntensity = b.Squid2AliveCount != p.Squid2AliveCount ? 1 : MathF.Max(0, _squid2AliveCountIntensity - delta);
		_squid3AliveCountIntensity = b.Squid3AliveCount != p.Squid3AliveCount ? 1 : MathF.Max(0, _squid3AliveCountIntensity - delta);
		_centipedeAliveCountIntensity = b.CentipedeAliveCount != p.CentipedeAliveCount ? 1 : MathF.Max(0, _centipedeAliveCountIntensity - delta);
		_gigapedeAliveCountIntensity = b.GigapedeAliveCount != p.GigapedeAliveCount ? 1 : MathF.Max(0, _gigapedeAliveCountIntensity - delta);
		_ghostpedeAliveCountIntensity = b.GhostpedeAliveCount != p.GhostpedeAliveCount ? 1 : MathF.Max(0, _ghostpedeAliveCountIntensity - delta);
		_spider1AliveCountIntensity = b.Spider1AliveCount != p.Spider1AliveCount ? 1 : MathF.Max(0, _spider1AliveCountIntensity - delta);
		_spider2AliveCountIntensity = b.Spider2AliveCount != p.Spider2AliveCount ? 1 : MathF.Max(0, _spider2AliveCountIntensity - delta);
		_spiderlingAliveCountIntensity = b.SpiderlingAliveCount != p.SpiderlingAliveCount ? 1 : MathF.Max(0, _spiderlingAliveCountIntensity - delta);
		_spiderEggAliveCountIntensity = b.SpiderEggAliveCount != p.SpiderEggAliveCount ? 1 : MathF.Max(0, _spiderEggAliveCountIntensity - delta);
		_leviathanAliveCountIntensity = b.LeviathanAliveCount != p.LeviathanAliveCount ? 1 : MathF.Max(0, _leviathanAliveCountIntensity - delta);
		_orbAliveCountIntensity = b.OrbAliveCount != p.OrbAliveCount ? 1 : MathF.Max(0, _orbAliveCountIntensity - delta);
		_thornAliveCountIntensity = b.ThornAliveCount != p.ThornAliveCount ? 1 : MathF.Max(0, _thornAliveCountIntensity - delta);
	}

	public static void Render()
	{
		bool renderRecordingValues = Root.GameMemoryService.IsInitialized && !RecordingLogic.ShowUploadResponse && (GameStatus)Root.GameMemoryService.MainBlock.Status is not (GameStatus.Title or GameStatus.Menu or GameStatus.Lobby);
		if (renderRecordingValues)
		{
			RenderRecordingValues();
		}
		else
		{
			ImGui.Text("Waiting for run...");
		}
	}

	private static void RenderRecordingValues()
	{
		Vector2 iconSize = new(16);

		if (ImGui.BeginChild("RecordingValues", new(288, 320)))
		{
			MainBlock b = Root.GameMemoryService.MainBlock;
			RenderValue("Status", ((GameStatus)b.Status).ToDisplayString(), Color.White, _statusIntensity);

			ImGui.Spacing();

			// TODO: Use spans here.
			ImGuiImage.Image(Root.InternalResources.IconEyeTexture.Handle, iconSize, Color.Orange);
			RenderValue("Player", GetPlayerText(b), Color.White, _playerIntensity);
			RenderValue("Time", b.Time.ToString(StringFormats.TimeFormat), Color.White, _timeIntensity);
			RenderValue("Hand", GetUpgrade(b).Name, GetUpgrade(b).Color.ToEngineColor(), _handIntensity);
			RenderValue("Level 2", b.LevelUpTime2 == 0 ? "-" : b.LevelUpTime2.ToString(StringFormats.TimeFormat), UpgradesV3_2.Level2.Color.ToEngineColor(), _level2Intensity);
			RenderValue("Level 3", b.LevelUpTime3 == 0 ? "-" : b.LevelUpTime3.ToString(StringFormats.TimeFormat), UpgradesV3_2.Level3.Color.ToEngineColor(), _level3Intensity);
			RenderValue("Level 4", b.LevelUpTime4 == 0 ? "-" : b.LevelUpTime4.ToString(StringFormats.TimeFormat), UpgradesV3_2.Level4.Color.ToEngineColor(), _level4Intensity);
			RenderValue("Death", b.IsPlayerAlive ? "-" : GetDeath(b)?.Name ?? "?", GetDeath(b)?.Color.ToEngineColor() ?? Color.White, _deathIntensity);

			ImGui.Spacing();
			ImGuiImage.Image(Root.GameResources.IconMaskGemTexture.Handle, iconSize, Color.Red);
			RenderValue("Gems collected", b.GemsCollected.ToString(), Color.Red, _gemsCollectedIntensity);
			RenderValue("Gems despawned", b.GemsDespawned.ToString(), Color.Gray(0.6f), _gemsDespawnedIntensity);
			RenderValue("Gems eaten", b.GemsEaten.ToString(), Color.Green, _gemsEatenIntensity);
			RenderValue("Gems total", b.GemsTotal.ToString(), Color.Red, _gemsTotalIntensity);

			ImGui.Spacing();
			ImGuiImage.Image(Root.GameResources.IconMaskHomingTexture.Handle, iconSize);
			RenderValue("Homing stored", b.HomingStored.ToString(), Color.Purple, _homingStoredIntensity);
			RenderValue("Homing eaten", b.HomingEaten.ToString(), Color.Red, _homingEatenIntensity);

			ImGui.Spacing();
			ImGuiImage.Image(Root.GameResources.IconMaskCrosshairTexture.Handle, iconSize, Color.Green);
			RenderValue("Daggers fired", b.DaggersFired.ToString(), Color.Yellow, _daggersFiredIntensity);
			RenderValue("Daggers hit", b.DaggersHit.ToString(), Color.Red, _daggersHitIntensity);
			RenderValue("Accuracy", GetAccuracy(b).ToString("0.00%"), Color.Orange, _accuracyIntensity);

			ImGui.Spacing();
			ImGuiImage.Image(Root.GameResources.IconMaskSkullTexture.Handle, iconSize, EnemiesV3_2.Skull4.Color.ToEngineColor());
			RenderValue("Enemies killed", b.EnemiesKilled.ToString(), Color.Red, _enemiesKilledIntensity);
			RenderValue("Enemies alive", b.EnemiesAlive.ToString(), Color.Yellow, _enemiesAliveIntensity);

			ImGui.Spacing();
			RenderValue("Skull Is killed", b.Skull1KillCount.ToString(), EnemiesV3_2.Skull1.Color.ToEngineColor(), _skull1KillCountIntensity);
			RenderValue("Skull IIs killed", b.Skull2KillCount.ToString(), EnemiesV3_2.Skull2.Color.ToEngineColor(), _skull2KillCountIntensity);
			RenderValue("Skull IIIs killed", b.Skull3KillCount.ToString(), EnemiesV3_2.Skull3.Color.ToEngineColor(), _skull3KillCountIntensity);
			RenderValue("Skull IVs killed", b.Skull4KillCount.ToString(), EnemiesV3_2.Skull4.Color.ToEngineColor(), _skull4KillCountIntensity);
			RenderValue("Squid Is killed", b.Squid1KillCount.ToString(), EnemiesV3_2.Squid1.Color.ToEngineColor(), _squid1KillCountIntensity);
			RenderValue("Squid IIs killed", b.Squid2KillCount.ToString(), EnemiesV3_2.Squid2.Color.ToEngineColor(), _squid2KillCountIntensity);
			RenderValue("Squid IIIs killed", b.Squid3KillCount.ToString(), EnemiesV3_2.Squid3.Color.ToEngineColor(), _squid3KillCountIntensity);
			RenderValue("Centipedes killed", b.CentipedeKillCount.ToString(), EnemiesV3_2.Centipede.Color.ToEngineColor(), _centipedeKillCountIntensity);
			RenderValue("Gigapedes killed", b.GigapedeKillCount.ToString(), EnemiesV3_2.Gigapede.Color.ToEngineColor(), _gigapedeKillCountIntensity);
			RenderValue("Ghostpedes killed", b.GhostpedeKillCount.ToString(), EnemiesV3_2.Ghostpede.Color.ToEngineColor(), _ghostpedeKillCountIntensity);
			RenderValue("Spider Is killed", b.Spider1KillCount.ToString(), EnemiesV3_2.SpiderEgg1.Color.ToEngineColor(), _spider1KillCountIntensity);
			RenderValue("Spider IIs killed", b.Spider2KillCount.ToString(), EnemiesV3_2.SpiderEgg2.Color.ToEngineColor(), _spider2KillCountIntensity);
			RenderValue("Spiderlings killed", b.SpiderlingKillCount.ToString(), EnemiesV3_2.Spiderling.Color.ToEngineColor(), _spiderlingKillCountIntensity);
			RenderValue("Spider Eggs killed", b.SpiderEggKillCount.ToString(), EnemiesV3_2.SpiderEgg1.Color.ToEngineColor(), _spiderEggKillCountIntensity);
			RenderValue("Leviathans killed", b.LeviathanKillCount.ToString(), EnemiesV3_2.Leviathan.Color.ToEngineColor(), _leviathanKillCountIntensity);
			RenderValue("Orbs killed", b.OrbKillCount.ToString(), EnemiesV3_2.TheOrb.Color.ToEngineColor(), _orbKillCountIntensity);
			RenderValue("Thorns killed", b.ThornKillCount.ToString(), EnemiesV3_2.Thorn.Color.ToEngineColor(), _thornKillCountIntensity);

			ImGui.Spacing();
			RenderValue("Skull Is alive", b.Skull1AliveCount.ToString(), EnemiesV3_2.Skull1.Color.ToEngineColor(), _skull1AliveCountIntensity);
			RenderValue("Skull IIs alive", b.Skull2AliveCount.ToString(), EnemiesV3_2.Skull2.Color.ToEngineColor(), _skull2AliveCountIntensity);
			RenderValue("Skull IIIs alive", b.Skull3AliveCount.ToString(), EnemiesV3_2.Skull3.Color.ToEngineColor(), _skull3AliveCountIntensity);
			RenderValue("Skull IVs alive", b.Skull4AliveCount.ToString(), EnemiesV3_2.Skull4.Color.ToEngineColor(), _skull4AliveCountIntensity);
			RenderValue("Squid Is alive", b.Squid1AliveCount.ToString(), EnemiesV3_2.Squid1.Color.ToEngineColor(), _squid1AliveCountIntensity);
			RenderValue("Squid IIs alive", b.Squid2AliveCount.ToString(), EnemiesV3_2.Squid2.Color.ToEngineColor(), _squid2AliveCountIntensity);
			RenderValue("Squid IIIs alive", b.Squid3AliveCount.ToString(), EnemiesV3_2.Squid3.Color.ToEngineColor(), _squid3AliveCountIntensity);
			RenderValue("Centipedes alive", b.CentipedeAliveCount.ToString(), EnemiesV3_2.Centipede.Color.ToEngineColor(), _centipedeAliveCountIntensity);
			RenderValue("Gigapedes alive", b.GigapedeAliveCount.ToString(), EnemiesV3_2.Gigapede.Color.ToEngineColor(), _gigapedeAliveCountIntensity);
			RenderValue("Ghostpedes alive", b.GhostpedeAliveCount.ToString(), EnemiesV3_2.Ghostpede.Color.ToEngineColor(), _ghostpedeAliveCountIntensity);
			RenderValue("Spider Is alive", b.Spider1AliveCount.ToString(), EnemiesV3_2.SpiderEgg1.Color.ToEngineColor(), _spider1AliveCountIntensity);
			RenderValue("Spider IIs alive", b.Spider2AliveCount.ToString(), EnemiesV3_2.SpiderEgg2.Color.ToEngineColor(), _spider2AliveCountIntensity);
			RenderValue("Spiderlings alive", b.SpiderlingAliveCount.ToString(), EnemiesV3_2.Spiderling.Color.ToEngineColor(), _spiderlingAliveCountIntensity);
			RenderValue("Spider Eggs alive", b.SpiderEggAliveCount.ToString(), EnemiesV3_2.SpiderEgg1.Color.ToEngineColor(), _spiderEggAliveCountIntensity);
			RenderValue("Leviathans alive", b.LeviathanAliveCount.ToString(), EnemiesV3_2.Leviathan.Color.ToEngineColor(), _leviathanAliveCountIntensity);
			RenderValue("Orbs alive", b.OrbAliveCount.ToString(), EnemiesV3_2.TheOrb.Color.ToEngineColor(), _orbAliveCountIntensity);
			RenderValue("Thorns alive", b.ThornAliveCount.ToString(), EnemiesV3_2.Thorn.Color.ToEngineColor(), _thornAliveCountIntensity);
		}

		ImGui.EndChild(); // End RecordingValues
	}

	private static void RenderValue(string label, string value, Color color, float intensity)
	{
		ImGui.Text(label);
		ImGui.SameLine(288 - 16 - ImGui.CalcTextSize(value).X);
		ImGui.PushStyleColor(ImGuiCol.Text, Color.Lerp(Color.White, color, intensity));
		ImGui.TextUnformatted(value);
		ImGui.PopStyleColor();
	}

	private static string GetPlayerText(MainBlock b)
	{
		return b.IsReplay ? $"{b.ReplayPlayerName} ({b.ReplayPlayerId})" : $"{b.PlayerName} ({b.PlayerId})";
	}

	private static float GetAccuracy(MainBlock b)
	{
		return b.DaggersFired == 0 ? 0 : b.DaggersHit / (float)b.DaggersFired;
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
