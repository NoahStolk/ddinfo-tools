using DevilDaggersInfo.App.Core.AssetInterop;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion.Layouts;
using DevilDaggersInfo.App.Ui.Base.Enums;
using DevilDaggersInfo.App.Ui.Base.Settings;
using DevilDaggersInfo.App.Ui.Base.States;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Enums;
using DevilDaggersInfo.Types.Core.Assets;
using Silk.NET.OpenGL;
using Warp.Content;
using Warp.InterpolationStates;
using Warp.Ui;
using Texture = Warp.Content.Texture;

namespace DevilDaggersInfo.App.Tools.Layouts;

public class MainLayout : Layout, IMainLayout
{
	private static readonly Vector3 _origin = new(0, 3, 0);
	private readonly Camera _camera = new(_origin);

	private MeshObject? _skull4;
	private readonly List<MeshObject> _tiles = new();

	public MainLayout()
		: base(Constants.Full)
	{
		Color ddse = Color.FromHsv(0, 1, 0.8f);
		Color ddae = Color.FromHsv(130, 1, 0.6f);
		Color ddre = Color.FromHsv(220, 1, 1);
		Color ddcl = Color.FromHsv(270, 1, 1);

		const int border = 10;
		NestingContext.Add(new Button(Rectangle.At(128, 128, 256, 128), LayoutManager.ToSurvivalEditorMainLayout, ddse.Intensify(64), ddse, ddse.Intensify(96), Color.White, "Survival Editor", TextAlign.Middle, border, FontSize.F12X12));
		NestingContext.Add(new Button(Rectangle.At(640, 128, 256, 128), LayoutManager.ToSurvivalEditorMainLayout, ddcl.Intensify(64), ddcl, ddcl.Intensify(96), Color.White, "Custom Leaderboards", TextAlign.Middle, border, FontSize.F12X12));
		NestingContext.Add(new Button(Rectangle.At(128, 512, 256, 128), LayoutManager.ToSurvivalEditorMainLayout, ddae.Intensify(64), ddae, ddae.Intensify(96), Color.White, "Asset Editor", TextAlign.Middle, border, FontSize.F12X12));
		NestingContext.Add(new Button(Rectangle.At(640, 512, 256, 128), LayoutManager.ToSurvivalEditorMainLayout, ddre.Intensify(64), ddre, ddre.Intensify(96), Color.White, "Replay Editor", TextAlign.Middle, border, FontSize.F12X12));
	}

	public void InitializeScene()
	{
		ModBinary modBinary = new(File.ReadAllBytes(Path.Combine(UserSettings.DevilDaggersInstallationDirectory, "res", "dd")), ModBinaryReadComprehensiveness.All);
		Mesh? skull4Mesh = GetMesh(modBinary, "boid4");
		Texture? skull4Texture = GetTexture(modBinary, "boid4");
		Mesh? tileMesh = GetMesh(modBinary, "tile");
		Texture? tileTexture = GetTexture(modBinary, "tile");

		if (skull4Mesh == null || skull4Texture == null || tileMesh == null || tileTexture == null)
			return;

		_skull4 = new(skull4Mesh, skull4Texture, Vector3.One, Quaternion.Identity, _origin);
		const int tileDimension = 3;
		const int start = -tileDimension / 2;
		const int end = tileDimension / 2;
		for (int i = start; i <= end; i++)
		{
			for (int j = start; j <= end; j++)
			{
				_tiles.Add(new(tileMesh, tileTexture, Vector3.One, Quaternion.Identity, new(i * 4, 0, j * 4)));
			}
		}
	}

	private static Mesh? GetMesh(ModBinary modBinary, string meshName)
	{
		if (!modBinary.AssetMap.TryGetValue(new(AssetType.Mesh, meshName), out AssetData? meshData))
			return null;

		Mesh mesh = MeshConverter.ToWarpMesh(meshData.Buffer);
		return mesh;
	}

	private static Texture? GetTexture(ModBinary modBinary, string textureName)
	{
		if (!modBinary.AssetMap.TryGetValue(new(AssetType.Texture, textureName), out AssetData? textureData))
			return null;

		Texture texture = TextureConverter.ToWarpTexture(textureData.Buffer);
		texture.Load();
		return texture;
	}

	public void Update()
	{
		_camera.Update();
	}

