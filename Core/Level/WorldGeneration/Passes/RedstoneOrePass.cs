using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level.WorldGeneration.Passes;

public class RedstoneOrePass : OrePass
{
    public override int Order => ORES + 4;
    
    public override Block Block => Minecraft.Instance.RedstoneOre;
    
    public override float Frequency => 6;
    
    public override float Threshold => 0.006f;
    public override int MaxY => 41;

    protected override int Seed => -312579125;
}