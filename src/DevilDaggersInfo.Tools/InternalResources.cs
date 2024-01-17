using DevilDaggersInfo.Tools.Engine.Content;
using DevilDaggersInfo.Tools.Utils;
using Silk.NET.OpenGL;

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
	Texture GitHubTexture,
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
	Texture SettingsTexture,
	Texture TileHitboxTexture,
	ModelContent TileHitboxModel)
{
	public static InternalResources Create(GL gl)
	{
#if DEBUG
		const string? ddInfoToolsContentRootDirectory = @"..\..\..\Content";
#else
		const string? ddInfoToolsContentRootDirectory = null;
#endif
		DecompiledContentFile ddInfoToolsContent = DecompiledContentFile.Create(ddInfoToolsContentRootDirectory, Path.Combine(AssemblyUtils.InstallationDirectory, "ddinfo-assets"));

		return new(
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
			GitHubTexture: GetTexture(ddInfoToolsContent, "GitHub"),
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
			SettingsTexture: GetTexture(ddInfoToolsContent, "Settings"),
			TileHitboxTexture: GetTexture(ddInfoToolsContent, "TileHitbox"),
			TileHitboxModel: GetModelContent(ddInfoToolsContent, "TileHitbox"));

		Shader GetShader(DecompiledContentFile content, string name)
		{
			content.Shaders.TryGetValue(name, out ShaderContent? shaderContent);
			if (shaderContent == null)
				throw new InvalidOperationException($"Could not find shader '{name}'.");

			return new(gl, shaderContent.VertexCode, shaderContent.FragmentCode);
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
			return new(gl, textureContent.Pixels, (uint)textureContent.Width, (uint)textureContent.Height);
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
