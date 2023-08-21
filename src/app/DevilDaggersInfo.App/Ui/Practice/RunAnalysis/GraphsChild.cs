using DevilDaggersInfo.App.Engine.Extensions;
using DevilDaggersInfo.App.Engine.Maths;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Extensions;
using DevilDaggersInfo.App.Ui.Practice.RunAnalysis.Data;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.Practice.RunAnalysis;

public static class GraphsChild
{
	private static readonly List<int> _gemsCollected = new();
	private static readonly List<int> _gemsDespawned = new();
	private static readonly List<int> _gemsEaten = new();
	private static readonly List<int> _gemsTotal = new();
	private static readonly List<int> _homingStored = new();
	private static readonly List<int> _homingEaten = new();

	// Allow up to an hour of data (roughly 3600 seconds in game).
	private static readonly Vector2[] _gemsCollectedPoints = new Vector2[60 * 60];
	private static readonly Vector2[] _gemsDespawnedPoints = new Vector2[60 * 60];
	private static readonly Vector2[] _gemsEatenPoints = new Vector2[60 * 60];
	private static readonly Vector2[] _gemsTotalPoints = new Vector2[60 * 60];
	private static readonly Vector2[] _homingStoredPoints = new Vector2[60 * 60];
	private static readonly Vector2[] _homingEatenPoints = new Vector2[60 * 60];

	private static int _maxGemsCollected;
	private static int _maxGemsDespawned;
	private static int _maxGemsEaten;
	private static int _maxGemsTotal;
	private static int _maxHomingStored;
	private static int _maxHomingEaten;

	private static bool _showGemsCollected = true;
	private static bool _showGemsDespawned = true;
	private static bool _showGemsEaten = true;
	private static bool _showGemsTotal = true;
	private static bool _showHomingStored = true;
	private static bool _showHomingEaten = true;

	public static void Update(IReadOnlyList<StatisticEntry> data)
	{
		_gemsCollected.Clear();
		_gemsDespawned.Clear();
		_gemsEaten.Clear();
		_gemsTotal.Clear();
		_homingStored.Clear();
		_homingEaten.Clear();

		for (int i = 0; i < data.Count; i++)
		{
			_gemsCollected.Add(data[i].GemsCollected);
			_gemsDespawned.Add(data[i].GemsDespawned);
			_gemsEaten.Add(data[i].GemsEaten);
			_gemsTotal.Add(data[i].GemsTotal);
			_homingStored.Add(data[i].HomingStored);
			_homingEaten.Add(data[i].HomingEaten);
		}

		_maxGemsCollected = _gemsCollected.Count > 0 ? _gemsCollected.Max() : 0;
		_maxGemsDespawned = _gemsDespawned.Count > 0 ? _gemsDespawned.Max() : 0;
		_maxGemsEaten = _gemsEaten.Count > 0 ? _gemsEaten.Max() : 0;
		_maxGemsTotal = _gemsTotal.Count > 0 ? _gemsTotal.Max() : 0;
		_maxHomingStored = _homingStored.Count > 0 ? _homingStored.Max() : 0;
		_maxHomingEaten = _homingEaten.Count > 0 ? _homingEaten.Max() : 0;
	}

