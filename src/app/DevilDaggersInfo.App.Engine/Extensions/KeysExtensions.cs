using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Engine.Extensions;

public static class KeysExtensions
{
	public static bool IsNumber(this Keys key)
	{
		int code = (int)key;
		return code is >= (int)Keys.Number0 and <= (int)Keys.Number9 or >= (int)Keys.Keypad0 and <= (int)Keys.Keypad9;
	}

	public static bool IsLetter(this Keys key)
	{
		int code = (int)key;
		return code is >= (int)Keys.A and <= (int)Keys.Z;
	}

	public static char? GetChar(this Keys key, bool isShiftKeyHeld)
	{
		if (key.IsLetter())
			return (char)((int)key + (isShiftKeyHeld ? 0 : 32));

		return key switch
		{
			Keys.Period => '.',
			Keys.Comma => ',',
			Keys.Slash => '/',
			Keys.Space => ' ',
			Keys.Minus => '-',
			Keys.Apostrophe => isShiftKeyHeld ? '"' : '\'',
			Keys.Semicolon => isShiftKeyHeld ? ':' : ';',
			Keys.BackSlash => isShiftKeyHeld ? '|' : '\\',
			Keys.Number0 or Keys.Keypad0 => '0',
			Keys.Number1 or Keys.Keypad1 => '1',
			Keys.Number2 or Keys.Keypad2 => '2',
			Keys.Number3 or Keys.Keypad3 => '3',
			Keys.Number4 or Keys.Keypad4 => '4',
			Keys.Number5 or Keys.Keypad5 => '5',
			Keys.Number6 or Keys.Keypad6 => '6',
			Keys.Number7 or Keys.Keypad7 => '7',
			Keys.Number8 or Keys.Keypad8 => '8',
			Keys.Number9 or Keys.Keypad9 => '9',
			_ => null,
		};
	}
}
