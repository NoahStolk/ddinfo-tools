namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Utils;

public static class ArenaEditingUtils
{
	public static Vector2i<int> Snap(Vector2i<int> vector, int snap)
	{
		return new(vector.X / snap * snap, vector.Y / snap * snap);
	}

	public static Vector2i<int> Snap(Vector2i<int> vector, Vector2i<int> snap)
	{
		return new(vector.X / snap.X * snap.X, vector.Y / snap.Y * snap.Y);
	}

	public static Vector2 Snap(Vector2 vector, float snap)
	{
		return new(MathF.Floor(vector.X / snap) * snap, MathF.Floor(vector.Y / snap) * snap);
	}

	public static Vector2 Snap(Vector2 vector, Vector2 snap)
	{
		return new(MathF.Floor(vector.X / snap.X) * snap.X, MathF.Floor(vector.Y / snap.Y) * snap.Y);
	}

	public static Rectangle GetRectangle(Vector2i<int> start, Vector2i<int> end)
	{
		bool startXMin = start.X < end.X;
		int minX, maxX;
		if (startXMin)
		{
			minX = start.X;
			maxX = end.X;
		}
		else
		{
			minX = end.X;
			maxX = start.X;
		}

		bool startYMin = start.Y < end.Y;
		int minY, maxY;
		if (startYMin)
		{
			minY = start.Y;
			maxY = end.Y;
		}
		else
		{
			minY = end.Y;
			maxY = start.Y;
		}

		return new(minX, minY, maxX - minX, maxY - minY);
	}

	private static bool PointIsBehindPlane(Vector2 linePoint, Vector2 lineNormal, Vector2 point)
	{
		return Vector2.Dot(lineNormal, Vector2.Normalize(point - linePoint)) < 0;
	}

	public readonly record struct Rectangle(int X, int Y, int Width, int Height)
	{
		public int X1 { get; } = X;

		public int Y1 { get; } = Y;

		public int X2 { get; } = X + Width;

		public int Y2 { get; } = Y + Height;
	}

	public readonly record struct LineSegment
	{
		public LineSegment(Vector2 start, Vector2 end)
		{
			Start = start;
			End = end;

			float deltaX = End.X - Start.X;
			float deltaY = End.Y - Start.Y;
			Normal = Vector2.Normalize(new(-deltaY, deltaX));
		}

		public Vector2 Start { get; }
		public Vector2 End { get; }
		public Vector2 Normal { get; }
	}

	public readonly record struct Stadium
	{
		public Stadium(Vector2 start, Vector2 end, float radius)
		{
			Start = start;
			End = end;
			Radius = radius;

			float deltaX = end.X - start.X;
			float deltaY = end.Y - start.Y;
			Direction = new(deltaX, deltaY);
			Normal = Vector2.Normalize(new(-deltaY, deltaX));
			Edge1Point = start + Normal * radius;
			Edge2Point = start - Normal * radius;
		}

		public Vector2 Start { get; }
		public Vector2 End { get; }
		public float Radius { get; }
		public Vector2 Direction { get; }
		public Vector2 Normal { get; }
		public Vector2 Edge1Point { get; }
		public Vector2 Edge2Point { get; }
	}

