using DevilDaggersInfo.Tools;
using DevilDaggersInfo.Tools.User.Cache;
using DevilDaggersInfo.Tools.User.Settings;
using StrongInject;

AppDomain.CurrentDomain.UnhandledException += (_, args) => Root.Log.Fatal(args.ExceptionObject.ToString());

UserSettings.Load();
UserCache.Load();

using Container container = new();
using Owned<Application> app = container.Resolve();
app.Value.Run();
