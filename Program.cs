using System;
using SFML;

namespace window_core
{
    internal static class Program
    {
        private static void Main()
        {
            var window = new Window();
            window.Run();
        }
    }

    internal class Window
    {
        public void Run()
        {
            var mode = new SFML.Window.VideoMode(800, 600);
            var window = new SFML.Graphics.RenderWindow(mode, "SFML works!");
            window.KeyPressed += Window_KeyPressed;
            window.Closed += Window_Close;

            // Start the game loop
            while (window.IsOpen)
            {
                window.DispatchEvents();



                window.Display();
            }
        }

        private void Window_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            var window = (SFML.Window.Window)sender;
            if (e.Code == SFML.Window.Keyboard.Key.Escape)
            {
                window.Close();
            }
        }

        private void Window_Close(object sender, EventArgs e)
        {
            var window = (SFML.Window.Window)sender;
            window.Close();
        }
    }
}