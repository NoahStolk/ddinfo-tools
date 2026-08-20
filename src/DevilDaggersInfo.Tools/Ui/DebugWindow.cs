using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Dialogs;
using DevilDaggersInfo.Tools.Encryption;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.GameWindow;
using DevilDaggersInfo.Tools.Networking;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Cache;
using Hexa.NET.ImGui;
using Serilog;
using Silk.NET.GLFW;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

internal sealed class DebugWindow(
	GlfwInput glfwInput,
	FrameCounter frameCounter,
	INativeFileDialog nativeFileDialog,
	IEncryptionService encryptionService,
	PopupManager popupManager,
	GameWindowService gameWindowService,
#if DEBUG
	ILogger logger,
#endif
	UserCache userCache,
	SurvivalFileWatcher survivalFileWatcher)
{
	private readonly List<string> _debugMessages = [];
	private readonly DateTime _startUpTime = DateTime.UtcNow;

	private long _previousAllocatedBytes;

	public bool ShowDemoWindow;

	public void Add(object? obj)
	{
		_debugMessages.Add(obj?.ToString() ?? "null");
	}

	private void ClearDebugMessages()
	{
		_debugMessages.Clear();
	}

	public void Render()
	{
		if (ImGui.Begin("Debug"))
		{
			ImGui.TextColored(nativeFileDialog.DialogOpen ? Color.White : Color.Gray(0.4f), nativeFileDialog.DialogOpen ? "Native dialog open" : "Native dialog closed");
			ImGui.TextColored(encryptionService.IsAvailable ? Color.Green : Color.Red, encryptionService.IsAvailable ? "Encryption available" : "Encryption unavailable");

			if (ImGui.CollapsingHeader("Popup debug info"))
			{
				RenderPopupDebugInfo();
			}

			if (ImGui.CollapsingHeader("Modded survival file"))
			{
				if (survivalFileWatcher.Exists)
				{
					ImGui.Text(survivalFileWatcher.HandLevel.AsUtf8Span());
					ImGui.Text(Inline.Utf8(survivalFileWatcher.AdditionalGems));
					ImGui.Text(Inline.Utf8(survivalFileWatcher.TimerStart, StringFormats.TimeFormat));
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
				ShowDemoWindow = true;

			ImGui.Separator();

			if (ImGui.Button("Error window"))
				popupManager.ShowError("Test error!", "Test stack trace.");

			if (ImGui.Button("3 error windows"))
			{
				for (int i = 0; i < 3; i++)
					popupManager.ShowError($"Test error {i + 1}!", "Test stack trace.");
			}

			if (ImGui.Button("Message window"))
				popupManager.ShowMessage("Message", "Test message!");

			if (ImGui.Button("Warning log"))
				logger.Warning("Test warning! This should be logged as WARNING.");

			if (ImGui.Button("Error log"))
				logger.Error("Test error! This should be logged as ERROR.");

			ImGui.PushStyleColor(ImGuiCol.Button, Color.Red with { A = 127 });
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Color.Red);
			if (ImGui.Button("FATAL CRASH"))
				throw new InvalidOperationException("Test crash! This should be logged as FATAL.");
			ImGui.PopStyleColor(2);

			ImGui.Separator();

			ColorsButton("Main Colors"u8, Colors.Main);
			ColorsButton("Spawnset Editor Colors"u8, Colors.SpawnsetEditor);
			ColorsButton("Asset Editor Colors"u8, Colors.AssetEditor);
			ColorsButton("Replay Editor Colors"u8, Colors.ReplayEditor);
			ColorsButton("Custom Leaderboards Colors"u8, Colors.CustomLeaderboards);
			ColorsButton("Practice Colors"u8, Colors.Practice);
			ColorsButton("Mod Manager Colors"u8, Colors.ModManager);

			static void ColorsButton(ReadOnlySpan<byte> label, ColorConfiguration colorConfiguration)
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

		ImGui.End();
	}

	private void RenderPopupDebugInfo()
	{
		ImGui.TextColored(popupManager.IsAnyOpen ? Color.White : Color.Gray(0.4f), popupManager.Popups.Count > 0 ? Inline.Utf8($"{popupManager.Popups.Count} popup(s) active") : "No popups active"u8);

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
				for (int i = 0; i < popupManager.Popups.Count; i++)
				{
					Popup popup = popupManager.Popups[i];
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

	private void RenderKeyboardInput()
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
			for (int i = 0; i < KeysGen.Values.Count; i++)
			{
				Keys key = KeysGen.Values[i];
				bool isDown = glfwInput.IsKeyDown(key);
				ImGui.TableNextColumn();
				ImGui.TextColored(isDown ? Color.White : Color.Gray(0.4f), key.AsUtf8Span());
			}

			ImGui.EndTable();
		}
	}

	private void RenderMetrics()
	{
		AddText("FPS (smoothed)"u8, Inline.Utf8(frameCounter.CountPerSecond));
		AddText("FPS"u8, Inline.Utf8(1f / frameCounter.LastRenderDelta, "000.000"));

		long allocatedBytes = GC.GetAllocatedBytesForCurrentThread();
		AddText("Total managed heap alloc in bytes"u8, Inline.Utf8(allocatedBytes));

		long allocatedBytesDiff = allocatedBytes - _previousAllocatedBytes;
		Color color = allocatedBytesDiff switch
		{
			> 10_000 => Color.Red,
			> 1_000 => Color.Orange,
			> 500 => Color.Yellow,
			> 0 => new Color(255, 255, 127, 255),
			_ => Color.Green,
		};
		AddText("Heap alloc bytes since last frame"u8, Inline.Utf8(allocatedBytesDiff), color);
		_previousAllocatedBytes = allocatedBytes;

		AddText("Gen 0 GCs"u8, Inline.Utf8(GC.CollectionCount(0)));
		AddText("Gen 1 GCs"u8, Inline.Utf8(GC.CollectionCount(1)));
		AddText("Gen 2 GCs"u8, Inline.Utf8(GC.CollectionCount(2)));
		AddText("Total GC pause duration"u8, Inline.Utf8(GC.GetTotalPauseDuration()));
		AddText("Total app time"u8, Inline.Utf8(DateTime.UtcNow - _startUpTime));
		AddText("Devil Daggers window position"u8, Inline.Utf8(gameWindowService.GetWindowPosition()));
	}

	private void RenderUserCache()
	{
		AddText("Player id"u8, Inline.Utf8(userCache.Model.PlayerId));

		ImGui.SeparatorText("Window");
		AddText("Maximized"u8, userCache.Model.WindowIsMaximized ? "True"u8 : "False"u8);
		AddText("Width"u8, Inline.Utf8(userCache.Model.WindowWidth));
		AddText("Height"u8, Inline.Utf8(userCache.Model.WindowHeight));
	}

	private static void AddText(ReadOnlySpan<byte> textLeft, ReadOnlySpan<byte> textRight)
	{
		AddText(textLeft, textRight, Color.White);
	}

	private static void AddText(ReadOnlySpan<byte> textLeft, ReadOnlySpan<byte> textRight, Color textColor)
	{
		ImGui.TextColored(textColor, textLeft);
		ImGui.SameLine(256);
		ImGui.TextColored(textColor, textRight);
	}
}
