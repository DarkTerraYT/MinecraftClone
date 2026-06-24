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

        int maxZ = Level.WorldDepth * Chunk.Depth;
        int maxY = Level.WorldHeight * Chunk.Height;
        
        float[] warpX = new float[maxZ];
        for (int z = 0; z < maxZ; z++) {
            warpX[z] = globalNoise.GetNoise(z, 0);
        }

        float[] warpZ = new float[maxY];
        for (int y = 0; y < maxY; y++) {
            warpZ[y] = globalNoise.GetNoise(y, 0);
        }
        
        float adjustedThreshold = (CoalThreshold * 2.0f) - 1.0f;
        
        for (int x = 0; x < Level.WorldWidth * Chunk.Width; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                float sample = y * CoalFrequency;
                for (int z = 0; z < maxZ; z++)
                {
                    float result = noise.GetNoise((x + warpX[z]) * CoalFrequency, sample, (z + warpZ[y]) * CoalFrequency);

                    if (result <= adjustedThreshold)
                    {
                        if (level.TryGetBlock(new Vector3Int(x, y, z), out BlockState block))
                        {
                            if (block.Block == Minecraft.Instance.Stone)
                            {
                                level.SetBlock(new Vector3Int(x, y, z), Minecraft.Instance.CoalOre);
                            }
                        }
                    }
                }
            }
        }
    }
}