using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.LeaderboardList;
using DevilDaggersInfo.Tools.Ui.Main.Previews;
using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;
using DevilDaggersInfo.Tools.Ui.Practice.Main;
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
	FontService fontService,
	PracticeLogic practiceLogic,
	ToolPreviewChild toolPreviewChild,
	SpawnsetEditorPreview spawnsetEditorPreview,
	AssetEditorPreview assetEditorPreview,
	ReplayEditorPreview replayEditorPreview,
	CustomLeaderboardsPreview customLeaderboardsPreview,
	PracticePreview practicePreview,
	ModManagerPreview modManagerPreview)
{
	private readonly string _version = $"{AssemblyUtils.EntryAssemblyVersionString} (ALPHA)";

	private IToolPreview? _toolPreview;

	public bool ShouldClose { get; private set; }

	public void Render()
	{
		Vector2 center = ImGui.GetCenter(ImGui.GetMainViewport());
		Vector2 windowSize = new(683, 768);
		Vector2 mainButtonsSize = new(208, 512);
		Vector2 previewSize = new(windowSize.X - mainButtonsSize.X - 32, 656);

		ImGui.SetNextWindowPos(center, ImGuiCond.Always, new Vector2(0.5f, 0.5f));
		ImGui.SetNextWindowSize(windowSize);

		const ImGuiWindowFlags windowFlags =
			ImGuiWindowFlags.NoResize |
			ImGuiWindowFlags.NoMove |
			ImGuiWindowFlags.NoCollapse |
			ImGuiWindowFlags.NoTitleBar |
			ImGuiWindowFlags.NoBringToFrontOnFocus |
			ImGuiWindowFlags.NoDocking;
		if (ImGui.Begin("Main Menu", windowFlags))
		{
			ImGui.PushFont(fontService.GoetheBold60);
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
				if (ToolButton(spawnsetEditorPreview))
					uiLayoutManager.Layout = LayoutType.SpawnsetEditor;
				if (ToolButton(assetEditorPreview))
					uiLayoutManager.Layout = LayoutType.AssetEditor;
				if (ToolButton(replayEditorPreview))
					uiLayoutManager.Layout = LayoutType.ReplayEditor;

				ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 16);
				if (ToolButton(customLeaderboardsPreview))
				{
					uiLayoutManager.Layout = LayoutType.CustomLeaderboards;
					leaderboardListChild.LoadAll();
				}

				if (ToolButton(practicePreview))
				{
					uiLayoutManager.Layout = LayoutType.Practice;
					practiceLogic.ShowWarning();
				}

				if (ToolButton(modManagerPreview))
				{
					uiLayoutManager.Layout = LayoutType.ModManager;
					modsDirectoryLogic.LoadModsDirectory();
				}
			}

			ImGui.EndChild();

			if (_toolPreview != null)
			{
				ImGui.SameLine();
				toolPreviewChild.Render(_toolPreview, previewSize);
			}
		}

		ImGui.End();
	}

	private static bool AppButton(Texture icon, ReadOnlySpan<byte> tooltip)
	{
		bool returnValue = false;

		Vector2 iconSize = new(36);
		if (ImGui.ImageButton(tooltip, icon.Id, iconSize))
			returnValue = true;

		if (ImGui.IsItemHovered())
			ImGui.SetTooltip(tooltip);

		return returnValue;
	}

	private bool ToolButton(IToolPreview toolPreview)
	{
		Vector4 color = GetColor(toolPreview.ColorConfiguration);

		ImGui.PushStyleColor(ImGuiCol.Button, color);
		ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color + new Vector4(0, 0, 0, 0.2f));
		ImGui.PushStyleColor(ImGuiCol.ButtonActive, color + new Vector4(0, 0, 0, 0.3f));
		ImGui.PushStyleColor(ImGuiCol.Border, color with { W = 1 });
		ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 2);

		ImGui.PushFont(fontService.GoetheBold20);
		bool returnValue = ImGui.Button(toolPreview.Title, new Vector2(198, 48));
		ImGui.PopFont();

		ImGui.PopStyleColor(4);
		ImGui.PopStyleVar();

		if (ImGui.IsItemHovered())
			_toolPreview = toolPreview;

		ImGui.Spacing();

		return returnValue;

		static Vector4 GetColor(ColorConfiguration colorConfiguration)
		{
			const float buttonColorDesaturation = 0.3f;
			return (Vector4.DesaturateColor(colorConfiguration.Primary, buttonColorDesaturation) * new Vector4(0.8f, 0.8f, 0.8f, 1)) with { W = 0.5f };
		}
	}
}
