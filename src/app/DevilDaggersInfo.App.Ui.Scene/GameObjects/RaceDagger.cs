using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.OpenGL;
using Warp.NET.Content;
using Warp.NET.InterpolationStates;

namespace DevilDaggersInfo.App.Ui.Scene.GameObjects;

public class RaceDagger
{
	private const float _yOffset = 4;

	private static uint _vao;

	private readonly Vector3State _meshPosition;
	private readonly QuaternionState _meshRotation;

	private readonly SpawnsetBinary _spawnsetBinary;

	private int _arenaDimension;
	private ImmutableArena _arenaTiles;
	private Vector2 _raceDaggerPosition;
	private int _arenaCoordX;
	private int _arenaCoordZ;

	public RaceDagger(SpawnsetBinary spawnsetBinary)
	{
		_spawnsetBinary = spawnsetBinary;

		_arenaDimension = _spawnsetBinary.ArenaDimension;
		_arenaTiles = _spawnsetBinary.ArenaTiles;
		_raceDaggerPosition = _spawnsetBinary.RaceDaggerPosition;
		(_arenaCoordX, _arenaCoordZ) = GetPosition();

		_meshPosition = new(new(_spawnsetBinary.RaceDaggerPosition.X, 0, _spawnsetBinary.RaceDaggerPosition.Y));
		_meshRotation = new(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 0.5f));
	}

	private (int ArenaCoordX, int ArenaCoordZ) GetPosition()
	{
		(int x, _, int z) = SpawnsetBinary.GetRaceDaggerTilePosition(_arenaDimension, _arenaTiles, _raceDaggerPosition);
		return (x, z);
	}

	public static unsafe void Initialize()
	{
		_vao = CreateVao(ContentManager.Content.DaggerMesh);

		static uint CreateVao(Mesh mesh)
		{
			uint vao = Gl.GenVertexArray();
			Gl.BindVertexArray(vao);

			uint vbo = Gl.GenBuffer();
			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

			fixed (Vertex* v = &mesh.Vertices[0])
				Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(mesh.Vertices.Length * sizeof(Vertex)), v, BufferUsageARB.StaticDraw);

			Gl.EnableVertexAttribArray(0);
			Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)0);

			Gl.EnableVertexAttribArray(1);
			Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(3 * sizeof(float)));

			// TODO: We don't do anything with normals here.
			Gl.EnableVertexAttribArray(2);
			Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(5 * sizeof(float)));

			Gl.BindVertexArray(0);

			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

			return vao;
		}
	}

	/// <summary>
	/// Updates the position based on the tiles edited in the 3D editor. TODO: Also implement being able to update the race dagger position in the 3D editor.
	/// </summary>
	public void UpdatePosition(int arenaDimension, ImmutableArena arenaTiles, Vector2 raceDaggerPosition)
	{
		_arenaDimension = arenaDimension;
		_arenaTiles = arenaTiles;
		_raceDaggerPosition = raceDaggerPosition;
		(_arenaCoordX, _arenaCoordZ) = GetPosition();
	}

	public void Update(int currentTick)
	{
		_meshPosition.PrepareUpdate();
		_meshRotation.PrepareUpdate();

		float currentTime = currentTick / 60f;
		Vector3 basePosition = _meshPosition.Physics with
		{
			Y = SpawnsetBinary.GetActualTileHeight(_arenaDimension, _arenaTiles, _spawnsetBinary.ShrinkStart, _spawnsetBinary.ShrinkEnd, _spawnsetBinary.ShrinkRate, _arenaCoordX, _arenaCoordZ, currentTime) + _yOffset,
		};
		_meshPosition.Physics = basePosition + new Vector3(0, 0.15f + MathF.Sin(currentTime) * 0.15f, 0);
		_meshRotation.Physics = _meshRotation.Start * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, currentTime);
	}

	public unsafe void Render()
	{
		_meshPosition.PrepareRender();
		_meshRotation.PrepareRender();

		ContentManager.Content.DaggerSilverTexture.Use();

		Matrix4x4 model = Matrix4x4.CreateScale(8) * Matrix4x4.CreateFromQuaternion(_meshRotation.Render) * Matrix4x4.CreateTranslation(_meshPosition.Render);
		Shader.SetMatrix4x4(MeshUniforms.Model, model);

		Gl.BindVertexArray(_vao);
		fixed (uint* i = &ContentManager.Content.DaggerMesh.Indices[0])
			Gl.DrawElements(PrimitiveType.Triangles, (uint)ContentManager.Content.DaggerMesh.Indices.Length, DrawElementsType.UnsignedInt, i);
		Gl.BindVertexArray(0);
	}
}
