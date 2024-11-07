using DevilDaggersInfo.Tools;
using DevilDaggersInfo.Tools.User.Cache;
using DevilDaggersInfo.Tools.User.Settings;
using StrongInject;

AppDomain.CurrentDomain.UnhandledException += (_, args) => Root.Log.Fatal(args.ExceptionObject.ToString());

UserSettings.Load();
UserCache.Load();

// TODO: Fix this:
// Always invoke this in case of incorrect cache.
// Graphics.Glfw.GetWindowSize(Graphics.Window, out int width, out int height);
// Graphics.OnChangeWindowSize.Invoke(width, height);

using Container container = new();
using Owned<Application> app = container.Resolve();
app.Value.Run();
