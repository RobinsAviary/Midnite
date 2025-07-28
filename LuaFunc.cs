using MoonSharp.Interpreter;
using SFML.Graphics;
using SFML.System;

namespace Midnite;

public class LuaFunc
{
    LuaScript luaScript;

    public LuaFunc(LuaScript _luaScript)
    {
        luaScript = _luaScript;
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

    public void HelloWorld()
    {
        Console.WriteLine("Hello, World!");
    }

    public void Clear()
    {
        luaScript.program.renderInfo.target.Clear(Color.Blue);
    }
}