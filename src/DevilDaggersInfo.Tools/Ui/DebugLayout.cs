using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.Popups;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui;

public static class DebugLayout
{
	private static long _previousAllocatedBytes;
	private static bool _showOverlay = true;

	private static readonly List<string> _debugMessages = new();
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

#if DEBUG
			if (ImGui.Button("Show demo window"))
				UiRenderer.ShowDemoWindow();

			ImGui.Separator();

			if (ImGui.Button("Error window"))
				PopupManager.ShowError("Test error!");

			if (ImGui.Button("Warning log"))
				Root.Log.Warning("Test warning! This should be logged as WARNING.");

			if (ImGui.Button("Error log"))
				Root.Log.Error("Test error! This should be logged as ERROR.");

			ImGui.PushStyleColor(ImGuiCol.Button, Color.Red with { A = 127 });
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Color.Red);
			if (ImGui.Button("FATAL CRASH"))
				throw new("Test crash! This should be logged as FATAL.");
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
#else
		ImGui.Text("Other debug options are not available in RELEASE builds.");
#endif

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
		AddText(ref y, "Total GC pause duration", Inline.Span(GC.GetTotalPauseDuration() ));
		AddText(ref y, "Total app time", Inline.Span(DateTime.UtcNow - _startUpTime));

		AddText(ref y, "Modal active", PopupManager.IsAnyOpen ? bool.TrueString : bool.FalseString);

		AddText(ref y, "Devil Daggers window position", Inline.Span(Root.GameWindowService.GetWindowPosition()));

		void AddText(ref float posY, ReadOnlySpan<char> textLeft, ReadOnlySpan<char> textRight, uint textColor = 0xffffffff)
		{
			drawList.AddText(new(0, posY), textColor, textLeft);
			drawList.AddText(new(256, posY), textColor, textRight);
			posY += 16;
		}
	}
}
