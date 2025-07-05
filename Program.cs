using MoonSharp.Interpreter;
using SFML.Graphics;
using SFML.Window;

class SaverToy
{
    internal class Program
    {
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
        static public char commandFlag = '-';
        public bool Verbose = false;
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
                        scr.DoFile("screens//testscreen//main.lua");
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

            Window = new(new(600, 400), $"SaverToy v{Program.Version}");
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

            Font font = new("resources/fonts/JetBrainsMono-Regular.ttf");
            font.SetSmooth(true);

            file.Font = font;

            while (Window.IsOpen)
            {
                Window.DispatchEvents();
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

        if (args.Length > 0)
        {
            string commandFlagTwice = Program.commandFlag.ToString() + Program.commandFlag.ToString();

            foreach (string arg in args)
            {
                if (arg == Program.commandFlag + "h" || arg == Program.commandFlag + "?" || arg == commandFlagTwice + "help")
                {
                    runProgram = false;
                    program.CLI.PrintHelp();
                }
                else if (arg == Program.commandFlag + "V" || arg == commandFlagTwice + "version")
                {
                    runProgram = false;
                    program.CLI.PrintVersion();
                }
                else if (arg == Program.commandFlag + "a" || arg == commandFlagTwice + "author")
                {
                    runProgram = false;
                    program.CLI.PrintAuthor();
                }
                else if (arg == Program.commandFlag + "v" || arg == commandFlagTwice + "verbose")
                {
                    program.ToggleVerbose();
                }
                else if (arg.StartsWith('-'))
                {
                    Console.WriteLine($"Unknown flag: {arg}");
                    Console.WriteLine("Use -h, -?, or --help to view help for SaverToy.");
                    runProgram = false;
                }
            }
        }

        if (runProgram) program.Run();
    }
}
