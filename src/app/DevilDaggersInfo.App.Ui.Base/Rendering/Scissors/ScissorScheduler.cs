namespace DevilDaggersInfo.App.Ui.Base.Rendering.Scissors;

public static class ScissorScheduler
{
	private static readonly List<Scissor> _activeScissors = new();

	public static Scissor? GetCalculatedScissor()
	{
		if (_activeScissors.Count == 0)
			return null;

		Scissor scissor = _activeScissors[0];

		for (int i = 1; i < _activeScissors.Count; i++)
		{
			Scissor scissorToCombine = _activeScissors[i];

			int x1 = Math.Max(scissor.X, scissorToCombine.X);
			int y1 = Math.Max(scissor.Y, scissorToCombine.Y);

			int aX2 = scissor.X + (int)scissor.Width;
			int aY2 = scissor.Y + (int)scissor.Height;
			int bX2 = scissorToCombine.X + (int)scissorToCombine.Width;
			int bY2 = scissorToCombine.Y + (int)scissorToCombine.Height;

			int x2 = Math.Min(aX2, bX2) - x1;
			int y2 = Math.Min(aY2, bY2) - y1;

			scissor = new(x1, y1, (uint)x2, (uint)y2);
		}

		return scissor;
	}

	/// <summary>
	/// Pushes a scissor test for future render calls. Future render calls will be batched with this scissor test.
	/// </summary>
	public static void PushScissor(Scissor scissor)
	{
		_activeScissors.Add(scissor);
	}

	/// <summary>
	/// Pops the most recent scissor test. Future render calls will not be batched with this scissor test.
	/// </summary>
	public static void PopScissor()
	{
		_activeScissors.RemoveAt(_activeScissors.Count - 1);
	}
}
