using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level.WorldGeneration.Passes;

public class GoldOrePass : OrePass
{
    public override int Order => ORES + 3;
    
    public override Block Block => Minecraft.Instance.GoldOre;
    
    public override float Frequency => 8;
    
    public override float Threshold => 0.0040f;
    public override int MaxY => 37;

    protected override int Seed => -1270310135;
}