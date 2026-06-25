namespace MinecraftClone.Core.Level.WorldGeneration.Passes;

public class EmeraldOrePass : OrePass
{
    public override int Order => ORES;
    public override Block Block => Minecraft.Instance.EmeraldOre;
    public override float Frequency => 1;
    public override float Threshold => 0.0003f;
    public override int MaxY => 20;

    protected override int Seed => 21501;
}