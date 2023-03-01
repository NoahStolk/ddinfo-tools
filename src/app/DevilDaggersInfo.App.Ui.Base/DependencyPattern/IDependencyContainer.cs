using DevilDaggersInfo.App.Core.GameMemory;
using DevilDaggersInfo.App.Core.NativeInterface.Services;
using DevilDaggersInfo.App.Ui.Base.Platforms;
using Serilog.Core;

namespace DevilDaggersInfo.App.Ui.Base.DependencyPattern;

public interface IDependencyContainer
{
	#region Main dependencies

	Logger Log { get; }
	IPlatformSpecificValues PlatformSpecificValues { get; }
	INativeFileSystemService NativeFileSystemService { get; }
	INativeDialogService NativeDialogService { get; }

	#endregion Main dependencies

	#region DDCL dependencies

	GameMemoryService GameMemoryService { get; }

	#endregion DDCL dependencies

	#region UI Base dependencies

	IExtendedLayout ConfigLayout { get; }
	IExtendedLayout SettingsLayout { get; }
	IExtendedLayout MainLayout { get; }

	#endregion UI Base dependencies

	#region UI DDSE dependencies

	IExtendedLayout SurvivalEditorMainLayout { get; }
	IExtendedLayout SurvivalEditor3dLayout { get; }

	#endregion UI DDSE dependencies

	#region UI DDCL dependencies

	IExtendedLayout CustomLeaderboardsRecorderMainLayout { get; }
	IExtendedLayout CustomLeaderboardsRecorderReplayViewer3dLayout { get; }

	#endregion UI DDCL dependencies

	#region UI DDRE dependencies

	IExtendedLayout ReplayEditorMainLayout { get; }
	IExtendedLayout ReplayEditor3dLayout { get; }

	#endregion UI DDRE dependencies
}
