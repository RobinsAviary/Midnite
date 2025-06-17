using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Text;

namespace SaverToy
{
    internal class CodeFile
    {
        public List<string> lines = [];

        public CodeFile()
        {
            lines.Add("");
        }
    }

    internal class SelectionState
    {
        public uint charStart = 0;
        public uint charEnd = 0;
        public uint lineStart = 0;
        public uint lineEnd = 0;
        public bool active = false;

        public SelectionState()
        {

        }
    }

    internal class Program
    {
        static void Window_Closed(object sender, EventArgs e)
        {
            ((WindowBase)sender).Close();
        }

        static void Window_KeyReleased(object sender, KeyEventArgs e)
        {
            Keyboard.Key key = e.Code;
        }

        static void Window_Resized(object sender, SizeEventArgs e)
        {
            FloatRect visible = new(0, 0, e.Width, e.Height);

            ((RenderWindow)sender).SetView(new(visible));
        }

        static void Main(string[] args)
        {
            Color codeBg = new(16, 16, 16);
            Color codeTextFill = new(250, 250, 250);

            CodeFile file = new();
            SelectionState selection = new();

            uint linePointer = 0;
            uint charPointer = 0;

            void Window_TextEntered(object sender, TextEventArgs e)
            {
                if (e.Unicode == "\b")
                {
                    string line = file.lines[(int)linePointer];

                    if (line.Length > 0)
                    {
                        if (charPointer > 0)
                        {
                            if (charPointer > 0) charPointer--;
                            file.lines[(int)linePointer] = line.Remove((int)charPointer, 1);
                        }
                    } else if (file.lines.Count > 1)
                    {
                        file.lines.RemoveAt((int)linePointer);
                        linePointer--;
                    }                        
                }
                else if (e.Unicode.First() == (char)13) // Enter
                {
                    Console.WriteLine("Enter!");
                }
                else if (e.Unicode.First() == (char)1) // Ctrl-A
                {

                }
                else if (e.Unicode.First() == (char)3) // Copy
                {

                }
                else if (e.Unicode.First() == (char)22) // Paste
                {
                    file.lines[(int)linePointer] = file.lines[(int)linePointer].Insert((int)charPointer, Clipboard.Contents);
                    charPointer += (uint)Clipboard.Contents.Length;
                    resetCursorTimer();
                }
                else if (e.Unicode.First() > 31 || e.Unicode.First() == '\t')
                {
                    byte[] textBytes = Encoding.Unicode.GetBytes(e.Unicode);

                    foreach (byte b in textBytes)
                    {
                        Console.WriteLine("{0}", b);
                    }

                    resetCursorTimer();

                    charPointer++;

                    string line = file.lines[(int)linePointer];


                    if (charPointer < line.Length + 1)
                    {
                        file.lines[(int)linePointer] = line.Insert((int)charPointer - 1, e.Unicode);
                    } else
                    {
                        file.lines[(int)linePointer] += e.Unicode;
                    }
                }
            }

            void Window_KeyPressed(object sender, KeyEventArgs e)
            {
                Keyboard.Key key = e.Code;

                if (key == Keyboard.Key.Escape)
                {
                    ((WindowBase)sender).Close();
                }

                if (key == Keyboard.Key.Right)
                {
                    if (charPointer < file.lines[(int)linePointer].Length)
                    {
                        charPointer++;
                    }

                    resetCursorTimer();
                }

                if (key == Keyboard.Key.Left)
                {
                    if (charPointer > 0)
                    {
                        charPointer--;
                    }

                    resetCursorTimer();
                }

                if (key == Keyboard.Key.Up)
                {
                    if (linePointer > 0)
                    {
                        linePointer--;
                        uint lineLength = (uint)file.lines[(int)linePointer].Length;
                        if (charPointer > lineLength)
                        {
                            charPointer = lineLength;
                        }
                    }
                }

                if (key == Keyboard.Key.Down)
                {
                    if (linePointer < file.lines.Count() - 1)
                    {
                        linePointer++;
                        uint lineLength = (uint)file.lines[(int)linePointer].Length;
                        if (charPointer > lineLength)
                        {
                            charPointer = lineLength;
                        }
                    }
                }

                if (key == Keyboard.Key.Enter)
                {
                    file.lines.Insert((int)linePointer + 1, "");
                    linePointer++;
                    charPointer = 0;
                }

                if (key == Keyboard.Key.Delete)
                {
                    string line = file.lines[(int)linePointer];

                    resetCursorTimer();

                    if (charPointer < line.Length)
                    {
                        file.lines[(int)linePointer] = line.Remove((int)charPointer, 1);
                    }
                }
            }

            Clock blinkTimer = new();
            blinkTimer.Restart();
            bool cursorActive = true;

            RenderWindow window = new(new(600, 400), "Test");
            window.Closed += Window_Closed;
            window.KeyPressed += Window_KeyPressed;
            window.KeyReleased += Window_KeyReleased;
            window.Resized += Window_Resized;
            window.TextEntered += Window_TextEntered;

            Font font = new("resources/fonts/JetBrainsMono-Regular.ttf");
            font.SetSmooth(true);
            uint fontSize = 16;

            float getFontWidth()
            {
                Text text = new();
                text.Font = font;
                text.CharacterSize = 16;
                text.DisplayedString = "M";

                return text.GetLocalBounds().Size.X + 1;
            }

            void resetCursorTimer()
            {
                blinkTimer.Restart();
                cursorActive = true;
            }

            while (window.IsOpen)
            {
                window.DispatchEvents();

                if (blinkTimer.ElapsedTime.AsSeconds() > .66)
                {
                    blinkTimer.Restart();
                    cursorActive = !cursorActive;
                }

                Text text = new();
                text.Font = font;
                text.FillColor = codeTextFill;
                text.Position = new(0, 0);
                text.CharacterSize = 16;
                RectangleShape pointerShape = new();
                pointerShape.FillColor = codeTextFill;

                // Used to position cursor
                text.DisplayedString = file.lines[(int)linePointer];
                Text tempText = new(text);
                tempText.DisplayedString = tempText.DisplayedString.Remove((int)charPointer);

                FloatRect bounds = tempText.GetLocalBounds();

                pointerShape.Position = new(bounds.Position.X + bounds.Size.X + 1, linePointer * fontSize);
                pointerShape.Size = new(1, 16);
                pointerShape.Origin = new(0, 0);

                window.Clear(codeBg);
                foreach (string line in file.lines)
                {
                    text.DisplayedString = line;
                    window.Draw(text);
                    text.Position = text.Position + new Vector2f(0, fontSize);
                }
                if (cursorActive) window.Draw(pointerShape);

                window.Display();
            }
        }
    }
}
