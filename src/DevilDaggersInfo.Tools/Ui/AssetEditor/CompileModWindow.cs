using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public static class CompileModWindow
{
	private static string? _outputDirectory;

	public static void Render()
	{
		if (ImGui.Begin("Compile mod"))
		{
			ImGui.Text(_outputDirectory ?? "No output path selected.");
			if (ImGui.Button("Browse"))
			{
				NativeFileDialog.SelectDirectory(s => _outputDirectory = s);
			}

			if (ImGui.Button("Compile"))
			{
				// TODO: Show progress bar and hide button when compiling.
				Task.Run(async () => await CompileAsync());
			}
		}

		ImGui.End();
	}

	private static async Task CompileAsync()
	{
		if (_outputDirectory == null)
			return;

		// TODO: Compile dd.
		ModBinaryBuilder builder = new(ModBinaryType.Audio);
		foreach (AudioAssetPath assetPath in FileStates.Mod.Object.Audio)
		{
			if (assetPath.AbsolutePath == null)
				continue;

			byte[] fileContents = await File.ReadAllBytesAsync(assetPath.AbsolutePath);
			builder.AddAsset(assetPath.AssetName, assetPath.AssetType, fileContents);
		}

		byte[] modContents = builder.Compile();

		// TODO: Pick file name.
		await File.WriteAllBytesAsync(Path.Combine(_outputDirectory, $"audio{FileStates.Mod.FileName}"), modContents);
	}
}
