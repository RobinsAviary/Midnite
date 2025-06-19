using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Text;
using System.Text.Encodings.Web;

class SaverToy
{
    internal class Program
    {
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

        program.Run();
    }
}
