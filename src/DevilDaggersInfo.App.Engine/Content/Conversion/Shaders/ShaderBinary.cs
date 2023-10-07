namespace DevilDaggersInfo.App.Engine.Content.Conversion.Shaders;

internal record ShaderBinary(ShaderContentType ShaderContentType, byte[] Code) : IBinary<ShaderBinary>
{
	public ContentType ContentType => ContentType.Shader;

	public byte[] ToBytes()
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		bw.Write((byte)ShaderContentType);
		bw.Write((ushort)Code.Length);
		bw.Write(Code);
		return ms.ToArray();
	}

	public static ShaderBinary FromStream(BinaryReader br)
	{
		ShaderContentType shaderContentType = (ShaderContentType)br.ReadByte();
		ushort codeLength = br.ReadUInt16();
		byte[] code = br.ReadBytes(codeLength);
		return new(shaderContentType, code);
	}
}
