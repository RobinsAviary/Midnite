using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Text;
using System.Text.Encodings.Web;

class SaverToy
{
    internal class Program
    {
        static public string version = "Alpha";
        static public char commandFlag = '-';

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

            RenderWindow window = new(new(600, 400), "SaverToy");
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

                file.Step();
                file.Draw(window);

                events.ClearKeys();
                events.ClearTypedText();
                window.Display();
            }
        }
    }

    static void Main(string[] args)
    {
        Program program = new();

        bool runProgram = true;

        if (args.Length > 0)
        {
            string commandFlagTwice = Program.commandFlag.ToString() + Program.commandFlag.ToString();

            foreach (string arg in args)
            {
                if (arg.StartsWith(Program.commandFlag + "h") || arg.StartsWith(Program.commandFlag + "?") || arg.StartsWith(commandFlagTwice + "help"))
                {
                    runProgram = false;
                    string helpText = "Usage: SaverToy [FLAGS]" +
                        "\n\nOptions:" +
                        "\n-v, --version  Get the current version of SaverToy." +
                        "\n-a, --author   Get info on the author of SaverToy." +
                        "\n-h, -?, --help Displays this help message.";
                    Console.WriteLine(helpText);
                }
                else if (arg.StartsWith(Program.commandFlag + "v") || arg.StartsWith(commandFlagTwice + "version"))
                {
                    runProgram = false;
                    Console.WriteLine($"SaverToy v{Program.version}");
                }
                else if (arg.StartsWith(commandFlagTwice + "author"))
                {
                    runProgram = false;
                    Console.WriteLine("Written by Robin <3");
                    Console.WriteLine("robinsaviary.com");
                } else
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
