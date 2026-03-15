using DevilDaggersInfo.Tools;
using StrongInject;

using Container container = new();
using Owned<Application> app = container.Resolve();
app.Value.Run();
