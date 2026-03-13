namespace DevilDaggersInfo.Tools;

[AttributeUsage(AttributeTargets.Assembly)]
internal sealed class BuildTimeAttribute : Attribute
{
	public BuildTimeAttribute(string buildTime)
	{
		BuildTime = buildTime;
	}

	public string BuildTime { get; }
}
