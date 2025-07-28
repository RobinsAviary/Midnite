using MoonSharp.Interpreter;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Midnite;

public class ProgramClass
{
    public ProgramClass()
    {
        luaScript = new(this);
        winState = new(this);
    }

    public string version = "Alpha";
    public bool verbose = false;
    public bool windowsMode = false;

    public RenderInfo renderInfo = new();
    public WinState winState;
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

    // Run the current project.
    public void RunProject()
    {
        winState.RemakeWindow();
        renderInfo.target = winState.window;
        luaScript.InitProject();

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