using MoonSharp.Interpreter;
using SFML.System;
using SFML.Graphics;
using SFML.Audio;

public class ProgramClass
{
    LuaFunc luaFunc = new();

    // Limited set of modules for users :)
    static CoreModules modules = CoreModules.Preset_HardSandbox | CoreModules.Metatables | CoreModules.ErrorHandling | CoreModules.Coroutine | CoreModules.OS_Time;

    Script script;

    void RemakeScript()
    {
        script = new(modules);

        script.Globals["HelloWorld"] = (Action)luaFunc.HelloWorld;


    }

    public ProgramClass()
    {
        RemakeScript();
    }

    WindowEvents events = new();

    public string version = "Alpha";
    public bool verbose = false;
    public bool windowsMode = false;

    RenderInfo renderInfo = new();

    public enum States
    {
        TextEditor,
        Screensaver,
    }

    public States state = States.Screensaver;

    public string BoolToString(bool value, string falseyText = "FALSE", string trueyText = "TRUE")
    {
        if (value)
        {
            return trueyText;
        }
        else

            return falseyText;
    }

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

        if (T.Type == DataType.Table)
        {
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

    public float DynValueToFloat(DynValue number, float defaultValue = 0)
    {
        if (number.Type == DataType.Number)
        {
            return (float)number.Number;
        }

        return defaultValue;
    }

    RenderWindow window;

    public void RemakeWindow()
    {
        window = new(new(500, 500), "Test");
        window.Closed += events.Closed;
        window.Resized += events.Resized;
        window.KeyPressed += events.KeyPressed;
        window.KeyReleased += events.KeyReleased;
        window.TextEntered += events.TextEntered;
        window.GainedFocus += events.FocusGained;
        window.LostFocus += events.FocusLost;
        window.MouseEntered += events.MouseEntered;
        window.MouseLeft += events.MouseLeft;
        window.MouseButtonPressed += events.MouseButtonPressed;
        window.MouseButtonReleased += events.MouseButtonReleased;
    }

    public void RunProject()
    {
        RemakeWindow();

        // Main loop
        while (window.IsOpen)
        {
            window.DispatchEvents();



            events.ClearKeys();
            window.Display();
        }
    }
}