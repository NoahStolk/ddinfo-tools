using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace DevilDaggersInfo.App.ContentSourceGen.Generators.Shader;

[Generator]
public class ShaderGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<string?> assemblyName = context.CompilationProvider.Select(static (c, _) => c.AssemblyName);
		IncrementalValueProvider<ImmutableArray<AdditionalText>> additionalTexts = context.AdditionalTextsProvider.Where(at => IsShaderFile(Path.GetExtension(at.Path))).Collect();

		context.RegisterSourceOutput(
			assemblyName.Combine(additionalTexts),
			static (spc, source) => ShaderBuilder.Execute(spc, source));
	}

	private static bool IsShaderFile(string fileExtension)
	{
		return fileExtension is ".vert" or ".geom" or ".frag";
	}
}
