namespace Midnite;

public class CLI
{
    ProgramClass program;

    public CLI(ProgramClass _program)
    {
        program = _program;
    }

    public static char flagPrefix = '-';
    public static char winFlagPrefix = '/';

    public void PrintHelp()
    {
        string helpText = "Usage: Midnite [FLAGS...]" +
                    "\n\nOptions:" +
                    "\n-V, --version  Get the current version of Midnite." +
                    "\n-a, --author   Get info on the author of Midnite." +
                    "\n-h, -?, --help Displays this help message." +
                    "\n-v, --verbose  Print verbose debug text." +
                    "\n-c, --cli      Launch the command line interface (Manipulate projects).";
        Console.WriteLine(helpText);
    }

    public void ProcessArgs(string[] args)
    {
        if (args.Length > 0)
        {
            string commandFlagTwice = CLI.flagPrefix.ToString() + CLI.flagPrefix.ToString();

            foreach (string arg in args)
            {
                if (arg == CLI.flagPrefix + "h" || arg == CLI.flagPrefix + "?" || arg == commandFlagTwice + "help")
                {
                    PrintHelp();
                }
                else if (arg == CLI.flagPrefix + "c" || arg == commandFlagTwice + "cli")
                {
                    Run();
                }
                else if (arg == CLI.flagPrefix + "V" || arg == commandFlagTwice + "version")
                {
                    PrintVersion();
                }
                else if (arg == CLI.flagPrefix + "a" || arg == commandFlagTwice + "author")
                {
                    PrintAuthor();
                }
                else if (arg == CLI.flagPrefix + "v" || arg == commandFlagTwice + "verbose")
                {
                    ToggleVerbose();
                }
                else if (arg == CLI.winFlagPrefix + "s")
                {
                    // Windows is attempting to launch this application as a fullscreen screensaver.
                    program.state = ProgramClass.States.Screensaver;
                    program.windowsMode = true;
                }
                else if (arg == CLI.winFlagPrefix + "c")
                {
                    // Windows is attempting to launch this application's configs.
                }
                else if (arg == CLI.winFlagPrefix + "p")
                {
                    // Windows wants us to display a preview of our screensaver on the provided handle.

                }
                else if (arg == CLI.winFlagPrefix + "d")
                {
                    // Windows is opening this program as debug in Visual Studio.
                }
                else if (arg.StartsWith('-'))
                {
                    Console.WriteLine($"Unknown flag: {arg}");
                    Console.WriteLine("Use -h, -?, or --help to view help for Midnite.");
                }
            }
        }
    }

    public void Run()
    {

    }

    public void PrintVersion()
    {
        Console.WriteLine($"Midnite v{program.version}");
    }

    public void PrintAuthor()
    {
        Console.WriteLine("Written by Robin <3");
        Console.WriteLine("robinsaviary.com");
    }

    public void ToggleVerbose()
    {
        program.verbose = !program.verbose;
        Console.WriteLine($"Verbose output {program.BoolToString(program.verbose, "OFF", "ON")}.");
    }
}