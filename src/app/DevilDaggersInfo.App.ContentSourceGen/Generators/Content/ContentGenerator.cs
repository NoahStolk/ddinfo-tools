using DevilDaggersInfo.App.ContentSourceGen.Utils;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace DevilDaggersInfo.App.ContentSourceGen.Generators.Content;

[Generator]
public class ContentGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterPostInitializationOutput(ctx => ctx.AddSource("IContentContainer.g.cs", SourceBuilderUtils.Build($$"""
			using System.Collections.Generic;

			namespace {{Constants.RootNamespace}};

			public interface IContentContainer<TContent>
				where TContent : class
			{
				static abstract void Initialize(IReadOnlyDictionary<string, TContent> content);
			}
			""")));

		IncrementalValueProvider<string?> assemblyName = context.CompilationProvider.Select(static (c, _) => c.AssemblyName);
		IncrementalValueProvider<ImmutableArray<AdditionalText>> additionalTexts = context.AdditionalTextsProvider.Where(at => IsNonShaderContentFile(Path.GetExtension(at.Path))).Collect();

		context.RegisterSourceOutput(
			assemblyName.Combine(additionalTexts),
			static (spc, source) => ContentBuilder.Execute(spc, source));
	}

	private static bool IsNonShaderContentFile(string fileExtension)
	{
		return fileExtension is ".bin" or ".txt" or ".map" or ".obj" or ".wav" or ".tga";
	}
}
