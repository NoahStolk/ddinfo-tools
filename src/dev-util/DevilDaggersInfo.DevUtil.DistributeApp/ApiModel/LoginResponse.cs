namespace DevilDaggersInfo.DevUtil.DistributeApp.ApiModel;

public record LoginResponse
{
	public required int Id { get; init; }

	public required string Name { get; init; }

	public required string Token { get; init; }
}
