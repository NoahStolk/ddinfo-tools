using DevilDaggersInfo.App;

AppDomain.CurrentDomain.UnhandledException += (_, args) => Root.Log.Fatal(args.ExceptionObject.ToString());

Application app = new();

app.Run();
app.Destroy();
