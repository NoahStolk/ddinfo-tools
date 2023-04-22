using DevilDaggersInfo.App.ContentSourceGen.Utils;

namespace DevilDaggersInfo.App.ContentSourceGen.Generators.Shader;

public record ShaderFile
{
	public ShaderFile(string filePath, string? sourceText)
	{
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
		string localName = SourceBuilderUtils.ToEscapedLocal(fileNameWithoutExtension);
		ShaderClassName = $"{fileNameWithoutExtension}Shader";

		if (sourceText != null)
		{
			// ! LINQ query filters out null values.
			List<GlslUtils.ShaderUniform> parsedShaderUniforms = sourceText.Split('\n').Select(GlslUtils.GetFromGlslLine).Where(su => su is not null).ToList()!;
			Uniforms = parsedShaderUniforms.ConvertAll(su => new ShaderUniform(su.Type, su.Name));
		}

		ShaderInitializer = $"{ShaderClassName}.Initialize(content.TryGetValue(\"{fileNameWithoutExtension}\", out {Constants.RootNamespace}.Content.Shader? {localName}) ? {localName}.Id : throw new InvalidOperationException(\"Content does not exist or has not been initialized.\"));";
	}

	public string ShaderClassName { get; }

	public List<ShaderUniform>? Uniforms { get; } = new();

	public string ShaderInitializer { get; }
}
