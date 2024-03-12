using DevilDaggersInfo.Tools.Ui.MemoryTool;
using ImGuiNET;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Collections.Immutable;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor;

public static class ScriptingWindow
{
	private static string _scriptInput = string.Empty;
	private static ImmutableArray<Diagnostic> _diagnostics = ImmutableArray<Diagnostic>.Empty;

	private static Script<object>? _script;

	public static void Render()
	{
		if (ImGui.Begin("Scripting"))
		{
			ImGui.InputTextMultiline("##script", ref _scriptInput, 10_000, new(0, 512), ImGuiInputTextFlags.AllowTabInput);

			if (ImGui.Button("Compile"))
				_script = CSharpScript.Create(_scriptInput, ScriptOptions.Default.WithReferences(typeof(ExperimentalMemory).Assembly));

			if (ImGui.Button("Clear"))
				_script = null;

			ImGui.EndDisabled();

			if (_diagnostics.Length > 0)
			{
				ImGui.Text("Diagnostics:");
				foreach (Diagnostic diagnostic in _diagnostics)
				{
					ImGui.Text(diagnostic.ToString());
				}
			}

			if (_script != null)
			{
				ImGui.Text("Script running:");
				ImGui.Text(_script.Code);
			}
			else
			{
				ImGui.Text("No script running.");
			}
		}

		ImGui.End();
	}

	public static void RunScript()
	{
		if (_script == null)
			return;

		Task.Run(async () =>
		{
			try
			{
				await _script.RunAsync();
			}
			catch (CompilationErrorException ex)
			{
				_diagnostics = ex.Diagnostics;
			}
		});
	}
}
