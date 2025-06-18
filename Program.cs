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

            RenderWindow window = new(new(600, 400), "Test");
            window.Closed += WindowEvent.Closed;
            window.KeyPressed += WindowEvent.KeyPressed;
            window.KeyReleased += WindowEvent.KeyReleased;
            window.Resized += WindowEvent.Resized;
            window.TextEntered += WindowEvent.TextEntered;
            window.GainedFocus += WindowEvent.FocusGained;
            window.LostFocus += WindowEvent.FocusLost;
            window.MouseLeft += WindowEvent.MouseLeft;
            window.MouseEntered += WindowEvent.MouseEntered;
            window.MouseButtonPressed += WindowEvent.MouseButtonReleased;

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
