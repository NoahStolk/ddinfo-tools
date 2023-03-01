using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Ui.Base.Rendering.Scissors;

/// <summary>
/// Responsible for setting the scissor test during rendering.
/// </summary>
public static class ScissorActivator
{
	private static Scissor? _cached;

	/// <summary>
	/// Sets the scissor test for rendering.
	/// </summary>
	public static void SetScissor(Scissor? scissor)
	{
		if (_cached == scissor)
			return;

		if (scissor.HasValue)
		{
			Gl.Enable(EnableCap.ScissorTest);
			Gl.Scissor(scissor.Value.X, scissor.Value.Y, scissor.Value.Width, scissor.Value.Height);
		}
		else
		{
			Gl.Disable(EnableCap.ScissorTest);
		}

		_cached = scissor;
	}
}
