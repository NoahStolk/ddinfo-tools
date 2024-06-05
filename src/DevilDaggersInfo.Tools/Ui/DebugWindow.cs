using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Tools.Engine;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Networking;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Cache;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using Silk.NET.GLFW;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

public static class DebugWindow
{
	private static long _previousAllocatedBytes;

	private static readonly List<string> _debugMessages = [];
	private static readonly DateTime _startUpTime = DateTime.UtcNow;

	public static void Add(object? obj)
	{
		_debugMessages.Add(obj?.ToString() ?? "null");
	}

	private static void ClearDebugMessages()
	{
		_debugMessages.Clear();
	}

	public static void Render()
	{
		if (ImGui.Begin("Debug"))
		{
			ImGui.TextColored(NativeFileDialog.DialogOpen ? Color.White : Color.Gray(0.4f), NativeFileDialog.DialogOpen ? "Native dialog open" : "Native dialog closed");
			ImGui.TextColored(Root.AesBase32Wrapper == null ? Color.Red : Color.Green, Root.AesBase32Wrapper == null ? "Encryption unavailable" : "Encryption available");

			if (ImGui.CollapsingHeader("Popup debug info"))
			{
				RenderPopupDebugInfo();
			}

			if (ImGui.CollapsingHeader("Modded survival file"))
			{
				if (SurvivalFileWatcher.Exists)
				{
					ImGui.Text(EnumUtils.HandLevelNames[SurvivalFileWatcher.HandLevel]);
					ImGui.Text(Inline.Span(SurvivalFileWatcher.AdditionalGems));
					ImGui.Text(Inline.Span(SurvivalFileWatcher.TimerStart, StringFormats.TimeFormat));
				}
				else
				{
					ImGui.Text("<No modded survival file>");
				}
			}

			if (ImGui.CollapsingHeader("Keyboard input"))
			{
				RenderKeyboardInput();
			}

			if (ImGui.CollapsingHeader("Metrics"))
			{
				RenderMetrics();
			}

			if (ImGui.CollapsingHeader("User cache"))
			{
				RenderUserCache();
			}

			if (ImGui.CollapsingHeader("Debug messages"))
			{
				if (ImGui.Button("Clear"))
					ClearDebugMessages();

				for (int i = 0; i < _debugMessages.Count; i++)
					ImGui.Text(_debugMessages[i]);
			}

#if DEBUG
			ImGui.Separator();

			bool failAll = AsyncHandler.AutoFailAllCallsForTesting;
			if (ImGui.Checkbox("Auto-fail all API calls", ref failAll))
				AsyncHandler.AutoFailAllCallsForTesting = failAll;

			ImGui.Separator();

			if (ImGui.Button("Show demo window"))
				UiRenderer.ShowDemoWindow();

			ImGui.Separator();

			if (ImGui.Button("Error window"))
				PopupManager.ShowError("Test error!", "Test stack trace.");

			if (ImGui.Button("3 error windows"))
			{
				for (int i = 0; i < 3; i++)
					PopupManager.ShowError($"Test error {i + 1}!", "Test stack trace.");
			}

			if (ImGui.Button("Message window"))
				PopupManager.ShowMessage("Message", "Test message!");

			if (ImGui.Button("Warning log"))
				Root.Log.Warning("Test warning! This should be logged as WARNING.");

			if (ImGui.Button("Error log"))
				Root.Log.Error("Test error! This should be logged as ERROR.");

			ImGui.PushStyleColor(ImGuiCol.Button, Color.Red with { A = 127 });
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Color.Red);
			if (ImGui.Button("FATAL CRASH"))
				throw new InvalidOperationException("Test crash! This should be logged as FATAL.");
			ImGui.PopStyleColor(2);

			ImGui.Separator();

			ColorsButton("Main Colors", Colors.Main);
			ColorsButton("Spawnset Editor Colors", Colors.SpawnsetEditor);
			ColorsButton("Asset Editor Colors", Colors.AssetEditor);
			ColorsButton("Replay Editor Colors", Colors.ReplayEditor);
			ColorsButton("Custom Leaderboards Colors", Colors.CustomLeaderboards);
			ColorsButton("Practice Colors", Colors.Practice);
			ColorsButton("Mod Manager Colors", Colors.ModManager);

			static void ColorsButton(ReadOnlySpan<char> label, ColorConfiguration colorConfiguration)
			{
				ImGui.PushStyleColor(ImGuiCol.Button, colorConfiguration.Tertiary);
				ImGui.PushStyleColor(ImGuiCol.ButtonHovered, colorConfiguration.Secondary);
				ImGui.PushStyleColor(ImGuiCol.ButtonActive, colorConfiguration.Primary);
				if (ImGui.Button(label))
					Colors.SetColors(colorConfiguration);
				ImGui.PopStyleColor(3);
			}
#else
			ImGui.Text("Other debug options are not available in RELEASE builds.");
#endif
		}

