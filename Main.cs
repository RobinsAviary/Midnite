bool launchGUI = false;
bool windowsScreensaver = false;
bool verbose = false;

Midnite.ProgramClass program = new();
Midnite.CLI cli = new(program);

cli.ProcessArgs(args);
program.RunProject();