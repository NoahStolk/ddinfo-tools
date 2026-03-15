using DevilDaggersInfo.Tools.Engine.Content.Parsers.Model;
using DevilDaggersInfo.Tools.Engine.Content.Parsers.Texture;
using System.Reflection;
using System.Text;

namespace DevilDaggersInfo.Tools.Engine.Content;

public static class InternalResourceReader
{
	private enum ShaderType
	{
		Vertex,
		Fragment,
	}

	public static InternalContent Read(Assembly assembly)
	{
		TextureContent blankTexture = new(1, 1, [0xFF, 0xFF, 0xFF, 0xFF]);

		Dictionary<string, ModelContent> models = new();
		Dictionary<string, ShaderContent> shaders = new();
		Dictionary<string, TextureContent> textures = new();

		Dictionary<string, ModelContext> modelContexts = new();
		Dictionary<string, ShaderSourceCollection> shaderSourceCollections = new();

		foreach (string resourceName in assembly.GetManifestResourceNames())
		{
			string extension = Path.GetExtension(resourceName);
			Stream? stream = assembly.GetManifestResourceStream(resourceName);
			if (stream == null)
				throw new InvalidOperationException($"Resource '{resourceName}' not found.");

			string assetName = Path.GetFileNameWithoutExtension(resourceName);
			int indexOfFinalPeriod = assetName.LastIndexOf('.');
			if (indexOfFinalPeriod != -1)
				assetName = assetName[(indexOfFinalPeriod + 1)..];

			switch (extension)
			{
				case ".obj": modelContexts[assetName] = GetModel(stream); break;
				case ".vert": SetShaderSource(shaderSourceCollections, stream, assetName, ShaderType.Vertex); break;
				case ".frag": SetShaderSource(shaderSourceCollections, stream, assetName, ShaderType.Fragment); break;
				case ".tga": textures[assetName] = GetTexture(stream); break;
				default: throw new NotSupportedException($"Reading content with extension '{extension}' is not supported.");
			}

			stream.Dispose();
		}

		// Post-processing for models and shaders.
		foreach (KeyValuePair<string, ModelContext> modelContext in modelContexts)
		{
			Dictionary<MeshContent, TextureContent> meshes = new();

			foreach ((string MaterialName, MeshContent Mesh) mesh in modelContext.Value.Meshes)
			{
				textures.TryGetValue(mesh.MaterialName, out TextureContent? texture);
				meshes.Add(mesh.Mesh, texture ?? blankTexture);
			}

			models[modelContext.Key] = new ModelContent(meshes);
		}

		foreach (KeyValuePair<string, ShaderSourceCollection> shaderSource in shaderSourceCollections)
		{
			if (shaderSource.Value.VertexCode == null)
				throw new InvalidOperationException($"Vertex shader source for '{shaderSource.Key}' not found.");
			if (shaderSource.Value.FragmentCode == null)
				throw new InvalidOperationException($"Fragment shader source for '{shaderSource.Key}' not found.");

			shaders[shaderSource.Key] = new ShaderContent(shaderSource.Value.VertexCode, shaderSource.Value.FragmentCode);
		}

		return new InternalContent(models, shaders, textures);
	}

	private static ModelContext GetModel(Stream stream)
	{
		using MemoryStream ms = new();
		stream.CopyTo(ms);

		ModelData modelData = ObjParser.Parse(ms.ToArray());
		return new ModelContext(modelData.Meshes.Select(m => (m.MaterialName, GetMesh(modelData, m))).ToList());

		static MeshContent GetMesh(ModelData modelData, MeshData meshData)
		{
			Vertex[] outVertices = new Vertex[meshData.Faces.Count];
			uint[] outFaces = new uint[meshData.Faces.Count];
			for (int j = 0; j < meshData.Faces.Count; j++)
			{
				ushort t = meshData.Faces[j].Texture;

				outVertices[j] = new Vertex(
					modelData.Positions[meshData.Faces[j].Position - 1],
					modelData.Textures.Count > t - 1 && t > 0 ? modelData.Textures[t - 1] : default, // TODO: Separate face type?
					modelData.Normals[meshData.Faces[j].Normal - 1]);
				outFaces[j] = (ushort)j;
			}

			return new MeshContent(outVertices, outFaces);
		}
	}

	private static void SetShaderSource(IDictionary<string, ShaderSourceCollection> shaderSources, Stream stream, string shaderName, ShaderType shaderType)
	{
		if (!shaderSources.TryGetValue(shaderName, out ShaderSourceCollection? value))
		{
			value = new ShaderSourceCollection();
			shaderSources.Add(shaderName, value);
		}

		using MemoryStream ms = new();
		stream.CopyTo(ms);
		string code = Encoding.UTF8.GetString(ms.ToArray());
		switch (shaderType)
		{
			case ShaderType.Vertex: value.VertexCode = code; break;
			case ShaderType.Fragment: value.FragmentCode = code; break;
			default: throw new NotSupportedException($"{nameof(ShaderType)} '{shaderType}' is not supported.");
		}
	}

	private static TextureContent GetTexture(Stream stream)
	{
		using MemoryStream ms = new();
		stream.CopyTo(ms);
		TextureData data = TgaParser.Parse(ms.ToArray());
		return new TextureContent(data.Width, data.Height, data.ColorData);
	}

	private sealed class ShaderSourceCollection
	{
		public string? VertexCode { get; set; }
		public string? FragmentCode { get; set; }
	}

	private sealed class ModelContext(List<(string MaterialName, MeshContent Mesh)> meshes)
	{
		public List<(string MaterialName, MeshContent Mesh)> Meshes { get; } = meshes;
	}
}
