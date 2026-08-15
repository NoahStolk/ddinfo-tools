using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Core.Wiki.Objects;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.GameMemory;
using Hexa.NET.ImGui;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards;

internal sealed class RecordingChild(ResourceManager resourceManager, RecordingLogic recordingLogic, GameMemoryService gameMemoryService)
{
	private float _statusIntensity;

	private float _playerIntensity;
	private float _timeIntensity;
	private float _handIntensity;
	private float _level2Intensity;
	private float _level3Intensity;
	private float _level4Intensity;
	private float _deathIntensity;

	private float _gemsCollectedIntensity;
	private float _gemsDespawnedIntensity;
	private float _gemsEatenIntensity;
	private float _gemsTotalIntensity;

	private float _homingStoredIntensity;
	private float _homingEatenIntensity;

	private float _daggersFiredIntensity;
	private float _daggersHitIntensity;
	private float _accuracyIntensity;

	private float _enemiesKilledIntensity;
	private float _enemiesAliveIntensity;

	private float _skull1KillCountIntensity;
	private float _skull2KillCountIntensity;
	private float _skull3KillCountIntensity;
	private float _skull4KillCountIntensity;
	private float _squid1KillCountIntensity;
	private float _squid2KillCountIntensity;
	private float _squid3KillCountIntensity;
	private float _centipedeKillCountIntensity;
	private float _gigapedeKillCountIntensity;
	private float _ghostpedeKillCountIntensity;
	private float _spider1KillCountIntensity;
	private float _spider2KillCountIntensity;
	private float _spiderlingKillCountIntensity;
	private float _spiderEggKillCountIntensity;
	private float _leviathanKillCountIntensity;
	private float _orbKillCountIntensity;
	private float _thornKillCountIntensity;

	private float _skull1AliveCountIntensity;
	private float _skull2AliveCountIntensity;
	private float _skull3AliveCountIntensity;
	private float _skull4AliveCountIntensity;
	private float _squid1AliveCountIntensity;
	private float _squid2AliveCountIntensity;
	private float _squid3AliveCountIntensity;
	private float _centipedeAliveCountIntensity;
	private float _gigapedeAliveCountIntensity;
	private float _ghostpedeAliveCountIntensity;
	private float _spider1AliveCountIntensity;
	private float _spider2AliveCountIntensity;
	private float _spiderlingAliveCountIntensity;
	private float _spiderEggAliveCountIntensity;
	private float _leviathanAliveCountIntensity;
	private float _orbAliveCountIntensity;
	private float _thornAliveCountIntensity;

