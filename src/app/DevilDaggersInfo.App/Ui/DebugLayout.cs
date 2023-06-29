using DevilDaggersInfo.App.Engine.Maths.Numerics;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui;

public static class DebugLayout
{
	private static long _previousAllocatedBytes;

	private static readonly List<string> _debugMessages = new();

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
		AddText(ref y, $"{Application.RenderCounter.CountPerSecond} FPS ({1f / Application.LastRenderDelta:000.000})");

		long allocatedBytes = GC.GetAllocatedBytesForCurrentThread();
		AddText(ref y, allocatedBytes + " bytes allocated on managed heap");
		AddText(ref y, allocatedBytes - _previousAllocatedBytes + " since last frame");
		_previousAllocatedBytes = allocatedBytes;

		AddText(ref y, GC.CollectionCount(0) + " gen 0 garbage collections");
		AddText(ref y, GC.CollectionCount(1) + " gen 1 garbage collections");
		AddText(ref y, GC.CollectionCount(2) + " gen 2 garbage collections");

		AddText(ref y, $"Modal active: {Modals.IsAnyOpen}");

		void AddText(ref float posY, string text)
		{
			drawList.AddText(new(0, posY), textColor, text);
			posY += 16;
		}

#if DEBUG
		ImGui.SetNextWindowSize(new(320, 256));
		if (ImGui.Begin("Debug"))
		{
			if (ImGui.Button("Error window"))
				Modals.ShowError("Test error!");

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

			foreach (string debugMessage in _debugMessages)
				ImGui.Text(debugMessage);

			ImGui.End();
		}
#endif
	}
}
