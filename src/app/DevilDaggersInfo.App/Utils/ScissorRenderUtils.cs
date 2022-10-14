using DevilDaggersInfo.App.Ui.Base.Rendering;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Utils;

public static class ScissorRenderUtils
{
	private static Scissor? _cached;

	public static void ActivateScissor(Scissor? scissor)
	{
		if (_cached == scissor)
			return;

		if (scissor != null)
		{
			Gl.Enable(EnableCap.ScissorTest);
			Gl.Scissor(scissor.X, scissor.Y, scissor.Width, scissor.Height);
		}
		else
		{
			Gl.Disable(EnableCap.ScissorTest);
		}

		_cached = scissor;
	}
}
