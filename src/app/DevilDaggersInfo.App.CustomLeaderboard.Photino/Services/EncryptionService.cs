using DevilDaggersInfo.Core.Encryption;
using DevilDaggersInfo.Razor.CustomLeaderboard.Services;
using Microsoft.Extensions.Configuration;

namespace DevilDaggersInfo.App.CustomLeaderboard.Photino.Services;

public class EncryptionService : IEncryptionService
{
	private readonly AesBase32Wrapper _encryptionWrapper;

	public EncryptionService(IConfiguration configuration)
	{
		IConfigurationSection section = configuration.GetRequiredSection("CustomLeaderboardSecrets");
		_encryptionWrapper = new(section["InitializationVector"]!, section["Password"]!, section["Salt"]!); // TODO: Use IOptions binding and require properties.
	}

	public string EncryptAndEncode(string input)
	{
		return _encryptionWrapper.EncryptAndEncode(input);
	}
}
