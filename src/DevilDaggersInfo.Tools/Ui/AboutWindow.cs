using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

public static class AboutWindow
{
	private static readonly string _versionInfo = $"Version {AssemblyUtils.EntryAssemblyVersionString} (build time: {AssemblyUtils.EntryAssemblyBuildTime})";

	public static void Render(ref bool show)
	{
		if (!show)
			return;

		Vector2 windowSize = new(640, 640);
		ImGuiUtils.SetNextWindowMinSize(windowSize);
		if (ImGui.Begin("About ddinfo tools", ref show, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking))
		{
			ImGui.PushStyleVar(ImGuiStyleVar.SeparatorTextPadding, new Vector2(20, 12));
			ImGui.PushTextWrapPos(ImGui.GetWindowWidth() - 16);

			ImGuiExt.Title("About");
			ImGui.Text("ddinfo tools is a collection of tools for Devil Daggers. The tools are part of the DevilDaggers.info project.");

			ImGui.SeparatorText("Alpha notice");
			ImGui.Text($"""
				The tools are currently in alpha. If you have any feature requests, or encounter any issues, please report them on Discord or GitHub.

				If the app crashes, please send me the ddinfo-{AssemblyUtils.EntryAssemblyVersionString}.log file. This file holds information about the crash. It can be found in the same folder as the executable.

				Thank you for testing!
				""");

			ImGui.SeparatorText("Open source");
			ImGui.Text("The source code is available on GitHub:");
			RenderLibrary("https://github.com/NoahStolk/ddinfo-tools", "ddinfo-tools", "Main repository for the tools (this app)");
			RenderLibrary("https://github.com/NoahStolk/ddinfo-core", "ddinfo-core", "Core libraries for DevilDaggers.info projects");
			RenderLibrary("https://github.com/NoahStolk/ddinfo-web", "ddinfo-web", "DevilDaggers.info website and web server");
			RenderLibrary("https://github.com/NoahStolk/imgui-glfw-dotnet", "imgui-glfw-dotnet", "ImGui.NET rendering back-end for GLFW");

			ImGui.SeparatorText("Third-party libraries");
			ImGui.Text("The app uses the following third-party libraries:");

			RenderLibrary("https://github.com/ocornut/imgui", "Dear ImGui", "Cross-platform UI framework");
			RenderLibrary("https://github.com/mlabbe/nativefiledialog", "NativeFileDialog", "Cross-platform file dialogs");
			RenderLibrary("https://github.com/dotnet/Silk.NET", "Silk.NET", "OpenGL, OpenAL, and GLFW bindings for C#");
			RenderLibrary("https://github.com/serilog/serilog", "Serilog", "Logging");
			RenderLibrary("https://github.com/SixLabors/ImageSharp", "ImageSharp", "Image processing");
			RenderLibrary("https://github.com/ImGuiNET/ImGui.NET", "ImGui.NET", "C# wrapper for Dear ImGui");
			RenderLibrary("https://github.com/milleniumbug/NativeFileDialogSharp", "NativeFileDialogSharp", "C# wrapper for NativeFileDialog");
			RenderLibrary("https://github.com/microsoft/CsWin32", "CsWin32", "Source-generator for Win32 P/Invoke methods");
			RenderFooter();

			ImGui.PopTextWrapPos();
			ImGui.PopStyleVar();
		}

		ImGui.End(); // End About ddinfo tools
	}

	private static void RenderLibrary(ReadOnlySpan<char> url, ReadOnlySpan<char> name, ReadOnlySpan<char> usage)
	{
		ImGui.Bullet();
		ImGuiExt.Hyperlink(url, name);
		ImGui.SameLine();
		ImGui.SetCursorPosX(192);
		ImGui.Text("-");
		ImGui.SameLine();
		ImGui.Text(usage);
	}

	private static void RenderFooter()
	{
		ImGui.SetCursorPos(new(8, ImGui.GetWindowHeight() - 72));

		ImGui.TextColored(Colors.TitleColor, "© DevilDaggers.info 2017-2024");

		ImGuiExt.Hyperlink("https://devildaggers.com/", "Devil Daggers");
		ImGui.SameLine();
		ImGui.Text("is created by");
		ImGui.SameLine();
		ImGuiExt.Hyperlink("https://sorath.com/", "Sorath");

		ImGuiExt.Hyperlink("https://devildaggers.info/", "DevilDaggers.info");
		ImGui.SameLine();
		ImGui.Text("is created by");
		ImGui.SameLine();
		ImGuiExt.Hyperlink("https://noahstolk.com/", "Noah Stolk");

		ImGui.TextColored(Color.Gray(0.6f), _versionInfo);
	}
}
