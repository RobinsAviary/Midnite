using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class WinState
{
    public WinState()
    {

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