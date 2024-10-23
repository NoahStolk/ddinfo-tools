using DevilDaggersInfo.Tools.Engine.Content;
using DevilDaggersInfo.Tools.Engine.Loaders;
using DevilDaggersInfo.Tools.Utils;

namespace DevilDaggersInfo.Tools;

public record InternalResources(
	Shader MeshShader,
	TextureContent ApplicationIconTexture,
	Texture ArrowEndTexture,
	Texture ArrowLeftTexture,
	Texture ArrowRightTexture,
	Texture ArrowStartTexture,
	Texture BinTexture,
	Texture BucketTexture,
	Texture CloseTexture,
	Texture ConfigurationTexture,
	Texture DaggerTexture,
	Texture DownloadTexture,
	Texture DragIndicatorTexture,
	Texture EllipseTexture,
	Texture IconEggTexture,
	Texture IconEyeTexture,
	Texture IconHandTexture,
	Texture IconHomingMaskTexture,
	Texture IconSpiderTexture,
	Texture InfoTexture,
	Texture LineTexture,
	Texture PencilTexture,
	Texture RectangleTexture,
	Texture ReloadTexture,
	Texture TileHitboxTexture,
	ModelContent TileHitboxModel)
{
	public static InternalResources Create()
	{
#if DEBUG && WINDOWS
		const string? ddInfoToolsContentRootDirectory = @"..\..\..\Content";
#elif DEBUG && LINUX
		const string? ddInfoToolsContentRootDirectory = @"./src/DevilDaggersInfo.Tools/Content/";
#else
		const string? ddInfoToolsContentRootDirectory = null;
#endif
		DecompiledContentFile ddInfoToolsContent = DecompiledContentFile.Create(ddInfoToolsContentRootDirectory, Path.Combine(AssemblyUtils.InstallationDirectory, "ddinfo-assets"));

		return new InternalResources(
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

		Shader GetShader(DecompiledContentFile content, string name)
		{
			content.Shaders.TryGetValue(name, out ShaderContent? shaderContent);
			if (shaderContent == null)
				throw new InvalidOperationException($"Could not find shader '{name}'.");

			uint id = ShaderLoader.Load(shaderContent.VertexCode, shaderContent.FragmentCode);
			return new Shader(id);
		}

		static TextureContent GetTextureContent(DecompiledContentFile content, string name)
		{
			content.Textures.TryGetValue(name, out TextureContent? textureContent);
			if (textureContent == null)
				throw new InvalidOperationException($"Could not find texture '{name}'.");

			return textureContent;
		}

		Texture GetTexture(DecompiledContentFile content, string name)
		{
			TextureContent textureContent = GetTextureContent(content, name);
			uint id = TextureLoader.Load(textureContent);
			return new Texture(id);
		}

		static ModelContent GetModelContent(DecompiledContentFile content, string name)
		{
			content.Models.TryGetValue(name, out ModelContent? modelContent);
			if (modelContent == null)
				throw new InvalidOperationException($"Could not find model '{name}'.");

			return modelContent;
		}
	}
}
