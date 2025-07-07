using MoonSharp.Interpreter;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

class Midnite
{
    internal class Program
    {
        Clock timer = new();

        private uint framerateLimit = 0;

        public Dictionary<string, Texture> textures = new();
        public Dictionary<string, Sound> sounds = new();

        public uint FramerateLimit
        {
            get
            {
                return framerateLimit;
            }
            set
            {
                framerateLimit = value;
                Window.SetFramerateLimit(value);
            }
        }

        public string BoolToString(bool value, string falseyText = "FALSE", string trueyText = "TRUE")
        {
            if (value)
            {
                return trueyText;
            }
            else

                return falseyText;
        }

        internal class Directories
        {
            public string Resources = "resources//";
            public string User
            {
                get
                {
                    return Resources + "user//";
                }
            }

            public string Fonts
            {
                get
                {
                    return Resources + "fonts//";
                }
            }
            public string Screens
            {
                get
                {
                    return User + "screens//";
                }
            }

            private string project = "testscreen//";

            public string Project
            {
                get
                {
                    return Screens + project;
                }
                set
                {
                    project = value;
                }
            }
        }

        public Directories directories = new();

        public Color DynValueToColor(DynValue T)
        {
            if (T.Type == DataType.Table)
            {
                Table t = T.Table;

                DynValue defaultVal = DynValue.NewNumber(0);

                DynValue r = t.Get("r");
                if (r.Type == DataType.Nil)
                {
                    r = defaultVal;
                }
                DynValue g = t.Get("g");
                if (g.Type == DataType.Nil)
                {
                    g = defaultVal;
                }
                DynValue b = t.Get("b");
                if (b.Type == DataType.Nil)
                {
                    b = defaultVal;
                }
                DynValue a = t.Get("a");

                if (r.Type == DataType.Number && g.Type == DataType.Number && b.Type == DataType.Number)
                {
                    if (a.Type != DataType.Number)
                    {
                        a = DynValue.NewNumber(255);
                    }

                    return new((byte)r.Number, (byte)g.Number, (byte)b.Number, (byte)a.Number);
                }
            }

            return Color.Black;
        }

        public Vector2f DynValueToVector2f(DynValue T)
        {
            Vector2f result = new(0, 0);

            if (T.Type == DataType.Table) {
                Table t = T.Table;
                DynValue x = t.Get("x");
                DynValue y = t.Get("y");

                if (x.Type == DataType.Number)
                {
                    result.X = ((float)x.Number);
                }

                if (y.Type == DataType.Number)
                {
                    result.Y = ((float)y.Number);
                }
            }

            return result;
        }

        Table Vec2New(Vector2f pos)
        {
            return scr.Call((DynValue)((Table)scr.Globals["Vec2"]).Get("New"), [pos.X, pos.Y]).Table;
        }

        Table ColorNew(uint r, uint g, uint b)
        {
            return scr.Call(((Table)scr.Globals["Color"]).Get("New"), [r, g, b]).Table;
        }

        public void ToggleVerbose()
        {
            Verbose = !Verbose;
            Console.WriteLine($"Verbose output {BoolToString(Verbose, "OFF", "ON")}.");
        }

        internal class CLIFunctions {
            public void PrintHelp()
            {
                string helpText = "Usage: SaverToy [FLAGS...]" +
                        "\n\nOptions:" +
                        "\n-V, --version  Get the current version of SaverToy." +
                        "\n-a, --author   Get info on the author of SaverToy." +
                        "\n-h, -?, --help Displays this help message." +
                        "\n-v, --verbose  Print verbose debug text.";
                Console.WriteLine(helpText);
            }

            public void PrintAuthor()
            {
                Console.WriteLine("Written by Robin <3");
                Console.WriteLine("robinsaviary.com");
            }

            public void PrintVersion()
            {
                Console.WriteLine($"Midnite v{Program.Version}");
            }
        }

        public CLIFunctions CLI = new();

        static public string Version = "Alpha";
        static public char flagPrefix = '-';
        static public char winFlagPrefix = '/';
        public bool Verbose = false;
        public bool WinScreen = false;
        public bool Fullscreen = false;
        public uint Antialiasing = 0;
        public Script scr = new();
        RenderWindow? Window;
        RenderTarget? Target;

