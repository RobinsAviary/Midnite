using MoonSharp.Interpreter;
using SFML.Graphics;
using SFML.Window;
using MonitorDetails;
using MonitorDetails.Models;

class SaverToy
{
    internal class Program
    {
        private bool fullscreen = false;

        public bool Fullscreen
        {
            get
            {
                return fullscreen;
            }
            set
            {
                if (value == true)
                {
                    Reader reader = new();

                    IEnumerable<MonitorDetails.Models.Monitor> monitors = reader.GetMonitorDetails();

                    // Get info on monitors
                    foreach (MonitorDetails.Models.Monitor monitor in monitors)
                    {
                        Console.WriteLine(monitor.MonitorCoordinates.Y);
                    }

                    VideoMode videoMode = VideoMode.DesktopMode;

                    Window.Close();
                    if (Window != null)
                    {
                        
                    }
                    Window = new(new(videoMode.Width, videoMode.Height), $"SaverToy v{Program.Version}", Styles.None);
                }
                else if (value == false)
                {
                    if (Window != null)
                    {
                        Window.Close();
                    }
                    Window = new(new(600, 400), $"SaverToy v{Program.Version}");
                }

                fullscreen = value;
            }
        }

        internal class Directories
        {
            public string Resources = "resources//";
            public string User {
                get
                {
                    return Resources + "user//";
                }
            }

            public string Fonts
            {
                get
                {
                    return Resources + "fonts//";
                }
            }
            public string Screens
            {
                get
                {
                    return User + "screens//";
                }
            }

            private string project = "testscreen//";

            public string Project
            {
                get
                {
                    return Screens + project;
                }
                set
                {
                    project = value;
                }
            }
        }

        public Directories directories = new();

        public string BoolToString(bool value, string falseyText = "FALSE", string trueyText = "TRUE")
        {
            if (value)
            {
                return trueyText;
            }
            else

                return falseyText;
        }

        public Color DynValueToColor(DynValue T)
        {
            if (T.Type == DataType.Table)
            {
                Table t = T.Table;

                DynValue defaultVal = DynValue.NewNumber(0);

                DynValue r = t.Get("r");
                if (r.Type == DataType.Nil)
                {
                    r = defaultVal;
                }
                DynValue g = t.Get("g");
                if (g.Type == DataType.Nil)
                {
                    g = defaultVal;
                }
                DynValue b = t.Get("b");
                if (b.Type == DataType.Nil)
                {
                    b = defaultVal;
                }
                DynValue a = t.Get("a");

                if (r.Type == DataType.Number && g.Type == DataType.Number && b.Type == DataType.Number)
                {
                    if (a.Type != DataType.Number)
                    {
                        a = DynValue.NewNumber(255);
                    }

                    return new((byte)r.Number, (byte)g.Number, (byte)b.Number, (byte)a.Number);
                }
            }

            return Color.Transparent;
        }

        public void ToggleVerbose()
        {
            Verbose = !Verbose;
            Console.WriteLine($"Verbose output {BoolToString(Verbose, "OFF", "ON")}.");
        }

        internal class CLIFunctions {
            public void PrintHelp()
            {
                string helpText = "Usage: SaverToy [FLAGS...]" +
                        "\n\nOptions:" +
                        "\n-V, --version  Get the current version of SaverToy." +
                        "\n-a, --author   Get info on the author of SaverToy." +
                        "\n-h, -?, --help Displays this help message." +
                        "\n-v, --verbose  Print verbose debug text.";
                Console.WriteLine(helpText);
            }

            public void PrintAuthor()
            {
                Console.WriteLine("Written by Robin <3");
                Console.WriteLine("robinsaviary.com");
            }

            public void PrintVersion()
            {
                Console.WriteLine($"SaverToy v{Program.Version}");
            }
        }

        public CLIFunctions CLI = new();

        static public string Version = "Alpha";
        static public char flagPrefix = '-';
        static public char winFlagPrefix = '/';
        public bool Verbose = false;
        public bool WinScreen = false;
        public Script scr = new();
        RenderWindow? Window;
        RenderTarget? Target;

        void Line(DynValue startPos, DynValue endPos, DynValue color)
        {
            if (startPos.Type == DataType.Table)
            {
                Table t = startPos.Table;

                DynValue x = t.Get("x");
                Console.WriteLine($"{x}");

                //if ()
            }
        }

        void ClearScreen(DynValue color)
        {
            Color _color = DynValueToColor(color);

            Target.Clear(_color);
        }

        public enum States
        {
            TextEditor,
            Screensaver,
        }

        private States _state = States.TextEditor;

