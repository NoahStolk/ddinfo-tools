using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

public class RaceDagger
{
	private const float _yOffset = 4;

	private static readonly Vector3 _hiddenPosition = new(-1000, -1000, -1000);

	private readonly Quaternion _meshRotationStart;

	public RaceDagger()
	{
		MeshPosition = default;
		MeshRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 0.5f);

		_meshRotationStart = MeshRotation;
	}

	public static uint Vao { get; private set; }

	public Vector3 MeshPosition { get; private set; }
	public Quaternion MeshRotation { get; private set; }

	public static void InitializeRendering(GL gl)
	{
		if (Vao != 0)
			throw new InvalidOperationException("Race dagger is already initialized.");

		Vao = MeshShaderUtils.CreateVao(gl, ContentManager.Content.DaggerMesh);
	}

	public void Update(SpawnsetBinary spawnset, int currentTick)
	{
		float currentTime = currentTick / 60f;
		float? raceDaggerHeight = spawnset.GetActualRaceDaggerHeight(currentTime);
		Vector3 basePosition = spawnset.GameMode != GameMode.Race || !raceDaggerHeight.HasValue ? _hiddenPosition : new Vector3
		{
			X = spawnset.RaceDaggerPosition.X,
			Y = raceDaggerHeight.Value + _yOffset,
			Z = spawnset.RaceDaggerPosition.Y,
		};
		MeshPosition = basePosition + new Vector3(0, 0.15f + MathF.Sin(currentTime) * 0.15f, 0);
		MeshRotation = _meshRotationStart * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, currentTime);
	}
}
