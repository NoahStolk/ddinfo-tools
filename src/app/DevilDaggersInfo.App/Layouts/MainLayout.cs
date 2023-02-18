using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Networking;
using DevilDaggersInfo.App.Ui.Base.Networking.TaskHandlers;
using DevilDaggersInfo.App.Ui.Base.Rendering.Text;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;
using DevilDaggersInfo.App.Ui.Scene;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Core.Versioning;
using Warp.NET.Text;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Layouts;

public class MainLayout : Layout, IExtendedLayout
{
	private static readonly string _version = VersionUtils.EntryAssemblyVersion;

	private readonly ArenaScene _arenaScene = new();

	public MainLayout()
	{
		TextButtonStyle textButtonStyle = new(Color.White, TextAlign.Middle, FontSize.H16);

		AddButton(0, 0, Color.FromHsv(000, 1, 0.8f), () => StateManager.Dispatch(new SetLayout(Root.Dependencies.SurvivalEditorMainLayout)), "Survival Editor (wip)");
		AddButton(1, 0, Color.FromHsv(130, 1, 0.6f), () => { }, "Asset Editor (todo)");
		AddButton(2, 0, Color.FromHsv(220, 1, 1.0f), () => StateManager.Dispatch(new SetLayout(Root.Dependencies.ReplayEditorMainLayout)), "Replay Editor (wip)");
		AddButton(0, 1, Color.FromHsv(270, 1, 1.0f), () => StateManager.Dispatch(new SetLayout(Root.Dependencies.CustomLeaderboardsRecorderMainLayout)), "Custom Leaderboards");
		AddButton(1, 1, Color.FromHsv(032, 1, 0.8f), () => { }, "Practice (todo)");
		AddButton(2, 1, Color.FromHsv(320, 1, 0.8f), () => { }, "Mod Manager (todo)");
		AddButton(0, 2, Color.Gray(0.3f), () => StateManager.Dispatch(new SetLayout(Root.Dependencies.ConfigLayout)), "Configuration");
		AddButton(1, 2, Color.Gray(0.3f), () => StateManager.Dispatch(new SetLayout(Root.Dependencies.SettingsLayout)), "Settings");
		AddButton(2, 2, Color.Gray(0.3f), () => Environment.Exit(0), "Exit");

		void AddButton(int x, int y, Color color, Action onClick, string text)
		{
			int xPos = x switch
			{
				0 => 160,
				1 => 416,
				_ => 672,
			};
			int yPos = y * 128 + 256;
			NestingContext.Add(new TextButton(new PixelBounds(xPos, yPos, 192, 96), onClick, GetStyle(color), textButtonStyle, text));

			static ButtonStyle GetStyle(Color color)
			{
				const int border = 5;
				return new(color.Intensify(64), color, color.Intensify(96), border);
			}
		}

		StateManager.Subscribe<InitializeContent>(() => _arenaScene.BuildMainMenu());

		AsyncHandler.Run(ShowUpdateAvailable, () => FetchLatestVersion.HandleAsync(Root.Game.AppVersion, Root.Dependencies.PlatformSpecificValues.BuildType));
	}

	private static void ShowUpdateAvailable(AppVersion? newAppVersion)
	{
		if (newAppVersion != null)
			Root.Dependencies.NativeDialogService.ReportMessage("Update available", $"Version {newAppVersion} is available. Re-run the launcher to install it.");
	}

	public void Update()
	{
		_arenaScene.Update(0);
	}

	public void Render3d()
	{
		_arenaScene.Render();
	}

	public void Render()
	{
		Game.Self.MonoSpaceFontRenderer64.Schedule(Vector2i<int>.One, new(512, 64), 0, Color.Red, "DDINFO", TextAlign.Middle);
		Game.Self.MonoSpaceFontRenderer32.Schedule(Vector2i<int>.One, new(512, 128), 0, new(255, 127, 0, 255), "TOOLS", TextAlign.Middle);
		Game.Self.MonoSpaceFontRenderer24.Schedule(Vector2i<int>.One, new(512, 176), 0, new(255, 191, 0, 255), _version, TextAlign.Middle);
		Game.Self.MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, new(512, 712), 0, Color.White, "Devil Daggers is created by Sorath", TextAlign.Middle);
		Game.Self.MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, new(512, 728), 0, Color.White, "DevilDaggers.info is created by Noah Stolk", TextAlign.Middle);
		Game.Self.MonoSpaceFontRenderer24.Schedule(Vector2i<int>.One, new(512, 752), 0, new(255, 0, 31, 255), "HTTPS://DEVILDAGGERS.INFO/", TextAlign.Middle);
	}
}
