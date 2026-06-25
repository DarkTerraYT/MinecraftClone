using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level.WorldGeneration.Passes;

public class IronGenPass : OrePass
{
    public override int Order => ORES + 5;
    public override Block Block => Minecraft.Instance.IronOre;
    public override float Frequency => 5;
    public override float Threshold => 0.0025f;
    public override int MaxY => 51;

    protected override int Seed => -1955584132;
}