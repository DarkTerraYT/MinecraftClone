using System.Reflection;
using Microsoft.Xna.Framework;

namespace MinecraftClone.Core.Mods;

public abstract class Mod
{
    internal Mod(Assembly assembly)
    {
        ModAssembly = assembly;
    }
    
    public abstract string Name { get; }
    public abstract string Description { get; }
    
    public abstract void Initialize();
    
    public Assembly ModAssembly { get; }
    
    public virtual void EarlyUpdate(GameTime gameTime) { }
    public virtual void Update(GameTime gameTime) { }
    public virtual void LateUpdate(GameTime gameTime) { }

    public virtual void OnUI(GameTime gameTime)
    {
        
    }
    public virtual void Draw(GameTime gameTime)
    {
        
    }

    public virtual void LoadContent()
    {
        
    }
}