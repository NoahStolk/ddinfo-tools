using DevilDaggersInfo.Core.Encryption;
using DevilDaggersInfo.Tools.Utils;

namespace DevilDaggersInfo.Tools.Encryption;

/// <summary>
/// Encrypts using the secrets in the embedded <c>encryption.ini</c> resource. That file is not in the repository; CI writes it before publishing, so local builds fall back to <see cref="DummyEncryptionService" />.
/// </summary>
internal sealed class EncryptionService : IEncryptionService
{
	private readonly AesBase32Wrapper _aesBase32Wrapper;

	private EncryptionService(AesBase32Wrapper aesBase32Wrapper)
	{
		_aesBase32Wrapper = aesBase32Wrapper;
	}

	public bool IsAvailable => true;

	/// <summary>
	/// Reads the secrets from the embedded <c>encryption.ini</c> resource.
	/// </summary>
	/// <returns>The service, or <see langword="null" /> when the resource is missing or incomplete.</returns>
	public static EncryptionService? TryCreate()
	{
		using Stream? stream = AssemblyUtils.EntryAssembly.GetManifestResourceStream("DevilDaggersInfo.Tools.Content.encryption.ini");
		if (stream == null)
			return null;

		using StreamReader reader = new(stream);
		string ini = reader.ReadToEnd();
		string[] lines = ini.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

		string? iv = GetValue(lines, "iv");
		string? pass = GetValue(lines, "pass");
		string? salt = GetValue(lines, "salt");

		if (string.IsNullOrWhiteSpace(iv) || string.IsNullOrWhiteSpace(pass) || string.IsNullOrWhiteSpace(salt))
			return null;

		return new EncryptionService(new AesBase32Wrapper(iv, pass, salt));

		static string? GetValue(string[] iniLines, string key)
		{
			string? line = Array.Find(iniLines, l => l.StartsWith(key, StringComparison.OrdinalIgnoreCase));
			string[]? values = line?.Split('=');
			return values?.Length != 2 ? null : values[1].Trim();
		}
	}

	public string EncryptAndEncode(string input)
	{
		return _aesBase32Wrapper.EncryptAndEncode(input);
	}
}
