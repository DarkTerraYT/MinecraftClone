using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using MinecraftClone;
using MinecraftClone.Core;
using MinecraftClone.Core.Internal;

public static class Program
{
    public static void Main()
    {
        using Mutex mutex = new Mutex(true, "LerpMcGerk.MinecraftClone", out bool isNew);

        if (!isNew)
        {
            return;
        }
        
        using var savedConsole = new SavedConsole();
        Console.SetOut(savedConsole);
        Minecraft minecraft = new Minecraft();
        minecraft.Run();
    }
}