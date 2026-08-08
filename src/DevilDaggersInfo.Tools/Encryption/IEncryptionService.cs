namespace DevilDaggersInfo.Tools.Encryption;

internal interface IEncryptionService
{
	/// <summary>
	/// Whether encryption is actually available. This is <see langword="false" /> when the encryption secrets could not be loaded, which is normal for local builds.
	/// </summary>
	bool IsAvailable { get; }

	string EncryptAndEncode(string input);
}
