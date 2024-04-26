using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Spawnset.Extensions;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Core.Wiki.Structs;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Collections.Immutable;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor;

public static class SpawnsWindow
{
	public const int MaxSpawns = 4096;

	private static readonly bool[] _selected = new bool[MaxSpawns];
	private static EnemyType _selectedEnemyType;

	private static int? _scrollToIndex;

	private static int _lastSelectedIndex = -1;
	private static float _editDelay;
	private static bool _delayEdited;

	private static float _addDelay;

	private static bool _windowIsFocused;

	public static void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(360, 400);
		if (ImGui.Begin("Spawns"))
		{
			_windowIsFocused = ImGui.IsWindowFocused(ImGuiFocusedFlags.ChildWindows);

			float? endLoopLength = GetEndLoopLength();
			bool isEndLoopTooShort = endLoopLength < 0.1f;

			if (endLoopLength.HasValue && isEndLoopTooShort)
			{
				ImGui.PushStyleColor(ImGuiCol.ChildBg, new Engine.Maths.Numerics.Color(127, 0, 0, 255));
				if (ImGui.BeginChild("Warning", new(0, 56)))
				{
					ImGui.TextWrapped(Inline.Span($"(!) The end loop is only {endLoopLength.Value} seconds long, which will probably result in severe lag or a crash. If you intend to have no end loop, make sure to add an empty spawn at the end of the spawns list."));
					ImGui.EndChild();
				}

				ImGui.PopStyleColor();
			}

			const float controlsHeight = 128;
			Vector2 listSize = new(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - controlsHeight - 4);
			if (ImGui.BeginChild("SpawnsListChild", listSize))
				RenderSpawnsTable();

			ImGui.EndChild(); // End SpawnsListChild

			if (ImGui.BeginChild("SpawnControlsChild"))
			{
				if (ImGui.BeginChild("AddAndInsertButtons", new(72, controlsHeight)))
				{
					if (ImGui.Button("Add", new(64, 32)))
					{
						if (FileStates.Spawnset.Object.Spawns.Length >= MaxSpawns)
						{
							PopupManager.ShowError("Reached max amount of spawns.");
						}
						else
						{
							FileStates.Spawnset.Update(FileStates.Spawnset.Object with { Spawns = FileStates.Spawnset.Object.Spawns.Add(new(_selectedEnemyType, _addDelay)) });
							SpawnsetHistoryUtils.Save(SpawnsetEditType.SpawnAdd);
							_scrollToIndex = FileStates.Spawnset.Object.Spawns.Length - 1;
						}
					}

					if (ImGui.Button("Insert", new(64, 32)))
					{
						int selectedIndex = Array.IndexOf(_selected, true);
						if (selectedIndex == -1)
							selectedIndex = 0;

						if (FileStates.Spawnset.Object.Spawns.Length >= MaxSpawns)
						{
							PopupManager.ShowError("Reached max amount of spawns.");
						}
						else
						{
							FileStates.Spawnset.Update(FileStates.Spawnset.Object with { Spawns = FileStates.Spawnset.Object.Spawns.Insert(selectedIndex, new(_selectedEnemyType, _addDelay)) });
							SpawnsetHistoryUtils.Save(SpawnsetEditType.SpawnInsert);
							_scrollToIndex = selectedIndex;
						}
					}
				}

				ImGui.EndChild(); // End AddAndInsertButtons

				ImGui.SameLine();

				if (ImGui.BeginChild("AddSpawnControls"))
				{
					EnemyButton(EnemyType.Squid1, false);
					EnemyButton(EnemyType.Squid2, true);
					EnemyButton(EnemyType.Squid3, true);

					EnemyButton(EnemyType.Spider1, false);
					EnemyButton(EnemyType.Spider2, true);

					EnemyButton(EnemyType.Centipede, false);
					EnemyButton(EnemyType.Gigapede, true);
					EnemyButton(EnemyType.Ghostpede, true);

					EnemyButton(EnemyType.Thorn, false);
					EnemyButton(EnemyType.Leviathan, true);
					EnemyButton(EnemyType.Empty, true);

					ImGui.InputFloat("Delay", ref _addDelay, 1, 2, "%.4f");
					_addDelay = Math.Max(0, _addDelay);
				}

				ImGui.EndChild(); // End AddSpawnControls
			}

			ImGui.EndChild(); // End SpawnControlsChild
		}

