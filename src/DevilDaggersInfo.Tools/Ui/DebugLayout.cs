using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using Silk.NET.Input;

namespace DevilDaggersInfo.Tools.Ui;

public static class DebugLayout
{
	private static long _previousAllocatedBytes;
	private static bool _showOverlay = true;

	private static readonly List<string> _debugMessages = [];
	private static readonly DateTime _startUpTime = DateTime.UtcNow;

	public static void Add(object? obj)
	{
		_debugMessages.Add(obj?.ToString() ?? "null");
	}

	public static void Clear()
	{
		_debugMessages.Clear();
	}

	public static void Render()
	{
		if (_showOverlay)
			RenderOverlay();

		if (ImGui.Begin("Debug"))
		{
			ImGui.Checkbox("Show overlay", ref _showOverlay);

			ImGui.TextColored(PopupManager.IsAnyOpen ? Color.White : Color.Gray(0.4f), PopupManager.IsAnyOpen ? "Modal active" : "Modal inactive");
			ImGui.TextColored(NativeFileDialog.DialogOpen ? Color.White : Color.Gray(0.4f), NativeFileDialog.DialogOpen ? "Native dialog open" : "Native dialog closed");
			ImGui.TextColored(Root.AesBase32Wrapper == null ? Color.Red : Color.Green, Root.AesBase32Wrapper == null ? "Encryption unavailable" : "Encryption available");

			ImGui.SeparatorText("Modded survival file");
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

			ImGui.SeparatorText("ImGui key modifiers");

			ImGuiIOPtr io = ImGui.GetIO();
			ImGui.TextColored(io.KeyCtrl ? Color.White : Color.Gray(0.4f), "CTRL");
			ImGui.SameLine();
			ImGui.TextColored(io.KeyShift ? Color.White : Color.Gray(0.4f), "SHIFT");
			ImGui.SameLine();
			ImGui.TextColored(io.KeyAlt ? Color.White : Color.Gray(0.4f), "ALT");
			ImGui.SameLine();
			ImGui.TextColored(io.KeySuper ? Color.White : Color.Gray(0.4f), "SUPER");

			ImGui.Separator();
			if (ImGui.CollapsingHeader("Silk.NET keys"))
			{
				IKeyboard? keyboard = Root.Keyboard;

				if (keyboard == null)
				{
					ImGui.TextColored(Color.Red, "Keyboard unavailable");
				}
				else
				{
					if (ImGui.BeginTable("SilkKeys", 8))
					{
						for (int i = 0; i < keyboard.SupportedKeys.Count; i++)
						{
							bool isDown = keyboard.IsKeyPressed(keyboard.SupportedKeys[i]);
							if (i == 0)
								ImGui.TableNextRow();

							ImGui.TableNextColumn();
							ImGui.TextColored(isDown ? Color.White : Color.Gray(0.4f), EnumUtils.KeyNames[keyboard.SupportedKeys[i]]);
						}

						ImGui.EndTable();
					}
				}
			}

#if DEBUG
			ImGui.Separator();

			if (ImGui.Button("Show demo window"))
				UiRenderer.ShowDemoWindow();

			if (ImGui.Button("Show update available"))
			{
				UpdateWindow.AvailableUpdateVersion = new Version(0, 0, 0, 0);
				UiRenderer.ShowUpdateAvailable();
			}

			ImGui.Separator();

			if (ImGui.Button("Error window"))
				PopupManager.ShowError("Test error!", "Test stack trace.");

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

			if (ImGui.Button("Clear"))
				_debugMessages.Clear();

			for (int i = 0; i < _debugMessages.Count; i++)
				ImGui.Text(_debugMessages[i]);

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

	private static void RenderOverlay()
	{
		ImDrawListPtr drawList = ImGui.GetForegroundDrawList();
		float y = 0;
		AddText(ref y, "FPS (smoothed)", Inline.Span(Root.Application.RenderCounter.CountPerSecond));
		AddText(ref y, "FPS", Inline.Span(1f / Root.Application.LastRenderDelta, "000.000"));

		long allocatedBytes = GC.GetAllocatedBytesForCurrentThread();
		AddText(ref y, "Total managed heap alloc in bytes", Inline.Span(allocatedBytes));

		long allocatedBytesDiff = allocatedBytes - _previousAllocatedBytes;
		uint color = allocatedBytesDiff switch
		{
			> 10_000 => 0xff0000ff,
			> 1_000 => 0xff0088ff,
			> 500 => 0xff00ffff,
			> 0 => 0xff88ffff,
			_ => 0xff00ff00,
		};
		AddText(ref y, "Heap alloc bytes since last frame", Inline.Span(allocatedBytesDiff), color);
		_previousAllocatedBytes = allocatedBytes;

		AddText(ref y, "Gen 0 GCs", Inline.Span(GC.CollectionCount(0)));
		AddText(ref y, "Gen 1 GCs", Inline.Span(GC.CollectionCount(1)));
		AddText(ref y, "Gen 2 GCs", Inline.Span(GC.CollectionCount(2)));
		AddText(ref y, "Total GC pause duration", Inline.Span(GC.GetTotalPauseDuration()));
		AddText(ref y, "Total app time", Inline.Span(DateTime.UtcNow - _startUpTime));
		AddText(ref y, "Devil Daggers window position", Inline.Span(Root.GameWindowService.GetWindowPosition()));

		void AddText(ref float posY, ReadOnlySpan<char> textLeft, ReadOnlySpan<char> textRight, uint textColor = 0xffffffff)
		{
			drawList.AddText(new(0, posY), textColor, textLeft);
			drawList.AddText(new(256, posY), textColor, textRight);
			posY += 16;
		}
	}
}
