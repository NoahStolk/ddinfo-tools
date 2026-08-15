using Hexa.NET.ImGui;

namespace DevilDaggersInfo.Tools.Ui;

/// <summary>
/// A font together with the size it was added at. Since Dear ImGui 1.92 the atlas rasterizes on demand and
/// <see cref="ImGui.PushFont(ImFontPtr, float)" /> requires an explicit size, so the two are kept together.
/// </summary>
/// <param name="Ptr">The ImGui font.</param>
/// <param name="Size">The size in logical pixels the font was added at.</param>
internal readonly record struct Font(ImFontPtr Ptr, float Size);
