using DevilDaggersInfo.App.ContentSourceGen.Extensions;
using DevilDaggersInfo.App.ContentSourceGen.Utils;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace DevilDaggersInfo.App.ContentSourceGen.Generators.Shader;

public static class ShaderBuilder
{
	private const string _namespace = $"%{nameof(_namespace)}%";
	private const string _shaderName = $"%{nameof(_shaderName)}%";
	private const string _uniformLocationFields = $"%{nameof(_uniformLocationFields)}%";
	private const string _uniformLocationFieldInitializers = $"%{nameof(_uniformLocationFieldInitializers)}%";
	private const string _uniformMethods = $"%{nameof(_uniformMethods)}%";

	private const string _shaderTemplate = $$"""
		using System;
		using System.Collections.Generic;
		using {{Constants.RootNamespace}}.Utils;

		namespace {{_namespace}};

		public static class {{_shaderName}}
		{
			private static uint _shaderId;

			{{_uniformLocationFields}}

			public static void Initialize(uint shaderId)
			{
				_shaderId = shaderId;

				{{_uniformLocationFieldInitializers}}
			}

			public static void Use()
			{
				{{Constants.RootNamespace}}.Graphics.Gl.UseProgram(_shaderId);
			}

		{{_uniformMethods}}
		}
		""";

	private const string _staticShaderInitializers = $"%{nameof(_staticShaderInitializers)}%";

	private const string _shadersTemplate = $$"""
		using System;
		using System.Collections.Generic;
		using {{Constants.RootNamespace}}.Utils;

		namespace {{_namespace}};

		public static class Shaders
		{
			public static void Initialize(IReadOnlyDictionary<string, {{Constants.RootNamespace}}.Content.Shader> content)
			{
				{{_staticShaderInitializers}}
			}
		}
		""";

	public static void Execute(SourceProductionContext context, (string? AssemblyName, ImmutableArray<AdditionalText> AdditionalTexts) args)
	{
		(string? assemblyName, ImmutableArray<AdditionalText> additionalTexts) = args;

		List<ShaderFile> shaderFiles = additionalTexts
			.Where(at => FileNameUtils.PathIsValid(at.Path))
			.Select(at => new ShaderFile(at.Path, at.GetText()?.ToString()))
			.ToList();

		foreach (IGrouping<string, ShaderFile> grouping in shaderFiles.GroupBy(sf => sf.ShaderClassName))
			CreateShaderFile(context, assemblyName, grouping.Key, grouping.ToList());

		CreateShadersFile(context, assemblyName, shaderFiles);
	}

	private static void CreateShaderFile(SourceProductionContext context, string? assemblyName, string shaderName, List<ShaderFile> shaderFiles)
	{
		// Sort shader uniforms by name to ensure the generated code is deterministic.
		// Note; .NET Standard 2.0 does not support DistinctBy.
		List<ShaderUniform> sortedShaderUniforms = new();
		foreach (ShaderUniform shaderUniform in shaderFiles.SelectMany(sf => sf.Uniforms))
		{
			if (sortedShaderUniforms.Any(cf => shaderUniform.Name == cf.Name))
				continue; // Filter out duplicates in case of caching issues.

			sortedShaderUniforms.Add(shaderUniform);
		}

		string sourceBuilder = _shaderTemplate
			.Replace(_namespace, assemblyName)
			.Replace(_shaderName, shaderName)
			.Replace(_uniformLocationFields, string.Join(Constants.NewLine, sortedShaderUniforms.ConvertAll(sf => sf.LocationField)).IndentCode(1))
			.Replace(_uniformLocationFieldInitializers, string.Join(Constants.NewLine, sortedShaderUniforms.ConvertAll(sf => sf.LocationFieldInitializer)).IndentCode(2))
			.Replace(_uniformMethods, string.Join(Constants.NewLine, sortedShaderUniforms.ConvertAll(sf => sf.UniformMethod)));

		context.AddSource(shaderName, SourceBuilderUtils.Build(sourceBuilder));
	}

	private static void CreateShadersFile(SourceProductionContext context, string? assemblyName, List<ShaderFile> shaderFiles)
	{
		// Sort shader uniforms by name to ensure the generated code is deterministic.
		// Note; .NET Standard 2.0 does not support DistinctBy.
		List<ShaderFile> sortedShaderFiles = new();
		foreach (ShaderFile shaderFile in shaderFiles)
		{
			if (sortedShaderFiles.Any(cf => shaderFile.ShaderClassName == cf.ShaderClassName))
				continue; // Filter out duplicates because a shader has multiple types (vertex, geometry, fragment) and we only want to initialize once.

			sortedShaderFiles.Add(shaderFile);
		}

		string sourceBuilder = _shadersTemplate
			.Replace(_namespace, assemblyName)
			.Replace(_staticShaderInitializers, string.Join(Constants.NewLine, sortedShaderFiles.ConvertAll(sf => sf.ShaderInitializer)).IndentCode(2));

		context.AddSource("Shaders", SourceBuilderUtils.Build(sourceBuilder));
	}
}
