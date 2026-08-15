using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Utils;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

internal sealed class AboutWindow(FrameCounter frameCounter, FontService fontService)
{
	private readonly string _versionInfo = $"Version {AssemblyUtils.EntryAssemblyVersionString} (build time: {AssemblyUtils.EntryAssemblyBuildTime})";

	public bool Show;

	public void Render()
	{
		if (!Show)
			return;

		Vector2 windowSize = new(640, 640);
		ImGuiUtils.SetNextWindowMinSize(windowSize);
		if (ImGui.Begin("About ddinfo tools", ref Show, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking))
		{
			ImGui.PushStyleVar(ImGuiStyleVar.SeparatorTextPadding, new Vector2(20, 12));
			ImGui.PushTextWrapPos(ImGui.GetWindowWidth() - 16);

			ImGuiExt.Title("About"u8, fontService.GoetheBold30);
			ImGui.Text("ddinfo tools is a collection of tools for Devil Daggers. The tools are part of the DevilDaggers.info project.");

			ImGui.SeparatorText("Alpha notice");
			ImGui.Text($"""
				The tools are currently in alpha. If you have any feature requests, or encounter any issues, please report them on Discord or GitHub.

				If the app crashes, please send me the ddinfo-{AssemblyUtils.EntryAssemblyVersionString}.log file. This file holds information about the crash. It can be found in the same folder as the executable.

				Thank you for testing!
				""");

			ImGui.SeparatorText("Open source");
			ImGui.Text("The source code is available on GitHub:");
			RenderLibrary("https://github.com/NoahStolk/ddinfo-tools"u8, "ddinfo-tools"u8, "Main repository for the tools (this app)"u8);
			RenderLibrary("https://github.com/NoahStolk/ddinfo-core"u8, "ddinfo-core"u8, "Core libraries for DevilDaggers.info projects"u8);
			RenderLibrary("https://github.com/NoahStolk/ddinfo-web"u8, "ddinfo-web"u8, "DevilDaggers.info website and web server"u8);

			ImGui.SeparatorText("Third-party libraries");
			ImGui.Text("The app uses the following third-party libraries:");

			RenderLibrary("https://github.com/ocornut/imgui"u8, "Dear ImGui"u8, "Cross-platform UI framework"u8);
			RenderLibrary("https://github.com/mlabbe/nativefiledialog"u8, "NativeFileDialog"u8, "Cross-platform file dialogs"u8);
			RenderLibrary("https://github.com/dotnet/Silk.NET"u8, "Silk.NET"u8, "OpenGL, OpenAL, and GLFW bindings for C#"u8);
			RenderLibrary("https://github.com/serilog/serilog"u8, "Serilog"u8, "Logging"u8);
			RenderLibrary("https://github.com/SixLabors/ImageSharp"u8, "ImageSharp"u8, "Image processing"u8);
			RenderLibrary("https://github.com/HexaEngine/Hexa.NET.ImGui"u8, "Hexa.NET.ImGui"u8, "C# wrapper for Dear ImGui"u8);
			RenderLibrary("https://github.com/milleniumbug/NativeFileDialogSharp"u8, "NativeFileDialogSharp"u8, "C# wrapper for NativeFileDialog"u8);
			RenderFooter();

			ImGui.PopTextWrapPos();
			ImGui.PopStyleVar();
		}

		ImGui.End();
	}

	private static void RenderLibrary(ReadOnlySpan<byte> url, ReadOnlySpan<byte> name, ReadOnlySpan<byte> usage)
	{
		ImGui.Bullet();
		ImGuiExt.Hyperlink(url, name);
		ImGui.SameLine();
		ImGui.SetCursorPosX(192);
		ImGui.Text("-");
		ImGui.SameLine();
		ImGui.Text(usage);
	}

	private void RenderFooter()
	{
		ImGui.SetCursorPos(new Vector2(8, ImGui.GetWindowHeight() - 72));

		ImGui.TextColored(Colors.TitleColor(frameCounter.TotalTime), "© DevilDaggers.info 2017-2026");

		ImGuiExt.Hyperlink("https://devildaggers.com/"u8, "Devil Daggers"u8);
		ImGui.SameLine();
		ImGui.Text("is created by");
		ImGui.SameLine();
		ImGuiExt.Hyperlink("https://sorath.com/"u8, "Sorath"u8);

		ImGuiExt.Hyperlink("https://devildaggers.info/"u8, "DevilDaggers.info"u8);
		ImGui.SameLine();
		ImGui.Text("is created by");
		ImGui.SameLine();
		ImGuiExt.Hyperlink("https://noahstolk.com/"u8, "Noah Stolk"u8);

		ImGui.TextColored(Color.Gray(0.6f), _versionInfo);
	}
}
