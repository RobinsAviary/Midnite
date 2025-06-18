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

        public uint FontSize
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

        public Font Font
        {
            get
            {
                return text.Font;
            }
            set
            {
                text.Font = value;
            }
        }

        List<string> lines = [];
        SelectionState selection = new();
        TextCursor cursor = new();

        private uint charPointer = 0;
        private uint linePointer = 0;

        List<string> Lines
        {
            get
            {
                return lines;
            }
        }

        void AddLine(string text = "")
        {
            lines.Add(text);
        }

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
                //Console.WriteLine(str);
                text.DisplayedString += str;
            }

            target.Clear(Colors.bg);

            target.Draw(text);
        }

        public Textbox()
        {
            AddLine("");
            text.CharacterSize = 16;
            text.FillColor = Color.White;
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
                   enteredText += e.Unicode;
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

            public static void FocusLost(object sender, EventArgs e)
            {

            }

            public static void FocusGained(object sender, EventArgs e)
            {

            }

            public static void MouseEntered(object sender, EventArgs e)
            {

            }

            public static void MouseLeft(object sender, EventArgs e)
            {

            }

            public static void MouseButtonPressed(object sender, MouseButtonEventArgs e)
            {

            }

            public static void MouseButtonReleased(object sender, MouseButtonEventArgs e)
            {

            }
        }

        static private string enteredText = "";

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

                file.CurrentLine += EnteredText;
                ClearEnteredText();

                file.Step();
                file.Draw(window);

                window.Display();
            }
        }
    }
}
