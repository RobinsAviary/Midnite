using MoonSharp.Interpreter;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Midnite
{
    static public bool IsFolderProject(string filename)
    {
        return (File.Exists(filename + "\\main.lua"));
    }

    static void ConsoleHighlightText()
    {
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.ForegroundColor = ConsoleColor.White;
    }

    internal class Program
    {
        public Program()
        {
            CLI = new(this);
        }

        Clock timer = new();
        Clock deltaTimer = new();

        // Limited set of modules for users :)
        static CoreModules modules = CoreModules.Preset_HardSandbox | CoreModules.Metatables | CoreModules.ErrorHandling | CoreModules.Coroutine | CoreModules.OS_Time;

        double delta = 0;

        private uint framerateLimit = 0;

        public Dictionary<string, Texture> textures = new();
        public Dictionary<string, SoundBuffer> sounds = new();
        public Dictionary<string, Sound> soundInsts = new();

        public uint FramerateLimit
        {
            get
            {
                return framerateLimit;
            }
            set
            {
                framerateLimit = value;
                if (Window != null)
                {
                    Window.SetFramerateLimit(value);
                }
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
            

            public string Resources = "resources\\";
            public string User
            {
                get
                {
                    return Resources + "user\\";
                }
            }

            public string Fonts
            {
                get
                {
                    return Resources + "fonts\\";
                }
            }
            public string Screens
            {
                get
                {
                    return User + "screens\\";
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
                    return Resources + "scripts\\";
                }
            }

            public string Libs
            {
                get
                {
                    return Scripts + "libs\\";
                }
            }

            public List<String> Projects
            {
                get
                {
                    List<string> result = new();

                    try
                    {
                        var dirs = Directory.GetDirectories(Screens);

                        if (dirs.Length > 0)
                        {
                            foreach (string dir in dirs)
                            {
                                if (IsFolderProject(dir)) {
                                    string dirFin = dir.Split('\\').Last();
                                    result.Add(dirFin);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }

                    return result;
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
            Program program;
            public CLIFunctions(Program _program)
            {
                program = _program;
            }
            public void PrintHelp()
            {
                string helpText = "Usage: Midnite [FLAGS...]" +
                        "\n\nOptions:" +
                        "\n-V, --version  Get the current version of Midnite." +
                        "\n-a, --author   Get info on the author of Midnite." +
                        "\n-h, -?, --help Displays this help message." +
                        "\n-v, --verbose  Print verbose debug text." +
                        "\n-c, --cli      Launch the command line interface (Manipulate projects).";
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

            private void NotAnOption(string val)
            {
                Console.WriteLine($"'{val}' is not a valid option in this menu.");
            }

            private string ReadLine()
            {
                string ReadMarker = "> ";
                Console.Write(ReadMarker);
                var val = Console.ReadLine();
                if (val == null) return "";
                return val;
            }

            public void RunCLI()
            {
                // Start the CLI
                bool loop = true;
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Welcome to Midnite v{Program.Version}!");
                Console.ResetColor();

                while (loop)
                {
                    loop = false;
                    ConsoleHighlightText();
                    Console.WriteLine("Menu:");
                    Console.ResetColor();
                    Console.WriteLine("1 - List Projects");
                    Console.WriteLine("2 - Run Project");
                    Console.WriteLine("3 - Create Project");
                    Console.WriteLine("4 - Delete Project");
                    Console.WriteLine("5 - Open User Folder");
                    Console.WriteLine("e - Exit");
                    Console.WriteLine("");

                    string val = ReadLine();

                    if (val == "1")
                    {
                        List<string> projects = program.directories.Projects;

                        if (projects.Count > 0)
                        {
                            Console.WriteLine();
                            ConsoleHighlightText();
                            Console.WriteLine("Projects:");
                            Console.ResetColor();
                            foreach (string project in projects)
                            {
                                Console.WriteLine(project);
                            }
                        }

                        Console.WriteLine();

                        loop = true;
                    }
                    else if (val == "2")
                    {
                        program.State = States.Screensaver;
                        Console.WriteLine();
                        Console.WriteLine("Enter a project name:");
                        string projectName = ReadLine();

                        string errorPrefix = $"Error while opening project '{projectName}': ";
                        string dir = program.directories.Screens + projectName;

                        if (projectName != "")
                        {
                            if (Directory.Exists(dir))
                            {
                                if (IsFolderProject(dir))
                                {
                                    program.LoadProject(ref program.scr, projectName);
                                    program.Run();
                                    program.Window = null;
                                }
                                else
                                {
                                    Console.WriteLine(errorPrefix + "Directory exists, but is not a project (missing main.lua).");
                                }
                            }
                            else
                            {
                                Console.WriteLine(errorPrefix + "No directory with this name exists.");
                            }
                        }
                            
                        Console.WriteLine();

                        loop = true;
                    }
                    else if (val == "3")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Enter a project name:");
                        string name = ReadLine();
                        Console.WriteLine();

                        if (name != "")
                        {
                            string projectError = $"Error creating project '{name}': ";

                            string dir = program.directories.Screens + name;

                            try
                            {
                                if (!IsFolderProject(dir))
                                {
                                    if (!Directory.Exists(dir))
                                    {
                                        Directory.CreateDirectory(dir);
                                        File.Create(dir + "\\" + "main.lua").Dispose();
                                    }
                                    else
                                    {
                                        Console.WriteLine(projectError + "A directory already exists here.");
                                        Console.WriteLine();
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(projectError + "A project already exists here.");
                                    Console.WriteLine();
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(projectError + e.Message);
                                Console.WriteLine();
                            }
                        }

                        loop = true;
                    }
                    else if (val == "4")
                    {
                        loop = true;
                        Console.WriteLine();
                        Console.WriteLine("Enter a project name:");
                        string name = ReadLine();
                        Console.WriteLine();

                        if (name != "")
                        {
                            string projectError = $"Error deleting project '{name}': ";

                            string dir = program.directories.Screens + name;

                            if (IsFolderProject(dir))
                            {
                                Directory.Delete(dir, true);
                            }
                            else if (Directory.Exists(dir))
                            {
                                Console.WriteLine(projectError + "Directory exists, but is not a project.");
                            }
                            else
                            {
                                Console.WriteLine(projectError + "Project does not exist.");
                            }
                        }
                    }
                    else if (val == "5")
                    {
                        Console.WriteLine();

                        loop = true;
                        string dir = AppContext.BaseDirectory + program.directories.User;

                        string prog = "";

                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            prog = "explorer.exe";
                        } 
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            prog = "open";
                        }
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            prog = "mimeopen";
                        }

                        if (prog != "")
                        {
                            try
                            {
                                Process.Start(prog, dir);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Error while trying to open user folder: {e.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("This is not supported on your OS yet, sorry!");
                        }
                    }
                    else if (val == "e")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Good night!");
                    }
                    else
                    {
                        NotAnOption(val);
                        loop = true;
                    }
                }
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

        public CLIFunctions CLI;

        static public string Version = "Alpha";
        static public string DefaultTitle = $"Midnite v{Version}";
        string title = DefaultTitle;

        public string Title {
            set
            {
                title = value;
                if (Window != null)
                {
                    Window.SetTitle(title);
                }
            }
            get
            {
                return title;
            }
        }
        static public char flagPrefix = '-';
        static public char winFlagPrefix = '/';
        public bool Verbose = false;
        public bool WinScreen = false;
        public bool Fullscreen = false;
        public uint Antialiasing = 0;
        public Script scr = new(modules);
        RenderWindow? Window;
        RenderTexture? Render;
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

        void DrawCircle(DynValue position, DynValue radius, DynValue color, double? sides)
        {
            Vector2f _position = DynValueToVector2f(position);
            float _radius = DynValueToFloat(radius);
            Vector2f _offset = new(_radius, _radius);
            Color _color = DynValueToColor(color);
            double _sides = 32;
            if (sides != null)
            {
                _sides = (double)sides;
            }
           

            CircleShape shape = new();
            shape.SetPointCount((uint)_sides);
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

        void SetTitle(string _title)
        {
            if (_title != null)
            {
                Title = _title;
            }
        }

        DynValue GetTitle()
        {
            return DynValue.NewString(Title);
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

        double FPS()
        {
            return Math.Floor(1 / Delta());
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

        void LoadAudio(string name, string filename)
        {
            SoundBuffer sound;
            try
            {
                sound = new(directories.Project + "//" + filename);
                sounds.Add(name, sound);
            }
            catch(Exception e)
            {

            }
        }

        void UnloadAudio(string name)
        {
            if (sounds.ContainsKey(name))
            {
                sounds[name].Dispose();
                sounds.Remove(name);
            }
        }

        void CreateSound(string soundName, string audioName)
        {
            if (soundName != null)
            {
                if (!soundInsts.ContainsKey(soundName))
                {
                    if (audioName != null)
                    {
                        if (sounds.ContainsKey(audioName))
                        {
                            soundInsts.Add(soundName, new(sounds[audioName]));
                        }
                    } else
                    {
                        soundInsts.Add(soundName, new());
                    }
                }
            }
            Sound sound = new();
        }

        void SoundSetAudio(string soundName, string audioName)
        {
            if (soundName != null && audioName != null)
            {
                if (soundInsts.ContainsKey(soundName) && sounds.ContainsKey(audioName))
                {
                    soundInsts[soundName].SoundBuffer = sounds[audioName];
                }
            }
        }

        DynValue SoundGetAudio(string soundName)
        {
            if (soundName != null)
            {
                if (soundInsts.ContainsKey(soundName))
                {
                    var test = sounds.First(x => x.Value == soundInsts[soundName].SoundBuffer).Key;

                    if (test != null)
                    {
                        return DynValue.NewString(test);
                    }
                }
            }

            return DynValue.NewNil();
        }

        void SoundPlay(string soundName)
        {
            if (soundName != null)
            {
                if (soundInsts.ContainsKey(soundName))
                {
                    soundInsts[soundName].Play();
                }
            }
        }

        void SoundPause(string soundName)
        {
            if (soundName != null)
            {
                if (soundInsts.ContainsKey(soundName))
                {
                    soundInsts[soundName].Pause();
                }
            }
        }

        void SoundStop(string soundName)
        {
            if (soundName != null)
            {
                if (soundInsts.ContainsKey(soundName))
                {
                    soundInsts[soundName].Stop();
                }
            }
        }

        DynValue SoundStatus(string soundName)
        {
            if (soundName != null)
            {
                if (soundInsts.ContainsKey(soundName))
                {
                    switch (soundInsts[soundName].Status)
                    {
                        case SFML.Audio.SoundStatus.Paused:
                            return DynValue.NewString("paused");

                        case SFML.Audio.SoundStatus.Stopped:
                            return DynValue.NewString("stopped");

                        case SFML.Audio.SoundStatus.Playing:
                            return DynValue.NewString("playing");
                    }
                }
            }

            return DynValue.NewNil();
        }

        void SoundSetLoop(string soundName, DynValue looping)
        {
            if (soundName != null) {
                if (soundInsts.ContainsKey(soundName))
                {
                    if (looping.Type == DataType.Boolean)
                    {
                        soundInsts[soundName].Loop = looping.Boolean;
                    }
                }
            }
        }

        DynValue SoundGetLooping(string soundName)
        {
            if (soundName != null)
            {
                if (soundInsts.ContainsKey(soundName))
                {
                    return DynValue.NewBoolean(soundInsts[soundName].Loop);
                }
            }

            return DynValue.NewNil();
        }

        void SoundSetPitch(string soundName, DynValue pitch)
        {
            if (soundName != null)
            {
                if (soundInsts.ContainsKey(soundName))
                {
                    if (pitch.Type == DataType.Number)
                    {
                        soundInsts[soundName].Pitch = (float)pitch.Number;
                    }
                }
            }
        }

        DynValue SoundGetPitch(string soundName)
        {
            if (soundName != null)
            {
                if (soundInsts.ContainsKey(soundName))
                {
                    return DynValue.NewNumber(soundInsts[soundName].Pitch);
                }
            }

            return DynValue.NewNil();
        }

        void SoundSetVolume(string soundName, DynValue volume)
        {
            if (soundName != null)
            {
                if (soundInsts.ContainsKey(soundName))
                {
                    if (volume.Type == DataType.Number)
                    {
                        soundInsts[soundName].Volume = (float)volume.Number;
                    }
                }
            }
        }

        DynValue SoundGetVolume(string soundName)
        {
            if (soundName != null)
            {
                if (soundInsts.ContainsKey(soundName))
                {
                    return DynValue.NewNumber(soundInsts[soundName].Volume);
                }
            }

            return DynValue.NewNil();
        }

        void DestroySound(string soundName)
        {
            if (soundName != null)
            {
                if (soundInsts.ContainsKey(soundName))
                {
                    soundInsts[soundName].Dispose();
                    soundInsts.Remove(soundName);
                }
            }
        }

        DynValue AudioLength(string name)
        {
            if (name != null)
            {
                if (sounds.ContainsKey(name))
                {
                    return DynValue.NewNumber(sounds[name].Duration.AsSeconds());
                }
            }

            return DynValue.NewNil();
        }

        DynValue AudioChannelCount(string name)
        {
            if (name != null)
            {
                if (sounds.ContainsKey(name))
                {
                    return DynValue.NewNumber(sounds[name].ChannelCount);
                }
            }

            return DynValue.NewNil();
        }

        DynValue AudioSampleRate(string name)
        {
            if (name != null)
            {
                if (sounds.ContainsKey(name))
                {
                    return DynValue.NewNumber(sounds[name].SampleRate);
                }
            }

            return DynValue.NewNil();
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

        Table GetAudioLoaded()
        {
            Table result = new(scr);

            foreach (var item in sounds)
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

        public void LoadProject(ref Script scr, string project)
        {
            title = DefaultTitle; // Don't update the window title now since we're about to remake the window. (It looks cleaner this way.)

            foreach (KeyValuePair<string, Texture> pair in textures)
            {
                pair.Value.Dispose();
            }
            textures.Clear();

            foreach (KeyValuePair<string, SoundBuffer> pair in sounds)
            {
                pair.Value.Dispose();
            }
            sounds.Clear();

            foreach (KeyValuePair<string, Sound> pair in soundInsts)
            {
                pair.Value.Dispose();
            }
            soundInsts.Clear();

            Antialiasing = 0;

            directories.Project = project + "//";

            scr = null; // Clear & reset
            scr = new(modules);

            //Script scr = new Script(); // Create script
            scr.Options.DebugPrint = s => { Console.WriteLine(s); }; // Set up debug

            // Define built-in functions
            scr.Globals["GetAntialiasingLevel"] = (Func<DynValue>)GetAntialiasingLevel;
            scr.Globals["SetAntialiasingLevel"] = (Action<DynValue>)SetAntialiasingLevel;
            scr.Globals["Time"] = (Func<double>)Time;
            scr.Globals["T"] = (Func<double>)T;
            scr.Globals["Delta"] = (Func<double>)Delta;

            // Make Texture Namespace
            Table TextureNS = new(scr);
            Table DrawNS = new(scr);
            Table WindowNS = new(scr);
            Table CursorNS = new(scr);
            Table AudioNS = new(scr);
            Table SoundNS = new(scr);

            scr.Globals.Set("Texture", DynValue.NewTable(TextureNS)); // Add it to global namespace
            scr.Globals.Set("Draw", DynValue.NewTable(DrawNS));
            scr.Globals.Set("Window", DynValue.NewTable(WindowNS));
            scr.Globals.Set("Cursor", DynValue.NewTable(CursorNS));
            scr.Globals.Set("Audio", DynValue.NewTable(AudioNS));
            scr.Globals.Set("Sound", DynValue.NewTable(SoundNS));

            scr.Globals["MIDNITE_VERSION"] = DynValue.NewString(Version);

            TextureNS["Load"] = (Action<string, string>)LoadTexture;
            TextureNS["Unload"] = (Action<string>)UnloadTexture;
            TextureNS["GetLoaded"] = (Func<Table>)GetTextures;
            TextureNS["Size"] = (Func<string, DynValue>)TextureSize;
            TextureNS["Width"] = (Func<string, DynValue>)TextureWidth;
            TextureNS["Height"] = (Func<string, DynValue>)TextureHeight;

            DrawNS["Rectangle"] = (Action<DynValue, DynValue, DynValue>)DrawRectangle;
            DrawNS["Circle"] = (Action<DynValue, DynValue, DynValue, double?>)DrawCircle;
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
            WindowNS["SetTitle"] = (Action<string>)SetTitle;
            WindowNS["GetTitle"] = (Func<DynValue>)GetTitle;
            WindowNS["SetFPSLimit"] = (Action<DynValue>)SetFramerateLimit;
            WindowNS["GetFPSLimit"] = (Func<DynValue>)GetFramerateLimit;
            WindowNS["FPS"] = (Func<double>)FPS;

            CursorNS["Position"] = (Func<Table>)GetCursorPosition;
            CursorNS["OnScreen"] = (Func<bool>)IsCursorOnscreen;

            AudioNS["Load"] = (Action<string, string>)LoadAudio;
            AudioNS["Unload"] = (Action<string>)UnloadAudio;
            AudioNS["Length"] = (Func<string, DynValue>)AudioLength;
            AudioNS["ChannelCount"] = (Func<string, DynValue>)AudioChannelCount;
            AudioNS["SampleRate"] = (Func<string, DynValue>)AudioSampleRate;
            AudioNS["GetLoaded"] = (Func<Table>)GetAudioLoaded;

            SoundNS["Create"] = (Action<string, string>)CreateSound;
            SoundNS["Destroy"] = (Action<string>)DestroySound;
            SoundNS["SetPitch"] = (Action<string, DynValue>)SoundSetPitch;
            SoundNS["GetPitch"] = (Func<string, DynValue>)SoundGetPitch;
            SoundNS["Status"] = (Func<string, DynValue>)SoundStatus;
            SoundNS["SetLooping"] = (Action<string, DynValue>)SoundSetLoop;
            SoundNS["GetLooping"] = (Func<string, DynValue>)SoundGetLooping;
            SoundNS["Play"] = (Action<string>)SoundPlay;
            SoundNS["Pause"] = (Action<string>)SoundPause;
            SoundNS["Stop"] = (Action<string>)SoundStop;
            SoundNS["SetAudio"] = (Action<string, string>)SoundSetAudio;
            SoundNS["SetVolume"] = (Action<string, DynValue>)SoundSetVolume;
            SoundNS["GetVolume"] = (Func<string, DynValue>)SoundGetVolume;

            try
            {;
                DirectoryInfo d = new(Directory.GetCurrentDirectory() + @"/resources/scripts/libs");
                FileInfo[] Files = d.GetFiles("*.lua");

                void LoadLibs(Script scr)
                {
                    foreach (FileInfo file in Files)
                    {
                        scr.DoFile(directories.Libs + file.Name);
                    }
                }

                timer.Restart();
                deltaTimer.Restart();

                LoadLibs(scr);
                directories.Project = project;
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

            videoMode = VideoMode.FullscreenModes.Last();

            if (Window != null)
            {
                if (!Fullscreen)
                {
                    Vector2u size = Window.Size;

                    videoMode = new(size.X, size.Y);
                    windowPos = Window.Position;
                    windowPosSet = true;
                }

                if (Fullscreen)
                {
                    windowStyle = Styles.None;
                    videoMode = VideoMode.DesktopMode;
                }

                Window.Close();
            }
            

            ContextSettings settings = new();
            settings.AntialiasingLevel = Antialiasing;

            // Since the size of Midnite is dynamic, we'll simply initialize the window size to something sensible by
            // getting the smallest resolution for a fullscreen window that the OS considers reasonable.

            Window = new(videoMode, Title, windowStyle, settings);
            if (windowPosSet)
            {
                Window.Position = windowPos;
            }
            

            Render = new(videoMode.Width, videoMode.Height, settings);
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

            if (Window != null)
            {
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
                            catch (InterpreterException e)
                            {
                                HandleException(e);
                            }

                            break;
                    }
                    events.ClearKeys();
                    events.ClearTypedText();
                    Texture renderTex = Render.Texture;
                    Sprite sprite = new();
                    sprite.Texture = renderTex;
                    Window.Draw(sprite);
                    Window.Display();
                    delta = deltaTimer.Restart().AsSeconds();
                }
            }
        }
    }

    static void Main(string[] args)
    {
        Program program = new();

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
                else if (arg == Program.flagPrefix + "c" || arg == commandFlagTwice + "cli")
                {
                    runProgram = false;
                    program.CLI.RunCLI();
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

        if (runProgram)
        {
            program.State = Program.States.Screensaver;
            program.LoadProject(ref program.scr, "MNS-BasicAnim");
            program.Run();
        }
    }
}