        public States State
        {
            get
            {
                return _state;
            }
            set
            {
                switch(value)
                {
                    case States.Screensaver:
                        //Script scr = new Script(); // Create script
                        scr.Options.DebugPrint = s => { Console.WriteLine(s); }; // Set up debug
                        // Include custom libraries
                        //scr.DoFile(libraryDir + "rc10.lua");

                        // Define in-built functions
                        scr.Globals["Line"] = (Action<DynValue,DynValue,DynValue>)Line;
                        scr.Globals["ClearScreen"] = (Action<DynValue>)ClearScreen;
                        scr.DoFile(directories.Project + "main.lua");
                        scr.Call(scr.Globals["init"]);
                        break;

                    case States.TextEditor:

                        break;
                }

                _state = value;
            }
        }

        WindowEvent events = new();

        public bool IsKeyPressed(Keyboard.Key key)
        {
            return events.PressedKeys.Contains(key);
        }

        public bool IsKeyReleased(Keyboard.Key key)
        {
            return events.ReleasedKeys.Contains(key);
        }

        public string TypedText
        {
            get
            {
                return events.TypedText;
            }
        }

        public void Run()
        {
            Textbox file = new(this);

            Target = Window;
            Window.Closed += events.Closed;
            Window.KeyPressed += events.KeyPressed;
            Window.KeyReleased += events.KeyReleased;
            Window.Resized += events.Resized;
            Window.TextEntered += events.TextEntered;
            Window.GainedFocus += events.FocusGained;
            Window.LostFocus += events.FocusLost;
            Window.MouseLeft += events.MouseLeft;
            Window.MouseEntered += events.MouseEntered;
            Window.MouseButtonPressed += events.MouseButtonPressed;
            Window.MouseButtonReleased += events.MouseButtonReleased;

            Font font = new(directories.Fonts + "JetBrainsMono-Regular.ttf");
            font.SetSmooth(true);

            file.Font = font;

            while (Window.IsOpen)
            {
                Window.DispatchEvents();

                if (Keyboard.IsKeyPressed(Keyboard.Key.Escape) || Keyboard.IsKeyPressed(Keyboard.Key.Space))
                {
                    Window.Close();
                }

                if (IsKeyPressed(Keyboard.Key.F11))
                {
                    Console.WriteLine("Test");
                    Fullscreen = !Fullscreen;
                }

                Window.Clear(Color.Black);
                switch (State)
                {
                    case States.TextEditor:
                        file.Step();
                        file.Draw(Window);
                    break;

                    case States.Screensaver:
                        scr.Call(scr.Globals["update"]);
                        break;
                }
                events.ClearKeys();
                events.ClearTypedText();
                Window.Display();
            }
        }
    }

    static void Main(string[] args)
    {
        Program program = new();
        program.State = Program.States.Screensaver;

        bool runProgram = true;
        bool launchWindowed = true;

        if (args.Length > 0)
        {
            string commandFlagTwice = Program.flagPrefix.ToString() + Program.flagPrefix.ToString();

            foreach (string arg in args)
            {
                if (arg == Program.flagPrefix + "h" || arg == Program.flagPrefix + "?" || arg == commandFlagTwice + "help")
                {
                    runProgram = false;
                    program.CLI.PrintHelp();
                }
                else if (arg == Program.flagPrefix + "V" || arg == commandFlagTwice + "version")
                {
                    runProgram = false;
                    program.CLI.PrintVersion();
                }
                else if (arg == Program.flagPrefix + "a" || arg == commandFlagTwice + "author")
                {
                    runProgram = false;
                    program.CLI.PrintAuthor();
                }
                else if (arg == Program.flagPrefix + "v" || arg == commandFlagTwice + "verbose")
                {
                    program.ToggleVerbose();
                }
                else if (arg == Program.winFlagPrefix + "s")
                {
                    // Windows is attempting to launch this application as a fullscreen screensaver.
                    program.State = Program.States.Screensaver;
                    program.WinScreen = true;
                    launchWindowed = false;
                }
                else if (arg == Program.winFlagPrefix + "c")
                {
                    // Windows is attempting to launch this application's configs.
                }
                else if (arg == Program.winFlagPrefix + "p")
                {
                    // Windows wants us to display a preview of our screensaver on the provided handle.
                    runProgram = false;
                }
                else if (arg == Program.winFlagPrefix + "d")
                {
                    // Windows is opening this program as debug in Visual Studio.
                }
                else if (arg.StartsWith('-'))
                {
                    Console.WriteLine($"Unknown flag: {arg}");
                    Console.WriteLine("Use -h, -?, or --help to view help for SaverToy.");
                    runProgram = false;
                }
            }
        }

        if (runProgram)
        {
            program.Fullscreen = !launchWindowed;
            program.Run();
        }
    }
}
