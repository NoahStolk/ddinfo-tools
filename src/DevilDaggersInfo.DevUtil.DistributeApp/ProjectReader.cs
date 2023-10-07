using System.Xml;

namespace DevilDaggersInfo.DevUtil.DistributeApp;

public static class ProjectReader
{
	public static string ReadVersionFromProjectFile(string projectFilePath)
	{
		XmlDocument doc = new();
		doc.LoadXml(File.ReadAllText(projectFilePath));

		string? version = doc.SelectNodes("/Project/PropertyGroup/Version/text()")?.Item(0)?.InnerText;
		if (version == null)
			throw new("Version not found in project file.");

		return version;
	}
}
