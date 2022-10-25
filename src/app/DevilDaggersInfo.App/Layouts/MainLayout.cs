using DevilDaggersInfo.App.Core.ApiClient.TaskHandlers;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion.Layouts;
using DevilDaggersInfo.App.Ui.Base.Enums;
using DevilDaggersInfo.App.Ui.Base.Rendering;
using DevilDaggersInfo.App.Ui.Base.States;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Core.Versioning;
using Silk.NET.OpenGL;
using Warp.Content;
using Warp.Debugging;
using Warp.InterpolationStates;
using Warp.Ui;
using Texture = Warp.Content.Texture;

namespace DevilDaggersInfo.App.Layouts;

public class MainLayout : Layout, IMainLayout
{
	private static readonly string _version = VersionUtils.EntryAssemblyVersion;

	private static readonly Vector3 _origin = new(0, 3, 0);
	private readonly Camera _camera = new();

	private MeshObject? _skull4;
	private readonly List<MeshObject> _tiles = new();

	public MainLayout()
		: base(Constants.Full)
	{
		Color ddse = Color.FromHsv(0, 1, 0.8f);
		Color ddae = Color.FromHsv(130, 1, 0.6f);
		Color ddre = Color.FromHsv(220, 1, 1);
		Color ddcl = Color.FromHsv(270, 1, 1);
		Color settings = Color.Gray(0.5f);
		Color exit = Color.Gray(0.3f);

		const int border = 10;
		NestingContext.Add(new Button(Rectangle.At(128, 192, 256, 96), LayoutManager.ToSurvivalEditorMainLayout, ddse.Intensify(64), ddse, ddse.Intensify(96), Color.White, "Survival Editor", TextAlign.Middle, border, FontSize.F12X12));
		NestingContext.Add(new Button(Rectangle.At(640, 192, 256, 96), LayoutManager.ToCustomLeaderboardsRecorderMainLayout, ddcl.Intensify(64), ddcl, ddcl.Intensify(96), Color.White, "Custom Leaderboards", TextAlign.Middle, border, FontSize.F12X12));
		NestingContext.Add(new Button(Rectangle.At(128, 384, 256, 96), () => { }, ddae.Intensify(64), ddae, ddae.Intensify(96), Color.White, "Asset Editor", TextAlign.Middle, border, FontSize.F12X12));
		NestingContext.Add(new Button(Rectangle.At(640, 384, 256, 96), () => { }, ddre.Intensify(64), ddre, ddre.Intensify(96), Color.White, "Replay Editor", TextAlign.Middle, border, FontSize.F12X12));
		NestingContext.Add(new Button(Rectangle.At(128, 576, 256, 96), LayoutManager.ToConfigLayout, settings.Intensify(64), settings, settings.Intensify(96), Color.White, "Configuration", TextAlign.Middle, border, FontSize.F12X12));
		NestingContext.Add(new Button(Rectangle.At(640, 576, 256, 96), () => Environment.Exit(0), exit.Intensify(64), exit, exit.Intensify(96), Color.White, "Exit", TextAlign.Middle, border, FontSize.F12X12));
	}

	public void InitializeScene()
	{
		CheckForUpdates();

		_skull4 = new(ContentManager.Content.Skull4Mesh, ContentManager.Content.Skull4Texture, Vector3.One, Quaternion.Identity, _origin);
		const int tileDimension = 3;
		const int start = -tileDimension / 2;
		const int end = tileDimension / 2;
		for (int i = start; i <= end; i++)
		{
			for (int j = start; j <= end; j++)
			{
				_tiles.Add(new(ContentManager.Content.TileMesh, ContentManager.Content.TileTexture, Vector3.One, Quaternion.Identity, new(i * 4, 0, j * 4)));
			}
		}
	}

	private void CheckForUpdates()
	{
		Root.Game.AsyncHandler.Run<FetchLatestDistribution, AppVersion>(ShowUpdateAvailable);

		void ShowUpdateAvailable(AppVersion? appVersion)
		{
			if (appVersion == null)
			{
				DebugStack.Add("Up to date", _version, 2);
				return;
			}

			Popup p = new(this, $"Version {appVersion} is available.");
			NestingContext.Add(p);
		}
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
		RenderBatchCollector.RenderMonoSpaceText(FontSize.F8X8, new(6), new(512, 64), 0, Color.Red, "DDINFO", TextAlign.Middle);
		RenderBatchCollector.RenderMonoSpaceText(FontSize.F8X8, new(4), new(512, 128), 0, new(255, 127, 0, 255), "TOOLS", TextAlign.Middle);
		RenderBatchCollector.RenderMonoSpaceText(FontSize.F8X8, new(2), new(512, 176), 0, new(255, 191, 0, 255), _version, TextAlign.Middle);
		RenderBatchCollector.RenderMonoSpaceText(FontSize.F8X8, new(1), new(512, 712), 0, Color.White, "Devil Daggers is created by Sorath", TextAlign.Middle);
		RenderBatchCollector.RenderMonoSpaceText(FontSize.F8X8, new(1), new(512, 728), 0, Color.White, "DevilDaggers.info is created by Noah Stolk", TextAlign.Middle);
		RenderBatchCollector.RenderMonoSpaceText(FontSize.F8X8, new(2), new(512, 752), 0, new(255, 0, 31, 255), "HTTPS://DEVILDAGGERS.INFO/", TextAlign.Middle);

		RenderBatchCollector.RenderMonoSpaceText(FontSize.F12X12, new(1), new(768, 448), 100, Color.Black, "(not implemented)", TextAlign.Middle);
		RenderBatchCollector.RenderMonoSpaceText(FontSize.F12X12, new(1), new(256, 448), 100, Color.Black, "(not implemented)", TextAlign.Middle);
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

			// TODO: We don't do anything with normals here.
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
		private readonly Vector3State _positionState = new(default);
		private readonly QuaternionState _rotationState = new(Quaternion.Identity);

		public Matrix4x4 Projection { get; private set; }
		public Matrix4x4 ViewMatrix { get; private set; }

		public void Update()
		{
			_positionState.PrepareUpdate();
			_rotationState.PrepareUpdate();

			_positionState.Physics = new(MathF.Sin(Root.Game.Tt) * 5, 4, MathF.Cos(Root.Game.Tt) * 5);
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
