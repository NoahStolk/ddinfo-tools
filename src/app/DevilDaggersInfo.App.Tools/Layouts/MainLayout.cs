using DevilDaggersInfo.App.Core.AssetInterop;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Enums;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Enums;
using DevilDaggersInfo.Types.Core.Assets;
using Silk.NET.OpenGL;
using Warp.Content;
using Warp.Debugging;
using Warp.InterpolationStates;
using Warp.Ui;
using Texture = Warp.Content.Texture;

namespace DevilDaggersInfo.App.Tools.Layouts;

public class MainLayout : Layout, IExtendedLayout
{
	private readonly Camera _camera = new();

	private Rendering? _rendering;

	public MainLayout()
		: base(new(0, 0, 1920, 1080))
	{
		Color ddse = Color.FromHsv(0, 1, 0.8f);
		Color ddae = Color.FromHsv(130, 1, 0.6f);
		Color ddre = Color.FromHsv(220, 1, 1);
		Color ddcl = Color.FromHsv(270, 1, 1);

		const int border = 10;
		NestingContext.Add(new Button(Rectangle.At(0256, 256, 320, 128), () => Root.Game.ActiveLayout = Root.Game.SurvivalEditorMainLayout, ddse.Intensify(64), ddse, ddse.Intensify(96), Color.White, "Survival Editor", TextAlign.Middle, border, false));
		NestingContext.Add(new Button(Rectangle.At(1280, 256, 320, 128), () => Root.Game.ActiveLayout = Root.Game.SurvivalEditorMainLayout, ddcl.Intensify(64), ddcl, ddcl.Intensify(96), Color.White, "Custom Leaderboards", TextAlign.Middle, border, false));
		NestingContext.Add(new Button(Rectangle.At(0256, 704, 320, 128), () => Root.Game.ActiveLayout = Root.Game.SurvivalEditorMainLayout, ddae.Intensify(64), ddae, ddae.Intensify(96), Color.White, "Asset Editor", TextAlign.Middle, border, false));
		NestingContext.Add(new Button(Rectangle.At(1280, 704, 320, 128), () => Root.Game.ActiveLayout = Root.Game.SurvivalEditorMainLayout, ddre.Intensify(64), ddre, ddre.Intensify(96), Color.White, "Replay Editor", TextAlign.Middle, border, false));

		NestingContext.Add(new Button(Rectangle.At(0512, 480, 320, 128), ExtractAsset, ddse.Intensify(64), ddse, ddse.Intensify(96), Color.White, "ENABLE", TextAlign.Middle, border, false));
	}

	private void ExtractAsset()
	{
		const string path = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\res\dd";
		ModBinary mb = new(File.ReadAllBytes(path), ModBinaryReadComprehensiveness.All);
		if (!mb.AssetMap.TryGetValue(new(AssetType.Mesh, "boid4"), out AssetData? tileMeshData) || !mb.AssetMap.TryGetValue(new(AssetType.Texture, "tile"), out AssetData? tileTextureData))
			return; // Assets not found in DD res.

		Mesh mesh = MeshConverter.ToWarpMesh(tileMeshData.Buffer);
		Texture texture = TextureConverter.ToWarpTexture(tileTextureData.Buffer);
		SetMesh(mesh, texture);
	}

	private unsafe void SetMesh(Mesh mesh, Texture texture)
	{
		uint vao = Gl.GenVertexArray();
		_rendering = new(vao, mesh, texture);

		Gl.BindVertexArray(_rendering.Vao);

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

	public void Update()
	{
		_camera.Update();
	}

	public unsafe void Render3d()
	{
		if (_rendering == null)
			return;

		_camera.PreRender();

		Shaders.Mesh.Use();
		Shaders.Mesh.SetMatrix4x4("view", _camera.ViewMatrix);
		Shaders.Mesh.SetMatrix4x4("projection", _camera.Projection);

		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(Vector3.One);
		Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion(Quaternion.Identity);
		Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(default);

		Shaders.Mesh.SetMatrix4x4("model", scaleMatrix * rotationMatrix * translationMatrix);

		Shaders.Mesh.SetInt("textureDiffuse", 0);
		_rendering.Texture.Use();

		DebugStack.Add("rendering", _rendering.Mesh.Indices.Length, true);
		Gl.BindVertexArray(_rendering.Vao);
		fixed (uint* i = &_rendering.Mesh.Indices[0])
			Gl.DrawElements(PrimitiveType.Triangles, (uint)_rendering.Mesh.Indices.Length, DrawElementsType.UnsignedInt, i);
		Gl.BindVertexArray(0);
	}

	public void Render()
	{
	}

	public void RenderText()
	{
	}

	private sealed class Rendering
	{
		public Rendering(uint vao, Mesh mesh, Texture texture)
		{
			Vao = vao;
			Mesh = mesh;
			Texture = texture;
		}

		public uint Vao { get; }
		public Mesh Mesh { get; }
		public Texture Texture { get; }
	}

	private sealed class Camera
	{
		private readonly Vector3State _positionState = new(new(0, 0, -6));
		private readonly QuaternionState _rotationState = new(Quaternion.Identity);

		public Matrix4x4 Projection { get; private set; }
		public Matrix4x4 ViewMatrix { get; private set; }

		public void Update()
		{
			_positionState.PrepareUpdate();
			_rotationState.PrepareUpdate();

			// _positionState.Physics = new(MathF.Sin(Base.Game.Tt) * 10, 1, MathF.Cos(Base.Game.Tt) * 10);
			// _rotationState.Physics = Quaternion.CreateFromRotationMatrix(Matrix4x4.CreateLookAt(_positionState.Physics, default, Vector3.UnitY));

			DebugStack.Add("cam pos", _positionState.Physics.ToString(), Base.Game.Dt);
			DebugStack.Add("cam rot", _rotationState.Physics.ToString(), Base.Game.Dt);
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
