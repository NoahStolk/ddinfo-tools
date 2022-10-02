using DevilDaggersInfo.App.Ui.Base.Rendering;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Tools.Utils;

public static class ScissorUtils
{
	public static void ActivateScissor(Scissor? scissor)
	{
		if (scissor != null)
		{
			// TODO: Check if already active.
			Gl.Enable(EnableCap.ScissorTest);
			Gl.Scissor(scissor.X, scissor.Y, scissor.Width, scissor.Height);
		}
		else
		{
			Gl.Disable(EnableCap.ScissorTest);
		}
	}
}
