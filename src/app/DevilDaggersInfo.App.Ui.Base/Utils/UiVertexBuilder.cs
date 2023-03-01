namespace DevilDaggersInfo.App.Ui.Base.Utils;

public static class UiVertexBuilder
{
	public static float[] RectangleTriangles()
	{
		return new[]
		{
			-0.5f, +0.5f, // top left
			+0.5f, +0.5f, // top right
			-0.5f, -0.5f, // bottom left

			+0.5f, +0.5f, // top right
			+0.5f, -0.5f, // bottom right
			-0.5f, -0.5f, // bottom left
		};
	}

	public static float[] CircleLines(int subdivisionCount)
	{
		float[] vertices = new float[(subdivisionCount + 2) * 2];
		for (uint i = 0; i <= subdivisionCount; i++)
		{
			float angle = i * (MathF.PI * 2) / subdivisionCount;
			vertices[i * 2] = MathF.Cos(angle);
			vertices[i * 2 + 1] = MathF.Sin(angle);
		}

		return vertices;
	}

	public static float[] Line()
	{
		return new[]
		{
			0f, 0f,
			1f, 0f,
		};
	}
}
