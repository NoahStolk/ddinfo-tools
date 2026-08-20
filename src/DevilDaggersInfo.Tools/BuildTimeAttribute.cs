namespace DevilDaggersInfo.Tools;

[AttributeUsage(AttributeTargets.Assembly)]
internal sealed class BuildTimeAttribute(string buildTime) : Attribute
{
	public string BuildTime { get; } = buildTime;
}
