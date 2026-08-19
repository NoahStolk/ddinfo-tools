using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.LeaderboardList;
using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;
using DevilDaggersInfo.Tools.Utils;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Main;

internal sealed class MainWindow(
	ResourceManager resourceManager,
	UiLayoutManager uiLayoutManager,
	FrameCounter frameCounter,
	LeaderboardListChild leaderboardListChild,
	AboutWindow aboutWindow,
	UpdateWindow updateWindow,
	ModsDirectoryLogic modsDirectoryLogic,
	FontService fontService)
{
	private readonly string _version = $"{AssemblyUtils.EntryAssemblyVersionString} (ALPHA)";

	private HoverText? _hoverText;

	public bool ShouldClose { get; private set; }

	public void Render()
	{
		Vector2 center = ImGui.GetCenter(ImGui.GetMainViewport());
		Vector2 windowSize = new(683, 768);
		Vector2 mainButtonsSize = new(208, 512);
		Vector2 previewSize = new(windowSize.X - mainButtonsSize.X - 16, 512);

		ImGui.SetNextWindowPos(center, ImGuiCond.Always, new Vector2(0.5f, 0.5f));
		ImGui.SetNextWindowSize(windowSize);

		if (ImGui.Begin("Main Menu", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoDocking))
		{
			ImGuiExt.PushFont(fontService.GoetheBold60);
			const string title = "ddinfo tools";
			ImGui.TextColored(Colors.TitleColor(frameCounter.TotalTime), title);
			float textWidth = ImGui.CalcTextSize(title).X;
			ImGui.PopFont();

			ImGui.SetCursorPos(new Vector2(textWidth + 16, 39));
			ImGui.Text(_version);
			ImGui.Text("Developed by Noah Stolk");

			ImGui.SetCursorPos(new Vector2(windowSize.X - 208, 8));
			if (AppButton(resourceManager.InternalResources.DownloadTexture, "Updates"u8))
				updateWindow.Show = true;

			ImGui.SameLine();
			if (AppButton(resourceManager.InternalResources.ConfigurationTexture, "Configuration"u8))
				uiLayoutManager.Layout = LayoutType.Config;

			ImGui.SameLine();
			if (AppButton(resourceManager.InternalResources.InfoTexture, "About"u8))
				aboutWindow.Show = true;

			ImGui.SameLine();
			if (AppButton(resourceManager.InternalResources.CloseTexture, "Exit application"u8))
				ShouldClose = true;

			ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 40);
			if (ImGui.BeginChild("ToolButtons", mainButtonsSize))
			{
				if (ToolButton(GetColor(Colors.SpawnsetEditor), "Spawnset Editor", StringResources.DescriptionSpawnsetEditor))
					uiLayoutManager.Layout = LayoutType.SpawnsetEditor;
				if (ToolButton(GetColor(Colors.AssetEditor), "Asset Editor", StringResources.DescriptionAssetEditor))
					uiLayoutManager.Layout = LayoutType.AssetEditor;
				if (ToolButton(GetColor(Colors.ReplayEditor), "Replay Editor", StringResources.DescriptionReplayEditor))
					uiLayoutManager.Layout = LayoutType.ReplayEditor;

				ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 16);
				if (ToolButton(GetColor(Colors.CustomLeaderboards), "Custom Leaderboards", StringResources.DescriptionCustomLeaderboards))
				{
					uiLayoutManager.Layout = LayoutType.CustomLeaderboards;
					leaderboardListChild.LoadAll();
				}

				if (ToolButton(GetColor(Colors.Practice), "Practice", StringResources.DescriptionPractice))
					uiLayoutManager.Layout = LayoutType.Practice;

				if (ToolButton(GetColor(Colors.ModManager), "Mod Manager", StringResources.DescriptionModManager))
				{
					uiLayoutManager.Layout = LayoutType.ModManager;
					modsDirectoryLogic.LoadModsDirectory();
				}

				static Color GetColor(ColorConfiguration colorConfiguration)
				{
					const byte buttonAlpha = 127;
					const float buttonColorDesaturation = 0.3f;
					return colorConfiguration.Primary.Desaturate(buttonColorDesaturation).Darken(0.2f) with { A = buttonAlpha };
				}
			}

			ImGui.EndChild();

			if (_hoverText is { } hoverText)
			{
				ImGui.SameLine();
				if (ImGui.BeginChild("Preview", previewSize))
				{
					ImGui.PushTextWrapPos(previewSize.X - 16);
					ImGuiExt.Title(hoverText.Title, fontService.GoetheBold30);
					ImGui.Text(hoverText.Description);
					ImGui.PopTextWrapPos();
				}

				ImGui.EndChild();
			}
		}

		ImGui.End();
	}

	private static bool AppButton(Texture icon, ReadOnlySpan<byte> tooltip)
	{
		bool returnValue = false;

		Vector2 iconSize = new(36);
		if (ImGuiImage.ImageButton(tooltip, icon.Id, iconSize))
			returnValue = true;

		if (ImGui.IsItemHovered())
			ImGui.SetTooltip(tooltip);

		return returnValue;
	}

	private bool ToolButton(Color color, string title, string description)
	{
		ImGui.PushStyleColor(ImGuiCol.Button, color);
		ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color + new Vector4(0, 0, 0, 0.2f));
		ImGui.PushStyleColor(ImGuiCol.ButtonActive, color + new Vector4(0, 0, 0, 0.3f));
		ImGui.PushStyleColor(ImGuiCol.Border, color with { A = 255 });
		ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 2);

		ImGuiExt.PushFont(fontService.GoetheBold20);
		bool returnValue = ImGui.Button(title, new Vector2(198, 48));
		ImGui.PopFont();

		ImGui.PopStyleColor(4);
		ImGui.PopStyleVar();

		if (ImGui.IsItemHovered())
			_hoverText = new HoverText(title, description);

		ImGui.Spacing();

		return returnValue;
	}

	private readonly record struct HoverText(string Title, string Description);
}
