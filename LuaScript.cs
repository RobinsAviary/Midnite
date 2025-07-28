using MoonSharp.Interpreter;
using SFML.Window;

public class LuaScript
{
    LuaFunc luaFunc = new();
    ProgramClass program;

    Directories directories = new();

    // Limited set of modules for users :)
    static CoreModules modules = CoreModules.Preset_HardSandbox | CoreModules.Metatables | CoreModules.ErrorHandling | CoreModules.Coroutine | CoreModules.OS_Time;

    public LuaScript(ProgramClass _program)
    {
        RemakeScript();
        program = _program;
    }

    // (Re)Set up the Lua scripting context.
    void RemakeScript()
    {
        script = new(modules);

        script.Globals["HelloWorld"] = (Action)luaFunc.HelloWorld;

        script.DoFile(directories.project.fullpath + directories.mainFile + directories.luaExt);
    }

    // Displays lua-scripting-related exceptions to the end user.
    public void LuaException(InterpreterException e)
    {
        Console.WriteLine("Runtime Exception: " + e.DecoratedMessage);
        if (program.winState.window != null)
        {
            program.winState.window.Close();
        }
    }

    // Attempt to call a global Lua function.
    public void CallLuaFunction(string function)
    {
        // Grab the object from globals
        object func = script.Globals[function];
        try
        {
            // If it exists, let's call it.
            if (func != null) script.Call(func);
        }
        catch (InterpreterException e)
        {
            // Throw exception for user if Lua script error occurs.
            LuaException(e);
        }
    }

    Script script;
}