namespace DevilDaggersInfo.App.ContentSourceGen.Utils;

internal static class FileNameUtils
{
	/// <summary>
	/// Returns if the path is valid. The file name (without the extension) will be converted to a C# property, so it can only contain alphanumeric characters and cannot start with a digit.
	/// </summary>
	public static bool PathIsValid(string path)
	{
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
		if (fileNameWithoutExtension.Length == 0)
			return false;

		if (fileNameWithoutExtension.Any(c => !char.IsLetterOrDigit(c) && c != '_'))
			return false;

		if (fileNameWithoutExtension.Contains(',') || fileNameWithoutExtension.Contains('.'))
			return false;

		return !char.IsDigit(fileNameWithoutExtension[0]);
	}
}
