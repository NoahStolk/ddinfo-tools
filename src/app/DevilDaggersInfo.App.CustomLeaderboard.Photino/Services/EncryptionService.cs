using DevilDaggersInfo.Core.CustomLeaderboard.Services;
using DevilDaggersInfo.Core.Encryption;
using Microsoft.Extensions.Configuration;

namespace DevilDaggersInfo.App.CustomLeaderboard.Photino.Services;

public class EncryptionService : IEncryptionService
{
	private readonly AesBase32Wrapper _encryptionWrapper;

	public EncryptionService(IConfiguration configuration)
	{
		IConfigurationSection section = configuration.GetRequiredSection("CustomLeaderboardSecrets");
		_encryptionWrapper = new(section["InitializationVector"], section["Password"], section["Salt"]);
	}

	public string EncryptAndEncode(string input)
	{
		return _encryptionWrapper.EncryptAndEncode(input);
	}
}
