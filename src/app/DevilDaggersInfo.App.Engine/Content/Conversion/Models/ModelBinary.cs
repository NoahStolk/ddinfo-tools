using DevilDaggersInfo.App.Engine.Extensions;
using DevilDaggersInfo.App.Engine.Parsers.Model;

namespace DevilDaggersInfo.App.Engine.Content.Conversion.Models;

internal record ModelBinary(IReadOnlyList<Vector3> Positions, IReadOnlyList<Vector2> Textures, IReadOnlyList<Vector3> Normals, IReadOnlyList<MeshData> Meshes) : IBinary<ModelBinary>
{
	public ContentType ContentType => ContentType.Model;

	public byte[] ToBytes()
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);

		bw.Write((ushort)Positions.Count);
		foreach (Vector3 position in Positions)
			bw.WriteAsHalfPrecision(position);

		bw.Write((ushort)Textures.Count);
		foreach (Vector2 texture in Textures)
			bw.WriteAsHalfPrecision(texture);

		bw.Write((ushort)Normals.Count);
		foreach (Vector3 normal in Normals)
			bw.WriteAsHalfPrecision(normal);

		bw.Write((ushort)Meshes.Count);
		foreach (MeshData mesh in Meshes)
		{
			bw.Write(mesh.MaterialName);

			bw.Write((ushort)mesh.Faces.Count);
			foreach (Face face in mesh.Faces)
			{
				bw.Write(face.Position);
				bw.Write(face.Texture);
				bw.Write(face.Normal);
			}
		}

		return ms.ToArray();
	}

	public static ModelBinary FromStream(BinaryReader br)
	{
		Vector3[] positions = new Vector3[br.ReadUInt16()];
		for (int i = 0; i < positions.Length; i++)
			positions[i] = br.ReadVector3AsHalfPrecision();

		Vector2[] textures = new Vector2[br.ReadUInt16()];
		for (int i = 0; i < textures.Length; i++)
			textures[i] = br.ReadVector2AsHalfPrecision();

		Vector3[] normals = new Vector3[br.ReadUInt16()];
		for (int i = 0; i < normals.Length; i++)
			normals[i] = br.ReadVector3AsHalfPrecision();

		MeshData[] meshes = new MeshData[br.ReadUInt16()];
		for (int i = 0; i < meshes.Length; i++)
		{
			string useMaterial = br.ReadString();

			Face[] faces = new Face[br.ReadUInt16()];
			for (int j = 0; j < faces.Length; j++)
				faces[j] = new(br.ReadUInt16(), br.ReadUInt16(), br.ReadUInt16());

			meshes[i] = new(useMaterial, faces);
		}

		return new(positions, textures, normals, meshes);
	}
}
