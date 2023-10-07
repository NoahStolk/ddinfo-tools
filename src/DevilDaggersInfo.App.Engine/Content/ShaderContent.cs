namespace DevilDaggersInfo.App.Engine.Content;

public class ShaderContent
{
	public ShaderContent(string vertexCode, string? geometryCode, string fragmentCode)
	{
		VertexCode = vertexCode;
		GeometryCode = geometryCode;
		FragmentCode = fragmentCode;
	}

	public string VertexCode { get; }
	public string? GeometryCode { get; }
	public string FragmentCode { get; }
}
