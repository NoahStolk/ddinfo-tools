using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui;
using Hexa.NET.ImGui;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Extensions;

internal static class ImGuiExtensions
{
	extension(ImGui)
	{
		public static void Hyperlink(ReadOnlySpan<byte> url, ReadOnlySpan<byte> text)
		{
			Vector4 hyperlinkColor = new(0, 0.625f, 1, 1);
			Vector4 hyperlinkHoverColor = new(0.25f, 0.875f, 1, 0.25f);

			ImGui.PushStyleColor(ImGuiCol.Text, hyperlinkColor);
			ImGui.PushStyleColor(ImGuiCol.Button, default(Vector4));
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, hyperlinkHoverColor);
			ImGui.PushStyleColor(ImGuiCol.ButtonActive, default(Vector4));
			ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);

			if (ImGui.SmallButton(text))
				Process.Start(new ProcessStartInfo(url.ToString()) { UseShellExecute = true });

			ImGui.PopStyleColor(4);
			ImGui.PopStyleVar();

			if (ImGui.IsItemHovered())
				ImGui.SetTooltip(url);
		}

		public static void Title(string title, Font font)
		{
			ImGui.PushFont(font);
			ImGui.Text(title);
			ImGui.PopFont();
		}

		public static void Title(ReadOnlySpan<byte> title, Font font)
		{
			ImGui.PushFont(font);
			ImGui.Text(title);
			ImGui.PopFont();
		}

		public static void PushFont(Font font)
		{
			ImGui.PushFont(font.Ptr, font.Size);
		}

		public static void Image(uint imageId, Vector2 size)
		{
			ImGui.Image(imageId, size, Vector4.One);
		}

		public static void Image(uint imageId, Vector2 size, Vector4 color)
		{
			// Dear ImGui 1.92 dropped the tint color from ImGui.Image, so the tinted variant goes through the draw list.
			// Dummy reserves the same layout space ImGui.Image would have.
			Vector2 position = ImGui.GetCursorScreenPos();
			ImGui.GetWindowDrawList().AddImage(ImGui.GetTextureRef(imageId), position, position + size, Vector2.Zero, Vector2.One, ImGui.GetColorU32(color));
			ImGui.Dummy(size);
		}

		public static bool ImageButton(ReadOnlySpan<byte> strId, uint imageId, Vector2 size, Vector4 backgroundColor = default)
		{
			return ImGui.ImageButton(strId, ImGui.GetTextureRef(imageId), size, Vector2.Zero, Vector2.One, backgroundColor);
		}

		/// <summary>
		/// Wraps a raw OpenGL texture handle for ImGui. These textures are owned by the app, not by ImGui, so they are
		/// referenced by id only and never go through the <see cref="ImTextureData" /> protocol in <see cref="ImGuiController" />.
		/// </summary>
		public static unsafe ImTextureRef GetTextureRef(uint imageId)
		{
			return new ImTextureRef(null, new ImTextureID(imageId));
		}

		public static bool IsEnterPressed()
		{
			return ImGui.IsKeyPressed(ImGuiKey.Enter) || ImGui.IsKeyPressed(ImGuiKey.KeypadEnter);
		}

		public static void SetNextWindowMinSize(float minSizeX, float minSizeY)
		{
			ImGui.SetNextWindowMinSize(new Vector2(minSizeX, minSizeY));
		}

		public static void SetNextWindowMinSize(Vector2 minSize)
		{
			ImGui.SetNextWindowSizeConstraints(minSize, new Vector2(float.MaxValue, float.MaxValue));
		}

		public static Vector4 GetColor(ImGuiCol color)
		{
			uint i = ImGui.GetColorU32(color);
			byte r = (byte)((i >> 00) & 0xFF);
			byte g = (byte)((i >> 08) & 0xFF);
			byte b = (byte)((i >> 16) & 0xFF);
			byte a = (byte)((i >> 24) & 0xFF);
			return new Vector4(r / 255f, g / 255f, b / 255f, a / 255f);
		}
	}
}
