using MoonSharp.Interpreter;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class ProgramClass
{
    public ProgramClass()
    {
        luaScript = new(this);
    }

    public string version = "Alpha";
    public bool verbose = false;
    public bool windowsMode = false;

    RenderInfo renderInfo = new();
    public WinState winState = new();
    LuaScript luaScript;

    public enum States
    {
        TextEditor,
        Screensaver,
    }

    public States state = States.Screensaver;

    // Spits out a different message defending on if the input is truey or falsey.
    public string BoolToString(bool value, string falseyText = "FALSE", string trueyText = "TRUE")
    {
        if (value)
        {
            return trueyText;
        }
        else

        return falseyText;
    }

    // Turns a DynValue into a Color.
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

    // Turns a dynvalue into a Vector2f.
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

    // Turns a dynvalue into a float. Has an optional default value (to match 'or' behavior in lua).
    public float DynValueToFloat(DynValue number, float defaultValue = 0)
    {
        if (number.Type == DataType.Number)
        {
            return (float)number.Number;
        }

        return defaultValue;
    }

    

    // Run the current project.
    public void RunProject()
    {
        winState.RemakeWindow();

        // Main loop
        while (winState.window.IsOpen)
        {
            winState.window.DispatchEvents();

            luaScript.CallLuaFunction("Update");

            winState.events.ClearKeys();
            winState.window.Display();
        }
    }
}