namespace DevilDaggersInfo.App.Engine.Content.Conversion;

internal interface IBinary<out TSelf>
	where TSelf : IBinary<TSelf>
{
	ContentType ContentType { get; }

	byte[] ToBytes();

	static abstract TSelf FromStream(BinaryReader br);
}
