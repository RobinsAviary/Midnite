bool launchGUI = false;
bool windowsScreensaver = false;
bool verbose = false;

ProgramClass program = new();
CLI cli = new(program);

cli.ProcessArgs(args);

program.RunProject();