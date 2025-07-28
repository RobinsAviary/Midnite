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

    public string? title;

    public void SetTitle(string _title)
    {
        title = _title;
    }

    public uint framerateLimit = 0;

    public void SetFramerateLimit(uint _framerateLimit)
    {
        framerateLimit = _framerateLimit;
    }

    Vector2i? position;
}