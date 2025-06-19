using SFML.Graphics;
using SFML.System;
using SFML.Window;
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
        public int OffsetX = 1;

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

        public void ResetBlinking()
        {
            blinkVisible = true;
            timer.Restart();
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
            if (timer.ElapsedTime.AsSeconds() > blinkTime)
            {
                blinkVisible = !blinkVisible;
                timer.Restart();
            }
        }

        public void Draw(RenderTarget target)
        {
            if (blinkVisible)
            {
                Text text = new();

                text.DisplayedString = textbox.CurrentLine.Remove((int)textbox.charPointer);
                text.Font = textbox.Font;
                text.CharacterSize = textbox.FontSize;
                text.FillColor = textbox.FontColor;
                uint width = (uint)text.GetLocalBounds().Width;

                Text textHeight = new(text);

                //uint numberOfLines = (uint)textbox.Lines.Count();
                textHeight.DisplayedString = "";

                for (uint i = 0; i < textbox.linePointer + 1; i++)
                {
                    if (i != 0)
                    {
                        textHeight.DisplayedString += "\n";
                    }
                    textHeight.DisplayedString += "A";
                }

                uint height = (uint)textHeight.GetGlobalBounds().Height;

                shape.Origin = new(0, textbox.FontSize * .6875f);

                shape.Position = new(width + OffsetX, height);

                target.Draw(shape);
            }
        }

        readonly private Clock timer = new();
        public TextCursor(Textbox _textbox)
        {
            timer.Restart();
            textbox = _textbox;
        }
    }

    Program program;

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

    public List<string> Lines
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
        cursor.ResetBlinking();
    }

    public void MoveDown()
    {
        if (linePointer < lines.Count() - 1)
        {
            linePointer++;
            AlignCursor();
        }
        cursor.ResetBlinking();
    }

    public void MoveLeft(uint dist = 1)
    {
        if (charPointer > 0) charPointer -= dist;
        else if (linePointer > 0)
        {
            linePointer--;
            charPointer = (uint)CurrentLine.Length;
        }
        cursor.ResetBlinking();
    }

    public void MoveRight(uint dist = 1)
    {
        if (charPointer < CurrentLine.Length) charPointer += dist;
        else if (linePointer < lines.Count() - 1)
        {
            linePointer++;
            charPointer = 0;
        }
        cursor.ResetBlinking();
    }

    public string CurrentLine
    {
        set
        {
            lines[(int)linePointer] = value;
        }
        get
        {
            return lines[(int)linePointer];
        }
    }

    private void AlignCursor()
    {
        if (charPointer >= CurrentLine.Length) charPointer = (uint)CurrentLine.Length;
    }

    public void InsertHere(string str)
    {
        if (CurrentLine.Length < 1)
        {
            CurrentLine = str;
        }
        else
        {
            CurrentLine = CurrentLine.Insert((int)charPointer, str);
        }

        MoveRight((uint)str.Length);
        cursor.ResetBlinking();
    }

    public void Step()
    {
        if (program.IsKeyPressed(Keyboard.Key.Left)) MoveLeft();
        if (program.IsKeyPressed(Keyboard.Key.Right)) MoveRight();
        if (program.IsKeyPressed(Keyboard.Key.Up)) MoveUp();
        if (program.IsKeyPressed(Keyboard.Key.Down)) MoveDown();
        if (program.IsKeyPressed(Keyboard.Key.Delete))
        {
            if (charPointer < CurrentLine.Length)
            {
                if (CurrentLine.Length > 0)
                {
                    CurrentLine = CurrentLine.Remove((int)charPointer, 1);
                }
            }
            else if (linePointer < lines.Count - 1)
            {
                CurrentLine += lines[(int)linePointer + 1];
                Lines.RemoveAt((int)linePointer + 1);
            }

            cursor.ResetBlinking();
        }
        if (program.IsKeyPressed(Keyboard.Key.Home))
        {
            charPointer = 0;
            cursor.ResetBlinking();
        }

        if (program.IsKeyPressed(Keyboard.Key.End))
        {
            charPointer = (uint)CurrentLine.Length;
            cursor.ResetBlinking();
        }
        if (program.IsKeyPressed(Keyboard.Key.Enter))
        {
            if (charPointer == 0)
            {
                lines.Insert((int)linePointer, "");
            } else
            {
                lines.Insert((int)linePointer + 1, CurrentLine.Substring((int)charPointer));
                CurrentLine = CurrentLine.Remove((int)charPointer);
                charPointer = 0;
            }
            
            //lines.Add("");
            linePointer++;
            AlignCursor();
            cursor.ResetBlinking();
        }

        foreach (char c in program.TypedText)
        {
            if ((char)c == 1) // Ctrl+A
            {

            }
            else if ((char)c == 24) // Cut
            {

            }
            else if ((char)c == 3) // Copy
            {

            }
            else if ((char)c == 22) // Paste
            {
                InsertHere(Clipboard.Contents);
            }
            else if ((char)c == '\b')
            {
                if (CurrentLine.Length > 0 && charPointer > 0)
                {
                    CurrentLine = CurrentLine.Remove((int)charPointer - 1, 1);
                    MoveLeft();
                }

                if (linePointer > 0 && charPointer == 0)
                {
                    cursor.ResetBlinking();
                    charPointer = (uint)lines[(int)linePointer - 1].Length;

                    Lines[(int)linePointer - 1] += CurrentLine;
                    Lines.RemoveAt((int)linePointer);
                    linePointer--;
                }
            }
            else if ((char)c > 31 || c == '\t')
            {
                InsertHere(c.ToString());
            }
        }

        cursor.Step();
    }

    public void Draw(RenderTarget target)
    {
        text.DisplayedString = "";

        foreach (string str in lines)
        {
            text.DisplayedString += str + "\n";
        }

        target.Clear(Colors.bg);

        target.Draw(text);

        cursor.Draw(target);
    }

    public Textbox(Program _program)
    {
        AddLine("");
        text.FillColor = Color.White;
        cursor = new(this);
        FontSize = 16;
        program = _program;
    }
}
