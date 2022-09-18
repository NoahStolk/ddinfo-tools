using DevilDaggersInfo.App.Tools.Enums;

namespace DevilDaggersInfo.App.Tools.States;

public record ArenaEditorState(float SelectedHeight, ArenaTool ArenaTool)
{
	public static ArenaEditorState GetDefault()
	{
		return new(0, ArenaTool.Pencil);
	}
}
