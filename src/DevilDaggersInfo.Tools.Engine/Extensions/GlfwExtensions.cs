using Silk.NET.GLFW;

namespace DevilDaggersInfo.Tools.Engine.Extensions;

public static class GlfwExtensions
{
	public static unsafe void CheckError(this Glfw glfw)
	{
		ErrorCode errorCode = glfw.GetError(out byte* c);
		if (errorCode == ErrorCode.NoError || c == (byte*)0)
			return;

		StringBuilder errorBuilder = new();
		while (*c != 0x00)
			errorBuilder.Append((char)*c++);

		throw new InvalidOperationException($"GLFW error {errorCode}: {errorBuilder}");
	}
}
