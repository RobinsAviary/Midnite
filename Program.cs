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
        Clock deltaTimer = new();

        // Limited set of modules for users :)
        static CoreModules modules = CoreModules.Preset_HardSandbox | CoreModules.Metatables | CoreModules.ErrorHandling | CoreModules.Coroutine | CoreModules.OS_Time;

        double delta = 0;

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

            private string project = "";

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

            public string RawProject
            {
                get
                {
                    return project;
                }
                set
                {
                    project = value;
                }
            }

            public string Scripts
            {
                get
                {
                    return Resources + "scripts//";
                }
            }

            public string Libs
            {
                get
                {
                    return Scripts + "libs//";
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
                string helpText = "Usage: Midnite [FLAGS...]" +
                        "\n\nOptions:" +
                        "\n-V, --version  Get the current version of Midnite." +
                        "\n-a, --author   Get info on the author of Midnite." +
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

        void HandleException(InterpreterException e)
        {
            Console.WriteLine("Runtime Exception: " + e.DecoratedMessage);
            if (Window != null)
            {
                Window.Close();
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
        public Script scr = new(modules);
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

        double Delta()
        {
            return delta;
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
            if (!textures.ContainsKey(name))
            {
                Texture texture;

                texture = new(directories.Project + "//" + filename);

                textures.Add(name, texture);
            }
        }

        void UnloadTexture(string name)
        {
            if (textures.ContainsKey(name))
            {
                textures[name].Dispose(); // Garbage day!
                textures.Remove(name);
            }
            
        }

        void DrawTexture(string name, DynValue position, DynValue tint, DynValue origin)
        {
            if (name != null) {
                if (textures.ContainsKey(name))
                {
                    Sprite sprite = new();
                    Texture texture = textures[name];
                    sprite.Texture = texture;
                    sprite.Position = DynValueToVector2f(position);

                    Color color = Color.White;

                    if (tint.Type == DataType.Table)
                    {
                        color = DynValueToColor(tint);
                    }
                    sprite.Color = color;
                    Vector2f sizef = ((Vector2f)texture.Size);
                    Vector2f originf = DynValueToVector2f(origin);
                    sprite.Origin = new(sizef.X * originf.X, sizef.Y * originf.Y);

                    Target.Draw(sprite);
                }
            }
        }

        void DrawTextureSR(string name, DynValue position, DynValue tint, DynValue origin, DynValue scale, DynValue rotation)
        {
            if (name != null)
            {
                if (textures.ContainsKey(name))
                {
                    Sprite sprite = new();
                    Texture texture = textures[name];
                    sprite.Texture = texture;
                    sprite.Position = DynValueToVector2f(position);
                    Color color = Color.White;

                    if (tint.Type == DataType.Table)
                    {
                        color = DynValueToColor(tint);
                    }
                    sprite.Color = color;
                    sprite.Scale = DynValueToVector2f(scale);
                    sprite.Rotation = DynValueToFloat(rotation);
                    Vector2f sizef = ((Vector2f)texture.Size);
                    Vector2f originf = DynValueToVector2f(origin);
                    sprite.Origin = new(sizef.X * originf.X, sizef.Y * originf.Y);  

                    Target.Draw(sprite);
                }
            }
        }

        DynValue TextureSize(string name)
        {
            if (textures.ContainsKey(name))
            {
                Vector2u size = textures[name].Size;

                return DynValue.NewTable(Vec2New(new(size.X, size.Y)));
            }

            return DynValue.NewNil();
        }

        DynValue TextureWidth(string name)
        {
            if (textures.ContainsKey(name))
            {
                uint width = textures[name].Size.X;

                return DynValue.NewNumber(width);
            }

            return DynValue.NewNil();
        }

        DynValue TextureHeight(string name)
        {
            if (textures.ContainsKey(name))
            {
                uint height = textures[name].Size.Y;

                return DynValue.NewNumber(height);
            }

            return DynValue.NewNil();
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

        void LoadProject(ref Script scr, string project)
        {
            foreach (KeyValuePair<string, Texture> pair in textures)
            {
                pair.Value.Dispose();
            }
            textures.Clear();

            foreach (KeyValuePair<string, Sound> pair in sounds)
            {
                pair.Value.Dispose();
            }
            sounds.Clear();

            Antialiasing = 0;

            directories.Project = project + "//";

            scr = null;
            scr = new(modules);

            //Script scr = new Script(); // Create script
            scr.Options.DebugPrint = s => { Console.WriteLine(s); }; // Set up debug

            // Define built-in functions
            scr.Globals["GetAntialiasingLevel"] = (Func<DynValue>)GetAntialiasingLevel;
            scr.Globals["SetAntialiasingLevel"] = (Action<DynValue>)SetAntialiasingLevel;
            scr.Globals["Time"] = (Func<double>)Time;
            scr.Globals["T"] = (Func<double>)T;
            scr.Globals["Delta"] = (Func<double>)Delta;
            scr.Globals["SetFramerateLimit"] = (Action<DynValue>)SetFramerateLimit;
            scr.Globals["GetFramerateLimit"] = (Func<DynValue>)GetFramerateLimit;

            // Make Texture Namespace
            Table TextureNS = new(scr);
            Table DrawNS = new(scr);
            Table WindowNS = new(scr);
            Table CursorNS = new(scr);

            scr.Globals.Set("Texture", DynValue.NewTable(TextureNS)); // Add it to global namespace
            scr.Globals.Set("Draw", DynValue.NewTable(DrawNS));
            scr.Globals.Set("Window", DynValue.NewTable(WindowNS));
            scr.Globals.Set("Cursor", DynValue.NewTable(CursorNS));

            TextureNS["Load"] = (Action<string, string>)LoadTexture;
            TextureNS["Unload"] = (Action<string>)UnloadTexture;
            TextureNS["GetLoaded"] = (Func<Table>)GetTextures;
            TextureNS["Size"] = (Func<string, DynValue>)TextureSize;
            TextureNS["Width"] = (Func<string, DynValue>)TextureWidth;
            TextureNS["Height"] = (Func<string, DynValue>)TextureHeight;

            DrawNS["Rectangle"] = (Action<DynValue, DynValue, DynValue>)DrawRectangle;
            DrawNS["Circle"] = (Action<DynValue, DynValue, DynValue>)DrawCircle;
            DrawNS["Line"] = (Action<DynValue, DynValue, DynValue>)DrawLine;
            DrawNS["Triangle"] = (Action<DynValue, DynValue, DynValue, DynValue>)DrawTriangle;
            DrawNS["Clear"] = (Action<DynValue>)ClearScreen;
            DrawNS["Texture"] = (Action<string, DynValue, DynValue, DynValue>)DrawTexture;
            DrawNS["TextureSR"] = (Action<string, DynValue, DynValue, DynValue, DynValue, DynValue>)DrawTextureSR;

            WindowNS["Size"] = (Func<Table>)GetWindowSize;
            WindowNS["Width"] = (Func<DynValue>)GetWindowWidth;
            WindowNS["Height"] = (Func<DynValue>)GetWindowHeight;
            WindowNS["Relaunch"] = scr.Globals["RelaunchWindow"] = (Action)RelaunchWindow;
            WindowNS["Close"] = (Action)Exit;

            CursorNS["Position"] = (Func<Table>)GetCursorPosition;
            CursorNS["OnScreen"] = (Func<bool>)IsCursorOnscreen;

            try
            {
                string[] Libs = {
                    "Utility",
                    "Vec2",
                    "Color",
                };

                void LoadLibs(string[] Libs, Script scr)
                {
                    foreach (string Lib in Libs)
                    {
                        scr.DoFile(directories.Libs + Lib + ".lua");
                    }
                }

                timer.Restart();
                deltaTimer.Restart();

                LoadLibs(Libs, scr);
                directories.Project = project;
                Console.WriteLine(project);
                scr.DoFile(directories.Project + "\\" + "main.lua");

                object init = scr.Globals["Init"];
                if (init != null)
                {
                    try
                    {
                        scr.Call(init);
                    }
                    catch (InterpreterException e)
                    {
                        HandleException(e);
                    }
                }
                else if (Verbose)
                {
                    Console.WriteLine("WARNING: No 'init()' function found. Skipping...");
                }

                RelaunchWindow();

                if (scr.Globals["Update"] == null)
                {
                    Console.WriteLine("WARNING: Your project is missing the typical 'Update()' entry point.");
                }

                
            }
            catch (InterpreterException e)
            {
                HandleException(e);
            }
        }

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
                            LoadProject(ref scr, "dvd");

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

            VideoMode videoMode;
            bool windowPosSet = false;
            Vector2i windowPos = new();

            if (Window == null)
            {
                videoMode = VideoMode.FullscreenModes.Last();
            }
            else
            {
                Vector2u size = Window.Size;

                videoMode = new(size.X, size.Y);
                windowPos = Window.Position;
                windowPosSet = true;

                Window.Close();
            }

            if (Fullscreen)
            {
                windowStyle = Styles.None;
                videoMode = VideoMode.DesktopMode;
            }

            ContextSettings settings = new();
            settings.AntialiasingLevel = Antialiasing;

            // Since the size of Midnite is dynamic, we'll simply initialize the window size to something sensible by
            // getting the smallest resolution for a fullscreen window that the OS considers reasonable.

            Window = new(videoMode, $"Midnite v{Program.Version}", windowStyle, settings);
            if (windowPosSet)
            {
                Window.Position = windowPos;
            }
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

                if (Keyboard.IsKeyPressed(Keyboard.Key.LControl) && IsKeyPressed(Keyboard.Key.R))
                {
                    LoadProject(ref scr, directories.RawProject);
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
                            HandleException(e);
                        }
                        
                        break;
                }
                events.ClearKeys();
                events.ClearTypedText();
                Window.Display();
                delta = deltaTimer.Restart().AsSeconds();
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
                    Console.WriteLine("Use -h, -?, or --help to view help for Midnite.");
                    runProgram = false;
                }
            }
        }

        if (runProgram) program.Run();
    }
}