		ImGui.End(); // End Debug
	}

	private static void RenderPopupDebugInfo()
	{
		ImGui.TextColored(PopupManager.IsAnyOpen ? Color.White : Color.Gray(0.4f), PopupManager.Popups.Count > 0 ? Inline.Span($"{PopupManager.Popups.Count} popup(s) active") : "No popups active");

		if (ImGui.BeginChild("PopupTableWrapper", new Vector2(0, 512)))
		{
			if (ImGui.BeginTable("PopupTable", 3, ImGuiTableFlags.ScrollY))
			{
				ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.WidthFixed, 100);
				ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 100);
				ImGui.TableSetupColumn("Has opened", ImGuiTableColumnFlags.WidthFixed, 100);

				ImGui.TableSetupScrollFreeze(0, 1);
				ImGui.TableHeadersRow();

				// ReSharper disable once ForCanBeConvertedToForeach
				for (int i = 0; i < PopupManager.Popups.Count; i++)
				{
					Popup popup = PopupManager.Popups[i];
					ImGui.TableNextRow();

					ImGui.TableNextColumn();
					ImGui.Text(popup.Id);

					ImGui.TableNextColumn();
					ImGui.Text(popup.GetType().Name);

					ImGui.TableNextColumn();
					ImGui.Text(popup.HasOpened ? "True" : "False");
				}

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}

	private static void RenderKeyboardInput()
	{
		ImGuiIOPtr io = ImGui.GetIO();
		ImGui.TextColored(io.KeyCtrl ? Color.White : Color.Gray(0.4f), "CTRL");
		ImGui.SameLine();
		ImGui.TextColored(io.KeyShift ? Color.White : Color.Gray(0.4f), "SHIFT");
		ImGui.SameLine();
		ImGui.TextColored(io.KeyAlt ? Color.White : Color.Gray(0.4f), "ALT");
		ImGui.SameLine();
		ImGui.TextColored(io.KeySuper ? Color.White : Color.Gray(0.4f), "SUPER");

		ImGui.Separator();
		if (ImGui.BeginTable("GLFW keys", 8))
		{
			ImGui.TableNextRow();
			for (int i = 0; i < EnumUtils.KeyNames.Count; i++)
			{
				Keys key = EnumUtils.Keys[i];
				bool isDown = Input.GlfwInput.IsKeyDown(key);
				ImGui.TableNextColumn();
				ImGui.TextColored(isDown ? Color.White : Color.Gray(0.4f), EnumUtils.KeyNames[key]);
			}

			ImGui.EndTable();
		}
	}

	private static void RenderMetrics()
	{
		AddText("FPS (smoothed)", Inline.Span(Root.Application.RenderCounter.CountPerSecond));
		AddText("FPS", Inline.Span(1f / Root.Application.LastRenderDelta, "000.000"));

		long allocatedBytes = GC.GetAllocatedBytesForCurrentThread();
		AddText("Total managed heap alloc in bytes", Inline.Span(allocatedBytes));

		long allocatedBytesDiff = allocatedBytes - _previousAllocatedBytes;
		Color color = allocatedBytesDiff switch
		{
			> 10_000 => Color.Red,
			> 1_000 => Color.Orange,
			> 500 => Color.Yellow,
			> 0 => new Color(255, 255, 127, 255),
			_ => Color.Green,
		};
		AddText("Heap alloc bytes since last frame", Inline.Span(allocatedBytesDiff), color);
		_previousAllocatedBytes = allocatedBytes;

		AddText("Gen 0 GCs", Inline.Span(GC.CollectionCount(0)));
		AddText("Gen 1 GCs", Inline.Span(GC.CollectionCount(1)));
		AddText("Gen 2 GCs", Inline.Span(GC.CollectionCount(2)));
		AddText("Total GC pause duration", Inline.Span(GC.GetTotalPauseDuration()));
		AddText("Total app time", Inline.Span(DateTime.UtcNow - _startUpTime));
		AddText("Devil Daggers window position", Inline.Span(Root.GameWindowService.GetWindowPosition()));
	}

	private static void RenderUserCache()
	{
		AddText("Player id", Inline.Span(UserCache.Model.PlayerId));

		ImGui.SeparatorText("Window");
		AddText("Maximized", UserCache.Model.WindowIsMaximized ? "True" : "False");
		AddText("Width", Inline.Span(UserCache.Model.WindowWidth));
		AddText("Height", Inline.Span(UserCache.Model.WindowHeight));
	}

	private static void AddText(ReadOnlySpan<char> textLeft, ReadOnlySpan<char> textRight)
	{
		AddText(textLeft, textRight, Color.White);
	}

	private static void AddText(ReadOnlySpan<char> textLeft, ReadOnlySpan<char> textRight, Color textColor)
	{
		ImGui.TextColored(textColor, textLeft);
		ImGui.SameLine(256);
		ImGui.TextColored(textColor, textRight);
	}
}
