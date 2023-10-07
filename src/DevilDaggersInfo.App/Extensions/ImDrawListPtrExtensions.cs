using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Extensions;

public static class ImDrawListPtrExtensions
{
	public static void AddEllipse(this ImDrawListPtr drawList, Vector2 center, Vector2 radius, uint col, int numSegments, float thickness)
	{
		if (numSegments <= 2)
			return;

		for (int i = 0; i < numSegments; i++)
		{
			float angle = MathF.PI * 2.0f * i / numSegments;
			drawList.PathLineTo(center + new Vector2(MathF.Cos(angle) * radius.X, MathF.Sin(angle) * radius.Y));
		}

		drawList.PathStroke(col, ImDrawFlags.Closed, thickness);
	}
}
