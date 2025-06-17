using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace SaverToy
{
    public struct Colors
    {
        static public Color bg = new(16, 16, 16);
        static public Color fg = new(250, 250, 250);

        public Colors()
        {

        }
    }

    internal class Textbox
    {
        internal class SelectionState
        {
            private uint charStart = 0;
            private uint charEnd = 0;
            private uint lineStart = 0;
            private uint lineEnd = 0;
            private bool active = false;

            public uint CharStart
            {
                get
                {
                    return charStart;
                }

                set
                {
                    charStart = value;
                }
            }

            public uint CharEnd
            {
                get
                {
                    return charEnd;
                }

                set
                {
                    charEnd = value;
                }
            }

            public uint LineStart
            {
                get
                {
                    return lineStart;
                }

                set
                {
                    lineStart = value;
                }
            }

            public uint LineEnd
            {
                get
                {
                    return lineEnd;
                }

                set
                {
                    lineEnd = value;
                }
            }

            public bool Active
            {
                get
                {
                    return active;
                }

                set
                {
                    active = value;
                }
            }

            public SelectionState()
            {

            }
        }

        internal class TextCursor
        {
            private float blinkTime = .66f;
            private bool blinkVisible = true;
            private RectangleShape shape = new();

            public Color Color
            {
                get
                {
                    return shape.FillColor;
                }

                set
                {
                    shape.FillColor = value;
                }
            }

            public float BlinkTime
            {
                get
                {
                    return blinkTime;
                }

                set
                {
                    blinkTime = value;
                }
            }

            public void Step()
            {
                if (timer.ElapsedTime.AsSeconds() > 1)
                {
                    blinkVisible = !blinkVisible;
                    timer.Restart();
                }
            }

            public void Draw(RenderTarget target)
            {
                target.Draw(shape);
            }

            readonly private Clock timer = new();
            public TextCursor()
            {
                timer.Restart();

            }
        }

        Text text = new();

        public uint fontSize
        {
            get
            {
                return text.CharacterSize;
            }

            set
            {
                text.CharacterSize = value;
            }
        }

        public Color FontColor
        {
            get
            {
                return text.FillColor;
            }
            set
            {
                text.FillColor = value;
            }
        }

        public List<string> lines = [];
        SelectionState selection = new();
        TextCursor cursor = new();

        private uint charPointer = 0;
        private uint linePointer = 0;

        public void MoveUp()
        {
            if (linePointer > 0)
            {
                linePointer--;
                AlignCursor();
            }
        }

        public void MoveDown()
        {
            if (linePointer < lines.Count())
            {
                linePointer++;
                AlignCursor();
            }
        }

        public void MoveLeft()
        {
            if (charPointer > 0) charPointer--;
        }

        public void MoveRight()
        {
            if (charPointer < CurrentLine.Length) charPointer++;
        }

        public string CurrentLine
        {
            set
            {
                lines[(int)charPointer] = value;
            }
            get
            {
                return lines[(int)charPointer];
            }
        }

        private void AlignCursor()
        {
            if (charPointer >= CurrentLine.Length) charPointer = (uint)CurrentLine.Length;
        }

        public void Step()
        {

        }

        public void Draw(RenderTarget target)
        {
            text.DisplayedString = "";

            foreach (string str in lines)
            {
                text.DisplayedString += str;
            }

            text.FillColor = Color.White;
            text.DisplayedString = "Hello, World!";
            text.CharacterSize = 16;

            target.Draw(text);
        }

        public Textbox()
        {
            lines.Add("");
        }
    }

    internal class Program
    {
        public static class WindowEvent
        {
            public static void Closed(object sender, EventArgs e)
            {
                ((WindowBase)sender).Close();
            }

            public static void KeyReleased(object sender, KeyEventArgs e)
            {
                Keyboard.Key key = e.Code;
            }

            public static void Resized(object sender, SizeEventArgs e)
            {
                FloatRect visible = new(0, 0, e.Width, e.Height);

                ((RenderWindow)sender).SetView(new(visible));
            }

            public static void TextEntered(object sender, TextEventArgs e)
            {
                if (e.Unicode == "\b")
                {

                }
                else if (e.Unicode.First() == (char)13) // Enter
                {

                }
                else if (e.Unicode.First() == (char)1) // Ctrl-A
                {

                }
                else if (e.Unicode.First() == (char)3) // Copy
                {

                }
                else if (e.Unicode.First() == (char)22) // Paste
                {

                }
                else if (e.Unicode.First() > 31 || e.Unicode.First() == '\t')
                {

                }
            }

            public static void KeyPressed(object sender, KeyEventArgs e)
            {
                Keyboard.Key key = e.Code;

                if (key == Keyboard.Key.Escape)
                {
                    ((WindowBase)sender).Close();
                }

                if (key == Keyboard.Key.Right)
                {

                }

                if (key == Keyboard.Key.Left)
                {

                }

                if (key == Keyboard.Key.Up)
                {

                }

                if (key == Keyboard.Key.Down)
                {

                }

                if (key == Keyboard.Key.Enter)
                {

                }

                if (key == Keyboard.Key.Delete)
                {

                }
            }
        }

        static void Main(string[] args)
        {
            Textbox file = new();

            RenderWindow window = new(new(600, 400), "Test");
            window.Closed += WindowEvent.Closed;
            window.KeyPressed += WindowEvent.KeyPressed;
            window.KeyReleased += WindowEvent.KeyReleased;
            window.Resized += WindowEvent.Resized;
            window.TextEntered += WindowEvent.TextEntered;

            Font font = new("resources/fonts/JetBrainsMono-Regular.ttf");
            font.SetSmooth(true);

            while (window.IsOpen)
            {
                window.DispatchEvents();

                RectangleShape shape = new();
                shape.FillColor = Color.White;
                shape.Size = new(100, 100);

                window.Draw(shape);

                file.Step();
                file.Draw(window);

                window.Display();
            }
        }
    }
}
