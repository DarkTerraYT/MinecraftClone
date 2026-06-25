using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level.WorldGeneration.Passes;

public class LapisOrePass : OrePass
{
    public override int Order => ORES + 2;
    
    public override Block Block => Minecraft.Instance.LapisOre;
    
    public override float Frequency => 3;
    
    public override float Threshold => 0.00375f;
    public override int MaxY => 35;

    protected override int Seed => -1024216235;
}