namespace DevilDaggersInfo.App;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class BuildTimeAttribute : Attribute
{
	public BuildTimeAttribute(string buildTime)
	{
		BuildTime = buildTime;
	}

	public string BuildTime { get; }
}
