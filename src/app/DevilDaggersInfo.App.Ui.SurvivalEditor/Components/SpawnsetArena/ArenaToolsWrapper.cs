using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Rendering.Text;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena.SettingsWrappers;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena;

public class ArenaToolsWrapper : AbstractComponent
{
	private const int _arenaButtonSize = 20;

	private static readonly ButtonStyle _activeToolButtonStyle = new(Color.Blue, Color.White, Color.Blue, 1);

	private readonly Dictionary<ArenaTool, TooltipIconButton> _toolButtons = new();
	private readonly Dictionary<ArenaTool, AbstractComponent> _toolSettingsWrappers = new();

	public ArenaToolsWrapper(IBounds bounds)
		: base(bounds)
	{
		AddToolButton(_arenaButtonSize * 0, 0, ArenaTool.Pencil, Textures.Pencil, "Pencil");
		AddToolButton(_arenaButtonSize * 1, 0, ArenaTool.Line, Textures.Line, "Line");
		AddToolButton(_arenaButtonSize * 2, 0, ArenaTool.Rectangle, Textures.Rectangle, "Rectangle");
		AddToolButton(_arenaButtonSize * 3, 0, ArenaTool.Ellipse, Textures.Ellipse, "Ellipse");
		AddToolButton(_arenaButtonSize * 4, 0, ArenaTool.Bucket, Textures.Bucket, "Bucket");
		AddToolButton(_arenaButtonSize * 5, 0, ArenaTool.Dagger, Textures.Dagger, "Race dagger");

		_toolSettingsWrappers.Add(ArenaTool.Pencil, new PencilToolSettingsWrapper(bounds.CreateNested(0, _arenaButtonSize, bounds.Size.X, 64)));
		_toolSettingsWrappers.Add(ArenaTool.Line, new LineToolSettingsWrapper(bounds.CreateNested(0, _arenaButtonSize, bounds.Size.X, 64)));
		_toolSettingsWrappers.Add(ArenaTool.Rectangle, new RectangleToolSettingsWrapper(bounds.CreateNested(0, _arenaButtonSize, bounds.Size.X, 64)));
		_toolSettingsWrappers.Add(ArenaTool.Ellipse, new EllipseToolSettingsWrapper(bounds.CreateNested(0, _arenaButtonSize, bounds.Size.X, 64)));
		_toolSettingsWrappers.Add(ArenaTool.Bucket, new BucketToolSettingsWrapper(bounds.CreateNested(0, _arenaButtonSize, bounds.Size.X, 64)));
		_toolSettingsWrappers.Add(ArenaTool.Dagger, new DaggerToolSettingsWrapper(bounds.CreateNested(0, _arenaButtonSize, bounds.Size.X, 64)));

		foreach (KeyValuePair<ArenaTool, TooltipIconButton> kvp in _toolButtons)
			NestingContext.Add(kvp.Value);

		foreach (KeyValuePair<ArenaTool, AbstractComponent> kvp in _toolSettingsWrappers)
			NestingContext.Add(kvp.Value);

		TextButton button3d = new(bounds.CreateNested(bounds.Size.X - 96, 0, 96, 20), () => StateManager.Dispatch(new SetLayout(Root.Dependencies.SurvivalEditor3dLayout)), ButtonStyles.Default, new(Color.White, TextAlign.Middle, FontSize.H12), "Open 3D editor");
		NestingContext.Add(button3d);

		UpdateActiveButtonAndSettings();

		StateManager.Subscribe<SetArenaTool>(UpdateActiveButtonAndSettings);

		void AddToolButton(int offsetX, int offsetY, ArenaTool arenaTool, Texture texture, string tooltipText)
		{
			TooltipIconButton button = new(Bounds.CreateNested(offsetX, offsetY, _arenaButtonSize, _arenaButtonSize), () => StateManager.Dispatch(new SetArenaTool(arenaTool)), ButtonStyles.Default, texture, tooltipText, TextAlign.Left, Color.HalfTransparentWhite, Color.White);
			_toolButtons.Add(arenaTool, button);
		}
	}

	private void UpdateActiveButtonAndSettings()
	{
		foreach (KeyValuePair<ArenaTool, TooltipIconButton> kvp in _toolButtons)
			kvp.Value.ButtonStyle = StateManager.ArenaEditorState.ArenaTool == kvp.Key ? _activeToolButtonStyle : ButtonStyles.Default;

		foreach (KeyValuePair<ArenaTool, AbstractComponent> kvp in _toolSettingsWrappers)
			kvp.Value.IsActive = StateManager.ArenaEditorState.ArenaTool == kvp.Key;
	}
}
