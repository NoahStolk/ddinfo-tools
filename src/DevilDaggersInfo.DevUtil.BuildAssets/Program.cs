using DevilDaggersInfo.Tools.Engine.Content.Conversion;

if (args.Length < 2)
{
	Console.WriteLine("Please provide the input root directory path and the output file path as arguments.");
	Environment.Exit(1);
	return;
}

ContentFileWriter.GenerateContentFile(args[0], args[1]);
