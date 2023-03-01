using System.Text;

namespace DevilDaggersInfo.App.Core.NativeInterface.Utils;

public static class FileDialogFilterUtils
{
	/// <summary>
	/// Builds a file extension filter for a file dialog.
	/// </summary>
	/// <param name="fileTypeName">The name of the type of the file; for example, use <c>Text</c> for text files.</param>
	/// <param name="includeAllFiles">Whether to include the All files (*.*) pattern.</param>
	/// <param name="fileExtensionPatterns">The file extension patterns; for example, use <c>*.txt</c> for text files.</param>
	/// <returns>The filter.</returns>
	public static string BuildFilter(string fileTypeName, bool includeAllFiles, params string[] fileExtensionPatterns)
	{
		string patterns = string.Join(";", fileExtensionPatterns);
		return $"{fileTypeName} files ({patterns})|{patterns}{(includeAllFiles ? "|All files (*.*)|*.*" : null)}";
	}

	public static string BuildFilterComdlg32(Dictionary<string, string> patterns)
	{
		if (patterns.Count == 0)
			patterns.Add("All files", "*.*");

		StringBuilder sb = new();
		foreach (KeyValuePair<string, string> pattern in patterns)
		{
			sb.Append(pattern.Key);
			sb.Append('\0');
			sb.Append(pattern.Value);
			sb.Append('\0');
		}

		return sb.ToString();
	}
}
