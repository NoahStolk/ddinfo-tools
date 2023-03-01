using DevilDaggersInfo.App.Ui.Base.DependencyPattern;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.Base.States;

public record LayoutState(IExtendedLayout? CurrentLayout)
{
	public static LayoutState GetDefault()
	{
		return new((IExtendedLayout?)null);
	}
}
