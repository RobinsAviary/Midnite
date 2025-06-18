using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Text;
using System.Text.Encodings.Web;

class SaverToy
{
    static internal string enteredText = "";

    static public string EnteredText
    {
        get
        {
            return enteredText;
        }
    }

    static public void ClearEnteredText()
    {
        enteredText = "";
    }

    internal class Program
    {
        

        static void Main(string[] args)
        {
            Textbox file = new();

            WindowEvent events = new();

            RenderWindow window = new(new(600, 400), "Test");
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

                ClearEnteredText();
                window.Display();
            }
        }
    }
}
