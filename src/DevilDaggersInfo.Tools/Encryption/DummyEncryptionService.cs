namespace DevilDaggersInfo.Tools.Encryption;

/// <summary>
/// Stand-in for <see cref="EncryptionService" /> when the encryption secrets are not available.
/// </summary>
internal sealed class DummyEncryptionService : IEncryptionService
{
	public bool IsAvailable => false;

	public string EncryptAndEncode(string input)
	{
		return "Encryption not available.";
	}
}
