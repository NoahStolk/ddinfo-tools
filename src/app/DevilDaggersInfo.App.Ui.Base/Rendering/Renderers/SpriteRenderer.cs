using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.Base.Rendering.Scissors;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Ui.Base.Rendering.Renderers;

public class SpriteRenderer
{
	private readonly uint _vao;

	private readonly List<Sprite> _collection = new();

	public unsafe SpriteRenderer()
	{
		_vao = Gl.GenVertexArray();
		Gl.BindVertexArray(_vao);

		uint vbo = Gl.GenBuffer();
		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

		float[] vertices =
		{
			-0.5f, +0.5f, +0.0f, +1.0f, // top left
			+0.5f, +0.5f, +1.0f, +1.0f, // top right
			-0.5f, -0.5f, +0.0f, +0.0f, // bottom left

			+0.5f, +0.5f, +1.0f, +1.0f, // top right
			+0.5f, -0.5f, +1.0f, +0.0f, // bottom right
			-0.5f, -0.5f, +0.0f, +0.0f, // bottom left
		};

		fixed (float* v = &vertices[0])
			Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)vertices.Length * sizeof(float), v, BufferUsageARB.StaticDraw);

		Gl.EnableVertexAttribArray(0);
		Gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)0);

		Gl.EnableVertexAttribArray(1);
		Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)(2 * sizeof(float)));

		Gl.BindVertexArray(0);
		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		Gl.DeleteBuffer(vbo);
	}

	public void Schedule(Vector2 scale, Vector2 centerPosition, float depth, Texture texture, Color color)
	{
		_collection.Add(new(scale, centerPosition, depth, texture, color, ScissorScheduler.GetCalculatedScissor()));
	}

	public void Render()
	{
		if (_collection.Count == 0)
			return;

		Gl.BindVertexArray(_vao);

		foreach (Sprite sprite in _collection)
		{
			ScissorActivator.SetScissor(sprite.Scissor);

			sprite.Texture.Use();

			Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(sprite.Scale.X, sprite.Scale.Y, 1);
			Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(sprite.CenterPosition.X, sprite.CenterPosition.Y, sprite.Depth);
			SpriteShader.SetModel(scaleMatrix * translationMatrix);
			SpriteShader.SetSpriteColor(sprite.Color);
			Gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
		}

		Gl.BindVertexArray(0);

		_collection.Clear();
	}

	private readonly record struct Sprite(Vector2 Scale, Vector2 CenterPosition, float Depth, Texture Texture, Color Color, Scissor? Scissor);
}
