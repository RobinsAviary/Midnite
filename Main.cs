using SFML.System;
using SFML.Window;
using SFML.Graphics;
using SFML.Audio;
using MoonSharp.Interpreter;

bool launchGUI = false;
bool windowsScreensaver = false;
bool verbose = false;

ProgramClass program = new();
CLI cli = new(program);

cli.ProcessArgs(args);

program.RunProject();