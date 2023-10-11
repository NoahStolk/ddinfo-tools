using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.LeaderboardList;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Main;

public static class MainWindow
{
	private static readonly string _version = $"{AssemblyUtils.EntryAssemblyVersion} (ALPHA)";

	private static Action? _hoveredButtonAction;

	public static bool ShouldClose { get; private set; }

	public static void Render()
	{
		Vector2 center = ImGui.GetMainViewport().GetCenter();
		Vector2 windowSize = new(683, 768);
		Vector2 mainButtonsSize = new(208, 512);
		Vector2 previewSize = new(windowSize.X - mainButtonsSize.X - 16, 512);

		ImGui.SetNextWindowPos(center, ImGuiCond.Always, new(0.5f, 0.5f));
		ImGui.SetNextWindowSize(windowSize);

		if (ImGui.Begin("Main Menu", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBringToFrontOnFocus))
		{
			ImGui.PushFont(Root.FontGoetheBold60);
			const string title = "ddinfo tools";
			ImGui.TextColored(Colors.TitleColor, title);
			float textWidth = ImGui.CalcTextSize(title).X;
			ImGui.PopFont();

			ImGui.SetCursorPos(new(textWidth + 16, 39));
			ImGui.Text(_version);
			ImGui.Text("Developed by Noah Stolk");

			ImGui.SetCursorPos(new(windowSize.X - 208, 8));
			AppButton(Root.InternalResources.ConfigurationTexture, "Configuration", static () => UiRenderer.Layout = LayoutType.Config);

			ImGui.SameLine();
			AppButton(Root.InternalResources.SettingsTexture, "Settings", UiRenderer.ShowSettings);

			ImGui.SameLine();
			AppButton(Root.InternalResources.InfoTexture, "About", UiRenderer.ShowAbout);

			ImGui.SameLine();
			AppButton(Root.InternalResources.CloseTexture, "Exit application", static () => ShouldClose = true);

			ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 40);
			if (ImGui.BeginChild("ToolButtons", mainButtonsSize))
			{
				ToolButton(GetColor(Colors.SpawnsetEditor.Primary), "Spawnset Editor", GoToSpawnsetEditor, ref _hoveredButtonAction, RenderSpawnsetEditorPreview);
				ToolButton(GetColor(Colors.AssetEditor.Primary), "Asset Editor", static () => { }, ref _hoveredButtonAction, RenderAssetEditorPreview);
				ToolButton(GetColor(Colors.ReplayEditor.Primary), "Replay Editor", GoToReplayEditor, ref _hoveredButtonAction, RenderReplayEditorPreview);

				ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 16);
				ToolButton(GetColor(Colors.CustomLeaderboards.Primary), "Custom Leaderboards", GoToCustomLeaderboards, ref _hoveredButtonAction, RenderCustomLeaderboardsPreview);
				ToolButton(GetColor(Colors.Practice.Primary), "Practice", GoToPractice, ref _hoveredButtonAction, RenderPracticePreview);
				ToolButton(GetColor(Colors.ModManager.Primary), "Mod Manager", static () => { }, ref _hoveredButtonAction, RenderModManagerPreview);

				static Color GetColor(Color primary)
				{
					const byte buttonAlpha = 127;
					const float buttonColorDesaturation = 0.3f;
					return primary.Desaturate(buttonColorDesaturation).Darken(0.2f) with { A = buttonAlpha };
				}

				static void GoToSpawnsetEditor() => UiRenderer.Layout = LayoutType.SpawnsetEditor;
				static void GoToReplayEditor() => UiRenderer.Layout = LayoutType.ReplayEditor;
				static void GoToPractice() => UiRenderer.Layout = LayoutType.Practice;

				static void GoToCustomLeaderboards()
				{
					UiRenderer.Layout = LayoutType.CustomLeaderboards;
					LeaderboardListChild.LoadAll();
				}
			}

			ImGui.EndChild(); // End ToolButtons