		ImGui.End(); // End Spawns
	}

	private static void EnemyButton(EnemyType enemyType, bool sameLine)
	{
		bool isCurrent = _selectedEnemyType == enemyType;
		Engine.Maths.Numerics.Color enemyColor = enemyType.GetColor(GameVersion.V3_2).ToEngineColor().Darken(isCurrent ? 0 : 0.8f);

		ImGui.PushStyleColor(ImGuiCol.Text, isCurrent ? enemyColor.ReadableColorForBrightness() : Engine.Maths.Numerics.Color.Gray(0.5f));
		ImGui.PushStyleColor(ImGuiCol.Button, enemyColor);
		ImGui.PushStyleColor(ImGuiCol.ButtonActive, Engine.Maths.Numerics.Color.Lerp(enemyColor, Engine.Maths.Numerics.Color.White, 0.25f));
		ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Engine.Maths.Numerics.Color.Lerp(enemyColor, Engine.Maths.Numerics.Color.White, 0.125f));
		ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, isCurrent ? 1 : 0);
		ImGui.PushStyleColor(ImGuiCol.Border, enemyColor.ReadableColorForBrightness());

		if (sameLine)
			ImGui.SameLine();

		if (ImGui.Button(Inline.Span(enemyType), new(80, 20)))
			_selectedEnemyType = enemyType;

		ImGui.PopStyleColor(5);
		ImGui.PopStyleVar();
	}

	private static void RenderSpawnsTable()
	{
		const int columnCount = 6;

		ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(4, 1));
		ImGui.PushStyleColor(ImGuiCol.Header, Colors.SpawnsetEditor.Primary with { A = 50 });
		ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.SpawnsetEditor.Primary with { A = 75 });
		ImGui.PushStyleColor(ImGuiCol.HeaderActive, Colors.SpawnsetEditor.Primary with { A = 100 });

		if (ImGui.BeginTable("SpawnsTable", columnCount, ImGuiTableFlags.ScrollY))
		{
			ImGuiIOPtr io = ImGui.GetIO();

			if (_windowIsFocused)
			{
				if (io.KeyCtrl && io.IsKeyDown(ImGuiKey.A))
					Array.Fill(_selected, true);

				if (io.IsKeyDown(ImGuiKey.Delete) && Array.Exists(_selected, b => b))
				{
					FileStates.Spawnset.Update(FileStates.Spawnset.Object with { Spawns = FileStates.Spawnset.Object.Spawns.Where((_, i) => !_selected[i]).ToImmutableArray() });
					Array.Fill(_selected, false);
					SpawnsetHistoryUtils.Save(SpawnsetEditType.SpawnDelete);
				}
			}

			ImGui.TableSetupColumn("#", ImGuiTableColumnFlags.WidthFixed, 24);
			ImGui.TableSetupColumn("Enemy", ImGuiTableColumnFlags.WidthStretch, 72);
			ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.WidthStretch, 72);
			ImGui.TableSetupColumn("Delay", ImGuiTableColumnFlags.WidthStretch, 72);
			ImGui.TableSetupColumn("Gems", ImGuiTableColumnFlags.WidthStretch, 48);
			ImGui.TableSetupColumn("Total", ImGuiTableColumnFlags.WidthStretch, 88);
			ImGui.TableSetupScrollFreeze(0, 1);
			ImGui.TableHeadersRow();

			for (int i = 0; i < columnCount; i++)
			{
				if (!ImGui.TableSetColumnIndex(i))
					continue;

				ImGui.TableHeader(ImGui.TableGetColumnName(i));
				if (!ImGui.IsItemHovered())
					continue;

				string tooltip = i switch
				{
					0 => "The spawn index",
					1 => "The enemy type",
					2 => "The in-game timer value at which the enemy spawns",
					3 => "The amount of time between the previous and current spawn",
					4 => "The amount of gems an enemy drops when killed without farming",
					5 => "Total amount of gems dropped by all spawned enemies at this point without farming",
					_ => string.Empty,
				};
				ImGui.SetTooltip(tooltip);
			}

			EditSpawnContext.BuildFrom(FileStates.Spawnset.Object);
			for (int i = 0; i < EditSpawnContext.Spawns.Count; i++)
			{
				SpawnUiEntry spawn = EditSpawnContext.Spawns[i];
				ImGui.TableNextRow();
				ImGui.TableNextColumn();

				if (_scrollToIndex.HasValue && spawn.Index == _scrollToIndex.Value)
				{
					ImGui.SetScrollHereY();
					_scrollToIndex = null;
				}

				if (ImGui.Selectable(Inline.Span(spawn.Index), ref _selected[spawn.Index], ImGuiSelectableFlags.SpanAllColumns))
				{
					if (!io.KeyCtrl)
					{
						Array.Clear(_selected);
						_selected[spawn.Index] = true;
					}

					if (io.KeyShift && _lastSelectedIndex != -1)
					{
						int start = Math.Clamp(Math.Min(spawn.Index, _lastSelectedIndex), 0, _selected.Length - 1);
						int end = Math.Clamp(Math.Max(spawn.Index, _lastSelectedIndex), 0, _selected.Length - 1);
						for (int j = start; j <= end; j++)
							_selected[j] = true;
					}

					_lastSelectedIndex = spawn.Index;
				}

				EditContextItem(spawn);

				ImGui.TableNextColumn();

				ImGui.TextColored(spawn.EnemyType.GetColor(GameConstants.CurrentVersion), EnumUtils.EnemyTypeNames[spawn.EnemyType]);
				ImGui.TableNextColumn();

				ImGui.Text(Inline.Span(spawn.Seconds, StringFormats.TimeFormat));
				ImGui.TableNextColumn();

				ImGui.Text(Inline.Span(spawn.Delay, StringFormats.TimeFormat));
				ImGui.TableNextColumn();

				ImGui.Text(spawn.NoFarmGems == 0 ? "-" : Inline.Span(spawn.NoFarmGems, "+0"));
				ImGui.TableNextColumn();

				ImGui.TextColored(spawn.GemState.HandLevel.GetColor(), Inline.Span(spawn.GemState.Value));
				ImGui.TableNextColumn();
			}

			ImGui.EndTable();
		}

		ImGui.PopStyleColor(3);
		ImGui.PopStyleVar();
	}

	private static void EditContextItem(SpawnUiEntry spawn)
	{
		bool saved = false;
		if (ImGui.BeginPopupContextItem(Inline.Span(spawn.Index)))
		{
			if (!_delayEdited)
				_editDelay = (float)spawn.Delay;

			ImGui.Text(Inline.Span($"Edit #{spawn.Index} ({EnumUtils.EnemyTypeNames[spawn.EnemyType]} at {spawn.Seconds:0.0000})"));

			for (int i = 0; i < EnumUtils.EnemyTypes.Count; i++)
			{
				EnemyType enemyType = EnumUtils.EnemyTypes[i];
				Color color = enemyType.GetColor(GameConstants.CurrentVersion);
				ImGui.PushStyleColor(ImGuiCol.Text, color.ToEngineColor().ReadableColorForBrightness());
				ImGui.PushStyleColor(ImGuiCol.Button, color);
				ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color + new Vector4(0.3f, 0.3f, 0.3f, 0));
				ImGui.PushStyleColor(ImGuiCol.ButtonActive, color + new Vector4(0.5f, 0.5f, 0.5f, 0));

				if (ImGui.Button(EnumUtils.EnemyTypeNames[enemyType], new(96, 18)))
				{
					SaveEditedSpawn(spawn.Index, enemyType, _editDelay);
					saved = true;
				}

				ImGui.PopStyleColor(4);
			}

			ImGui.InputFloat("Delay", ref _editDelay, 1, 5, "%.4f");
			_editDelay = Math.Max(0, _editDelay);

			if (!saved && Math.Abs(_editDelay - spawn.Delay) > 0.0001f)
				_delayEdited = true;

			if (ImGui.Button("Save", new(128, 20)))
				SaveEditedSpawn(spawn.Index, spawn.EnemyType, _editDelay);

			ImGui.EndPopup();
		}

		static void SaveEditedSpawn(int spawnIndex, EnemyType enemyType, float delay)
		{
			FileStates.Spawnset.Update(FileStates.Spawnset.Object with
			{
				Spawns = FileStates.Spawnset.Object.Spawns.SetItem(spawnIndex, new(enemyType, delay)),
			});

			SpawnsetHistoryUtils.Save(SpawnsetEditType.SpawnEdit);
			ImGui.CloseCurrentPopup();
			_delayEdited = false;
		}
	}

	public static void ClearUnusedSelections()
	{
		for (int i = FileStates.Spawnset.Object.Spawns.Length; i < _selected.Length; i++)
			_selected[i] = false;
	}

	public static void ClearAllSelections()
	{
		Array.Clear(_selected);
	}

	private static float? GetEndLoopLength()
	{
		if (FileStates.Spawnset.Object.GameMode != GameMode.Survival)
			return null;

		(SpawnSectionInfo PreLoopSection, SpawnSectionInfo LoopSection) sections = FileStates.Spawnset.Object.CalculateSections();
		return sections.LoopSection.Length;
	}
}
