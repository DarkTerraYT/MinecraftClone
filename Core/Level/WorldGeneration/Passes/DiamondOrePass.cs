using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level.WorldGeneration.Passes;

public class DiamondOrePass : OrePass
{
    public override int Order => ORES + 1;
    
    public override Block Block => Minecraft.Instance.DiamondOre;
    
    public override float Frequency => 3;
    
    public override float Threshold => 0.0006f;
    public override int MaxY => 22;

    protected override int Seed => 2131851235;
}