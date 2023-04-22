namespace DevilDaggersInfo.App.Engine.Intersections;

public readonly record struct Ray(Vector3 Position, Vector3 Direction)
{
	public RayVsAabbIntersection? Intersects(Vector3 aabbMin, Vector3 aabbMax)
	{
		const float epsilon = 1e-6f;

		float? tMin = null, tMax = null;

		Axis axis = Axis.X;

		if (Math.Abs(Direction.X) < epsilon)
		{
			if (Position.X < aabbMin.X || Position.X > aabbMax.X)
				return null;
		}
		else
		{
			tMin = (aabbMin.X - Position.X) / Direction.X;
			tMax = (aabbMax.X - Position.X) / Direction.X;

			if (tMin > tMax)
				(tMin, tMax) = (tMax, tMin.Value);
		}

		if (Math.Abs(Direction.Y) < epsilon)
		{
			if (Position.Y < aabbMin.Y || Position.Y > aabbMax.Y)
				return null;
		}
		else
		{
			float tMinY = (aabbMin.Y - Position.Y) / Direction.Y;
			float tMaxY = (aabbMax.Y - Position.Y) / Direction.Y;

			if (tMinY > tMaxY)
				(tMaxY, tMinY) = (tMinY, tMaxY);

			if (tMin > tMaxY || tMinY > tMax)
				return null;

			if (!tMin.HasValue || tMinY > tMin)
			{
				tMin = tMinY;
				axis = Axis.Y;
			}

			if (!tMax.HasValue || tMaxY < tMax)
				tMax = tMaxY;
		}

		if (Math.Abs(Direction.Z) < epsilon)
		{
			if (Position.Z < aabbMin.Z || Position.Z > aabbMax.Z)
				return null;
		}
		else
		{
			float tMinZ = (aabbMin.Z - Position.Z) / Direction.Z;
			float tMaxZ = (aabbMax.Z - Position.Z) / Direction.Z;

			if (tMinZ > tMaxZ)
				(tMaxZ, tMinZ) = (tMinZ, tMaxZ);

			if (tMin > tMaxZ || tMinZ > tMax)
				return null;

			if (!tMin.HasValue || tMinZ > tMin)
			{
				tMin = tMinZ;
				axis = Axis.Z;
			}

			if (!tMax.HasValue || tMaxZ < tMax)
				tMax = tMaxZ;
		}

		return tMin switch
		{
			// Having a positive tMax and a negative tMin means the ray is inside the box.
			// We expect the intersection distance to be 0 in that case.
			< 0 when tMax > 0 => new(0, axis),

			// A negative tMin means that the intersection point is behind the ray's origin.
			// We discard these as not hitting the box.
			< 0 => null,
			_ => tMin.HasValue ? new(tMin.Value, axis) : null,
		};
	}
}
