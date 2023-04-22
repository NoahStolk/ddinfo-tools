using DevilDaggersInfo.App.Engine.Extensions;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Engine;

public sealed class KeyboardInput
{
	private const float _cursorTimerMax = 0.9f;

	private readonly KeySubmitter _keySubmitter = new();

	private readonly bool _isNumeric;
	private readonly Action<string>? _onEnter;
	private readonly Action<string>? _onInput;

	public KeyboardInput(bool isNumeric, Action<string>? onEnter = null, Action<string>? onInput = null)
	{
		_isNumeric = isNumeric;
		_onEnter = onEnter;
		_onInput = onInput;
	}

	public StringBuilder Value { get; } = new();

	public float CursorTimer { get; set; }

	public int CursorPositionStart { get; set; }
	public int CursorPositionEnd { get; set; }

	public void Update()
	{
		CursorTimer += EngineNodes.Game.Dt;
		if (CursorTimer > _cursorTimerMax)
			CursorTimer = 0;

		Keys? key = _keySubmitter.GetKey();
		if (key.HasValue)
			HandleKey(key.Value);
	}

	private void HandleKey(Keys key)
	{
		CursorTimer = 0;
		bool shift = Input.IsKeyHeld(Keys.ShiftLeft) || Input.IsKeyHeld(Keys.ShiftRight);
		bool ctrl = Input.IsKeyHeld(Keys.ControlLeft) || Input.IsKeyHeld(Keys.ControlRight);
		string? selection = GetSelection();
		int selectionLength = GetSelectionLength();
		switch (key)
		{
			case Keys.Backspace: Backspace(); break;
			case Keys.Delete: Delete(); break;
			case Keys.Left: MoveLeft(); break;
			case Keys.Right: MoveRight(); break;
			case Keys.Home: GoToStart(); break;
			case Keys.End: GoToEnd(); break;
			case Keys.A when ctrl: SelectAll(); break;
			case Keys.X when ctrl && selectionLength > 0: SetClipboardString(true); break;
			case Keys.C when ctrl && selectionLength > 0: SetClipboardString(false); break;
			case Keys.V when ctrl: Paste(); break;
			case Keys.Enter: _onEnter?.Invoke(Value.ToString()); break;
			default: TypeKey(); break;
		}

		void Backspace()
		{
			if (selectionLength == 0)
			{
				if (CursorPositionStart <= 0)
					return;

				Value.Remove(CursorPositionStart - 1, 1);
				DecrementCursorPosition();
			}
			else
			{
				DeleteSelection();
			}

			_onInput?.Invoke(Value.ToString());
		}

		void Delete()
		{
			if (selectionLength == 0 && CursorPositionStart >= Value.Length)
				return;

			DeleteSelection();
			_onInput?.Invoke(Value.ToString());
		}

		void MoveLeft()
		{
			if (shift)
				CursorPositionStart = Math.Max(0, CursorPositionStart - 1);
			else
				DecrementCursorPosition();
		}

		void MoveRight()
		{
			if (shift)
				CursorPositionStart = Math.Min(Value.Length, CursorPositionStart + 1);
			else
				IncrementCursorPosition(Math.Min(CursorPositionStart, CursorPositionEnd), 1);
		}

		void GoToStart()
		{
			CursorPositionEnd = 0;
			if (!shift)
				CursorPositionStart = 0;
		}

		void GoToEnd()
		{
			CursorPositionEnd = Value.Length;
			if (!shift)
				CursorPositionStart = Value.Length;
		}

		void SelectAll()
		{
			CursorPositionStart = 0;
			CursorPositionEnd = Value.Length;
		}

		void Paste()
		{
			string clipboardString = GetClipboardString().Replace("\n", string.Empty).Replace("\r", string.Empty);
			int min = Math.Min(CursorPositionStart, CursorPositionEnd);
			if (selection != null)
			{
				Value.Remove(min, selectionLength);
				CursorPositionStart -= selectionLength;
				CursorPositionEnd -= selectionLength;
			}

			Value.Insert(min, clipboardString);
			IncrementCursorPosition(min, clipboardString.Length);

			_onInput?.Invoke(Value.ToString());
		}

		void TypeKey()
		{
			if (_isNumeric && key != Keys.Period && key != Keys.Minus && !key.IsNumber())
				return;

			char? c = key.GetChar(shift);
			if (c == null)
				return;

			int min = Math.Min(CursorPositionStart, CursorPositionEnd);
			if (selection != null)
				Value.Remove(min, selectionLength);

			Value.Insert(min, c);
			IncrementCursorPosition(min, 1);

			_onInput?.Invoke(Value.ToString());
		}
	}

	private unsafe void SetClipboardString(bool deleteSelection)
	{
		string? selection = GetSelection();
		if (selection == null)
			return;

		Graphics.Glfw.SetClipboardString(Graphics.Window, selection);
		if (!deleteSelection)
			return;

		int min = Math.Min(CursorPositionStart, CursorPositionEnd);
		Value.Remove(min, selection.Length);
		CursorPositionStart = min;
		CursorPositionEnd = min;
	}

	private static unsafe string GetClipboardString()
	{
		return Graphics.Glfw.GetClipboardString(Graphics.Window);
	}

	private void DeleteSelection()
	{
		int min = Math.Min(CursorPositionStart, CursorPositionEnd);
		Value.Remove(min, Math.Max(1, GetSelectionLength()));
		CursorPositionStart = min;
		CursorPositionEnd = min;
	}

	public int GetSelectionLength()
		=> Math.Abs(CursorPositionEnd - CursorPositionStart);

	private string? GetSelection()
	{
		if (CursorPositionStart == CursorPositionEnd)
			return null;

		int start = CursorPositionStart;
		int end = CursorPositionEnd;
		if (CursorPositionStart > CursorPositionEnd)
		{
			start = CursorPositionEnd;
			end = CursorPositionStart;
		}

		string value = Value.ToString();
		return value.Length < CursorPositionEnd ? null : value[start..end];
	}

	private void IncrementCursorPosition(int min, int increment)
	{
		CursorPositionStart = Math.Min(Value.Length, min + increment);
		CursorPositionEnd = CursorPositionStart;
	}

	private void DecrementCursorPosition()
	{
		CursorPositionStart = Math.Max(0, CursorPositionStart - 1);
		CursorPositionEnd = CursorPositionStart;
	}

	public void SetText(string text)
	{
		Value.Clear();
		Value.Append(text);

		CursorPositionStart = Math.Min(CursorPositionStart, Value.Length);
		CursorPositionEnd = Math.Min(CursorPositionEnd, Value.Length);
	}
}