	public void Render3d()
	{
		_camera.PreRender();

		Shaders.Mesh.Use();
		Shaders.Mesh.SetMatrix4x4("view", _camera.ViewMatrix);
		Shaders.Mesh.SetMatrix4x4("projection", _camera.Projection);

		_skull4?.Render();
		foreach (MeshObject tile in _tiles)
			tile.Render();
	}

	public void Render()
	{
	}

	public void RenderText()
	{
	}

	private sealed class MeshObject
	{
		private readonly Mesh _mesh;
		private readonly Texture _texture;
		private readonly Matrix4x4 _modelMatrix;
		private readonly uint _vao;

		public unsafe MeshObject(Mesh mesh, Texture texture, Vector3 scale, Quaternion rotation, Vector3 position)
		{
			_mesh = mesh;
			_texture = texture;

			Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(scale);
			Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion(rotation);
			Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(position);
			_modelMatrix = scaleMatrix * rotationMatrix * translationMatrix;

			_vao = Gl.GenVertexArray();
			Gl.BindVertexArray(_vao);

			uint vbo = Gl.GenBuffer();
			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

			fixed (Vertex* v = &mesh.Vertices[0])
				Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(mesh.Vertices.Length * sizeof(Vertex)), v, BufferUsageARB.StaticDraw);

			Gl.EnableVertexAttribArray(0);
			Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)0);

			Gl.EnableVertexAttribArray(1);
			Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(3 * sizeof(float)));

			Gl.EnableVertexAttribArray(2);
			Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(5 * sizeof(float)));

			Gl.BindVertexArray(0);

			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		}

		public unsafe void Render()
		{
			Shaders.Mesh.SetMatrix4x4("model", _modelMatrix);
			Shaders.Mesh.SetInt("textureDiffuse", 0);
			_texture.Use();

			Gl.BindVertexArray(_vao);
			fixed (uint* i = &_mesh.Indices[0])
				Gl.DrawElements(PrimitiveType.Triangles, (uint)_mesh.Indices.Length, DrawElementsType.UnsignedInt, i);
			Gl.BindVertexArray(0);
		}
	}

	private sealed class Camera
	{
		private readonly Vector3 _origin;
		private readonly Vector3State _positionState = new(default);
		private readonly QuaternionState _rotationState = new(Quaternion.Identity);

		public Camera(Vector3 origin)
		{
			_origin = origin;
		}

		public Matrix4x4 Projection { get; private set; }
		public Matrix4x4 ViewMatrix { get; private set; }

		public void Update()
		{
			_positionState.PrepareUpdate();
			_rotationState.PrepareUpdate();

			_positionState.Physics = new(MathF.Sin(Base.Game.Tt) * 5, 4, MathF.Cos(Base.Game.Tt) * 5);
			_rotationState.Physics = Quaternion.CreateFromRotationMatrix(SetRotationFromDirectionalVector(_origin - _positionState.Physics));

			static Matrix4x4 SetRotationFromDirectionalVector(Vector3 direction)
			{
				Vector3 m3 = Vector3.Normalize(direction);
				Vector3 m1 = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, m3));
				Vector3 m2 = Vector3.Normalize(Vector3.Cross(m3, m1));

				Matrix4x4 matrix = Matrix4x4.Identity;

				matrix.M11 = m1.X;
				matrix.M12 = m1.Y;
				matrix.M13 = m1.Z;

				matrix.M21 = m2.X;
				matrix.M22 = m2.Y;
				matrix.M23 = m2.Z;

				matrix.M31 = m3.X;
				matrix.M32 = m3.Y;
				matrix.M33 = m3.Z;

				return matrix;
			}
		}

		public void PreRender()
		{
			_positionState.PrepareRender();
			_rotationState.PrepareRender();

			Vector3 upDirection = Vector3.Transform(Vector3.UnitY, _rotationState.Render);
			Vector3 lookDirection = Vector3.Transform(Vector3.UnitZ, _rotationState.Render);
			ViewMatrix = Matrix4x4.CreateLookAt(_positionState.Render, _positionState.Render + lookDirection, upDirection);

			float aspectRatio = WindowWidth / (float)WindowHeight;

			const int fieldOfView = 2;
			const float nearPlaneDistance = 0.05f;
			const float farPlaneDistance = 10000f;
			Projection = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4 * fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
		}
	}
}
