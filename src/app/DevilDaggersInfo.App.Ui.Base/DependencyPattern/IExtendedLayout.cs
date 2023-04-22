using DevilDaggersInfo.App.Engine.Ui;

namespace DevilDaggersInfo.App.Ui.Base.DependencyPattern;

public interface IExtendedLayout : ILayout
{
	void Update();

	void Render3d();

	void Render();
}