	public static unsafe void Render()
	{
		Debug.Assert(_gemsCollected.Count == _gemsDespawned.Count, "All lists should have the same length.");
		Debug.Assert(_gemsCollected.Count == _gemsEaten.Count, "All lists should have the same length.");
		Debug.Assert(_gemsCollected.Count == _gemsTotal.Count, "All lists should have the same length.");
		Debug.Assert(_gemsCollected.Count == _homingStored.Count, "All lists should have the same length.");
		Debug.Assert(_gemsCollected.Count == _homingEaten.Count, "All lists should have the same length.");
		int count = _gemsCollected.Count;

		if (count == 0)
			return;

		const float graphHeight = 256;
		if (ImGui.BeginChild("Graphs"))
		{
			ImDrawListPtr drawListPtr = ImGui.GetWindowDrawList();
			Vector2 mousePos = ImGui.GetMousePos();

			ImGuiExt.Title("Gems", Root.FontGoetheBold20);

			ImGui.Checkbox("Gems Collected", ref _showGemsCollected);
			ImGui.SameLine();
			ImGui.Checkbox("Gems Despawned", ref _showGemsDespawned);
			ImGui.SameLine();
			ImGui.Checkbox("Gems Eaten", ref _showGemsEaten);
			ImGui.SameLine();
			ImGui.Checkbox("Gems Total", ref _showGemsTotal);
			RenderGemsGraph(drawListPtr, mousePos);

			ImGui.SetCursorPosY(ImGui.GetCursorPosY() + graphHeight + 8);

			ImGuiExt.Title("Homing", Root.FontGoetheBold20);

			ImGui.Checkbox("Homing Stored", ref _showHomingStored);
			ImGui.SameLine();
			ImGui.Checkbox("Homing Eaten", ref _showHomingEaten);
			RenderHomingGraph(drawListPtr, mousePos);

			ImGui.EndChild();
		}

		void RenderGemsGraph(ImDrawListPtr drawListPtr, Vector2 mousePos)
		{
			RenderGraphBackground(drawListPtr, out Vector2 pos, out Vector2 size);

			ReadOnlySpan<int> maxValues = stackalloc int[] { _showGemsCollected ? _maxGemsCollected : 0, _showGemsDespawned ? _maxGemsDespawned : 0, _showGemsEaten ? _maxGemsEaten : 0, _showGemsTotal ? _maxGemsTotal : 0 };
			int max = MathUtils.Max(maxValues);
			RenderGraphScales(drawListPtr, pos, size, max);
			RenderGraphSplits(drawListPtr, pos, size);

			if (_showGemsCollected)
				RenderGraphLine(drawListPtr, _gemsCollected, max, _gemsCollectedPoints, 0xff0000ff, pos, size);

			if (_showGemsDespawned)
				RenderGraphLine(drawListPtr, _gemsDespawned, max, _gemsDespawnedPoints, 0xff888888, pos, size);

			if (_showGemsEaten)
				RenderGraphLine(drawListPtr, _gemsEaten, max, _gemsEatenPoints, 0xff00ff00, pos, size);

			if (_showGemsTotal)
				RenderGraphLine(drawListPtr, _gemsTotal, max, _gemsTotalPoints, 0xff000066, pos, size);

			if (mousePos.X >= pos.X && mousePos.X <= pos.X + size.X && mousePos.Y >= pos.Y && mousePos.Y <= pos.Y + size.Y)
			{
				int index = Math.Clamp((int)((mousePos.X - pos.X) / size.X * count), 0, count - 1);
				int gemsCollected = _gemsCollected[index];
				int gemsDespawned = _gemsDespawned[index];
				int gemsEaten = _gemsEaten[index];
				int gemsTotal = _gemsTotal[index];

				ImGui.BeginTooltip();

				AddTooltipText("Time", UnsafeSpan.Get(GetTimeFromIndex(index), StringFormats.TimeFormat), Color.White);
				AddTooltipText("Gems Collected", UnsafeSpan.Get(gemsCollected), Color.Red);
				AddTooltipText("Gems Despawned", UnsafeSpan.Get(gemsDespawned), Color.Gray(0.5f));
				AddTooltipText("Gems Eaten", UnsafeSpan.Get(gemsEaten), Color.Green);
				AddTooltipText("Gems Total", UnsafeSpan.Get(gemsTotal), new(127, 0, 0, 255));

				ImGui.EndTooltip();
			}
		}

		void RenderHomingGraph(ImDrawListPtr drawListPtr, Vector2 mousePos)
		{
			RenderGraphBackground(drawListPtr, out Vector2 pos, out Vector2 size);

			ReadOnlySpan<int> maxValues = stackalloc int[] { _showHomingStored ? _maxHomingStored : 0, _showHomingEaten ? _maxHomingEaten : 0 };
			int max = MathUtils.Max(maxValues);
			RenderGraphScales(drawListPtr, pos, size, max);
			RenderGraphSplits(drawListPtr, pos, size);

			if (_showHomingStored)
				RenderGraphLine(drawListPtr, _homingStored, max, _homingStoredPoints, ImGui.GetColorU32(UpgradeColors.Level4.ToEngineColor()), pos, size);

			if (_showHomingEaten)
				RenderGraphLine(drawListPtr, _homingEaten, max, _homingEatenPoints, 0xff0000ff, pos, size);

			if (mousePos.X >= pos.X && mousePos.X <= pos.X + size.X && mousePos.Y >= pos.Y && mousePos.Y <= pos.Y + size.Y)
			{
				int index = Math.Clamp((int)((mousePos.X - pos.X) / size.X * count), 0, count - 1);
				int homingStored = _homingStored[index];
				int homingEaten = _homingEaten[index];

				ImGui.BeginTooltip();

				AddTooltipText("Time", UnsafeSpan.Get(GetTimeFromIndex(index), StringFormats.TimeFormat), Color.White);
				AddTooltipText("Homing Stored", UnsafeSpan.Get(homingStored), UpgradeColors.Level4.ToEngineColor());
				AddTooltipText("Homing Eaten", UnsafeSpan.Get(homingEaten), Color.Red);

				ImGui.EndTooltip();
			}
		}

		void RenderGraphBackground(ImDrawListPtr drawListPtr, out Vector2 pos, out Vector2 size)
		{
			pos = ImGui.GetCursorScreenPos();
			size = new(ImGui.GetWindowWidth(), graphHeight);
			drawListPtr.AddRectFilled(pos, pos + size, 0xff080808);
		}

		void RenderGraphScales(ImDrawListPtr drawListPtr, Vector2 pos, Vector2 size, int maxY)
		{
			float timerStart = RunAnalysisWindow.StatsData.TimerStart;
			float timerEnd = RunAnalysisWindow.StatsData.TimerEnd;

			const int timerEndBufferSize = 16;
			Span<char> timerEndSpan = stackalloc char[timerEndBufferSize];
			timerEnd.TryFormat(timerEndSpan, out _, StringFormats.TimeFormat);
			timerEndSpan = timerEndSpan.SliceUntilNull(timerEndBufferSize);
			Vector2 timerEndTextSize = ImGui.CalcTextSize(timerEndSpan);

			drawListPtr.AddText(pos, 0xffffffff, UnsafeSpan.Get(maxY));
			drawListPtr.AddText(pos + new Vector2(0, size.Y - timerEndTextSize.Y), 0xffffffff, UnsafeSpan.Get(timerStart, StringFormats.TimeFormat));
			drawListPtr.AddText(pos + size - timerEndTextSize, 0xffffffff, timerEndSpan);
		}

		void RenderGraphLine(ImDrawListPtr drawListPtr, IReadOnlyList<int> data, int maxDataEntry, Vector2[] pointsArray, uint color, Vector2 cursorScreenPos, Vector2 graphSize)
		{
			pointsArray.AsSpan().Clear();
			for (int i = 0; i < data.Count; i++)
			{
				float normalizedX = i / (float)(data.Count - 1);
				float normalizedY = data[i] / (float)maxDataEntry;
				pointsArray[i] = new(cursorScreenPos.X + normalizedX * graphSize.X, cursorScreenPos.Y + graphHeight - normalizedY * graphSize.Y);
			}

			fixed (Vector2* p = pointsArray)
				drawListPtr.AddPolyline(ref p[0], data.Count, color, ImDrawFlags.None, 1);
		}

		void RenderGraphSplits(ImDrawListPtr drawListPtr, Vector2 pos, Vector2 size)
		{
			float timerStart = RunAnalysisWindow.StatsData.TimerStart;
			float timerEnd = RunAnalysisWindow.StatsData.TimerEnd;

			for (int i = 0; i < SplitsData.SplitData.Count; i++)
			{
				int time = SplitsData.SplitData[i].Seconds;
				if (time < timerStart || time > timerEnd)
					continue;

				float normalizedX = (time - timerStart) / (timerEnd - timerStart);
				float posX = pos.X + normalizedX * ImGui.GetWindowWidth();

				drawListPtr.AddLine(new(posX, pos.Y), new(posX, pos.Y + size.Y), 0xff404040);
			}
		}
	}

	private static float GetTimeFromIndex(int index)
	{
		return Math.Clamp((int)(index + RunAnalysisWindow.StatsData.TimerStart), RunAnalysisWindow.StatsData.TimerStart, RunAnalysisWindow.StatsData.TimerEnd);
	}

	private static void AddTooltipText(ReadOnlySpan<char> textLeft, ReadOnlySpan<char> textRight, Color textColor)
	{
		float posX = ImGui.GetCursorPosX();

		ImGui.TextColored(textColor, textLeft);
		ImGui.SameLine();

		float textWidth = ImGui.CalcTextSize(textRight).X;

		ImGui.SetCursorPosX(posX + 160 - textWidth);
		ImGui.Text(textRight);
		ImGui.SetCursorPosX(posX);
	}
}