	public readonly record struct Square(Vector2 Min, Vector2 Max)
	{
		public Vector2 TopLeft => Min;
		public Vector2 TopRight => new(Max.X, Min.Y);
		public Vector2 BottomLeft => new(Min.X, Max.Y);
		public Vector2 BottomRight => Max;

		public static Square FromCenter(Vector2 center, float size)
		{
			Vector2 halfSize = new(size / 2f);
			return new(center - halfSize, center + halfSize);
		}

		public bool IntersectsLineSegment(LineSegment lineSegment)
		{
			// Calculate booleans for all 4 points; return true immediately when any of the booleans do not hold the same value.
			bool tlBehind = PointIsBehindPlane(lineSegment.Start, lineSegment.Normal, TopLeft);
			if (PointIsBehindPlane(lineSegment.Start, lineSegment.Normal, TopRight) != tlBehind)
				return true;

			if (PointIsBehindPlane(lineSegment.Start, lineSegment.Normal, BottomLeft) != tlBehind)
				return true;

			return PointIsBehindPlane(lineSegment.Start, lineSegment.Normal, BottomRight) != tlBehind;
		}

		public bool Contains(Vector2 point)
		{
			return point.X >= Min.X && point.X <= Max.X && point.Y >= Min.Y && point.Y <= Max.Y;
		}

		public bool IntersectsStadium(Stadium stadium)
		{
			if (Contains(stadium.Start) || Contains(stadium.End))
				return true;

			float radiusSquared = stadium.Radius * stadium.Radius;

			// Check if any of the 4 corners of the square are inside the stadium start or end.
			if (Vector2.DistanceSquared(TopLeft, stadium.Start) < radiusSquared || Vector2.DistanceSquared(TopLeft, stadium.End) < radiusSquared ||
			    Vector2.DistanceSquared(TopRight, stadium.Start) < radiusSquared || Vector2.DistanceSquared(TopRight, stadium.End) < radiusSquared ||
			    Vector2.DistanceSquared(BottomLeft, stadium.Start) < radiusSquared || Vector2.DistanceSquared(BottomLeft, stadium.End) < radiusSquared ||
			    Vector2.DistanceSquared(BottomRight, stadium.Start) < radiusSquared || Vector2.DistanceSquared(BottomRight, stadium.End) < radiusSquared)
			{
				return true;
			}

			// Check if any of the 4 corners of the square are outside the stadium start or end.
			if (PointIsBehindPlane(stadium.Start, stadium.Direction, TopLeft) == PointIsBehindPlane(stadium.End, stadium.Direction, TopLeft))
				return false;
			if (PointIsBehindPlane(stadium.Start, stadium.Direction, TopRight) == PointIsBehindPlane(stadium.End, stadium.Direction, TopRight))
				return false;
			if (PointIsBehindPlane(stadium.Start, stadium.Direction, BottomLeft) == PointIsBehindPlane(stadium.End, stadium.Direction, BottomLeft))
				return false;
			if (PointIsBehindPlane(stadium.Start, stadium.Direction, BottomRight) == PointIsBehindPlane(stadium.End, stadium.Direction, BottomRight))
				return false;

			// Check if any of the 4 corners of the square are between the stadium edge planes.
			bool tlBehind1 = PointIsBehindPlane(stadium.Edge1Point, stadium.Normal, TopLeft);
			bool tlBehind2 = PointIsBehindPlane(stadium.Edge2Point, stadium.Normal, TopLeft);
			if (tlBehind1 ^ tlBehind2)
				return true;

			bool trBehind1 = PointIsBehindPlane(stadium.Edge1Point, stadium.Normal, TopRight);
			bool trBehind2 = PointIsBehindPlane(stadium.Edge2Point, stadium.Normal, TopRight);
			if (trBehind1 ^ trBehind2)
				return true;

			bool blBehind1 = PointIsBehindPlane(stadium.Edge1Point, stadium.Normal, BottomLeft);
			bool blBehind2 = PointIsBehindPlane(stadium.Edge2Point, stadium.Normal, BottomLeft);
			if (blBehind1 ^ blBehind2)
				return true;

			bool brBehind1 = PointIsBehindPlane(stadium.Edge1Point, stadium.Normal, BottomRight);
			bool brBehind2 = PointIsBehindPlane(stadium.Edge2Point, stadium.Normal, BottomRight);
			if (brBehind1 ^ brBehind2)
				return true;

			// Check if one of the 4 corners is in front of both edges, and if one of the 4 corners is behind both edges. If so, the stadium crosses the square.
			bool oneBehind = tlBehind1 || trBehind1 || blBehind1 || brBehind1; // We only need to check one edge, since we already know that the other edge holds the same value (XOR).
			bool oneInFront = !tlBehind1 || !trBehind1 || !blBehind1 || !brBehind1;
			return oneBehind && oneInFront;
		}
	}

	public readonly record struct AlignedEllipse(Vector2 Center, Vector2 Radius)
	{
		public bool Contains(Vector2 point)
		{
			float deltaX = point.X - Center.X;
			float deltaY = point.Y - Center.Y;
			return deltaX * deltaX / (Radius.X * Radius.X) + deltaY * deltaY / (Radius.Y * Radius.Y) <= 1f;
		}

		public bool Contains(Square square)
		{
			return Contains(square.TopLeft) || Contains(square.TopRight) || Contains(square.BottomLeft) || Contains(square.BottomRight);
		}

		public bool Intersects(Square square)
		{
			bool tlContained = Contains(square.TopLeft);
			if (Contains(square.TopRight) != tlContained)
				return true;

			if (Contains(square.BottomLeft) != tlContained)
				return true;

			return Contains(square.BottomRight) != tlContained;
		}
	}
}
