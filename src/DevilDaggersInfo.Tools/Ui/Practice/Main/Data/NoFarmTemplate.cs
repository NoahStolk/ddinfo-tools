using DevilDaggersInfo.Core.Spawnset;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main.Data;

internal readonly record struct NoFarmTemplate(string Name, Vector4 Color, HandLevel HandLevel, int AdditionalGems, float TimerStart);