        void ClearScreen(DynValue color)
        {
            Color _color = DynValueToColor(color);

            Target.Clear(_color);
        }

        void DrawLine(DynValue pos1, DynValue pos2, DynValue color)
        {
            VertexArray array = new();
            array.PrimitiveType = PrimitiveType.Lines;
            Vertex v1 = new Vertex();
            Vertex v2 = new Vertex();
            v1.Position = DynValueToVector2f(pos1);
            v2.Position = DynValueToVector2f(pos2);
            Color _color = DynValueToColor(color);
            v1.Color = _color;
            v2.Color = _color;
            array.Append(v1);
            array.Append(v2);
            Target.Draw(array);
        }

        void DrawTriangle(DynValue pos1, DynValue pos2, DynValue pos3, DynValue color)
        {
            VertexArray array = new();
            array.PrimitiveType = PrimitiveType.Triangles;
            Vertex v1 = new Vertex();
            Vertex v2 = new Vertex();
            Vertex v3 = new Vertex();
            v1.Position = DynValueToVector2f(pos1);
            v2.Position = DynValueToVector2f(pos2);
            v3.Position = DynValueToVector2f(pos3);
            Color _color = DynValueToColor(color);
            v1.Color = _color;
            v2.Color = _color;
            v3.Color = _color;
            array.Append(v1);
            array.Append(v2);
            array.Append(v3);
            Target.Draw(array);
        }

        void DrawRectangle(DynValue position, DynValue size, DynValue color)
        {
            Vector2f _position = DynValueToVector2f(position);
            Vector2f _size = DynValueToVector2f(size);
            Color _color = DynValueToColor(color);

            RectangleShape shape = new(_size);
            shape.Position = _position;
            shape.FillColor = _color;

            Target.Draw(shape);
        }

        float DynValueToFloat(DynValue number, float defaultValue = 0)
        {
            if (number.Type == DataType.Number)
            {
                return (float)number.Number;
            }

            return defaultValue;
        }

        void DrawCircle(DynValue position, DynValue radius, DynValue color)
        {
            Vector2f _position = DynValueToVector2f(position);
            float _radius = DynValueToFloat(radius);
            Vector2f _offset = new(_radius, _radius);
            Color _color = DynValueToColor(color);

            CircleShape shape = new();
            shape.Position = _position - _offset;
            shape.Radius = _radius;
            shape.FillColor = _color;

            Target.Draw(shape);
        }

        Table GetWindowSize()
        {
            Table t = new(scr);

            Vector2u windowSize = Window.Size;

            return Vec2New(new(windowSize.X, windowSize.Y));
        }

        DynValue GetWindowWidth()
        {
            Vector2u windowSize = Window.Size;

            return DynValue.NewNumber(windowSize.X);
        }

        DynValue GetWindowHeight()
        {
            Vector2u windowSize = Window.Size;

            return DynValue.NewNumber(windowSize.Y);
        }

        Table GetCursorPosition()
        {
            Vector2i mousePosition = Mouse.GetPosition(Window);

            return Vec2New(new(mousePosition.X, mousePosition.Y));
        }

        bool IsCursorOnscreen()
        {
            Vector2i _position = Mouse.GetPosition(Window);

            if (_position.X < 0 || _position.Y < 0)
            {
                return false;
            }

            Vector2u _size = Window.Size;

            if (_position.X > _size.X || _position.Y > _size.Y)
            {
                return false;
            }

            return true;
        }

        void RelaunchWindow()
        {
            RemakeWindow();
        }

        void Exit()
        {
            Window.Close();
        }

        double Time()
        {
            return timer.ElapsedTime.AsSeconds();
        }

        double T()
        {
            return Time();
        }

        void SetFramerateLimit(DynValue limit)
        {
            FramerateLimit = (uint)limit.Number;
        }

        DynValue GetFramerateLimit()
        {
            return DynValue.NewNumber(FramerateLimit);
        }

        void LoadTexture(string name, string filename)
        {
            Texture texture;

            texture = new(directories.Project + filename);
            textures.Add(name, texture);
        }

        void UnloadTexture(string name)
        {
            if (textures.ContainsKey(name))
            {
                textures[name].Dispose(); // Garbage day!
                textures.Remove(name);
            }
            
        }

