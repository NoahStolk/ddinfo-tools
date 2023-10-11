using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.Popups;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui;

public static class DebugLayout
{
	private static long _previousAllocatedBytes;

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
		ImDrawListPtr drawList = ImGui.GetForegroundDrawList();
		const uint textColor = 0xffffffff;
		float y = 0;
		AddText(ref y, "FPS (smoothed)", Inline.Span(Root.Application.RenderCounter.CountPerSecond));
		AddText(ref y, "FPS", Inline.Span(1f / Root.Application.LastRenderDelta, "000.000"));

		long allocatedBytes = GC.GetAllocatedBytesForCurrentThread();
		AddText(ref y, "Total managed heap alloc in bytes", Inline.Span(allocatedBytes));
		AddText(ref y, "Heap alloc bytes since last frame", Inline.Span(allocatedBytes - _previousAllocatedBytes));
		_previousAllocatedBytes = allocatedBytes;

		AddText(ref y, "Gen 0 GCs", Inline.Span(GC.CollectionCount(0)));
		AddText(ref y, "Gen 1 GCs", Inline.Span(GC.CollectionCount(1)));
		AddText(ref y, "Gen 2 GCs", Inline.Span(GC.CollectionCount(2)));
		AddText(ref y, "Total GC pause duration", Inline.Span(GC.GetTotalPauseDuration() ));
		AddText(ref y, "Total app time", Inline.Span(DateTime.UtcNow - _startUpTime));

		AddText(ref y, "Modal active", PopupManager.IsAnyOpen ? bool.TrueString : bool.FalseString);

		AddText(ref y, "Devil Daggers window position", Inline.Span(Root.GameWindowService.GetWindowPosition()));

		void AddText(ref float posY, ReadOnlySpan<char> textLeft, ReadOnlySpan<char> textRight)
		{
			drawList.AddText(new(0, posY), textColor, textLeft);
			drawList.AddText(new(256, posY), textColor, textRight);
			posY += 16;
		}

#if DEBUG
		if (ImGui.Begin("Debug"))
		{
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

			if (ImGui.Button("Clear"))
				_debugMessages.Clear();

			for (int i = 0; i < _debugMessages.Count; i++)
				ImGui.Text(_debugMessages[i]);
		}

		ImGui.End(); // End Debug
#endif
	}
}