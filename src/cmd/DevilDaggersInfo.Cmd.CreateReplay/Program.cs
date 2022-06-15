using DevilDaggersInfo.Cmd.CreateReplay;

// File.WriteAllBytes(@"C:\Users\NOAH\AppData\Roaming\DevilDaggers\replays\TEST.ddreplay", new RandomReplayWriter().Write().Compile());
File.WriteAllBytes(@"C:\Users\NOAH\AppData\Roaming\DevilDaggers\replays\skull-2-analysis-done_113.72-xvlv-0be4d3b8[2d05a83a].ddreplay", new Skull2TargetVisualizerReplayWriter().Write().Compile());
