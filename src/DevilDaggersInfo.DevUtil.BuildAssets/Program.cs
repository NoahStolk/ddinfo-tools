using DevilDaggersInfo.Tools.Engine.Content.Conversion;

if (args.Length < 2)
{
	Console.WriteLine("Please provide the input root directory path and the output file path as arguments.");
	Environment.Exit(1);
	return;
}

Console.WriteLine($"Generating content file from directory '{args[0]}'. Outputting to file '{args[1]}'...");
ContentFileWriter.GenerateContentFile(args[0], args[1]);
