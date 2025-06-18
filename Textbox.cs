using SFML.Graphics;
using SFML.System;

using static SaverToy;

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

        private Textbox textbox;

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

        public Vector2f Size
        {
            get
            {
                return shape.Size;
            }
            set
            {
                shape.Size = value;
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
            //text.DisplayedString = 
            //shape.Position = new(, 0);

            target.Draw(shape);
        }

        readonly private Clock timer = new();
        public TextCursor(Textbox _textbox)
        {
            timer.Restart();
            textbox = _textbox;
        }
    }

    static private Text text = new();

    public uint FontSize
    {
        get
        {
            return text.CharacterSize;
        }

        set
        {
            text.CharacterSize = value;

            cursor.Size = new(1, value);
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
    TextCursor cursor;

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
        foreach (char c in EnteredText)
        {
            if ((char)c == '\b')
            {
                if (CurrentLine.Length > 0)
                {
                    CurrentLine = CurrentLine.Remove(CurrentLine.Length - 1, 1);
                }
            }
            else if ((char)c > 31 || c == '\t')
            {
                CurrentLine += c;
            }
        }

        cursor.Step();
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

        cursor.Draw(target);
    }

    public Textbox()
    {
        AddLine("");
        text.FillColor = Color.White;
        cursor = new(this);
        FontSize = 16;
    }
}
