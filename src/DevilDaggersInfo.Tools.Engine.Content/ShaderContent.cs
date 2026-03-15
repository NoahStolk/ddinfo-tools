namespace DevilDaggersInfo.Tools.Engine.Content;

public sealed class ShaderContent(string vertexCode, string fragmentCode)
{
	public string VertexCode { get; } = vertexCode;
	public string FragmentCode { get; } = fragmentCode;
}
