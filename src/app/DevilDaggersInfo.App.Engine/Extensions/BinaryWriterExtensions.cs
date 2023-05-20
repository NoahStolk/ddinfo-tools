namespace DevilDaggersInfo.App.Engine.Extensions;

internal static class BinaryWriterExtensions
{
	public static void WriteAsHalfPrecision(this BinaryWriter binaryWriter, Vector2 vector)
	{
		binaryWriter.Write((Half)vector.X);
		binaryWriter.Write((Half)vector.Y);
	}

	public static void WriteAsHalfPrecision(this BinaryWriter binaryWriter, Vector3 vector)
	{
		binaryWriter.Write((Half)vector.X);
		binaryWriter.Write((Half)vector.Y);
		binaryWriter.Write((Half)vector.Z);
	}
}
