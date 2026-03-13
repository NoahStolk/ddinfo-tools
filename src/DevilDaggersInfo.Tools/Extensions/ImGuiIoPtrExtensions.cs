using ImGuiNET;

namespace DevilDaggersInfo.Tools.Extensions;

internal static class ImGuiIoPtrExtensions
{
	public static bool IsKeyDown(this ImGuiIOPtr io, ImGuiKey key)
	{
		int keyCode = key - ImGuiKey.NamedKey_BEGIN;

		if (keyCode < 0 || keyCode >= io.KeysData.Count)
			return false;

		return io.KeysData[keyCode].Down == 1;
	}
}
