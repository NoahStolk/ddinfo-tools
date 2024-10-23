using DevilDaggersInfo.Tools.Engine.Content;
using DevilDaggersInfo.Tools.Engine.Loaders;
using DevilDaggersInfo.Tools.Utils;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools;

public sealed class ResourceManager
{
	private readonly GL _gl;
	private readonly ShaderLoader _shaderLoader;
	private readonly TextureLoader _textureLoader;

	public ResourceManager(GL gl, ShaderLoader shaderLoader, TextureLoader textureLoader)
	{
		_gl = gl;
		_shaderLoader = shaderLoader;
		_textureLoader = textureLoader;

#if DEBUG && WINDOWS
		const string? ddInfoToolsContentRootDirectory = @"..\..\..\Content";
#elif DEBUG && LINUX
		const string? ddInfoToolsContentRootDirectory = @"./src/DevilDaggersInfo.Tools/Content/";
#else
		const string? ddInfoToolsContentRootDirectory = null;
#endif
		DecompiledContentFile ddInfoToolsContent = DecompiledContentFile.Create(ddInfoToolsContentRootDirectory, Path.Combine(AssemblyUtils.InstallationDirectory, "ddinfo-assets"));

		InternalResources = new InternalResources(
			MeshShader: GetShader(ddInfoToolsContent, "Mesh"),
			ApplicationIconTexture: GetTextureContent(ddInfoToolsContent, "ApplicationIcon"),
			ArrowEndTexture: GetTexture(ddInfoToolsContent, "ArrowEnd"),
			ArrowLeftTexture: GetTexture(ddInfoToolsContent, "ArrowLeft"),
			ArrowRightTexture: GetTexture(ddInfoToolsContent, "ArrowRight"),
			ArrowStartTexture: GetTexture(ddInfoToolsContent, "ArrowStart"),
			BinTexture: GetTexture(ddInfoToolsContent, "Bin"),
			BucketTexture: GetTexture(ddInfoToolsContent, "Bucket"),
			CloseTexture: GetTexture(ddInfoToolsContent, "Close"),
			ConfigurationTexture: GetTexture(ddInfoToolsContent, "Configuration"),
			DaggerTexture: GetTexture(ddInfoToolsContent, "Dagger"),
			DownloadTexture: GetTexture(ddInfoToolsContent, "Download"),
			DragIndicatorTexture: GetTexture(ddInfoToolsContent, "DragIndicator"),
			EllipseTexture: GetTexture(ddInfoToolsContent, "Ellipse"),
			IconEggTexture: GetTexture(ddInfoToolsContent, "IconEgg"),
			IconEyeTexture: GetTexture(ddInfoToolsContent, "IconEye"),
			IconHandTexture: GetTexture(ddInfoToolsContent, "IconHand"),
			IconHomingMaskTexture: GetTexture(ddInfoToolsContent, "IconHomingMask"),
			IconSpiderTexture: GetTexture(ddInfoToolsContent, "IconSpider"),
			InfoTexture: GetTexture(ddInfoToolsContent, "Info"),
			LineTexture: GetTexture(ddInfoToolsContent, "Line"),
			PencilTexture: GetTexture(ddInfoToolsContent, "Pencil"),
			RectangleTexture: GetTexture(ddInfoToolsContent, "Rectangle"),
			ReloadTexture: GetTexture(ddInfoToolsContent, "Reload"),
			TileHitboxTexture: GetTexture(ddInfoToolsContent, "TileHitbox"),
			TileHitboxModel: GetModelContent(ddInfoToolsContent, "TileHitbox"));
	}

	// TODO: Rewrite to dictionary.

	/// <summary>
	/// Holds the internal resources, such as shaders and icons.
	/// </summary>
	public InternalResources InternalResources { get; }

	// TODO:
	// 1. Rewrite to dictionary.
	// 2. Make non-nullable.
	// 3. When uninitialized, return some default resource.

	/// <summary>
	/// Holds the game resources, such as the tile texture and dagger mesh.
	/// </summary>
	public GameResources? GameResources { get; private set; }

	public void LoadGameResources()
	{
		GameResources = new GameResources(
			IconMaskCrosshairTexture: new Texture(_gl, _textureLoader.Load(ContentManager.Content.IconMaskCrosshairTexture)),
			IconMaskDaggerTexture: new Texture(_gl, _textureLoader.Load(ContentManager.Content.IconMaskDaggerTexture)),
			IconMaskGemTexture: new Texture(_gl, _textureLoader.Load(ContentManager.Content.IconMaskGemTexture)),
			IconMaskHomingTexture: new Texture(_gl, _textureLoader.Load(ContentManager.Content.IconMaskHomingTexture)),
			IconMaskSkullTexture: new Texture(_gl, _textureLoader.Load(ContentManager.Content.IconMaskSkullTexture)),
			IconMaskStopwatchTexture: new Texture(_gl, _textureLoader.Load(ContentManager.Content.IconMaskStopwatchTexture)),
			DaggerSilverTexture: new Texture(_gl, _textureLoader.Load(ContentManager.Content.DaggerSilverTexture)),
			Skull4Texture: new Texture(_gl, _textureLoader.Load(ContentManager.Content.Skull4Texture)),
			Skull4JawTexture: new Texture(_gl, _textureLoader.Load(ContentManager.Content.Skull4JawTexture)),
			TileTexture: new Texture(_gl, _textureLoader.Load(ContentManager.Content.TileTexture)),
			PillarTexture: new Texture(_gl, _textureLoader.Load(ContentManager.Content.PillarTexture)),
			PostLut: new Texture(_gl, _textureLoader.Load(ContentManager.Content.PostLut)),
			Hand4Texture: new Texture(_gl, _textureLoader.Load(ContentManager.Content.Hand4Texture)));
	}

	private Shader GetShader(DecompiledContentFile content, string name)
	{
		content.Shaders.TryGetValue(name, out ShaderContent? shaderContent);
		if (shaderContent == null)
			throw new InvalidOperationException($"Could not find shader '{name}'.");

		uint id = _shaderLoader.Load(shaderContent.VertexCode, shaderContent.FragmentCode);
		return new Shader(_gl, id);
	}

	private static TextureContent GetTextureContent(DecompiledContentFile content, string name)
	{
		content.Textures.TryGetValue(name, out TextureContent? textureContent);
		if (textureContent == null)
			throw new InvalidOperationException($"Could not find texture '{name}'.");

		return textureContent;
	}

	private Texture GetTexture(DecompiledContentFile content, string name)
	{
		TextureContent textureContent = GetTextureContent(content, name);
		uint id = _textureLoader.Load(textureContent);
		return new Texture(_gl, id);
	}

	private static ModelContent GetModelContent(DecompiledContentFile content, string name)
	{
		content.Models.TryGetValue(name, out ModelContent? modelContent);
		if (modelContent == null)
			throw new InvalidOperationException($"Could not find model '{name}'.");

		return modelContent;
	}
}
