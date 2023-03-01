using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Networking;
using DevilDaggersInfo.App.Ui.Base.Networking.TaskHandlers;
using DevilDaggersInfo.App.Ui.Base.Settings;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Extensions;
using DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Utils;
using System.Security.Cryptography;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.State;

public class StateWrapper : AbstractComponent
{
	private readonly Label _labelMemoryValue;
	private readonly Label _labelStateValue;
	private readonly Label _labelSpawnsetValue;
	private readonly Label _labelSubmissionValue;

#pragma warning disable S1450 // Cannot change this into a local. The events would not be raised.
	private FileSystemWatcher? _survivalFileWatcher;
#pragma warning restore S1450

	public StateWrapper(IBounds bounds)
		: base(bounds)
	{
		const int labelSeparatorPosition = 80;
		const int labelHeight = 16;
		int labelDepth = Depth + 2;

		string[] labelTexts =
		{
			"Memory", "State", "Spawnset", "Last upload",
		};

		for (int i = 0; i < labelTexts.Length; i++)
		{
			Label label = new(bounds.CreateNested(0, i * labelHeight, labelSeparatorPosition, labelHeight), labelTexts[i], LabelStyles.DefaultLeft) { Depth = labelDepth };
			NestingContext.Add(label);
		}

		_labelMemoryValue = new(bounds.CreateNested(labelSeparatorPosition, labelHeight * 0, labelSeparatorPosition, labelHeight), string.Empty, LabelStyles.DefaultLeft) { Depth = labelDepth };
		_labelStateValue = new(bounds.CreateNested(labelSeparatorPosition, labelHeight * 1, labelSeparatorPosition, labelHeight), string.Empty, LabelStyles.DefaultLeft) { Depth = labelDepth };
		_labelSpawnsetValue = new(bounds.CreateNested(labelSeparatorPosition, labelHeight * 2, labelSeparatorPosition, labelHeight), string.Empty, LabelStyles.DefaultLeft) { Depth = labelDepth };
		_labelSubmissionValue = new(bounds.CreateNested(labelSeparatorPosition, labelHeight * 3, labelSeparatorPosition, labelHeight), string.Empty, LabelStyles.DefaultLeft) { Depth = labelDepth };

		NestingContext.Add(_labelMemoryValue);
		NestingContext.Add(_labelStateValue);
		NestingContext.Add(_labelSpawnsetValue);
		NestingContext.Add(_labelSubmissionValue);
	}

	public void Initialize()
	{
		UpdateActiveSpawnsetBasedOnHash();

		_survivalFileWatcher = new(UserSettings.ModsDirectory, "survival");
		_survivalFileWatcher.NotifyFilter = NotifyFilters.CreationTime
			| NotifyFilters.DirectoryName
			| NotifyFilters.FileName
			| NotifyFilters.LastWrite
			| NotifyFilters.Size;
		_survivalFileWatcher.IncludeSubdirectories = true; // This needs to be enabled for some reason.
		_survivalFileWatcher.EnableRaisingEvents = true;
		_survivalFileWatcher.Changed += (_, _) => UpdateActiveSpawnsetBasedOnHash();
		_survivalFileWatcher.Deleted += (_, _) => UpdateActiveSpawnsetBasedOnHash();
		_survivalFileWatcher.Created += (_, _) => UpdateActiveSpawnsetBasedOnHash();
		_survivalFileWatcher.Renamed += (_, _) => UpdateActiveSpawnsetBasedOnHash();
		_survivalFileWatcher.Error += (_, _) => UpdateActiveSpawnsetBasedOnHash();

		void UpdateActiveSpawnsetBasedOnHash()
		{
			if (!File.Exists(UserSettings.ModsSurvivalPath))
			{
				StateManager.Dispatch(new SetActiveSpawnset(null));
			}
			else
			{
				try
				{
					byte[] fileContents = File.ReadAllBytes(UserSettings.ModsSurvivalPath);
					byte[] fileHash = MD5.HashData(fileContents);
					AsyncHandler.Run(s => StateManager.Dispatch(new SetActiveSpawnset(s?.Name)), () => FetchSpawnsetByHash.HandleAsync(fileHash));
				}
				catch (Exception ex)
				{
					Root.Dependencies.Log.Error(ex, "Failed to update active spawnset based on hash.");
				}
			}
		}
	}

	public void SetState()
	{
		_labelMemoryValue.Text = StateManager.MarkerState.Marker.HasValue ? $"0x{StateManager.MarkerState.Marker.Value:X}" : "Waiting...";
		_labelStateValue.Text = StateManager.RecordingState.RecordingStateType.ToDisplayString();
		_labelSpawnsetValue.Text = StateManager.ActiveSpawnsetState.Name ?? "(unknown)";
		_labelSubmissionValue.Text = DateTimeUtils.FormatTimeAgo(StateManager.RecordingState.LastSubmission);
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		const int border = 1;
		Root.Game.RectangleRenderer.Schedule(Bounds.Size, Bounds.Center + scrollOffset, Depth, Color.Red);
		Root.Game.RectangleRenderer.Schedule(Bounds.Size - new Vector2i<int>(border * 2), Bounds.Center + scrollOffset, Depth + 1, Color.Gray(0.1f));
	}
}
