using MoonSharp.Interpreter;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFML.Window.Keyboard;

namespace Midnite;
public class Assets<T>
{
    protected Dictionary<string, T> data = [];

    protected uint GetNextSlot()
    {
        uint i = 1;
        while (data.ContainsKey(i.ToString()))
        {

        }

        return i;
    }

    public T? TryGet(string key)
    {
        data.TryGetValue(key, out T? value);

        return value;
    }
}

public class Textures : Assets<Texture>
{
    public uint? Load(string? filename)
    {
        if (filename != null)
        {
            uint i = GetNextSlot();

            if (File.Exists(filename))
            {
                data[i.ToString()] = new(filename);
            }

            Console.WriteLine($"Loaded asset into slot: {i}");
            return i;
        }

        return null;
    }

    public DynValue LoadLua(string? filename)
    {
        if (filename != null)
        {
            uint? ind = Load(filename);
            if (ind != null)
            {
                return DynValue.NewString(ind.ToString());
            }
        }

        return DynValue.NewNil();
    }

    public void Unload(string? key)
    {
        if (key != null)
        {
            Texture? value = TryGet(key);
            value?.Dispose();
            data.Remove(key);
        }
    }

    public Vector2u? Size(string key)
    {
        if (key != null)
        {
            Texture? texture = TryGet(key);

            return texture?.Size;
        }

        return null;
    }
}

public class Audio : Assets<SoundBuffer>
{
    public uint? Load(string? filename)
    {
        if (filename != null)
        {
            uint i = GetNextSlot();

            if (File.Exists(filename))
            {
                data[i.ToString()] = new(filename);
            }
        }

        return null;
    }

    public void Unload(string? key)
    {
        if (key != null)
        {
            SoundBuffer? value = TryGet(key);
            value?.Dispose();
            data.Remove(key);
        }
    }

    public float? Length(string? key)
    {
        if (key != null)
        {
            SoundBuffer? audio = TryGet(key);

            return audio?.Duration.AsSeconds();
        }

        return null;
    }
}