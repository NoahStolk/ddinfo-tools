namespace DevilDaggersInfo.App.ContentSourceGen.Utils;

internal static class GlslUtils
{
	public static ShaderUniform? GetFromGlslLine(string line)
	{
		string trimmedLine = line.Trim();

		string[] parts = trimmedLine.Split(' ');
		if (parts.Length < 3)
			return null;
		if (parts[0] != "uniform")
			return null;

		string type = GetType(parts);
		string name = GetName(parts);
		if (name.Length == 0)
			return null;

		return new(type, name);
	}

	private static string GetType(string[] parts)
	{
		string underlyingType = parts[1];
		bool isArray = parts[2].Contains('[');
		return isArray ? underlyingType + "[]" : underlyingType;
	}

	private static string GetName(string[] parts)
	{
		string end = parts[2];

		for (int i = 0; i < end.Length; i++)
		{
			char c = end[i];
			if (char.IsLetterOrDigit(c) || c == '_')
				continue;

			return end.Substring(0, i);
		}

		return end;
	}

	public record ShaderUniform(string Type, string Name)
	{
		public string Type { get; } = Type;

		public string Name { get; } = Name;
	}
}