        void DrawTexture(string name, DynValue position)
        {
            if (textures.ContainsKey(name))
            {
                Sprite sprite = new();
                sprite.Texture = textures[name];
                sprite.Position = DynValueToVector2f(position);

                Target.Draw(sprite);
            }
        }

        Table GetTextures()
        {
            Table result = new(scr);

            foreach (var item in textures)
            {
                result.Append(DynValue.NewString(item.Key));
            }

            return result;
        }

        public enum States
        {
            TextEditor,
            Screensaver,
        }

        private States _state = States.TextEditor;

        public States State
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state != value)
                {
                    switch (value)
                    {
                        case States.Screensaver:
                            //Script scr = new Script(); // Create script
                            scr.Options.DebugPrint = s => { Console.WriteLine(s); }; // Set up debug

                            // Define built-in functions
                            scr.Globals["DrawRectangle"] = (Action<DynValue, DynValue, DynValue>)DrawRectangle;
                            scr.Globals["DrawCircle"] = (Action<DynValue, DynValue, DynValue>)DrawCircle;
                            scr.Globals["DrawLine"] = (Action<DynValue, DynValue, DynValue>)DrawLine;
                            scr.Globals["DrawTriangle"] = (Action<DynValue, DynValue, DynValue, DynValue>)DrawTriangle;
                            scr.Globals["ClearScreen"] = (Action<DynValue>)ClearScreen;
                            scr.Globals["GetWindowSize"] = (Func<Table>)GetWindowSize;
                            scr.Globals["GetWindowWidth"] = (Func<DynValue>)GetWindowWidth;
                            scr.Globals["GetWindowHeight"] = (Func<DynValue>)GetWindowHeight;
                            scr.Globals["GetCursorPosition"] = (Func<Table>)GetCursorPosition;
                            scr.Globals["IsCursorOnscreen"] = (Func<bool>)IsCursorOnscreen;
                            scr.Globals["RelaunchWindow"] = (Action)RelaunchWindow;
                            scr.Globals["GetAntialiasingLevel"] = (Func<DynValue>)GetAntialiasingLevel;
                            scr.Globals["SetAntialiasingLevel"] = (Action<DynValue>)SetAntialiasingLevel;
                            scr.Globals["Exit"] = (Action)Exit;
                            scr.Globals["Time"] = (Func<double>)Time;
                            scr.Globals["T"] = (Func<double>)T;
                            scr.Globals["SetFramerateLimit"] = (Action<DynValue>)SetFramerateLimit;
                            scr.Globals["GetFramerateLimit"] = (Func<DynValue>)GetFramerateLimit;
                            scr.Globals["LoadTexture"] = (Action<string, string>)LoadTexture;
                            scr.Globals["DrawTexture"] = (Action<string, DynValue>)DrawTexture;
                            scr.Globals["UnloadTexture"] = (Action<string>)UnloadTexture;
                            scr.Globals["GetTextures"] = (Func<Table>)GetTextures;

                            try
                            {
                                scr.DoFile(directories.Resources + "scripts//libs//Utility.lua");
                                scr.DoFile(directories.Resources + "scripts//libs//Vec2.lua");
                                scr.DoFile(directories.Resources + "scripts//libs//Color.lua");
                                scr.DoFile(directories.Project + "main.lua");

                                object init = scr.Globals["Init"];
                                if (init != null)
                                {
                                    scr.Call(init);
                                }
                                else if (Verbose)
                                {
                                    Console.WriteLine("WARNING: No 'init()' function found. Skipping...");
                                }

                                if (scr.Globals["Update"] == null)
                                {
                                    Console.WriteLine("WARNING: Your project is missing the typical 'Update()' entry point.");
                                }

                                timer.Restart();
                            }
                            catch(MoonSharp.Interpreter.InterpreterException e)
                            {
                                Console.WriteLine(e.DecoratedMessage);
                            }

                            break;

                        case States.TextEditor:

                            break;
                    }

                    _state = value;
                }
            }
        }

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

        public void RemakeWindow()
        {
            Styles windowStyle = Styles.Default;

            VideoMode videoMode = VideoMode.FullscreenModes.Last();

            if (Fullscreen)
            {
                windowStyle = Styles.None;
                videoMode = VideoMode.DesktopMode;
            }

            ContextSettings settings = new();
            settings.AntialiasingLevel = Antialiasing;

            // Since the size of SaverToy is dynamic, we'll simply initialize the window size to something sensible by
            // getting the smallest resolution for a fullscreen window that the OS considers reasonable.
            if (Window != null)
            {
                Window.Close();
            }
            Window = new(videoMode, $"SaverToy v{Program.Version}", windowStyle, settings);
            Target = Window;

            Window.SetFramerateLimit(FramerateLimit);

            Window.Closed += events.Closed;
            Window.KeyPressed += events.KeyPressed;
            Window.KeyReleased += events.KeyReleased;
            Window.Resized += events.Resized;
            Window.TextEntered += events.TextEntered;
            Window.GainedFocus += events.FocusGained;
            Window.LostFocus += events.FocusLost;
            Window.MouseLeft += events.MouseLeft;
            Window.MouseEntered += events.MouseEntered;
            Window.MouseButtonPressed += events.MouseButtonPressed;
            Window.MouseButtonReleased += events.MouseButtonReleased;
        }

        public void SetAntialiasingLevel(DynValue level)
        {
            float _level = DynValueToFloat(level);
            Antialiasing = (uint)_level;
        }

        public DynValue GetAntialiasingLevel()
        {
            return DynValue.NewNumber(Antialiasing);
        }

        public void Run()
        {
            Textbox file = new(this);

            RemakeWindow();

            //Font font = new("resources/fonts/JetBrainsMono-Regular.ttf");
            //font.SetSmooth(true);

            //file.Font = font;

            while (Window.IsOpen)
            {
                Window.DispatchEvents();

                if (IsKeyPressed(Keyboard.Key.Escape) || (WinScreen && IsKeyPressed(Keyboard.Key.Space)))
                {
                    Window.Close();
                }

                if (IsKeyPressed(Keyboard.Key.F11))
                {
                    Fullscreen = !Fullscreen;
                    RemakeWindow();
                }

                Window.Clear(Color.Black);
                switch (State)
                {
                    case States.TextEditor:
                        file.Step();
                        file.Draw(Window);
                    break;

                    case States.Screensaver:
                        object update = scr.Globals["Update"];
                        try
                        {
                            if (update != null) scr.Call(update);
                        }
                        catch(InterpreterException e)
                        {
                            Console.WriteLine(e.DecoratedMessage);
                        }
                        
                        break;
                }
                events.ClearKeys();
                events.ClearTypedText();
                Window.Display();
            }
        }
    }

    static void Main(string[] args)
    {
        Program program = new();
        program.State = Program.States.Screensaver;

        bool runProgram = true;

        if (args.Length > 0)
        {
            string commandFlagTwice = Program.flagPrefix.ToString() + Program.flagPrefix.ToString();

            foreach (string arg in args)
            {
                if (arg == Program.flagPrefix + "h" || arg == Program.flagPrefix + "?" || arg == commandFlagTwice + "help")
                {
                    runProgram = false;
                    program.CLI.PrintHelp();
                }
                else if (arg == Program.flagPrefix + "V" || arg == commandFlagTwice + "version")
                {
                    runProgram = false;
                    program.CLI.PrintVersion();
                }
                else if (arg == Program.flagPrefix + "a" || arg == commandFlagTwice + "author")
                {
                    runProgram = false;
                    program.CLI.PrintAuthor();
                }
                else if (arg == Program.flagPrefix + "v" || arg == commandFlagTwice + "verbose")
                {
                    program.ToggleVerbose();
                }
                else if (arg == Program.winFlagPrefix + "s")
                {
                    // Windows is attempting to launch this application as a fullscreen screensaver.
                    program.State = Program.States.Screensaver;
                    program.WinScreen = true;
                }
                else if (arg == Program.winFlagPrefix + "c")
                {
                    // Windows is attempting to launch this application's configs.
                }
                else if (arg == Program.winFlagPrefix + "p")
                {
                    // Windows wants us to display a preview of our screensaver on the provided handle.
                    runProgram = false;
                }
                else if (arg == Program.winFlagPrefix + "d")
                {
                    // Windows is opening this program as debug in Visual Studio.
                }
                else if (arg.StartsWith('-'))
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
