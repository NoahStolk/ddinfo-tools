using DevilDaggersInfo.App.Engine.Content;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App;

public record InternalResources(
	BlobContent Value,
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
	Texture DragIndicatorTexture,
	Texture EllipseTexture,
	Texture GitHubTexture,
	Texture IconCalendarTexture,
	Texture IconCrosshairTexture,
	Texture IconDaggerTexture,
	Texture IconEggTexture,
	Texture IconEyeTexture,
	Texture IconGemTexture,
	Texture IconHandTexture,
	Texture IconHomingTexture,
	Texture IconHomingMaskTexture,
	Texture IconRankTexture,
	Texture IconSkullTexture,
	Texture IconSpiderTexture,
	Texture IconStopwatchTexture,
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
		DecompiledContentFile ddInfoToolsContent = DecompiledContentFile.Create(ddInfoToolsContentRootDirectory, "ddinfo-assets");

		return new(
#if SKIP_VALUE
			Value: new(Array.Empty<byte>()),
#else
			Value: GetBlobContent(ddInfoToolsContent, "Value"),
#endif
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
			DragIndicatorTexture: GetTexture(ddInfoToolsContent, "DragIndicator"),
			EllipseTexture: GetTexture(ddInfoToolsContent, "Ellipse"),
			GitHubTexture: GetTexture(ddInfoToolsContent, "GitHub"),
			IconCalendarTexture: GetTexture(ddInfoToolsContent, "IconCalendar"),
			IconCrosshairTexture: GetTexture(ddInfoToolsContent, "IconCrosshair"),
			IconDaggerTexture: GetTexture(ddInfoToolsContent, "IconDagger"),
			IconEggTexture: GetTexture(ddInfoToolsContent, "IconEgg"),
			IconEyeTexture: GetTexture(ddInfoToolsContent, "IconEye"),
			IconGemTexture: GetTexture(ddInfoToolsContent, "IconGem"),
			IconHandTexture: GetTexture(ddInfoToolsContent, "IconHand"),
			IconHomingTexture: GetTexture(ddInfoToolsContent, "IconHoming"),
			IconHomingMaskTexture: GetTexture(ddInfoToolsContent, "IconHomingMask"),
			IconRankTexture: GetTexture(ddInfoToolsContent, "IconRank"),
			IconSkullTexture: GetTexture(ddInfoToolsContent, "IconSkull"),
			IconSpiderTexture: GetTexture(ddInfoToolsContent, "IconSpider"),
			IconStopwatchTexture: GetTexture(ddInfoToolsContent, "IconStopwatch"),
			InfoTexture: GetTexture(ddInfoToolsContent, "Info"),
			LineTexture: GetTexture(ddInfoToolsContent, "Line"),
			PencilTexture: GetTexture(ddInfoToolsContent, "Pencil"),
			RectangleTexture: GetTexture(ddInfoToolsContent, "Rectangle"),
			ReloadTexture: GetTexture(ddInfoToolsContent, "Reload"),
			SettingsTexture: GetTexture(ddInfoToolsContent, "Settings"),
			TileHitboxTexture: GetTexture(ddInfoToolsContent, "TileHitbox"),
			TileHitboxModel: GetModelContent(ddInfoToolsContent, "TileHitbox"));

		static BlobContent GetBlobContent(DecompiledContentFile content, string name)
		{
			content.Blobs.TryGetValue(name, out BlobContent? blobContent);
			if (blobContent == null)
				throw new InvalidOperationException($"Could not find blob '{name}'.");

			return blobContent;
		}

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
