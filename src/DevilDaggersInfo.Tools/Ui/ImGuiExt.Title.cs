using Hexa.NET.ImGui;

namespace DevilDaggersInfo.Tools.Ui;

internal static partial class ImGuiExt
{
	public static void Title(string title, Font font)
	{
		PushFont(font);
		ImGui.Text(title);
		ImGui.PopFont();
	}

	public static void Title(ReadOnlySpan<byte> title, Font font)
	{
		PushFont(font);
		ImGui.Text(title);
		ImGui.PopFont();
	}

	public static void PushFont(Font font)
	{
		ImGui.PushFont(font.Ptr, font.Size);
	}
}
