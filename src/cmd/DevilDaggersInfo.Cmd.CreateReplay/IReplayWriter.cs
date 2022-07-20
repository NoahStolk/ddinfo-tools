namespace DevilDaggersInfo.Cmd.CreateReplay;

public interface IReplayWriter
{
	ReplayBinary<LocalReplayBinaryHeader> Write();
}
