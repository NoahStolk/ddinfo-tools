using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;

public static class ModPreviewWindow
{
	private static ModBinaryToc? _modBinaryToc;
	private static long? _modFileSize;
	private static string? _selectedFileName;

	public static string? SelectedFileName
	{
		get => _selectedFileName;
		set
		{
			_selectedFileName = value;
			LoadChunks();
		}
	}

	public static void LoadChunks()
	{
		if (_selectedFileName == null)
			return;

		string filePath = Path.Combine(UserSettings.ModsDirectory, _selectedFileName);
		if (!File.Exists(filePath))
			return;

		try
		{
			using FileStream fs = new(filePath, FileMode.Open);
			_modFileSize = fs.Length;
			using BinaryReader reader = new(fs);
			_modBinaryToc = ModBinaryToc.FromReader(reader);
		}
		catch (InvalidModBinaryException)
		{
			_selectedFileName = null;
			_modBinaryToc = null;
			_modFileSize = null;
		}
		catch (Exception ex)
		{
			Root.Log.Error(ex, $"Error loading mod binary '{_selectedFileName}'.");
			PopupManager.ShowError($"Error loading mod binary '{_selectedFileName}'.\n\n" + ex.Message);
			_selectedFileName = null;
			_modBinaryToc = null;
			_modFileSize = null;
		}
	}

	public static void Render()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(640, 360));
		if (ImGui.Begin("Mod Preview", ImGuiWindowFlags.NoCollapse))
		{
			ImGui.PopStyleVar();

			if (_modBinaryToc == null || _selectedFileName == null)
			{
				ImGui.Text("Select a valid mod from the Mod Manager window to preview its contents.");
			}
			else
			{
				if (ImGui.BeginTable("File info", 2, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders, new(400, 0)))
				{
					ImGui.TableNextColumn();
					ImGui.Text("File name");

					ImGui.TableNextColumn();
					ImGui.Text(_selectedFileName);

					ImGui.TableNextColumn();
					ImGui.Text("Binary type");

					ImGui.TableNextColumn();
					ImGui.Text(EnumUtils.ModBinaryTypeNames[_modBinaryToc.Type]);

					ImGui.TableNextColumn();
					ImGui.Text("File size");

					ImGui.TableNextColumn();
					ImGui.Text(FileSizeUtils.Format(_modFileSize ?? 0));

					ImGui.TableNextColumn();
					ImGui.Text("Asset count");

					ImGui.TableNextColumn();
					ImGui.Text(Inline.Span(_modBinaryToc.Chunks.Count));

					ImGui.TableNextColumn();
					ImGui.Text("Prohibited asset count");

					ImGui.TableNextColumn();
					ImGui.Text(Inline.Span(_modBinaryToc.Chunks.Count(c => AssetContainer.GetIsProhibited(c.AssetType, c.Name))));

					ImGui.EndTable();
				}

				if (ImGui.Button("Toggle prohibited"))
					ModsDirectoryWindow.Logic.ToggleProhibitedAssets(_selectedFileName);

				if (ImGui.BeginTable("Chunks", 4, ImGuiTableFlags.Resizable | ImGuiTableFlags.Sortable))
				{
					ImGui.TableSetupColumn("Asset name", ImGuiTableColumnFlags.DefaultSort, 256, 0);
					ImGui.TableSetupColumn("Asset type", ImGuiTableColumnFlags.None, 128, 1);
					ImGui.TableSetupColumn("Prohibited", ImGuiTableColumnFlags.None, 64, 2);
					ImGui.TableSetupColumn("Raw size", ImGuiTableColumnFlags.None, 64, 3);
					ImGui.TableHeadersRow();

					for (int i = 0; i < _modBinaryToc.Chunks.Count; i++)
					{
						ModBinaryChunk chunk = _modBinaryToc.Chunks[i];

						ImGui.TableNextColumn();
						ImGui.Text(chunk.Name);

						ImGui.TableNextColumn();
						ImGui.TextColored(GetColor(chunk.AssetType), EnumUtils.AssetTypeNames[chunk.AssetType]);

						ImGui.TableNextColumn();
						if (AssetContainer.GetIsProhibited(chunk.AssetType, chunk.Name))
							ImGui.TextColored(Color.Orange, "Prohibited");
						else
							ImGui.TextColored(Color.Green, "OK");

						ImGui.TableNextColumn();
						ColumnTextRight(FileSizeUtils.Format(chunk.Size));
					}

					ImGui.EndTable();
				}
			}
		}
		else
		{
			ImGui.PopStyleVar();
		}

		ImGui.End(); // End Mod preview

		static void ColumnTextRight(ReadOnlySpan<char> span)
		{
			ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - ImGui.CalcTextSize(span).X - ImGui.GetScrollX());
			ImGui.Text(span);
		}
	}

	private static Vector4 GetColor(this AssetType assetType) => assetType switch
	{
		AssetType.Audio => new(1, 0.25f, 1, 1),
		AssetType.ObjectBinding => new(0.25f, 1, 1, 1),
		AssetType.Mesh => new(1, 0.25f, 0.25f, 1),
		AssetType.Shader => new(0.25f, 1, 0.25f, 1),
		AssetType.Texture => new(1, 0.66f, 0.25f, 1),
		_ => Vector4.One,
	};
}
