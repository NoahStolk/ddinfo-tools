using DevilDaggersInfo.App.Tools.Spawns;
using DevilDaggersInfo.App.Tools.States;
using Silk.NET.OpenGL;
using Warp;
using Warp.Extensions;
using Warp.Ui;
using Warp.Ui.Components;

namespace DevilDaggersInfo.App.Tools.Components.SpawnsetSpawns;

public class Spawns : AbstractComponent
{
	private const int _scrollMultiplier = 64;
	public const int SpawnEntryHeight = 24;

	private readonly SpawnsWrapper _spawnsWrapper;
	private readonly List<AbstractComponent> _spawnComponents = new();

	public Spawns(Rectangle metric, SpawnsWrapper spawnsWrapper)
		: base(metric)
	{
		_spawnsWrapper = spawnsWrapper;
	}

	public override void Update(Vector2i<int> parentPosition)
	{
		base.Update(parentPosition);

		bool hoverWithoutBlock = Metric.Contains(MouseUiContext.MousePosition.RoundToVector2Int32() - parentPosition);
		if (!hoverWithoutBlock)
			return;

		int scroll = Input.GetScroll();
		if (scroll != 0)
			_spawnsWrapper.SetScroll(scroll * _scrollMultiplier);
	}

	public void SetScrollOffset(Vector2i<int> scrollOffset)
	{
		NestingContext.ScrollOffset = Vector2i<int>.Clamp(scrollOffset, new(0, -_spawnComponents.Count * SpawnEntryHeight + Metric.Size.Y), default);
	}

	public void SetSpawnset()
	{
		foreach (AbstractComponent component in _spawnComponents)
			NestingContext.Remove(component);

		_spawnComponents.Clear();

		NestingContext.ScrollOffset = default;

		int i = 0;
		foreach (EditableSpawn spawn in EditSpawnContext.GetFrom(StateManager.SpawnsetState.Spawnset))
		{
			SpawnEntry spawnEntry = new(Rectangle.At(0, i++ * SpawnEntryHeight, 512, SpawnEntryHeight), spawn);
			_spawnComponents.Add(spawnEntry);
		}

		foreach (AbstractComponent component in _spawnComponents)
			NestingContext.Add(component);
	}

	public override void Render(Vector2i<int> parentPosition)
	{
		Gl.Enable(EnableCap.ScissorTest);
		SetScissor(parentPosition);

		Base.Game.UiRenderer.RenderTopLeft(Metric.Size, parentPosition + new Vector2i<int>(Metric.X1, Metric.Y1), Depth, Color.Black);

		base.Render(parentPosition);

		Gl.Disable(EnableCap.ScissorTest);
	}

	public override void RenderText(Vector2i<int> parentPosition)
	{
		Gl.Enable(EnableCap.ScissorTest);
		SetScissor(parentPosition);

		base.RenderText(parentPosition);

		Gl.Disable(EnableCap.ScissorTest);
	}

	private void SetScissor(Vector2i<int> parentPosition)
	{
		Vector2 viewportOffset = Base.Game.ViewportOffset;
		Gl.Scissor(Metric.X1 + (int)viewportOffset.X + parentPosition.X, Base.Game.InitialWindowHeight - (Metric.Size.Y + parentPosition.Y) + (int)viewportOffset.Y, (uint)Metric.Size.X, (uint)Metric.Size.Y);
	}
}
