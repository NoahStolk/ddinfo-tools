using DevilDaggersInfo.App.ContentSourceGen.Extensions;
using DevilDaggersInfo.App.ContentSourceGen.Generators.Shader;
using DevilDaggersInfo.App.ContentSourceGen.Utils;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace DevilDaggersInfo.App.ContentSourceGen.Generators.Content;

/// <summary>
/// Generates classes that contain all content files for their respective content type. A class for shader content is not generated, this is done by <see cref="ShaderBuilder"/>.
/// </summary>
public static class ContentBuilder
{
	private const string _namespace = $"%{nameof(_namespace)}%";
	private const string _className = $"%{nameof(_className)}%";
	private const string _contentFieldInitializers = $"%{nameof(_contentFieldInitializers)}%";
	private const string _contentFields = $"%{nameof(_contentFields)}%";
	private const string _contentProperties = $"%{nameof(_contentProperties)}%";
	private const string _contentType = $"%{nameof(_contentType)}%";

	// TODO: Somehow prevent name collisions with content properties. Currently, a content property named "Initialize" or "InternalContentDictionary" would cause a name collision.
	// We could disallow underscores in the beginning of the property name, and rename "Initialize" and "InternalContentDictionary" to start with an underscore.
	// It would also be a good idea to emit diagnostics when incorrect property names are used.
	private const string _template = $$"""
		using System;
		using System.Collections.Generic;

		namespace {{_namespace}};

		public sealed class {{_className}} : {{Constants.RootNamespace}}.IContentContainer<{{_contentType}}>
		{
			{{_contentFields}}

			private static IReadOnlyDictionary<string, {{_contentType}}>? _internalContentDictionary;

			public static void Initialize(IReadOnlyDictionary<string, {{_contentType}}> content)
			{
				{{_contentFieldInitializers}}

				_internalContentDictionary = content;
			}

			{{_contentProperties}}

			public static IReadOnlyDictionary<string, {{_contentType}}> InternalContentDictionary => _internalContentDictionary ?? throw new InvalidOperationException("Content has not been initialized.");
		}
		""";

	public static void Execute(SourceProductionContext context, (string? AssemblyName, ImmutableArray<AdditionalText> AdditionalTexts) args)
	{
		(string? assemblyName, ImmutableArray<AdditionalText> additionalTexts) = args;

		List<ContentFile> contentFiles = additionalTexts
			.Where(at => FileNameUtils.PathIsValid(at.Path))
			.Select(at => at.Path)
			.Select(contentFilePath => new ContentFile(contentFilePath))
			.ToList();

		foreach (IGrouping<ContentType, ContentFile> grouping in contentFiles.GroupBy(cf => cf.ContentType))
			CreateFile(context, assemblyName, grouping.Key, grouping.ToList());
	}

	private static void CreateFile(SourceProductionContext context, string? assemblyName, ContentType contentType, List<ContentFile> contentFiles)
	{
		// Sort content files by file name to ensure the generated code is deterministic.
		// Note; .NET Standard 2.0 does not support DistinctBy.
		List<ContentFile> sortedContentFiles = new();
		foreach (ContentFile contentFile in contentFiles.OrderBy(cf => cf.FileName))
		{
			if (sortedContentFiles.Any(cf => contentFile.FileName == cf.FileName))
				continue; // Caching issues sometimes cause the same file to be added twice.

			sortedContentFiles.Add(contentFile);
		}

		string className = contentType.GetClassName();
		string contentTypeName = contentType.GetContentTypeName();
		string sourceBuilder = _template
			.Replace(_namespace, assemblyName)
			.Replace(_className, className)
			.Replace(_contentType, contentTypeName)
			.Replace(_contentFieldInitializers, string.Join(Constants.NewLine, sortedContentFiles.ConvertAll(cf => cf.FieldInitializer)).IndentCode(2))
			.Replace(_contentFields, string.Join(Constants.NewLine, sortedContentFiles.ConvertAll(cf => cf.Field)).IndentCode(1))
			.Replace(_contentProperties, string.Join(Constants.NewLine, sortedContentFiles.ConvertAll(cf => cf.Property)).IndentCode(1));

		context.AddSource(className, SourceBuilderUtils.Build(sourceBuilder));
	}
}
