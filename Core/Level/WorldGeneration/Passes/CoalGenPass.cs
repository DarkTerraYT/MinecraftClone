using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level.WorldGeneration.Passes;

public class CoalGenPass : GenPass
{
    private const float CoalFrequency = 7;
    private const float CoalThreshold = 0.001f * CoalFrequency;
    
    public override int Order => ORES + 5; // Do coal last as it's the most common
    public override void Pass(Level level, FastNoiseLite globalNoise)
    {
        FastNoiseLite noise = new FastNoiseLite(level.Seed ^ -1095584316, FastNoiseLite.NoiseType.Cellular); // Merge the two seeds

        for (int x = 0; x < Level.WorldWidth * Chunk.Width; x++)
        {
            for (int y = 0; y < Level.WorldDepth * Chunk.Height; y++)
            {
                for (int z = 0; z < Level.WorldDepth * Chunk.Width; z++)
                {
                    float result = (noise.GetNoise((x + globalNoise.GetNoise(z, 0)) * CoalFrequency, y * CoalFrequency, (z + globalNoise.GetNoise(y, 0)) * CoalFrequency) + 1) / 2.0f;

                    if (result <= CoalThreshold)
                    {
                        if (level.TryGetBlock(new Vector3Int(x, y, z), out BlockState block))
                        {
                            if (block.Block == Minecraft.Instance.Stone)
                            {
                                level.SetBlock(new Vector3Int(x, y, z), new BlockState(Minecraft.Instance.CoalOre, new Vector3(x, y, z), level));
                            }
                        }
                    }
                }
            }
        }
    }
}