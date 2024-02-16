using System.Security;

namespace DevilDaggersInfo.Tools.Extensions;

public static class ExceptionExtensions
{
	public static bool IsFileIoException(this Exception ex)
	{
		return ex is PathTooLongException or DirectoryNotFoundException or IOException or UnauthorizedAccessException or FileNotFoundException or NotSupportedException or SecurityException;
	}
}
