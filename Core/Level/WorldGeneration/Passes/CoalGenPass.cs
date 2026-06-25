using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level.WorldGeneration.Passes;

public class CoalGenPass : OrePass
{
    public override int Order => ORES + 6; // Do coal last as it's the most common

    public override Block Block => Minecraft.Instance.CoalOre;
    public override float Frequency => 7f;
    public override float Threshold => 0.0014f;
    public override int MaxY => 85;

    protected override int Seed => -1095584316;
}