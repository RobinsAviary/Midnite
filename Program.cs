using MoonSharp.Interpreter;
using SFML.Graphics;
using SFML.Window;

class SaverToy
{
    internal class Program
    {
        static public string Version = "Alpha";
        static public char commandFlag = '-';
        public bool Verbose = false;
        RenderWindow? Window;

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
                        Script scr = new Script(); // Create script
                        scr.Options.DebugPrint = s => { Console.WriteLine(s); }; // Set up debug
                        // Include custom libraries
                        //scr.DoFile(libraryDir + "rc10.lua");

                        // Define in-built functions
                        scr.Globals["Line"] = (Action<int>)Horizontal;
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

            RenderWindow window = new(new(600, 400), $"SaverToy v{Program.Version}");
            window.Closed += events.Closed;
            window.KeyPressed += events.KeyPressed;
            window.KeyReleased += events.KeyReleased;
            window.Resized += events.Resized;
            window.TextEntered += events.TextEntered;
            window.GainedFocus += events.FocusGained;
            window.LostFocus += events.FocusLost;
            window.MouseLeft += events.MouseLeft;
            window.MouseEntered += events.MouseEntered;
            window.MouseButtonPressed += events.MouseButtonPressed;
            window.MouseButtonReleased += events.MouseButtonReleased;

            Font font = new("resources/fonts/JetBrainsMono-Regular.ttf");
            font.SetSmooth(true);

            file.Font = font;

            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear(Color.Black);
                switch (State)
                {
                    case States.TextEditor:
                        file.Step();
                        file.Draw(window);
                    break;

                    case States.Screensaver:

                    break;
                }
                events.ClearKeys();
                events.ClearTypedText();
                window.Display();
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
                    string helpText = "Usage: SaverToy [FLAGS...]" +
                        "\n\nOptions:" +
                        "\n-V, --version  Get the current version of SaverToy." +
                        "\n-a, --author   Get info on the author of SaverToy." +
                        "\n-h, -?, --help Displays this help message." +
                        "\n-v, --verbose  Print verbose debug text.";
                    Console.WriteLine(helpText);
                }
                else if (arg == Program.commandFlag + "V" || arg == commandFlagTwice + "version")
                {
                    runProgram = false;
                    Console.WriteLine($"SaverToy v{Program.Version}");
                }
                else if (arg == Program.commandFlag + "a" || arg == commandFlagTwice + "author")
                {
                    runProgram = false;
                    Console.WriteLine("Written by Robin <3");
                    Console.WriteLine("robinsaviary.com");
                }
                else if (arg == Program.commandFlag + "v" || arg == commandFlagTwice + "verbose")
                {
                    program.Verbose = true;
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
