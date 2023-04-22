using DevilDaggersInfo.App.ContentSourceGen.Utils;

namespace DevilDaggersInfo.App.ContentSourceGen.Extensions;

internal static class StringExtensions
{
	public static string FirstCharToLowerCase(this string str)
	{
		if (string.IsNullOrEmpty(str))
			return string.Empty;

		if (char.IsUpper(str[0]))
			return str.Length == 1 ? char.ToLower(str[0]).ToString() : char.ToLower(str[0]) + str.Substring(1);

		return str;
	}

	public static string FirstCharToUpperCase(this string str)
	{
		if (string.IsNullOrEmpty(str))
			return string.Empty;

		if (char.IsLower(str[0]))
			return str.Length == 1 ? char.ToUpper(str[0]).ToString() : char.ToUpper(str[0]) + str.Substring(1);

		return str;
	}

	public static string IndentCode(this string code, int count)
	{
		string indentation = new('\t', count);
		return code.Replace(Constants.NewLine, Constants.NewLine + indentation);
	}
}
