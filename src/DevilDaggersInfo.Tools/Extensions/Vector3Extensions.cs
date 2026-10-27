using System.Numerics;

namespace DevilDaggersInfo.Tools.Extensions;

internal static class Vector3Extensions
{
	extension(Vector3 vector)
	{
		public Vector4 ToVector4()
		{
			return new Vector4(vector, 1);
		}
	}
}
