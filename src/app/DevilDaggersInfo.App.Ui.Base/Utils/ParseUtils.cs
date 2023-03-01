namespace DevilDaggersInfo.App.Ui.Base.Utils;

public static class ParseUtils
{
	public static void TryParseAndExecute<T>(string input, Action<T> action)
		where T : IParsable<T>
	{
		if (T.TryParse(input, null, out T? v))
			action(v);
	}

	public static void TryParseAndExecute<T>(string input, T min, Action<T> action)
		where T : IParsable<T>, INumber<T>
	{
		if (T.TryParse(input, null, out T? v) && v >= min)
			action(v);
	}
}
