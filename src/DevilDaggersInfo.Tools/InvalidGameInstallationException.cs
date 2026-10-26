using System.Diagnostics.CodeAnalysis;

namespace DevilDaggersInfo.Tools;

// ReSharper disable UnusedMember.Global
[SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Exceptions should be public.")]
public sealed class InvalidGameInstallationException : Exception
{
	public InvalidGameInstallationException()
	{
	}

	public InvalidGameInstallationException(string? message)
		: base(message)
	{
	}

	public InvalidGameInstallationException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
