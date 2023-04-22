using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;

namespace DevilDaggersInfo.App.Ui.Base;

public readonly record struct TooltipContext(string Text, Color ForegroundColor, Color BackgroundColor, TextAlign TextAlign);
