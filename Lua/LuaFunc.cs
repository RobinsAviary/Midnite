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

    public RenderTarget? GetTarget()
    {
        return luaScript.program.renderInfo.target;
    }

    public void Clear(DynValue color)
    {
        RenderTarget? target = GetTarget();

        if (target != null)
        {
            target.Clear(DynValueToColor(color));
        }
    }

    public void DrawLine(DynValue pos1, DynValue pos2, DynValue color)
    {
        RenderTarget? target = GetTarget();

        if (target != null)
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
            target.Draw(array);
        }
    }

    public void DrawTriangle(DynValue pos1, DynValue pos2, DynValue pos3, DynValue color)
    {
        RenderTarget? target = GetTarget();

        if (target != null)
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
            target.Draw(array);
        }
    }

    public void DrawRectangle(DynValue position, DynValue size, DynValue color)
    {
        RenderTarget? target = GetTarget();

        if (target != null)
        {
            Vector2f _position = DynValueToVector2f(position);
            Vector2f _size = DynValueToVector2f(size);
            Color _color = DynValueToColor(color);

            RectangleShape shape = new(_size);
            shape.Position = _position;
            shape.FillColor = _color;

            target.Draw(shape);
        }
    }

    public void DrawCircle(DynValue position, DynValue radius, DynValue color, double? sides)
    {
        RenderTarget? target = GetTarget();

        if (target != null)
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

            target.Draw(shape);
        }
    }
}