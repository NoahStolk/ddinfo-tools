#if LINUX
using DevilDaggersInfo.App.Core.NativeInterface.Services.Linux;
#elif WINDOWS
using DevilDaggersInfo.App.Core.NativeInterface.Services.Windows;
#endif

using DevilDaggersInfo.App.Core.GameMemory;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Platforms;
using DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Layouts;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Layouts;

namespace DevilDaggersInfo.App;

public class DependencyContainer : IDependencyContainer
{
	public DependencyContainer()
	{
#if WINDOWS
		GameMemoryService = new(new WindowsMemoryService());
		PlatformSpecificValues = new WindowsValues();
#elif LINUX
		GameMemoryService = new(new LinuxMemoryService());
		PlatformSpecificValues = new LinuxValues();
#endif
	}

	public IExtendedLayout ConfigLayout { get; } = new Layouts.ConfigLayout();
	public IExtendedLayout MainLayout { get; } = new Layouts.MainLayout();

	public IExtendedLayout SurvivalEditorMainLayout { get; } = new SurvivalEditorMainLayout();
	public IExtendedLayout SurvivalEditorOpenLayout { get; } = new SurvivalEditorOpenLayout();
	public IExtendedLayout SurvivalEditorSaveLayout { get; } = new SurvivalEditorSaveLayout();
	public IExtendedLayout SurvivalEditor3dLayout { get; } = new SurvivalEditor3dLayout();
	public IExtendedLayout CustomLeaderboardsRecorderMainLayout { get; } = new CustomLeaderboardsRecorderMainLayout();
	public IExtendedLayout CustomLeaderboardsRecorderReplayViewer3dLayout { get; } = new ReplayViewer3dLayout();
	public GameMemoryService GameMemoryService { get; }
	public IPlatformSpecificValues PlatformSpecificValues { get; }
}