			if (_hoveredButtonAction != null)
			{
				ImGui.SameLine();
				if (ImGui.BeginChild("Preview", previewSize))
				{
					ImGui.PushTextWrapPos(previewSize.X - 16);
					_hoveredButtonAction.Invoke();
					ImGui.PopTextWrapPos();
				}

				ImGui.EndChild(); // End Preview
			}
		}

		ImGui.End(); // End Main Menu
	}

	private static void AppButton(Texture icon, ReadOnlySpan<char> tooltip, Action action)
	{
		Vector2 iconSize = new(36);
		if (ImGuiImage.ImageButton(tooltip, icon.Handle, iconSize))
			action();

		if (ImGui.IsItemHovered())
			ImGui.SetTooltip(tooltip);
	}

	private static void ToolButton(Color color, ReadOnlySpan<char> text, Action action, ref Action? hoveredAction, Action onHover)
	{
		ImGui.PushStyleColor(ImGuiCol.Button, color);
		ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color + new Vector4(0, 0, 0, 0.2f));
		ImGui.PushStyleColor(ImGuiCol.ButtonActive, color + new Vector4(0, 0, 0, 0.3f));
		ImGui.PushStyleColor(ImGuiCol.Border, color with { A = 255 });
		ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 2);

		ImGui.PushFont(Root.FontGoetheBold20);
		bool clicked = ImGui.Button(text, new(198, 48));
		ImGui.PopFont();

		ImGui.PopStyleColor(4);
		ImGui.PopStyleVar();

		if (ImGui.IsItemHovered())
			hoveredAction = onHover;

		if (clicked)
			action.Invoke();

		ImGui.Spacing();
	}

	private static void RenderSpawnsetEditorPreview()
	{
		ImGuiExt.Title("Spawnset Editor");
		ImGui.Text("""
			WORK IN PROGRESS

			Create and edit custom spawnsets (levels) for Devil Daggers.

			Some things you can do:
			- Create your own set of enemy spawns.
			- Create a custom arena.
			- Start with any hand upgrade.
			- Give yourself 10,000 homing daggers.
			- Use the Time Attack game mode, where the goal is to kill all enemies as fast as possible.
			- Use the Race game mode, where the goal is to reach the dagger as fast as possible.

			Be sure to check out the custom leaderboards to see what's possible.

			Note that using custom spawnsets will not submit your score to the official leaderboards.

			Spawnsets can only be used to practice, not cheat. They're completely safe to use.
			""");
	}

	private static void RenderAssetEditorPreview()
	{
		ImGuiExt.Title("Asset Editor");
		ImGui.Text("""
			NOT IMPLEMENTED YET

			Create and edit mods for Devil Daggers.

			The following assets can be changed:
			- Audio
			- Bindings
			- Meshes
			- Shaders
			- Textures

			Some mods are prohibited, meaning that you cannot submit scores over 1000 seconds with them.
			""");
	}

	private static void RenderReplayEditorPreview()
	{
		ImGuiExt.Title("Replay Editor");
		ImGui.Text("""
			WORK IN PROGRESS

			Create, analyze, and edit replays for Devil Daggers.
			""");
	}

	private static void RenderCustomLeaderboardsPreview()
	{
		ImGuiExt.Title("Custom Leaderboards");
		ImGui.Text("""
			WINDOWS ONLY (FOR NOW)

			Custom leaderboards are leaderboards for custom spawnsets.

			All game modes are supported:
			- Survival
			- Time Attack
			- Race

			Leaderboards can be sorted by:
			- Time
			- Gems collected
			- Gems despawned
			- Gems eaten
			- Enemies killed
			- Enemies alive
			- Homing stored
			- Homing eaten

			Criteria can also be set for custom leaderboards, meaning that in order to submit a score, it has to meet all the criteria.

			This applies to almost every stat in the game, including specific enemy kill counts.

			Examples:
			- Gems eaten must be less than 30
			- Squid I kills must be equal to 0
			- Skull II kills must be greater than or equal to 3
			- Daggers fired must be less than gems collected + 2
			- Skull III kills must be greater than Skull I kills + Skull II kills
			""");
	}

	private static void RenderPracticePreview()
	{
		ImGuiExt.Title("Practice");
		ImGui.Text("""
			Practice the main game by starting at any point in time with any hand upgrade and amount of gems/homing.

			Save templates to quickly load your desired practice settings.

			View live data from the current run (Windows only for now):
			- Splits
			- Homing usage
			- Gem collection
			""");
	}

	private static void RenderModManagerPreview()
	{
		ImGuiExt.Title("Mod Manager");
		ImGui.Text("""
			NOT IMPLEMENTED YET

			Manage mods downloaded from the website.
			""");
	}
}