	public void Update(float delta)
	{
		const float tolerance = 0.0001f;

		MainBlock b = gameMemoryService.MainBlock;
		MainBlock p = gameMemoryService.MainBlockPrevious;

		_statusIntensity = b.Status != p.Status ? 1 : MathF.Max(0, _statusIntensity - delta);
		_playerIntensity = !b.ArePlayersEqual(p) ? 1 : MathF.Max(0, _playerIntensity - delta);
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

	public void Render()
	{
		bool renderRecordingValues = gameMemoryService.IsInitialized && !recordingLogic.ShowUploadResponse && (GameStatus)gameMemoryService.MainBlock.Status is not (GameStatus.Title or GameStatus.Menu or GameStatus.Lobby);
		if (renderRecordingValues)
		{
			RenderRecordingValues();
		}
		else
		{
			ImGui.Text("Waiting for run...");
		}
	}

	private void RenderRecordingValues()
	{
		Debug.Assert(resourceManager.GameResources != null, $"{nameof(resourceManager.GameResources)} is null, which should never happen in this UI.");

		Vector2 iconSize = new(16);

		if (ImGui.BeginChild("RecordingValues", new Vector2(288, 320)))
		{
			MainBlock b = gameMemoryService.MainBlock;
			RenderValue("Status"u8, Inline.Utf8(((GameStatus)b.Status).ToDisplayString()), Color.White, _statusIntensity);

			ImGui.Spacing();

			// TODO: Use spans here.
			ImGuiImage.Image(resourceManager.InternalResources.IconEyeTexture.Id, iconSize, Color.Orange);
			RenderValue("Player"u8, Inline.Utf8(GetPlayerText(b)), Color.White, _playerIntensity);
			RenderValue("Time"u8, Inline.Utf8(b.Time, StringFormats.TimeFormat), Color.White, _timeIntensity);
			RenderValue("Hand"u8, Inline.Utf8(GetUpgrade(b).Name), GetUpgrade(b).Color.ToEngineColor(), _handIntensity);
			RenderValue("Level 2"u8, b.LevelUpTime2 == 0 ? "-"u8 : Inline.Utf8(b.LevelUpTime2, StringFormats.TimeFormat), UpgradesV3_2.Level2.Color.ToEngineColor(), _level2Intensity);
			RenderValue("Level 3"u8, b.LevelUpTime3 == 0 ? "-"u8 : Inline.Utf8(b.LevelUpTime3, StringFormats.TimeFormat), UpgradesV3_2.Level3.Color.ToEngineColor(), _level3Intensity);
			RenderValue("Level 4"u8, b.LevelUpTime4 == 0 ? "-"u8 : Inline.Utf8(b.LevelUpTime4, StringFormats.TimeFormat), UpgradesV3_2.Level4.Color.ToEngineColor(), _level4Intensity);
			RenderValue("Death"u8, Inline.Utf8(b.IsPlayerAlive ? "-" : GetDeath(b)?.Name ?? "?"), GetDeath(b)?.Color.ToEngineColor() ?? Color.White, _deathIntensity);

			ImGui.Spacing();
			ImGuiImage.Image(resourceManager.GameResources.IconMaskGemTexture.Id, iconSize, Color.Red);
			RenderValue("Gems collected"u8, Inline.Utf8(b.GemsCollected), Color.Red, _gemsCollectedIntensity);
			RenderValue("Gems despawned"u8, Inline.Utf8(b.GemsDespawned), Color.Gray(0.6f), _gemsDespawnedIntensity);
			RenderValue("Gems eaten"u8, Inline.Utf8(b.GemsEaten), Color.Green, _gemsEatenIntensity);
			RenderValue("Gems total"u8, Inline.Utf8(b.GemsTotal), Color.Red, _gemsTotalIntensity);

			ImGui.Spacing();
			ImGuiImage.Image(resourceManager.GameResources.IconMaskHomingTexture.Id, iconSize);
			RenderValue("Homing stored"u8, Inline.Utf8(b.HomingStored), Color.Purple, _homingStoredIntensity);
			RenderValue("Homing eaten"u8, Inline.Utf8(b.HomingEaten), Color.Red, _homingEatenIntensity);

			ImGui.Spacing();
			ImGuiImage.Image(resourceManager.GameResources.IconMaskCrosshairTexture.Id, iconSize, Color.Green);
			RenderValue("Daggers fired"u8, Inline.Utf8(b.DaggersFired), Color.Yellow, _daggersFiredIntensity);
			RenderValue("Daggers hit"u8, Inline.Utf8(b.DaggersHit), Color.Red, _daggersHitIntensity);
			RenderValue("Accuracy"u8, Inline.Utf8(GetAccuracy(b), "0.00%"), Color.Orange, _accuracyIntensity);

			ImGui.Spacing();
			ImGuiImage.Image(resourceManager.GameResources.IconMaskSkullTexture.Id, iconSize, EnemiesV3_2.Skull4.Color.ToEngineColor());
			RenderValue("Enemies killed"u8, Inline.Utf8(b.EnemiesKilled), Color.Red, _enemiesKilledIntensity);
			RenderValue("Enemies alive"u8, Inline.Utf8(b.EnemiesAlive), Color.Yellow, _enemiesAliveIntensity);

			ImGui.Spacing();
			RenderValue("Skull Is killed"u8, Inline.Utf8(b.Skull1KillCount), EnemiesV3_2.Skull1.Color.ToEngineColor(), _skull1KillCountIntensity);
			RenderValue("Skull IIs killed"u8, Inline.Utf8(b.Skull2KillCount), EnemiesV3_2.Skull2.Color.ToEngineColor(), _skull2KillCountIntensity);
			RenderValue("Skull IIIs killed"u8, Inline.Utf8(b.Skull3KillCount), EnemiesV3_2.Skull3.Color.ToEngineColor(), _skull3KillCountIntensity);
			RenderValue("Skull IVs killed"u8, Inline.Utf8(b.Skull4KillCount), EnemiesV3_2.Skull4.Color.ToEngineColor(), _skull4KillCountIntensity);
			RenderValue("Squid Is killed"u8, Inline.Utf8(b.Squid1KillCount), EnemiesV3_2.Squid1.Color.ToEngineColor(), _squid1KillCountIntensity);
			RenderValue("Squid IIs killed"u8, Inline.Utf8(b.Squid2KillCount), EnemiesV3_2.Squid2.Color.ToEngineColor(), _squid2KillCountIntensity);
			RenderValue("Squid IIIs killed"u8, Inline.Utf8(b.Squid3KillCount), EnemiesV3_2.Squid3.Color.ToEngineColor(), _squid3KillCountIntensity);
			RenderValue("Centipedes killed"u8, Inline.Utf8(b.CentipedeKillCount), EnemiesV3_2.Centipede.Color.ToEngineColor(), _centipedeKillCountIntensity);
			RenderValue("Gigapedes killed"u8, Inline.Utf8(b.GigapedeKillCount), EnemiesV3_2.Gigapede.Color.ToEngineColor(), _gigapedeKillCountIntensity);
			RenderValue("Ghostpedes killed"u8, Inline.Utf8(b.GhostpedeKillCount), EnemiesV3_2.Ghostpede.Color.ToEngineColor(), _ghostpedeKillCountIntensity);
			RenderValue("Spider Is killed"u8, Inline.Utf8(b.Spider1KillCount), EnemiesV3_2.SpiderEgg1.Color.ToEngineColor(), _spider1KillCountIntensity);
			RenderValue("Spider IIs killed"u8, Inline.Utf8(b.Spider2KillCount), EnemiesV3_2.SpiderEgg2.Color.ToEngineColor(), _spider2KillCountIntensity);
			RenderValue("Spiderlings killed"u8, Inline.Utf8(b.SpiderlingKillCount), EnemiesV3_2.Spiderling.Color.ToEngineColor(), _spiderlingKillCountIntensity);
			RenderValue("Spider Eggs killed"u8, Inline.Utf8(b.SpiderEggKillCount), EnemiesV3_2.SpiderEgg1.Color.ToEngineColor(), _spiderEggKillCountIntensity);
			RenderValue("Leviathans killed"u8, Inline.Utf8(b.LeviathanKillCount), EnemiesV3_2.Leviathan.Color.ToEngineColor(), _leviathanKillCountIntensity);
			RenderValue("Orbs killed"u8, Inline.Utf8(b.OrbKillCount), EnemiesV3_2.TheOrb.Color.ToEngineColor(), _orbKillCountIntensity);
			RenderValue("Thorns killed"u8, Inline.Utf8(b.ThornKillCount), EnemiesV3_2.Thorn.Color.ToEngineColor(), _thornKillCountIntensity);

			ImGui.Spacing();
			RenderValue("Skull Is alive"u8, Inline.Utf8(b.Skull1AliveCount), EnemiesV3_2.Skull1.Color.ToEngineColor(), _skull1AliveCountIntensity);
			RenderValue("Skull IIs alive"u8, Inline.Utf8(b.Skull2AliveCount), EnemiesV3_2.Skull2.Color.ToEngineColor(), _skull2AliveCountIntensity);
			RenderValue("Skull IIIs alive"u8, Inline.Utf8(b.Skull3AliveCount), EnemiesV3_2.Skull3.Color.ToEngineColor(), _skull3AliveCountIntensity);
			RenderValue("Skull IVs alive"u8, Inline.Utf8(b.Skull4AliveCount), EnemiesV3_2.Skull4.Color.ToEngineColor(), _skull4AliveCountIntensity);
			RenderValue("Squid Is alive"u8, Inline.Utf8(b.Squid1AliveCount), EnemiesV3_2.Squid1.Color.ToEngineColor(), _squid1AliveCountIntensity);
			RenderValue("Squid IIs alive"u8, Inline.Utf8(b.Squid2AliveCount), EnemiesV3_2.Squid2.Color.ToEngineColor(), _squid2AliveCountIntensity);
			RenderValue("Squid IIIs alive"u8, Inline.Utf8(b.Squid3AliveCount), EnemiesV3_2.Squid3.Color.ToEngineColor(), _squid3AliveCountIntensity);
			RenderValue("Centipedes alive"u8, Inline.Utf8(b.CentipedeAliveCount), EnemiesV3_2.Centipede.Color.ToEngineColor(), _centipedeAliveCountIntensity);
			RenderValue("Gigapedes alive"u8, Inline.Utf8(b.GigapedeAliveCount), EnemiesV3_2.Gigapede.Color.ToEngineColor(), _gigapedeAliveCountIntensity);
			RenderValue("Ghostpedes alive"u8, Inline.Utf8(b.GhostpedeAliveCount), EnemiesV3_2.Ghostpede.Color.ToEngineColor(), _ghostpedeAliveCountIntensity);
			RenderValue("Spider Is alive"u8, Inline.Utf8(b.Spider1AliveCount), EnemiesV3_2.SpiderEgg1.Color.ToEngineColor(), _spider1AliveCountIntensity);
			RenderValue("Spider IIs alive"u8, Inline.Utf8(b.Spider2AliveCount), EnemiesV3_2.SpiderEgg2.Color.ToEngineColor(), _spider2AliveCountIntensity);
			RenderValue("Spiderlings alive"u8, Inline.Utf8(b.SpiderlingAliveCount), EnemiesV3_2.Spiderling.Color.ToEngineColor(), _spiderlingAliveCountIntensity);
			RenderValue("Spider Eggs alive"u8, Inline.Utf8(b.SpiderEggAliveCount), EnemiesV3_2.SpiderEgg1.Color.ToEngineColor(), _spiderEggAliveCountIntensity);
			RenderValue("Leviathans alive"u8, Inline.Utf8(b.LeviathanAliveCount), EnemiesV3_2.Leviathan.Color.ToEngineColor(), _leviathanAliveCountIntensity);
			RenderValue("Orbs alive"u8, Inline.Utf8(b.OrbAliveCount), EnemiesV3_2.TheOrb.Color.ToEngineColor(), _orbAliveCountIntensity);
			RenderValue("Thorns alive"u8, Inline.Utf8(b.ThornAliveCount), EnemiesV3_2.Thorn.Color.ToEngineColor(), _thornAliveCountIntensity);
		}

		ImGui.EndChild();
	}

	private static void RenderValue(ReadOnlySpan<byte> label, ReadOnlySpan<byte> value, Color color, float intensity)
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

	private static Upgrade GetUpgrade(MainBlock block)
	{
		return block.LevelGems switch
		{
			< 10 => UpgradesV3_2.Level1,
			< 70 => UpgradesV3_2.Level2,
			70 => UpgradesV3_2.Level3,
			_ => UpgradesV3_2.Level4,
		};
	}

	private static Death? GetDeath(MainBlock block)
	{
		return Deaths.GetDeathByType(GameConstants.CurrentVersion, block.DeathType);
	}
}
