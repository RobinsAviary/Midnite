using MoonSharp.Interpreter;

namespace Midnite;

public class LuaScript
{
    LuaFunc luaFunc;
    public ProgramClass program;
    public Script? script;

    Directories directories = new();

    // Limited set of modules for users :)
    static CoreModules modules = CoreModules.Preset_HardSandbox | CoreModules.Metatables | CoreModules.ErrorHandling | CoreModules.Coroutine | CoreModules.OS_Time;

    public LuaScript(ProgramClass _program)
    {
        program = _program;
        luaFunc = new(this);
    }

    // (Re)Set up the Lua scripting context.
    void RemakeScript()
    {
        // Set up a clean script object for users.
        script = new(modules);

        // Load standard libraries
        string[] libs = Directory.GetFiles(directories.libs.fullpath);
        foreach (string lib in libs)
        {
            if (lib.EndsWith(directories.luaExt))
            {
                script.DoFile(lib);
            }
        }

        script.Globals["HelloWorld"] = (Action)luaFunc.HelloWorld;

        Table DrawNS = new(script);
        DrawNS["Clear"] = (Action<DynValue>)luaFunc.Clear;
        DrawNS["Line"] = (Action<DynValue, DynValue, DynValue>)luaFunc.DrawLine;
        DrawNS["Triangle"] = (Action<DynValue, DynValue, DynValue, DynValue>)luaFunc.DrawTriangle;
        DrawNS["Rectangle"] = (Action<DynValue, DynValue, DynValue>)luaFunc.DrawRectangle;
        DrawNS["Circle"] = (Action <DynValue, DynValue, DynValue, double?>)luaFunc.DrawCircle;

        script.Globals["Draw"] = DynValue.NewTable(DrawNS);

        Table WindowNS = new(script);
        WindowNS["Size"] = (Func<DynValue>)luaFunc.WindowSize;
        script.Globals["Window"] = DynValue.NewTable(WindowNS);
    }

    // 
    public void InitProject()
    {
        RemakeScript();

        script.DoFile(directories.project.fullpath + directories.mainFile + directories.luaExt);

        CallLuaFunction("Init");
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
        if (script != null)
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
    }
}