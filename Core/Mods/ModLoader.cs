using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Xna.Framework;

namespace MinecraftClone.Core.Mods;

public class ModLoader
{
    public static readonly string ModsPath = Path.Combine(AppContext.BaseDirectory, "Mods");
    
    public static ModLoader Instance { get; private set; }

    public IReadOnlyList<Mod> Mods => _mods;
    private readonly List<Mod> _mods = new();

    private Logger logger = new Logger("Mod Loader");
    
    private AssemblyLoadContext _loadContext = new ("MinecraftClone");
    
    public void Initialize()
    {
        Instance = this;

        foreach (var file in Directory.GetFiles(ModsPath, "*.dll"))
        {
            LoadMod(file);
        }
    }

    public void LoadMod(string file)
    {
        Assembly modAssembly;
        try
        {
            modAssembly = _loadContext.LoadFromAssemblyPath(file);
        }
        catch (BadImageFormatException)
        {
            logger.Error(
                $"Assembly at file {file} is not a valid .NET assembly! Please remove from the mods folder.");
            return;
        }
        catch (Exception e)
        {
            logger.Error($"Failed to load assembly {file}: {e}");
            return;
        }

        try
        {
            Type modType = modAssembly.GetTypes().FirstOrDefault(type => type.IsSubclassOf(typeof(Mod)), null);
            if (modType == null)
            {
                return;
            }

            Mod mod = Activator.CreateInstance(modType, BindingFlags.NonPublic, null, [modAssembly], null) as Mod;
            if (mod == null)
            {
                logger.Error($"Failed to instantiate mod from assembly {modAssembly.FullName}!");
                return;
            }

            mod.Initialize();
            _mods.Add(mod);
            logger.Info($"Loaded mod {mod.Name}");
        }
        catch (Exception e)
        {
            logger.Error($"Failed to load mod {file}: {e}");
        }
    }

    internal void LoadModContent()
    {
        foreach (var mod in _mods)
        {
            mod.LoadContent();
        }
    }
    
    internal void UpdateAllMods(GameTime gameTime)
    {
        foreach (var mod in _mods)
        {
            mod.EarlyUpdate(gameTime);
        }
        foreach (var mod in _mods)
        {
            mod.Update(gameTime);
        }
        foreach (var mod in _mods)
        {
            mod.LateUpdate(gameTime);
        }
    }

    internal void DrawAllModsUi(GameTime gameTime)
    {
        foreach (var mod in _mods)
        {
            mod.OnUI(gameTime);
        }
    }
    internal void DrawAllModsMesh(GameTime gameTime)
    {
        foreach (var mod in _mods)
        {
            mod.Draw(gameTime);
        }
    }
}