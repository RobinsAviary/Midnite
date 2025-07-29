using MoonSharp.Interpreter;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Midnite;

public class WinState
{
    public RenderWindow? window;

    public WindowEvents events = new();

    ProgramClass program;

    public WinState(ProgramClass _program)
    {
        program = _program;
        title = "Midnite v" + program.version;
    }

    // (Re)Creates the window with all the necessary events.
    public void RemakeWindow()
    {
        // Fallback resolution
        VideoMode mode = new(420, 360);

        if (VideoMode.FullscreenModes.Length > 0)
        {
            mode = VideoMode.FullscreenModes.Last();
        }

        window = new(mode, "Midnite v" + program.version);
        WindowEvents();
    }

    public void WindowEvents()
    {
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

    string title;

    public void SetTitle(string _title)
    {
        title = _title;

        if (window != null)
        {
            window.SetTitle(title);
        }
    }

    public uint framerateLimit = 0;

    public void SetFramerateLimit(uint _framerateLimit)
    {
        framerateLimit = _framerateLimit;

        if (window != null)
        {
            window.SetFramerateLimit(framerateLimit);
        }
    }

    Vector2i? position;

    Vector2u? size;

    public Vector2u? GetSize()
    {
        if (window != null)
        {
            return window.Size;
        }
        else if (size != null)
        {
            return size;
        }

        return null;
    }
}